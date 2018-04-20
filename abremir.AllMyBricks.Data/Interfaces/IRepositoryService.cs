using LiteDB;

namespace abremir.AllMyBricks.Data.Interfaces
{
    public interface IRepositoryService
    {
        LiteRepository GetRepository();
    }
}