using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System.Linq;

namespace abremir.AllMyBricks.Data.Migrations
{
    public static class MigrationsAndIndexes_V0
    {
        public static void Apply(ILiteDatabase liteDatabase)
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
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Theme);
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Subtheme);
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.ThemeGroup);
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Category);
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Year);
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Barcodes.Select(barcode => barcode.Type));
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Barcodes.Select(barcode => barcode.Value));
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Tags.Select(tag => tag.Value));
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Prices.Select(price => price.Region));
            liteDatabase.GetCollection<Set>().EnsureIndex(set => set.Prices.Select(price => price.Value));
            liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.BricksetUsername, true);
            liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.UserType);
            liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.Sets.Select(set => set.Wanted));
            liteDatabase.GetCollection<BricksetUser>().EnsureIndex(bricksetUser => bricksetUser.Sets.Select(set => set.Owned));

            liteDatabase.UserVersion = 1;
        }
    }
}
