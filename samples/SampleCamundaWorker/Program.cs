using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace SampleCamundaWorker
{
    public class Program
    {
        private const string LogTemplate = "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u4}] ({SourceContext}) " +
                                           "{Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((hostBuilder, logger) =>
                {
                    logger
                        .ReadFrom.Configuration(hostBuilder.Configuration)
                        .WriteTo.Console(outputTemplate: LogTemplate)
                        .Enrich.FromLogContext();
                });
    }
}
