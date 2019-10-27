using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstituteAutoMocker.Standard;
using SharpCompress.Readers;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Tests.Implementations
{
    [TestClass]
    public class AssetUncompressionTests
    {
        private NSubstituteAutoMocker<AssetUncompression> _assetUncompression;

        [TestInitialize]
        public void TestInitialize()
        {
            _assetUncompression = new NSubstituteAutoMocker<AssetUncompression>();
        }

        [DataTestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void UncompressAsset_ForStreamAndInvalidParameters_ReturnsFalse(bool validStream, bool validDestinationFolder)
        {
            _assetUncompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(validDestinationFolder ? FileAttributes.Directory : FileAttributes.Archive);

            var result = _assetUncompression.ClassUnderTest.UncompressAsset(validStream ? new MemoryStream() : null, string.Empty);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null, true, true)]
        [DataRow("", true, true)]
        [DataRow("test_file.txt", false, true)]
        [DataRow("test_file.txt", true, false)]
        public void UncompressAsset_ForFileAndInvalidParameters_ReturnsFalse(string originFilePath, bool originFileExists, bool originFileIsFile)
        {
            _assetUncompression.Get<IFile>()
                .Exists(Arg.Any<string>())
                .Returns(originFileExists);
            _assetUncompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(originFileIsFile ? FileAttributes.Archive : FileAttributes.Directory);

            var result = _assetUncompression.ClassUnderTest.UncompressAsset(originFilePath, string.Empty);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void UncompressAsset_ValidParameters_ReturnsTrue()
        {
            _assetUncompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(FileAttributes.Directory);
            _assetUncompression.Get<IDirectory>()
                .Exists(Arg.Any<string>())
                .Returns(true);

            var reader = Substitute.For<IReader>();
            reader
                .MoveToNextEntry()
                .Returns(true, false);
            reader.Entry
                .IsDirectory
                .Returns(false);
            reader.Entry
                .Key
                .Returns("test_file.txt");

            _assetUncompression.Get<IReaderFactory>()
                .Open(Arg.Any<Stream>())
                .Returns(reader);

            var result = _assetUncompression.ClassUnderTest.UncompressAsset(new MemoryStream(), string.Empty);

            result.Should().BeTrue();
            reader.Received().WriteEntryTo(Arg.Any<Stream>());
        }
    }
}
