using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.Platform.Interfaces;
using LightInject;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    internal class Program
    {
        private static ILogger Logger;

        private static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption();

            var unattendeOption = app
                .Option(DatabaseSeederConstants.UnattendedOption, "Run in unattended mode (non-interactive)", CommandOptionType.NoValue);
            var logVerbosityOption = app
                .Option<string>(DatabaseSeederConstants.LogVerbosityOption, $"Logging verbosity: {DatabaseSeederConstants.LogVerbosityValueNone}, {DatabaseSeederConstants.LogVerbosityValueMinimal}, {DatabaseSeederConstants.LogVerbosityValueFull}", CommandOptionType.SingleValue)
                .Accepts(builder => builder.Values(true, DatabaseSeederConstants.LogVerbosityValueNone, DatabaseSeederConstants.LogVerbosityValueMinimal, DatabaseSeederConstants.LogVerbosityValueFull));
            var logDestinationOption = app
                .Option<string>(DatabaseSeederConstants.LogDestinationOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Logging destination: {DatabaseSeederConstants.LogDestinationValueConsole}, {DatabaseSeederConstants.LogDestinationValueFile}", CommandOptionType.MultipleValue)
                .Accepts(builder => builder.Values(true, DatabaseSeederConstants.LogDestinationValueConsole, DatabaseSeederConstants.LogDestinationValueFile));
            var compressOption = app
                .Option(DatabaseSeederConstants.CompressOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Compress the seeded database using LZip", CommandOptionType.NoValue);
            var uncompressOption = app
                .Option(DatabaseSeederConstants.UncompressOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Uncompress the seeded database using LZip", CommandOptionType.NoValue);
            var datasetOption = app
                .Option<string>(DatabaseSeederConstants.DatasetOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Select which dataset(s) to synchronize: {DatabaseSeederConstants.DatasetValueSets}, {DatabaseSeederConstants.DatasetValuePrimaryUsers}, {DatabaseSeederConstants.DatasetValueFriends}, {DatabaseSeederConstants.DatasetValueAll}, {DatabaseSeederConstants.DatasetValueNone}", CommandOptionType.MultipleValue)
                .Accepts(builder => builder.Values(true, DatabaseSeederConstants.DatasetValueSets, DatabaseSeederConstants.DatasetValuePrimaryUsers, DatabaseSeederConstants.DatasetValueFriends, DatabaseSeederConstants.DatasetValueAll, DatabaseSeederConstants.DatasetValueNone));
            var dataFolderOption = app
                .Option<string>(DatabaseSeederConstants.DataFolderOption, "Override the default folder path for the data file", CommandOptionType.SingleValue);
            var encryptedOption = app
                .Option(DatabaseSeederConstants.EncryptedOption, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Compressed file is encrypted", CommandOptionType.NoValue);
            var bricksetApiKeyOption = app
                .Option<string>(DatabaseSeederConstants.BricksetApiKey, $"[this option is only valid when run with {DatabaseSeederConstants.UnattendedOption}] Brickset API key", CommandOptionType.SingleValue);

            app.OnValidate(_ =>
            {
                if (unattendeOption.HasValue())
                {
                    if (uncompressOption.HasValue()
                        && datasetOption.HasValue()
                        && !datasetOption.Values.Contains(DatabaseSeederConstants.DatasetValueNone))
                    {
                        return new ValidationResult($"{DatabaseSeederConstants.DatasetOption} invalid with {DatabaseSeederConstants.UncompressOption}.");
                    }

                    if (datasetOption.HasValue()
                        && !datasetOption.Values.Contains(DatabaseSeederConstants.DatasetValueNone)
                        && !bricksetApiKeyOption.HasValue())
                    {
                        return new ValidationResult($"{DatabaseSeederConstants.BricksetApiKey} is required with {DatabaseSeederConstants.DatasetOption}");
                    }

                    if (compressOption.HasValue()
                        && uncompressOption.HasValue())
                    {
                        return new ValidationResult($"{DatabaseSeederConstants.CompressOption} invalid with {DatabaseSeederConstants.UncompressOption}.");
                    }
                }

                return ValidationResult.Success;
            });

            app.OnExecute(() =>
            {
                IoC.Configure();

                var logVerbosity = GetLogVerbosity(logVerbosityOption, unattendeOption);
                var logDestination = GetLogDestination(logVerbosityOption, unattendeOption, logDestinationOption);

                Logging.Configure(logDestination, logVerbosity);

                Logger = Logging.CreateLogger<Program>();

                var datasetOptions = datasetOption.Values;

                if (datasetOptions.Count == 0)
                {
                    datasetOptions.Add(DatabaseSeederConstants.DatasetValueNone);
                }

                if (logVerbosity != LogVerbosityEnum.NoLogging)
                {
                    Logger.LogInformation($"Running All My Bricks database seeder with arguments: { (unattendeOption.HasValue() ? $" { DatabaseSeederConstants.UnattendedOption }" : string.Empty) }{ (logVerbosityOption.HasValue() ? $" { DatabaseSeederConstants.LogVerbosityOption }={ logVerbosityOption.Value() }" : string.Empty) }{ (logDestinationOption.HasValue() ? $" { DatabaseSeederConstants.LogDestinationOption }={ string.Join(", ", logDestinationOption.Values) }" : string.Empty) }{ (compressOption.HasValue() ? $" { DatabaseSeederConstants.CompressOption }" : string.Empty) }{ (uncompressOption.HasValue() ? $" { DatabaseSeederConstants.UncompressOption }" : string.Empty) }{ $" { DatabaseSeederConstants.DatasetOption }={ string.Join(", ", datasetOptions) }" }{ (dataFolderOption.HasValue() ? $" { DatabaseSeederConstants.DataFolderOption }={ dataFolderOption.Value() }" : string.Empty) }{ (encryptedOption.HasValue() ? $" { DatabaseSeederConstants.EncryptedOption }" : string.Empty) }{ (bricksetApiKeyOption.HasValue() ? $" { DatabaseSeederConstants.BricksetApiKey }" : string.Empty) }");
                }

                var folderOverride = dataFolderOption.HasValue()
                    ? dataFolderOption.Value()
                    : null;
                var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
                fileSystem.EnsureLocalDataFolder(folderOverride);

                if (unattendeOption.HasValue())
                {
                    Settings.BricksetApiKey = bricksetApiKeyOption.Value() ?? string.Empty;

                    NonInteractiveConsole.Run(datasetOptions, compressOption.HasValue(), uncompressOption.HasValue(), encryptedOption.HasValue()).GetAwaiter().GetResult();
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

            return (logVerbosityCommand.Value()) switch
            {
                DatabaseSeederConstants.LogVerbosityValueNone => LogVerbosityEnum.NoLogging,
                DatabaseSeederConstants.LogVerbosityValueMinimal => LogVerbosityEnum.MinimalLogging,
                DatabaseSeederConstants.LogVerbosityValueFull => LogVerbosityEnum.FullLogging,
                _ => GetLogVerbosityBasedOnUnattendedCommand(unattendedCommand),
            };
        }

        private static LogVerbosityEnum GetLogVerbosityBasedOnUnattendedCommand(CommandOption unattendedCommand)
        {
            return unattendedCommand.HasValue()
                ? LogVerbosityEnum.MinimalLogging
                : LogVerbosityEnum.FullLogging;
        }

        private static LogDestinationEnum? GetLogDestination(CommandOption logVerbosityCommand, CommandOption unattendedCommand, CommandOption logDestinationCommand)
        {
            if (GetLogVerbosity(logVerbosityCommand, unattendedCommand) == LogVerbosityEnum.NoLogging)
            {
                return null;
            }

            if (!unattendedCommand.HasValue())
            {
                return LogDestinationEnum.File;
            }

            var logDestination = (LogDestinationEnum)0;

            if (logDestinationCommand.Values.Contains(DatabaseSeederConstants.LogDestinationValueFile))
            {
                logDestination |= LogDestinationEnum.File;
            }

            if (logDestinationCommand.Values.Contains(DatabaseSeederConstants.LogDestinationValueConsole)
                || logDestination == 0)
            {
                logDestination |= LogDestinationEnum.Console;
            }

            return logDestination;
        }
    }
}
