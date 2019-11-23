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

            var unattendeOption = app
                .Option(DatabaseSeederConstants.UnattentedOption, "Run in unattended mode (non-interactive)", CommandOptionType.NoValue);
            var logVerbosityOption = app
                .Option<string>(DatabaseSeederConstants.LogVerbosityOption, $"Logging verbosity: {DatabaseSeederConstants.LogVerbosityValueNone}, {DatabaseSeederConstants.LogVerbosityValueMinimal}, {DatabaseSeederConstants.LogVerbosityValueFull}", CommandOptionType.SingleValue)
                .Accepts(builder => ValidationExtensions.Values(builder, true, DatabaseSeederConstants.LogVerbosityValueNone, DatabaseSeederConstants.LogVerbosityValueMinimal, DatabaseSeederConstants.LogVerbosityValueFull));
            var logDestinationOption = app
                .Option<string>(DatabaseSeederConstants.LogDestinationOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattentedOption}] Logging destination: {DatabaseSeederConstants.LogDestinationValueConsole}, {DatabaseSeederConstants.LogDestinationValueFile}", CommandOptionType.SingleValue)
                .Accepts(builder => ValidationExtensions.Values(builder, true, DatabaseSeederConstants.LogDestinationValueConsole, DatabaseSeederConstants.LogDestinationValueFile));
            var compressOption = app
                .Option(DatabaseSeederConstants.CompressOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattentedOption}] Compress the seeded database using LZip", CommandOptionType.NoValue);
            var datasetOption = app
                .Option<string>(DatabaseSeederConstants.DatasetOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattentedOption}] Select which dataset(s) to synchronize: {DatabaseSeederConstants.DatasetValueSets}, {DatabaseSeederConstants.DatasetValuePrimaryUsers}, {DatabaseSeederConstants.DatasetValueFriends}, {DatabaseSeederConstants.DatasetValueAll}, {DatabaseSeederConstants.DatasetValueNone}", CommandOptionType.MultipleValue)
                .Accepts(builder => ValidationExtensions.Values(builder, true, DatabaseSeederConstants.DatasetValueSets, DatabaseSeederConstants.DatasetValuePrimaryUsers, DatabaseSeederConstants.DatasetValueFriends, DatabaseSeederConstants.DatasetValueAll, DatabaseSeederConstants.DatasetValueNone));
            var dataFolderOption = app
                .Option<string>(DatabaseSeederConstants.DataFolderOption, "Override the default folder path for the data file", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                IoC.Configure();
                IoC.ConfigureOnboarding(Settings.OnboardingUrl);

                var logVerbosity = GetLogVerbosity(logVerbosityOption, unattendeOption);
                var logDestination = GetLogDestination(logVerbosityOption, unattendeOption, logDestinationOption);

                Logging.Configure(logDestination, logVerbosity);

                Logger = Logging.CreateLogger<Program>();

                if (logVerbosity != LogVerbosityEnum.NoLogging)
                {
                    Logger.LogInformation($"Running All My Bricks database seeder with arguments: { (unattendeOption.HasValue() ? $" { DatabaseSeederConstants.UnattentedOption }" : string.Empty) }{ (logVerbosityOption.HasValue() ? $" { DatabaseSeederConstants.LogVerbosityOption }={ logVerbosityOption.Value() }" : string.Empty) }{ (logDestinationOption.HasValue() ? $" { DatabaseSeederConstants.LogDestinationOption }={ logDestinationOption.Value() }" : string.Empty) }{ (compressOption.HasValue() ? $" { DatabaseSeederConstants.CompressOption }" : string.Empty) }{ (datasetOption.HasValue() ? $" { DatabaseSeederConstants.DatasetOption }={ string.Join(", ", datasetOption.Values) }" : string.Empty) }{ (dataFolderOption.HasValue() ? $" { DatabaseSeederConstants.DataFolderOption }={ dataFolderOption.Value() }" : string.Empty) }");
                }

                var folderOverride = dataFolderOption.HasValue()
                    ? dataFolderOption.Value()
                    : null;
                var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
                fileSystem.EnsureLocalDataFolder(folderOverride);

                if (unattendeOption.HasValue())
                {
                    NonInteractiveConsole.Run(datasetOption.Values, compressOption.HasValue()).GetAwaiter().GetResult();
                }
                else
                {
                    InteractiveConsole.Run();
                }
            });

            app.OnValidationError((validationResult) =>
            {
                Logger.LogError(validationResult.ErrorMessage);
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
                case DatabaseSeederConstants.LogVerbosityValueNone:
                    return LogVerbosityEnum.NoLogging;
                case DatabaseSeederConstants.LogVerbosityValueMinimal:
                    return LogVerbosityEnum.MinimalLogging;
                case DatabaseSeederConstants.LogVerbosityValueFull:
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
                case DatabaseSeederConstants.LogDestinationValueFile:
                    return LogDestinationEnum.File;
                default:
                    return LogDestinationEnum.Console;
            }
        }
    }
}
