using LiteDB;

namespace abremir.AllMyBricks.Data.Interfaces
{
    internal interface IMigration
    {
        int MigrationId { get; }

        void Apply(ILiteDatabase liteDatabase);
    }
}
