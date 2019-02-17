using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Device.Interfaces;
using Easy.MessageHub;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SharpCompress.Readers;
using System.IO;

namespace abremir.AllMyBricks.AssetManagement.Tests.Implementations
{
    [TestClass]
    public class AssetUncompressionTests
    {
        private IAssetUncompression _assetUncompression;
        private IFile _file;
        private IDirectory _directory;
        private IReaderFactory _readerFactory;
        private IMessageHub _messagehub;

        [TestInitialize]
        public void TestInitialize()
        {
            _file = Substitute.For<IFile>();
            _directory = Substitute.For<IDirectory>();
            _readerFactory = Substitute.For<IReaderFactory>();
            _messagehub = Substitute.For<IMessageHub>();

            _assetUncompression = new AssetUncompression(_file, _directory, _readerFactory, _messagehub);
        }

        [DataTestMethod]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void UncompressAsset_ForStreamAndInvalidParameters_ReturnsFalse(bool validStream, bool validDestinationFolder)
        {
            _file.GetAttributes(Arg.Any<string>()).Returns(validDestinationFolder ? FileAttributes.Directory : FileAttributes.Archive);

            var result = _assetUncompression.UncompressAsset(validStream ? new MemoryStream() : null, string.Empty);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null, true, true)]
        [DataRow("", true, true)]
        [DataRow("test_file.txt", false, true)]
        [DataRow("test_file.txt", true, false)]
        public void UncompressAsset_ForFileAndInvalidParameters_ReturnsFalse(string originFilePath, bool originFileExists, bool originFileIsFile)
        {
            _file.Exists(Arg.Any<string>()).Returns(originFileExists);
            _file.GetAttributes(Arg.Any<string>()).Returns(originFileIsFile ? FileAttributes.Archive : FileAttributes.Directory);

            var result = _assetUncompression.UncompressAsset(originFilePath, string.Empty);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void UncompressAsset_ValidParameters_ReturnsTrue()
        {
            _file.GetAttributes(Arg.Any<string>()).Returns(FileAttributes.Directory);
            _directory.Exists(Arg.Any<string>()).Returns(true);
            var reader = Substitute.For<IReader>();
            reader.MoveToNextEntry().Returns(true, false);
            reader.Entry.IsDirectory.Returns(false);
            reader.Entry.Key.Returns("test_file.txt");
            _readerFactory.Open(Arg.Any<Stream>()).Returns(reader);

            var result = _assetUncompression.UncompressAsset(new MemoryStream(), string.Empty);

            result.Should().BeTrue();
            reader.Received(1).WriteEntryTo(Arg.Any<Stream>());
        }
    }
}
