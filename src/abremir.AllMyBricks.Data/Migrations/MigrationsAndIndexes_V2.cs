using LiteDB;

namespace abremir.AllMyBricks.Data.Migrations
{
    public static class MigrationsAndIndexes_V2
    {
        public static void Apply(ILiteDatabase liteDatabase)
        {
            liteDatabase.Rebuild();

            liteDatabase.UserVersion = 3;
        }
    }
}
