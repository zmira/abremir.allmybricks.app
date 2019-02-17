using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using SharpCompress.Common;
using SharpCompress.Writers.Tar;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Tests.Implementations
{
    [TestClass]
    public class AssetCompressionTests
    {
        private NSubstituteAutoMocker<AssetCompression> _assetCompression;

        [TestInitialize]
        public void TestInitialize()
        {
            _assetCompression = new NSubstituteAutoMocker<AssetCompression>();
        }

        [DataTestMethod]
        [DataRow("", true, null, false)]
        [DataRow(null, true, null, false)]
        [DataRow("origin_file_path.txt", false, null, false)]
        [DataRow("origin_file_path.txt", true, ".", false)]
        public void CompressAsset_InvalidParameters_ReturnsFalse(string originFilePath, bool originFileExists, string destinationFolderPath, bool destinationFolderPathIsFolder)
        {
            _assetCompression.Get<IFile>()
                .Exists(Arg.Any<string>())
                .Returns(originFileExists);
            _assetCompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(destinationFolderPathIsFolder ? FileAttributes.Directory : FileAttributes.Archive);

            var result = _assetCompression.ClassUnderTest.CompressAsset(originFilePath, destinationFolderPath);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CompressAsset_InvalidDestinationFilePath_ReturnsFalse()
        {
            var originFilePath = "test_file.txt";
            _assetCompression.Get<IFile>()
                .Exists(originFilePath)
                .Returns(true);
            _assetCompression.Get<IFile>()
                .Exists(AssetCompression.GetCompressedAssetFileName(originFilePath))
                .Returns(true);

            var result = _assetCompression.ClassUnderTest.CompressAsset(originFilePath, string.Empty, overwrite: false);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CompressAsset_ValidData_ReturnsTrue()
        {
            var originFilePath = "test_file.txt";
            _assetCompression.Get<IFile>()
                .Exists(originFilePath)
                .Returns(true);
            _assetCompression.Get<ITarWriter>()
                .CreateTarWriter(Arg.Any<Stream>(), Arg.Any<TarWriterOptions>())
                .Returns(Substitute.For<TarWriter>(new MemoryStream(), new TarWriterOptions(CompressionType.LZip, true)));

            var result = _assetCompression.ClassUnderTest.CompressAsset(originFilePath, string.Empty);

            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void GetCompressedAssetFileName_InvalidFilePath_ReturnsNull(string fileName)
        {
            var result = AssetCompression.GetCompressedAssetFileName(fileName);

            result.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(@"F:\test\this_is_a_file.txt")]
        [DataRow(@"C:\this_is_a_file")]
        public void GetCompressedAssetFileName_ValidFilePath_ReturnsNewFilename(string fileName)
        {
            var result = AssetCompression.GetCompressedAssetFileName(fileName);

            result.Should().NotBeNull();
            result.Should().Be("this_is_a_file.lz");
        }
    }
}
