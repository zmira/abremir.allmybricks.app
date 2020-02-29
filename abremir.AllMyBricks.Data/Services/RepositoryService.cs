using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB;
using System.Linq;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFileSystemService _fileSystemService;

        public RepositoryService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public ILiteRepository GetRepository()
        {
            var repository = new LiteRepository(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile));

            SetupIndexes(repository.Database);

            return repository;
        }

        public long CompactRepository()
        {
            using (var repository = GetRepository())
            {
                return repository.Database.Rebuild();
            }
        }

        public static void SetupIndexes(ILiteDatabase liteDatabase)
        {
            if (liteDatabase.UserVersion == 0)
            {
                liteDatabase.GetCollection<Theme>().EnsureIndex(theme => theme.Name, true);
                liteDatabase.GetCollection<Theme>().EnsureIndex(theme => theme.YearFrom);
                liteDatabase.GetCollection<Theme>().EnsureIndex(theme => theme.YearTo);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.SubthemeKey, true);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.Name);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.Theme);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.YearFrom);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.YearTo);
                liteDatabase.GetCollection<ThemeGroup>().EnsureIndex(themeGroup => themeGroup.Value, true);
                liteDatabase.GetCollection<PackagingType>().EnsureIndex(packagingType => packagingType.Value, true);
                liteDatabase.GetCollection<Category>().EnsureIndex(category => category.Value, true);
                liteDatabase.GetCollection<Tag>().EnsureIndex(tag => tag.Value, true);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Number);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Name);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Ean);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Upc);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Theme);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Subtheme);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.ThemeGroup);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Category);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Year);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Tags.Select(tag => tag.Value));
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Prices.Select(price => price.Region));
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Prices.Select(price => price.Value));
                liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.UserType);

                liteDatabase.UserVersion = 1;
            }
        }
    }
}
