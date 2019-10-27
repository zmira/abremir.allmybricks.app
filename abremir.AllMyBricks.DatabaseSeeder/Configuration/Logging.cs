using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;
using System;
using System.IO;
using System.Reflection;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class Logging
    {
        public static ILoggerFactory Factory { get; private set; }

        public static ILogger CreateLogger<T>() => Factory.CreateLogger<T>();

        public static LogVerbosityEnum LogVerbosity { get; set; }
        public static LogDestinationEnum LogDestination { get; set; }

        public static void Configure(LogDestinationEnum logDestination, LogVerbosityEnum logVerbosity)
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

            if (LogVerbosity != LogVerbosityEnum.NoLogging)
            {
                var dataSynchronizationServiceLogger = IoC.IoCContainer.GetInstance<DataSynchronizationServiceLogger>();
                var themeSynchronizerLogger = IoC.IoCContainer.GetInstance<ThemeSynchronizerLogger>();
                var subthemeSynchronizerLogger = IoC.IoCContainer.GetInstance<SubthemeSynchronizerLogger>();
                var setSynchronizerLogger = IoC.IoCContainer.GetInstance<SetSynchronizerLogger>();
                var thumbnailSynchronizerLogger = IoC.IoCContainer.GetInstance<ThumbnailSynchronizerLogger>();
                var assetUncompressionLogger = IoC.IoCContainer.GetInstance<AssetUncompressionLogger>();
                var userSynchronizerLogger = IoC.IoCContainer.GetInstance<UserSynchronizerLogger>();
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

            Factory = new LoggerFactory();

            Factory.AddProvider(new FileLoggerProvider(logFile, new FileLoggerOptions
            {
                MaxRollingFiles = 5,
                FileSizeLimitBytes = 5 * 1024 * 1024
            }));
        }

        private static void SetupConsoleLogging()
        {
            Factory = LoggerFactory.Create(builder => builder.AddConsole());
        }
    }
}
