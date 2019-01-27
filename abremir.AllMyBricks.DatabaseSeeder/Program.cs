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
            var logVerbosityCommand = app.Option("--log-verbosity", "Logging verbosity: none, minimal or full", CommandOptionType.SingleValue);
            var logDestinationCommand = app.Option("--log-destination", "[this option is only valid when run with --unattended] Logging destination: console or file", CommandOptionType.SingleValue);
            var distributionFileCommand = app.Option("--distribution-file", "[this option is only valid when run with --unattended] Prepare distribution file by LZip compresing the database file", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                IoC.Configure();
                IoC.ConfigureOnboarding(Settings.OnboardingUrl);

                var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
                fileSystem.EnsureLocalDataFolder();

                var logVerbosity = GetLogVerbosity(logVerbosityCommand, unattendedCommand);
                var logDestination = GetLogDestination(logVerbosityCommand, unattendedCommand, logDestinationCommand);

                Logging.Configure(logDestination, logVerbosity);

                Logger = Logging.CreateLogger<Program>();

                if (logVerbosity != LogVerbosityEnum.NoLogging)
                {
                    Logger.LogInformation($"Running All My Bricks database seeder with arguments: { (unattendedCommand.HasValue() ? "--unattended" : string.Empty)} { (logVerbosityCommand.HasValue() ? $"--logging-verbosity={logVerbosityCommand.Value()}" : string.Empty) } { (logDestinationCommand.HasValue() ? $"--log-destination={logDestinationCommand.Value()}" : string.Empty) }");
                }

                if (unattendedCommand.HasValue())
                {
                    NonInteractiveConsole.Run(distributionFileCommand.HasValue()).GetAwaiter().GetResult();
                }
                else
                {
                    InteractiveConsole.Run();
                }
            });

            return app.Execute(args);
        }

        private static LogVerbosityEnum GetLogVerbosity(CommandOption logVerbosityCommand, CommandOption unattendedCommand)
        {
            if (!logVerbosityCommand.HasValue())
            {
                return GetLogVerbosityBasedOnUnattendedCommand(unattendedCommand);
            }

            switch (logVerbosityCommand.Value())
            {
                case "none":
                    return LogVerbosityEnum.NoLogging;
                case "minimal":
                    return LogVerbosityEnum.MinimalLogging;
                case "full":
                    return LogVerbosityEnum.FullLogging;
                default:
                    return GetLogVerbosityBasedOnUnattendedCommand(unattendedCommand);
            }
        }

        private static LogVerbosityEnum GetLogVerbosityBasedOnUnattendedCommand(CommandOption unattendedCommand)
        {
            return unattendedCommand.HasValue()
                ? LogVerbosityEnum.MinimalLogging
                : LogVerbosityEnum.FullLogging;
        }

        private static LogDestinationEnum GetLogDestination(CommandOption logVerbosityCommand, CommandOption unattendedCommand, CommandOption logDestinationCommand)
        {
            if (GetLogVerbosity(logVerbosityCommand, unattendedCommand) == LogVerbosityEnum.NoLogging)
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