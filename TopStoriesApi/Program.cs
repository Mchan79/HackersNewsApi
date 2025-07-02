using TopStoriesApi.Services;

namespace TopStoriesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add services
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IService, Service>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => "Hackers stories api...");

            app.Run();
        }
    }
}
