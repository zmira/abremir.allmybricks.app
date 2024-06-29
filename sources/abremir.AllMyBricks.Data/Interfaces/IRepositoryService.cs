using System.Threading.Tasks;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IRepositoryService
    {
        ILiteRepositoryAsync GetRepository();
        Task<long> CompactRepository();
    }
}
