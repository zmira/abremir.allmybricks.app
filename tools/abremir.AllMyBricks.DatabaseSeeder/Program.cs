using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DatabaseSeeder.Handlers;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    internal class Program
    {
        private static ILogger Logger;

        private static async Task<int> Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddDatabaseSeederServices();
            using IHost host = builder.Build();

            var encryptedOption = new Option<bool?>(DatabaseSeederConstants.EncryptedOption, parseArgument: (result) =>
            {
                if (!result.Parent.Parent.Children.Any(child => ((OptionResult)child).Option.Name == DatabaseSeederConstants.BricksetApiKeyOption.Trim('-')))
                {
                    result.ErrorMessage = $"{DatabaseSeederConstants.EncryptedOption} invalid without {DatabaseSeederConstants.BricksetApiKeyOption}.";
                }

                return true;
            }, description: "Compressed file is encrypted");
            var logDestinationOption = new Option<LogDestination?>(DatabaseSeederConstants.LogDestinationOption, "Logging destination");
            var datasetOption = new Option<Dataset[]>(DatabaseSeederConstants.DatasetOption, "Dataset(s) to synchronize") { IsRequired = true };
            var dataFolderOption = new Option<string>(DatabaseSeederConstants.DataFolderOption, "Override the default folder path for the data file");
            var logVerbosityOption = new Option<LogVerbosity?>(DatabaseSeederConstants.LogVerbosityOption, "Log verbosity");

            static Option<string> GetBricksetApiKeyOption(bool required) => new(DatabaseSeederConstants.BricksetApiKeyOption, "Brickset API key") { IsRequired = required };

            var syncCommand = new Command(DatabaseSeederConstants.SyncCommand, "Synchronize database in non-interactive mode");
            syncCommand.AddOption(GetBricksetApiKeyOption(true));
            syncCommand.AddOption(datasetOption);
            syncCommand.AddOption(logDestinationOption);
            syncCommand.AddOption(logVerbosityOption);
            syncCommand.AddOption(dataFolderOption);

            syncCommand.SetHandler(async (context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var dataset = context.ParseResult.CommandResult.GetValueForOption(datasetOption);
                var logDestination = context.ParseResult.CommandResult.GetValueForOption(logDestinationOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);
                var bricksetApiKeyOption = ((OptionResult)context.ParseResult.CommandResult.Children.First(child => ((OptionResult)child).Option.Name == DatabaseSeederConstants.BricksetApiKeyOption.Trim('-'))).Option as Option<string>;
                string bricksetApiKey = context.ParseResult.CommandResult.GetValueForOption(bricksetApiKeyOption);

                var verbosity = GetLogVerbosity(true, logVerbosity);
                Logging.Configure(host, logDestination ?? LogDestination.Console, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks {DatabaseSeederConstants.SyncCommand} with arguments: {DatabaseSeederConstants.BricksetApiKeyOption}{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(logDestination.HasValue ? $" {DatabaseSeederConstants.LogDestinationOption}={logDestination.Value}" : string.Empty)}{$" {DatabaseSeederConstants.DatasetOption}={string.Join(", ", dataset)}"}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                Settings.BricksetApiKey = bricksetApiKey;

                await SyncHandler.Run(host, dataset);
            });

            var expandCommand = new Command(DatabaseSeederConstants.ExpandCommand, "Expand All My Bricks database");
            expandCommand.AddOption(encryptedOption);
            expandCommand.AddOption(GetBricksetApiKeyOption(false));
            expandCommand.AddOption(logDestinationOption);
            expandCommand.AddOption(logVerbosityOption);
            expandCommand.AddOption(dataFolderOption);

            expandCommand.SetHandler((context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var encrypted = context.ParseResult.CommandResult.GetValueForOption(encryptedOption);
                var bricksetApiKeyOption = ((OptionResult)context.ParseResult.CommandResult.Children.FirstOrDefault(child => ((OptionResult)child).Option.Name == DatabaseSeederConstants.BricksetApiKeyOption.Trim('-')))?.Option as Option<string>;
                string bricksetApiKey = null;
                if (bricksetApiKeyOption is not null)
                {
                    bricksetApiKey = context.ParseResult.CommandResult.GetValueForOption(bricksetApiKeyOption);
                }
                var logDestination = context.ParseResult.CommandResult.GetValueForOption(logDestinationOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);

                var verbosity = GetLogVerbosity(true, logVerbosity);
                Logging.Configure(host, logDestination ?? LogDestination.Console, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks {DatabaseSeederConstants.ExpandCommand} with arguments:{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(logDestination.HasValue ? $" {DatabaseSeederConstants.LogDestinationOption}={logDestination.Value}" : string.Empty)}{(encrypted.HasValue ? $" {DatabaseSeederConstants.EncryptedOption}" : string.Empty)}{(bricksetApiKeyOption is not null ? $" {DatabaseSeederConstants.BricksetApiKeyOption}" : string.Empty)}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                if (bricksetApiKeyOption is not null)
                {
                    Settings.BricksetApiKey = bricksetApiKey;
                }

                ExpandHandler.Run(host, encrypted ?? false);
            });

            var compressCommand = new Command(DatabaseSeederConstants.CompressCommand, "Compress All My Bricks database");
            compressCommand.AddOption(encryptedOption);
            compressCommand.AddOption(GetBricksetApiKeyOption(false));
            compressCommand.AddOption(logDestinationOption);
            compressCommand.AddOption(logVerbosityOption);
            compressCommand.AddOption(dataFolderOption);

            compressCommand.SetHandler((context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var encrypted = context.ParseResult.CommandResult.GetValueForOption(encryptedOption);
                var bricksetApiKeyOption = ((OptionResult)context.ParseResult.CommandResult.Children.FirstOrDefault(child => ((OptionResult)child).Option.Name == DatabaseSeederConstants.BricksetApiKeyOption.Trim('-')))?.Option as Option<string>;
                string bricksetApiKey = null;
                if (bricksetApiKeyOption is not null)
                {
                    bricksetApiKey = context.ParseResult.CommandResult.GetValueForOption(bricksetApiKeyOption);
                }
                var logDestination = context.ParseResult.CommandResult.GetValueForOption(logDestinationOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);

                var verbosity = GetLogVerbosity(true, logVerbosity);
                Logging.Configure(host, logDestination ?? LogDestination.Console, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks {DatabaseSeederConstants.CompressCommand} with arguments:{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(logDestination.HasValue ? $" {DatabaseSeederConstants.LogDestinationOption}={logDestination.Value}" : string.Empty)}{(encrypted.HasValue ? $" {DatabaseSeederConstants.EncryptedOption}" : string.Empty)}{(bricksetApiKeyOption is not null ? $" {DatabaseSeederConstants.BricksetApiKeyOption}" : string.Empty)}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                if (bricksetApiKeyOption is not null)
                {
                    Settings.BricksetApiKey = bricksetApiKey;
                }

                CompressHandler.Run(host, encrypted ?? false);
            });

            var compactCommand = new Command(DatabaseSeederConstants.CompactCommand, "Compact All My Bricks database");
            compactCommand.AddOption(logDestinationOption);
            compactCommand.AddOption(logVerbosityOption);
            compactCommand.AddOption(dataFolderOption);

            compactCommand.SetHandler(async (context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var logDestination = context.ParseResult.CommandResult.GetValueForOption(logDestinationOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);

                var verbosity = GetLogVerbosity(true, logVerbosity);
                Logging.Configure(host, logDestination ?? LogDestination.Console, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks {DatabaseSeederConstants.CompactCommand} with arguments:{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(logDestination.HasValue ? $" {DatabaseSeederConstants.LogDestinationOption}={logDestination.Value}" : string.Empty)}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                await CompactHandler.Run(host);
            });

            var sanitizeCommand = new Command(DatabaseSeederConstants.SanitizeCommand, "Sanitize All My Bricks database");
            sanitizeCommand.AddOption(GetBricksetApiKeyOption(true));
            sanitizeCommand.AddOption(logDestinationOption);
            sanitizeCommand.AddOption(logVerbosityOption);
            sanitizeCommand.AddOption(dataFolderOption);

            sanitizeCommand.SetHandler(async (context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var logDestination = context.ParseResult.CommandResult.GetValueForOption(logDestinationOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);
                var bricksetApiKeyOption = ((OptionResult)context.ParseResult.CommandResult.Children.First(child => ((OptionResult)child).Option.Name == DatabaseSeederConstants.BricksetApiKeyOption.Trim('-'))).Option as Option<string>;
                string bricksetApiKey = context.ParseResult.CommandResult.GetValueForOption(bricksetApiKeyOption);

                var verbosity = GetLogVerbosity(true, logVerbosity);
                Logging.Configure(host, logDestination ?? LogDestination.Console, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks {DatabaseSeederConstants.SanitizeCommand} with arguments:{DatabaseSeederConstants.BricksetApiKeyOption}{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(logDestination.HasValue ? $" {DatabaseSeederConstants.LogDestinationOption}={logDestination.Value}" : string.Empty)}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                await SanitizeHandler.Run(host);
            });

            var rootCommand = new RootCommand("All My Bricks database seeder");
            rootCommand.AddOption(dataFolderOption);
            rootCommand.AddOption(logVerbosityOption);
            rootCommand.AddCommand(syncCommand);
            rootCommand.AddCommand(compressCommand);
            rootCommand.AddCommand(expandCommand);
            rootCommand.AddCommand(compactCommand);
            rootCommand.AddCommand(sanitizeCommand);

            rootCommand.SetHandler((context) =>
            {
                var dataFolder = context.ParseResult.CommandResult.GetValueForOption(dataFolderOption);
                var logVerbosity = context.ParseResult.CommandResult.GetValueForOption(logVerbosityOption);

                var verbosity = GetLogVerbosity(false, logVerbosity);
                Logging.Configure(host, LogDestination.File, verbosity);

                Logger = Logging.CreateLogger<Program>();
                Logger.LogInformation($"Running All My Bricks database seeder with arguments:{(logVerbosity.HasValue ? $" {DatabaseSeederConstants.LogVerbosityOption}={logVerbosity.Value}" : string.Empty)}{(!string.IsNullOrWhiteSpace(dataFolder) ? $" {DatabaseSeederConstants.DataFolderOption}={dataFolder}" : string.Empty)}");

                ConfigureDataFolder(host, dataFolder);

                AppHandler.Run(host);
            });

            return await rootCommand.InvokeAsync(args);
        }

        private static LogVerbosity GetLogVerbosity(bool isUnattended, LogVerbosity? logVerbosity)
        {
            return logVerbosity
                ?? (isUnattended
                    ? LogVerbosity.Minimal
                    : LogVerbosity.Full);
        }

        private static void ConfigureDataFolder(IHost host, string dataFolder)
        {
            var folderOverride = !string.IsNullOrWhiteSpace(dataFolder)
                    ? dataFolder
                    : null;
            var fileSystem = host.Services.GetService<IFileSystemService>();
            fileSystem.EnsureLocalDataFolder(folderOverride);
        }
    }
}
