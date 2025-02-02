using abremir.AllMyBricks.Data.Configuration;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Services;
using abremir.AllMyBricks.Data.Tests.Shared.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using LiteDB.Async;
using LiteDB.Engine;
using NSubstitute;

namespace abremir.AllMyBricks.Data.Tests.Shared.Services
{
    public class TestRepositoryService : IRepositoryService, IMemoryRepositoryService
    {
        private TempStream? _tempStream;

        private readonly IFileSystemService _fileSystemService;
        private readonly string _filename;

        public TestRepositoryService(string filename)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

            _filename = filename;
            _fileSystemService = Substitute.For<IFileSystemService>();

            LiteDbConfiguration.Configure();
        }

        public ILiteRepositoryAsync GetRepository()
        {
            if (_tempStream is null)
            {
                _tempStream = new TempStream(_filename);

                _fileSystemService.GetStreamForLocalPathToFile(Arg.Any<string>(), Arg.Any<string>()).Returns(_tempStream);
            }

            var repositoryService = new RepositoryService(_fileSystemService, new MigrationRunner());

            return repositoryService.GetRepository();
        }

        public Task<long> CompactRepository()
        {
            throw new NotImplementedException();
        }

        public void ResetDatabase()
        {
            _tempStream?.Dispose();
            _tempStream = null;
        }
    }
}
