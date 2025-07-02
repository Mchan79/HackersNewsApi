using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopStoriesApi.Models;

namespace TopStoriesConsoleApp
{
    internal class ThreadSyncTest
    {
        private readonly int MAX;
        private readonly int TIME;
        private readonly int _stories;

        private List<Item> _calls;
        private object syncCall = new object();
        private AutoResetEvent _areCalls = new AutoResetEvent(true);
        private AutoResetEvent _areSync = new AutoResetEvent(true);
        private int _index;

        public ThreadSyncTest(int max, int time, int stories)
        {
            MAX = max;
            TIME = time;
            _calls = new List<Item>();
            _stories = stories;
        }

        public void Start(int count)
        {
            var items = Enumerable.Range(1, count);

            var tasks = new List<Task>();

            items.ToList().ForEach(item =>
            {
                var task = this.Running(item);
                tasks.Add(task);
            });

            Task.WaitAll(tasks.ToArray());
        }
        private Task Running(int index)
        {
            return Task.Run(() =>
            {
                var isReady = false;
                do
                {
                    _areCalls.WaitOne();
                    lock (syncCall)
                    {
                        if (_calls.Count > 0)
                        {
                            var first = _calls[0];
                            var dif = DateTime.Now - first.Date;
                            if (dif.TotalSeconds > TIME)
                            {
                                Console.WriteLine($"Call {index} - Time exceeded: {dif.TotalSeconds} seconds");
                                _calls.RemoveAt(0);
                            }
                        }
                        if (_calls.Count < MAX)
                        {
                            isReady = true;
                        }
                        if (!isReady)
                        {
                            _areCalls.Set();
                        }
                    }
                }
                while (!isReady);

                _areSync.WaitOne();
                lock (syncCall)
                {
                    _calls.Add(new Item
                    {
                        Index = index,
                        Date = DateTime.Now
                    });
                }
                this.RunningHelper(index);
                _areSync.Set();
                _areCalls.Set();

            });
        }
        private void RunningHelper(int index)
        {
            _index++;
            Console.WriteLine($"{_index} - Running: {index} <START>");
            Thread.Sleep(TimeSpan.FromSeconds(.2));

            var client = new HttpClient();
            var baseUrl = "http://localhost:5256";

            var url = $"{baseUrl}/api/stories/best?n={_stories}";

            var response = client.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStreamAsync();
            var result = new StreamReader(response.Result).ReadToEnd();
            var json = JsonConvert.DeserializeObject<List<StoryDto?>>(result);

            Console.WriteLine($"Call {index}");
            if (json != null)
            {
                Console.WriteLine($"Call {index} - Stories: {json.Count}");

                foreach (var story in json)
                {
                    if (story != null)
                    {
                        Console.WriteLine($"Call {index} - Story: {story.CommentCount} - {story.Score} - Story: {story.Title} - {story.Uri} - {story.PostedBy}");
                    }
                    else
                    {
                        Console.WriteLine($"Call {index} - Story: NULL");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Call {index} - No stories found");
            }

            Console.WriteLine($"{_index} - Running: {index} <END>\n");


        }


    }

    internal class Item
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
    }
}
