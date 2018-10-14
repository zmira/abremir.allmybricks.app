using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.Device.Enumerations;
using abremir.AllMyBricks.Device.Interfaces;
using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Net;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class ThumbnailSynchronizerTests
    {
        private ThumbnailSynchronizer _thumbnailSynchronizer;
        private IPreferencesService _preferencesService;
        private IFileSystemService _fileSystemService;
        private HttpTest _httpTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpTest = new HttpTest();
            _preferencesService = Substitute.For<IPreferencesService>();
            _fileSystemService = Substitute.For<IFileSystemService>();

            _thumbnailSynchronizer = new ThumbnailSynchronizer(_preferencesService, _fileSystemService, Substitute.For<IDataSynchronizerEventManager>());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpTest.Dispose();
        }

        [TestMethod]
        public void Synchronize_InvalidCachingPreferences_HttpNotInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.NeverCache);

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>
                {
                    new Image
                    {
                        ThumbnailUrl = "THUMBNAIL_URL"
                    }
                }
            });

            _httpTest.ShouldNotHaveMadeACall();
        }

        [TestMethod]
        public void Synchronize_InvalidSet_HttpNotInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);

            _thumbnailSynchronizer.Synchronize(null);

            _httpTest.ShouldNotHaveMadeACall();
        }

        [TestMethod]
        public void Synchronize_NoImages_HttpNotInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>()
            });

            _httpTest.ShouldNotHaveMadeACall();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Synchronize_InvalidThumbnailUrl_HttpNotInvoked(string thumbnailUrl)
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>
                {
                    new Image
                    {
                        ThumbnailUrl = thumbnailUrl
                    }
                }
            });

            _httpTest.ShouldNotHaveMadeACall();
        }

        [TestMethod]
        public void Synchronize_InvalidImage_SaveThumbnailToCacheNotInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);
            _httpTest.RespondWith(string.Empty);

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>
                {
                    new Image
                    {
                        ThumbnailUrl = "http://www.url.com/thumbnails/thumbnail.jpg"
                    }
                }
            });

            _httpTest.ShouldHaveMadeACall();
            _fileSystemService.DidNotReceiveWithAnyArgs().SaveThumbnailToCache(null, null, null, null);
        }

        [TestMethod]
        public void Synchronize_NotOkResult_SaveThumbnailToCacheNotInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);
            _httpTest.RespondWith(status: (int)HttpStatusCode.NotFound);

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>
                {
                    new Image
                    {
                        ThumbnailUrl = "http://www.url.com/thumbnails/thumbnail.jpg"
                    }
                }
            });

            _httpTest.ShouldHaveMadeACall();
            _fileSystemService.DidNotReceiveWithAnyArgs().SaveThumbnailToCache(null, null, null, null);
        }

        [TestMethod]
        public void Synchronize_ValidImage_SaveThumbnailToCacheInvoked()
        {
            _preferencesService.ThumbnailCachingStrategy.Returns(ThumbnailCachingStrategyEnum.CacheAllThumbnailsWhenSynchronizing);
            _httpTest.RespondWith("THUMBNAIL_IMAGE");

            _thumbnailSynchronizer.Synchronize(new Set
            {
                Images = new List<Image>
                {
                    new Image
                    {
                        ThumbnailUrl = "http://www.url.com/thumbnails/thumbnail.jpg"
                    }
                },
                Theme = new Theme(),
                Subtheme = new Subtheme()
            });

            _httpTest.ShouldHaveMadeACall();
            _fileSystemService.ReceivedWithAnyArgs().SaveThumbnailToCache(null, null, null, null);
        }
    }
}