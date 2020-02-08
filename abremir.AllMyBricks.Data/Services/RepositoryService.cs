using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IFileSystemService _fileSystemService;

        public RepositoryService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public LiteRepository GetRepository()
        {
            var repository = new LiteRepository(_fileSystemService.GetLocalPathToFile(Constants.AllMyBricksDbFile));

            SetupIndexes(repository.Database);

            return repository;
        }

        public long CompactRepository()
        {
            return GetRepository().Database.Rebuild();
        }

        private void SetupIndexes(ILiteDatabase liteDatabase)
        {
            if (liteDatabase.UserVersion == 0)
            {
                liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.UserType);
                liteDatabase.GetCollection<Price>().EnsureIndex(price => price.Region);
                liteDatabase.GetCollection<RatingItem>().EnsureIndex(ratingItem => ratingItem.Type);
                liteDatabase.GetCollection<RatingItem>().EnsureIndex(ratingItem => ratingItem.Value);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Number);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Name);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Description);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Ean);
                liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Upc);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.YearFrom);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.YearTo);
                liteDatabase.GetCollection<Subtheme>().EnsureIndex(subtheme => subtheme.Name);
                liteDatabase.GetCollection<Theme>().EnsureIndex(theme => theme.YearFrom);
                liteDatabase.GetCollection<Theme>().EnsureIndex(theme => theme.YearTo);
                liteDatabase.GetCollection<YearSetCount>().EnsureIndex(yearSetCount => yearSetCount.Year);

                liteDatabase.UserVersion = 1;
            }
        }
    }
}
