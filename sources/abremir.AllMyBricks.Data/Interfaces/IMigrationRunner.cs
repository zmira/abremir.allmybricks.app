using LiteDB;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IMigrationRunner
    {
        void ApplyMigrations(ILiteDatabase liteDatabase);
    }
}
