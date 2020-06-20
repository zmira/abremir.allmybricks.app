using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
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
        public void UncompressAsset_ForStreamAndInvalidParameters_ReturnsFalse(bool validStream, bool validTargetFolder)
        {
            _assetUncompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(validTargetFolder ? FileAttributes.Directory : FileAttributes.Archive);
            _assetUncompression.Get<IDirectory>()
                .Exists(Arg.Any<string>())
                .Returns(!validTargetFolder);

            var result = _assetUncompression.ClassUnderTest.UncompressAsset(validStream ? new MemoryStream() : null, ".");

            Check.That(result).IsFalse();
        }

        [DataTestMethod]
        [DataRow(null, true, true)]
        [DataRow("", true, true)]
        [DataRow("test_file.txt", false, true)]
        [DataRow("test_file.txt", true, false)]
        public void UncompressAsset_ForFileAndInvalidParameters_ReturnsFalse(string sourceFilePath, bool sourceFileExists, bool sourceFileIsFile)
        {
            _assetUncompression.Get<IFile>()
                .Exists(Arg.Any<string>())
                .Returns(sourceFileExists);
            _assetUncompression.Get<IFile>()
                .GetAttributes(Arg.Any<string>())
                .Returns(sourceFileIsFile ? FileAttributes.Archive : FileAttributes.Directory);

            var result = _assetUncompression.ClassUnderTest.UncompressAsset(sourceFilePath, string.Empty);

            Check.That(result).IsFalse();
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

            Check.That(result).IsTrue();
            reader.Received()
                .WriteEntryTo(Arg.Any<Stream>());
        }
    }
}
