using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Services;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB.Async;
using LiteDB.Engine;
using NSubstitute;

namespace abremir.AllMyBricks.Data.Tests.Configuration
{
    public class TestRepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private TempStream _tempStream;

        private readonly IFileSystemService _fileSystemService;

        public TestRepositoryService()
        {
            _fileSystemService = Substitute.For<IFileSystemService>();

            LiteDbConfiguration.Configure();
        }

        public Task<long> CompactRepository()
        {
            throw new System.NotImplementedException();
        }

        public ILiteRepositoryAsync GetRepository()
        {
            if (_tempStream is null)
            {
                _tempStream = new TempStream("abremir.AllMyBricks.Data.Tests.litedb");

                _fileSystemService.GetStreamForLocalPathToFile(Arg.Any<string>(), Arg.Any<string>()).Returns(_tempStream);
            }

            var repositoryService = new RepositoryService(_fileSystemService, new MigrationRunner());

            return repositoryService.GetRepository();
        }

        public void ResetDatabase()
        {
            _tempStream?.Dispose();
            _tempStream = null;
        }
    }
}
