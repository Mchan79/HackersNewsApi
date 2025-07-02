namespace TopStoriesConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<Start console app to test TopStoriesApi>");

            //set the numbers of stories to fetch
            var _numberOfStories = 2;
            //set the maximum number of threads
            var _max = 3;
            //set the time in seconds to wait until the max number {max} of threads are finished  
            var _time = 3;
            
            //create a new instance of ThreadSyncTest
            var sync = new ThreadSyncTest(max: _max, time: _time, stories: _numberOfStories);
            
            sync.Start(10);

            Console.WriteLine("<End>");

            Console.ReadLine();
        }
    }
}
