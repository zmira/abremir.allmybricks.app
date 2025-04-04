using System.Threading.Tasks;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Interfaces
{
    internal interface IMigration
    {
        int MigrationId { get; }

        Task Apply(ILiteDatabaseAsync liteDatabase);
    }
}
