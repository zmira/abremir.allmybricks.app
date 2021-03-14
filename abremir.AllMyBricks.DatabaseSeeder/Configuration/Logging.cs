using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DatabaseSeeder.Loggers;
using LightInject;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;
using System;
using System.IO;
using System.Reflection;

namespace abremir.AllMyBricks.DatabaseSeeder.Configuration
{
    public static class Logging
    {
        public static ILoggerFactory Factory { get; private set; } = new LoggerFactory();

        public static ILogger CreateLogger<T>() => Factory.CreateLogger<T>();

        public static LogVerbosity LogVerbosity { get; set; }
        public static LogDestinations LogDestination { get; set; }

        public static void Configure(LogDestinations? logDestination, LogVerbosity logVerbosity)
        {
            LogVerbosity = logVerbosity;

            if (LogVerbosity == LogVerbosity.NoLogging)
            {
                return;
            }

            LogDestination = logDestination.Value;

            SetupConsoleLogging();
            SetupFileLogging();

            IoC.IoCContainer.RegisterInstance<ILoggerFactory>(Logging.Factory);

            IoC.IoCContainer.GetInstance<SetSynchronizationServiceLogger>();
            IoC.IoCContainer.GetInstance<ThemeSynchronizerLogger>();
            IoC.IoCContainer.GetInstance<SubthemeSynchronizerLogger>();
            IoC.IoCContainer.GetInstance<SetSynchronizerLogger>();
            IoC.IoCContainer.GetInstance<ThumbnailSynchronizerLogger>();
            IoC.IoCContainer.GetInstance<AssetUncompressionLogger>();
            IoC.IoCContainer.GetInstance<UserSynchronizationServiceLogger>();
            IoC.IoCContainer.GetInstance<UserSynchronizerLogger>();
        }

        private static void SetupFileLogging()
        {
            if (!LogDestination.HasFlag(LogDestinations.File))
            {
                return;
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var logFile = Path.Combine(folderPath, $"{DateTime.Now:yyyyMMdd}_{Assembly.GetExecutingAssembly().GetName().Name}.log");

            Factory.AddProvider(new FileLoggerProvider(logFile, new FileLoggerOptions
            {
                MaxRollingFiles = 5,
                FileSizeLimitBytes = 5 * 1024 * 1024
            }));
        }

        private static void SetupConsoleLogging()
        {
            if (!LogDestination.HasFlag(LogDestinations.Console))
            {
                return;
            }

            Factory = LoggerFactory.Create(builder => builder.AddConsole());
        }
    }
}
