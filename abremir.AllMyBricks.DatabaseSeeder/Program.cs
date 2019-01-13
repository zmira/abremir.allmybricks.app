using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    class Program
    {
        private static ILogger Logger;

        static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption();

            var unattendedCommand = app.Option("--unattended", "Run in unattended mode (non-interactive)", CommandOptionType.NoValue);
            var loggingVerbosityCommand = app.Option("--logging-verbosity", "Logging verbosity: none, minimal or full", CommandOptionType.SingleValue);
            var logDestinationCommand = app.Option("--log-destination", "[this option is only valid when run with --unattended]: Log destination: console or file", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                IoC.Configure();
                IoC.ConfigureOnboarding(Settings.OnboardingUrl);

                var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
                fileSystem.EnsureLocalDataFolder();

                var loggingVerbosity = GetLoggingVerbosity(loggingVerbosityCommand, unattendedCommand);
                var logDestination = GetLogDestination(loggingVerbosityCommand, unattendedCommand, logDestinationCommand);

                Logging.Configure(logDestination, loggingVerbosity);

                Logger = Logging.CreateLogger<Program>();

                Logger.LogInformation("Starting database seeder...");

                if (unattendedCommand.HasValue())
                {
                    NonInteractiveConsole.Run().GetAwaiter().GetResult();
                }
                else
                {
                    InteractiveConsole.Run();
                }
            });

            return app.Execute(args);
        }

        private static LoggingVerbosityEnum GetLoggingVerbosity(CommandOption loggingVerbosityCommand, CommandOption unattendedCommand)
        {
            if (!loggingVerbosityCommand.HasValue())
            {
                return GetLoggingVerbosityBasedOnUnattendedCommand(unattendedCommand);
            }

            switch (loggingVerbosityCommand.Value())
            {
                case "none":
                    return LoggingVerbosityEnum.NoLogging;
                case "minimal":
                    return LoggingVerbosityEnum.MinimalLogging;
                case "full":
                    return LoggingVerbosityEnum.FullLogging;
                default:
                    return GetLoggingVerbosityBasedOnUnattendedCommand(unattendedCommand);
            }
        }

        private static LoggingVerbosityEnum GetLoggingVerbosityBasedOnUnattendedCommand(CommandOption unattendedCommand)
        {
            return unattendedCommand.HasValue()
                ? LoggingVerbosityEnum.MinimalLogging
                : LoggingVerbosityEnum.FullLogging;
        }

        private static LogDestinationEnum GetLogDestination(CommandOption loggingVerbosityCommand, CommandOption unattendedCommand, CommandOption logDestinationCommand)
        {
            if (GetLoggingVerbosity(loggingVerbosityCommand, unattendedCommand) == LoggingVerbosityEnum.NoLogging)
            {
                return LogDestinationEnum.DevNull;
            }

            if (!unattendedCommand.HasValue())
            {
                return LogDestinationEnum.File;
            }

            switch (logDestinationCommand.Value())
            {
                case "file":
                    return LogDestinationEnum.File;
                default:
                    return LogDestinationEnum.Console;
            }
        }
    }
}