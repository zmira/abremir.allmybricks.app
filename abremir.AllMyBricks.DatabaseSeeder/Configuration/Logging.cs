using Microsoft.Extensions.Logging;
using NReco.Logging.File;
using System;
using System.IO;
using System.Reflection;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class Logging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        public static void Configure()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var logFile = Path.Combine(folderPath, $"{DateTime.Now.ToString("yyyyMMdd")}_{Assembly.GetExecutingAssembly().GetName().Name}.log");

            LoggerFactory.AddProvider(new FileLoggerProvider(logFile, new FileLoggerOptions {
                MaxRollingFiles = 5,
                FileSizeLimitBytes = 5 * 1024 * 1024
            }));
        }
    }
}