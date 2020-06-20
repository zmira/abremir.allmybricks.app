using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.AssetManagement.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.AssetManagement.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container ??= new Container();

            container.Register<IAssetCompression, AssetCompression>(Lifestyle.Transient);
            container.Register<IAssetUncompression, AssetUncompression>(Lifestyle.Transient);
            container.Register<IAssetManagementService, AssetManagementService>(Lifestyle.Transient);
            container.Register<IFileStream, FileStreamImplementation>(Lifestyle.Transient);
            container.Register<ITarWriter, TarWriterImplementation>(Lifestyle.Transient);
            container.Register<IReaderFactory, ReaderFactoryImplementation>(Lifestyle.Transient);

            return Platform.Configuration.IoC.ConfigureIO(container);
        }
    }
}
