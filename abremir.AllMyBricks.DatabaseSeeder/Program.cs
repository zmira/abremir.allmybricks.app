using System.ComponentModel.DataAnnotations;
using System.Linq;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.Platform.Interfaces;
using LightInject;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

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

                    if (encryptedOption.HasValue()
                        && !bricksetApiKeyOption.HasValue())
                    {
                        return new ValidationResult($"{DatabaseSeederConstants.EncryptedOption} invalid without {DatabaseSeederConstants.BricksetApiKey}.");
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

                var datasetOptions = datasetOption.Values.ToList();

                if (datasetOptions.Count == 0)
                {
                    datasetOptions.Add(DatabaseSeederConstants.DatasetValueNone);
                }

                if (logVerbosity != LogVerbosity.NoLogging)
                {
                    Logger.LogInformation($"Running All My Bricks database seeder with arguments: {(unattendeOption.HasValue() ? $" {DatabaseSeederConstants.UnattendedOption}" : string.Empty)}{(logVerbosityOption.HasValue() ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosityOption.Value()}" : string.Empty)}{(logDestinationOption.HasValue() ? $" {DatabaseSeederConstants.LogDestinationOption}={string.Join(", ", logDestinationOption.Values)}" : string.Empty)}{(compressOption.HasValue() ? $" {DatabaseSeederConstants.CompressOption}" : string.Empty)}{(uncompressOption.HasValue() ? $" {DatabaseSeederConstants.UncompressOption}" : string.Empty)}{$" {DatabaseSeederConstants.DatasetOption}={string.Join(", ", datasetOptions)}"}{(dataFolderOption.HasValue() ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolderOption.Value()}" : string.Empty)}{(encryptedOption.HasValue() ? $" {DatabaseSeederConstants.EncryptedOption}" : string.Empty)}{(bricksetApiKeyOption.HasValue() ? $" {DatabaseSeederConstants.BricksetApiKey}" : string.Empty)}");
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

        private static LogVerbosity GetLogVerbosity(CommandOption logVerbosityCommand, CommandOption unattendedCommand)
        {
            if (!logVerbosityCommand.HasValue())
            {
                return GetLogVerbosityBasedOnUnattendedCommand(unattendedCommand);
            }

            return logVerbosityCommand.Value() switch
            {
                DatabaseSeederConstants.LogVerbosityValueNone => LogVerbosity.NoLogging,
                DatabaseSeederConstants.LogVerbosityValueMinimal => LogVerbosity.MinimalLogging,
                DatabaseSeederConstants.LogVerbosityValueFull => LogVerbosity.FullLogging,
                _ => GetLogVerbosityBasedOnUnattendedCommand(unattendedCommand),
            };
        }

        private static LogVerbosity GetLogVerbosityBasedOnUnattendedCommand(CommandOption unattendedCommand)
        {
            return unattendedCommand.HasValue()
                ? LogVerbosity.MinimalLogging
                : LogVerbosity.FullLogging;
        }

        private static LogDestinations? GetLogDestination(CommandOption logVerbosityCommand, CommandOption unattendedCommand, CommandOption logDestinationCommand)
        {
            if (GetLogVerbosity(logVerbosityCommand, unattendedCommand) == LogVerbosity.NoLogging)
            {
                return null;
            }

            if (!unattendedCommand.HasValue())
            {
                return LogDestinations.File;
            }

            var logDestination = (LogDestinations)0;

            if (logDestinationCommand.Values.Contains(DatabaseSeederConstants.LogDestinationValueFile))
            {
                logDestination |= LogDestinations.File;
            }

            if (logDestinationCommand.Values.Contains(DatabaseSeederConstants.LogDestinationValueConsole)
                || logDestination == 0)
            {
                logDestination |= LogDestinations.Console;
            }

            return logDestination;
        }
    }
}
