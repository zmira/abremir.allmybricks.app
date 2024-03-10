using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Migrations
{
    internal class _1_SetupIndexes : IMigration
    {
        public int MigrationId => 1;

        public Task Apply(ILiteDatabaseAsync liteDatabase)
        {
            Task<bool>[] liteDatabaseIndexes = [
                liteDatabase.GetCollection<Theme>().EnsureIndexAsync(theme => theme.Name, true),
                liteDatabase.GetCollection<Theme>().EnsureIndexAsync(theme => theme.YearFrom),
                liteDatabase.GetCollection<Theme>().EnsureIndexAsync(theme => theme.YearTo),
                liteDatabase.GetCollection<Subtheme>().EnsureIndexAsync(subtheme => subtheme.SubthemeKey, true),
                liteDatabase.GetCollection<Subtheme>().EnsureIndexAsync(subtheme => subtheme.Name),
                liteDatabase.GetCollection<Subtheme>().EnsureIndexAsync(subtheme => subtheme.Theme),
                liteDatabase.GetCollection<Subtheme>().EnsureIndexAsync(subtheme => subtheme.YearFrom),
                liteDatabase.GetCollection<Subtheme>().EnsureIndexAsync(subtheme => subtheme.YearTo),
                liteDatabase.GetCollection<ThemeGroup>().EnsureIndexAsync(themeGroup => themeGroup.Value, true),
                liteDatabase.GetCollection<PackagingType>().EnsureIndexAsync(packagingType => packagingType.Value, true),
                liteDatabase.GetCollection<Category>().EnsureIndexAsync(category => category.Value, true),
                liteDatabase.GetCollection<Tag>().EnsureIndexAsync(tag => tag.Value, true),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Number),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Name),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Theme),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Subtheme),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.ThemeGroup),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Category),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Year),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Barcodes.Select(barcode => barcode.Type)),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Barcodes.Select(barcode => barcode.Value)),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Prices.Select(price => price.Region)),
                liteDatabase.GetCollection<Set>().EnsureIndexAsync(set => set.Prices.Select(price => price.Value)),
                liteDatabase.GetCollection<BricksetUser>().EnsureIndexAsync(bricksetUser => bricksetUser.BricksetUsername, true),
                liteDatabase.GetCollection<BricksetUser>().EnsureIndexAsync(bricksetUser => bricksetUser.UserType),
                liteDatabase.GetCollection<BricksetUser>().EnsureIndexAsync(bricksetUser => bricksetUser.Sets.Select(set => set.Wanted)),
                liteDatabase.GetCollection<BricksetUser>().EnsureIndexAsync(bricksetUser => bricksetUser.Sets.Select(set => set.Owned))
            ];

            Task.WaitAll(liteDatabaseIndexes);

            liteDatabase.UserVersion++;

            return Task.CompletedTask;
        }
    }
}
