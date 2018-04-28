using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Providers;
using LiteDB;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFilePathProvider _filePathProvider;

        public RepositoryService(IFilePathProvider filePathProvider)
        {
            _filePathProvider = filePathProvider;
        }

        public LiteRepository GetRepository()
        {
            var liteRepository = new LiteRepository(_filePathProvider.GetLocalPathToFile(Constants.AllMyBricksDbFile));

            SetupIndexes(liteRepository.Engine);

            return liteRepository;
        }

        public static void SetupIndexes(LiteEngine liteEngine)
        {
            if (liteEngine.UserVersion == 0)
            {
                liteEngine.EnsureIndex(nameof(Theme), "YearFrom");
                liteEngine.EnsureIndex(nameof(Theme), "YearTo");
                liteEngine.EnsureIndex(nameof(Subtheme), "YearFrom");
                liteEngine.EnsureIndex(nameof(Subtheme), "YearTo");
                liteEngine.EnsureIndex(nameof(Subtheme), "Theme.Name");
                liteEngine.EnsureIndex(nameof(ThemeYearCount), "Theme.Name");
                liteEngine.EnsureIndex(nameof(ThemeYearCount), "Year");

                liteEngine.UserVersion = 1;
            }
        }
    }
}