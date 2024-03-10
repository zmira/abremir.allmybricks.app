using System.Threading.Tasks;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IMigrationRunner
    {
        Task ApplyMigrations(ILiteDatabaseAsync liteDatabase);
    }
}
