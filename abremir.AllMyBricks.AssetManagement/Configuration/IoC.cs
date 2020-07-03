using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.AssetManagement.Services;
using LightInject;

namespace abremir.AllMyBricks.AssetManagement.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IAssetCompression, AssetCompression>();
            container.Register<IAssetUncompression, AssetUncompression>();
            container.Register<IAssetManagementService, AssetManagementService>();
            container.Register<IFileStream, FileStreamImplementation>();
            container.Register<ITarWriter, TarWriterImplementation>();
            container.Register<IReaderFactory, ReaderFactoryImplementation>();

            return Platform.Configuration.IoC.ConfigureIO(container);
        }
    }
}
