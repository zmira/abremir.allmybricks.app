using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System.Linq;

namespace abremir.AllMyBricks.Data.Migrations
{
    public static class MigrationsAndIndexes_V1
    {
        public static void Apply(ILiteDatabase liteDatabase)
        {
            liteDatabase.RenameCollection(nameof(Set), nameof(Set_V1));

            var newSetList = liteDatabase.GetCollection<Set_V1>()
                .Include(set => set.Theme)
                .Include(set => set.ThemeGroup)
                .Include(set => set.Subtheme)
                .Include(set => set.PackagingType)
                .Include(set => set.Category)
                .Include(set => set.Tags)
                .FindAll()
                .Select(set => set.ToV2())
                .ToList();

            liteDatabase.DropCollection(nameof(Set_V1));

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

            liteDatabase.GetCollection<Set>().InsertBulk(newSetList);

            liteDatabase.Rebuild();

            liteDatabase.UserVersion = 2;
        }
    }
}
