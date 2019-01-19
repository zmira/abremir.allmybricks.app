using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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

        public static LoggingVerbosityEnum LogVerbosity { get; set; }
        public static LogDestinationEnum LogDestination { get; set; }

        public static void Configure(LogDestinationEnum logDestination, LoggingVerbosityEnum logVerbosity)
        {
            LogVerbosity = logVerbosity;
            LogDestination = logDestination;

            switch (logDestination)
            {
                case LogDestinationEnum.File:
                    SetupFileLogging();
                    break;
                case LogDestinationEnum.Console:
                    SetupConsoleLogging();
                    break;
            }

            if (LogVerbosity != LoggingVerbosityEnum.NoLogging)
            {
                var dataSynchronizationServiceEventHandler = IoC.IoCContainer.GetInstance<DataSynchronizationServiceLogger>();
                var themeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThemeSynchronizerLogger>();
                var subthemeSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SubthemeSynchronizerLogger>();
                var setSynchronizerEventHandler = IoC.IoCContainer.GetInstance<SetSynchronizerLogger>();
                var thumbnailSynchronizerEventHandler = IoC.IoCContainer.GetInstance<ThumbnailSynchronizerLogger>();
            }
        }

        private static void SetupFileLogging()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var logFile = Path.Combine(folderPath, $"{DateTime.Now.ToString("yyyyMMdd")}_{Assembly.GetExecutingAssembly().GetName().Name}.log");

            LoggerFactory.AddProvider(new FileLoggerProvider(logFile, new FileLoggerOptions
            {
                MaxRollingFiles = 5,
                FileSizeLimitBytes = 5 * 1024 * 1024
            }));
        }

        private static void SetupConsoleLogging()
        {
            LoggerFactory.AddProvider(new ConsoleLoggerProvider((_, __) => true, true));
        }
    }
}