using LiteDB;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IRepositoryService
    {
        ILiteRepository GetRepository();
        long CompactRepository();
    }
}
