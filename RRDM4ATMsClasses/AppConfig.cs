using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace RRDM4ATMs
{
    public static class AppConfig
    {
        public static IConfiguration Configuration { get; private set; }

        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        // Helper for easier access mimicking older behavior optionally
        public static string Get(string key)
        {
            return Configuration[key];
        }

        public static string GetConnectionString(string name)
        {
            return Configuration.GetSection("ConnectionStrings")[name];
        }
    }
}
