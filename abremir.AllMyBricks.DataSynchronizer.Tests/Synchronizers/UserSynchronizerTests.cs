using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Easy.MessageHub;
using fastJSON;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class UserSynchronizerTests : DataSynchronizerTestsBase
    {
        private static IBricksetUserRepository _bricksetUserRepository;
        private static IThemeRepository _themeRepository;
        private static ISubthemeRepository _subthemeRepository;
        private static ISetRepository _setRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _bricksetUserRepository = new BricksetUserRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
            _setRepository = new SetRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task SynchronizeBricksetPrimaryUser_UserSynchronizationTimestampNotSetAndDoesNotHaveRemoteSets_DoesNotUpdateLocalSetsAndUpdatesUserSynchronizationTimestamp()
        {
            var testUser = "TESTUSER";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, testUser);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets>());

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets>());

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(string.Empty, testUser, string.Empty);

            var user = _bricksetUserRepository.Get(testUser);

            user.Sets.Should().BeEmpty();
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetPrimaryUser_SynchronizationTimestampNotSetAndHasRemoteSets_UpdatesLocalSetsAndUpdatesUserSynchronizationTimestamp()
        {
            var apiKey = "APIKEY";
            var userHash = "USERHASH";
            var testUser = "TESTUSER";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, testUser);

            var themesList = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));

            var testSetOwned = setsList[0];
            testSetOwned.Owned = true;
            testSetOwned.QtyOwned = 2;

            var ownedTheme = themesList.First(theme => theme.Theme == testSetOwned.Theme).ToTheme();
            var ownedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetOwned.Theme && subtheme.Subtheme == testSetOwned.Subtheme).ToSubtheme();
            ownedSubtheme.Theme = ownedTheme;

            var ownedSet = testSetOwned.ToSet();
            ownedSet.Theme = ownedTheme;
            ownedSet.Subtheme = ownedSubtheme;

            var testSetWanted = setsList[1];
            testSetWanted.Wanted = true;

            var wantedTheme = themesList.First(theme => theme.Theme == testSetWanted.Theme).ToTheme();
            var wantedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetWanted.Theme && subtheme.Subtheme == testSetWanted.Subtheme).ToSubtheme();
            wantedSubtheme.Theme = wantedTheme;

            var wantedSet = testSetWanted.ToSet();
            wantedSet.Theme = wantedTheme;
            wantedSet.Subtheme = wantedSubtheme;

            _themeRepository.AddOrUpdate(ownedTheme);
            _subthemeRepository.AddOrUpdate(ownedSubtheme);
            _setRepository.AddOrUpdate(ownedSet);

            _themeRepository.AddOrUpdate(wantedTheme);
            _subthemeRepository.AddOrUpdate(wantedSubtheme);
            _setRepository.AddOrUpdate(wantedSet);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets> { testSetOwned });

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets> { testSetWanted });

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, testUser, userHash);

            var user = _bricksetUserRepository.Get(testUser);

            user.Sets.Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == testSetOwned.SetId && userSet.Owned == testSetOwned.Owned && userSet.QuantityOwned == testSetOwned.QtyOwned).Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == testSetWanted.SetId && userSet.Wanted == testSetWanted.Wanted).Should().NotBeEmpty();
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetPrimaryUser_SynchronizationTimestampSetAndDoesNotHaveSetsToUpdate_DoesNotUpdateRemoteCollection()
        {
            var apiKey = "APIKEY";
            var userHash = "USERHASH";
            var testUser = "TESTUSER";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, testUser);

            var themesList = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));

            var testSetOwned = setsList[0];
            var ownedTheme = themesList.First(theme => theme.Theme == testSetOwned.Theme).ToTheme();
            var ownedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetOwned.Theme && subtheme.Subtheme == testSetOwned.Subtheme).ToSubtheme();
            ownedSubtheme.Theme = ownedTheme;

            var ownedSet = testSetOwned.ToSet();
            ownedSet.Theme = ownedTheme;
            ownedSet.Subtheme = ownedSubtheme;

            _themeRepository.AddOrUpdate(ownedTheme);
            _subthemeRepository.AddOrUpdate(ownedSubtheme);
            _setRepository.AddOrUpdate(ownedSet);

            var bricksetUserSet = new BricksetUserSet
            {
                SetId = ownedSet.SetId,
                Owned = true,
                QuantityOwned = 2
            };

            _bricksetUserRepository.AddOrUpdateSet(testUser, bricksetUserSet);

            _bricksetUserRepository.UpdateUserSynchronizationTimestamp(testUser, DateTimeOffset.Now.AddSeconds(1));

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets>());

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets>());

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, testUser, userHash);

            var user = _bricksetUserRepository.Get(testUser);

            await bricksetApiService.DidNotReceive().SetCollection(Arg.Any<ParameterSetCollection>());
            user.Sets.Should().NotBeEmpty();
            user.Sets.Count.Should().Be(1);
            user.Sets[0].SetId.Should().Be(bricksetUserSet.SetId);
            user.Sets[0].QuantityOwned.Should().Be(bricksetUserSet.QuantityOwned);
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetPrimaryUser_SynchronizationTimestampSetAndHasSetsToUpdate_UpdatesRemoteCollection()
        {
            var apiKey = "APIKEY";
            var userHash = "USERHASH";
            var testUser = "TESTUSER";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, testUser);

            var themesList = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));

            var testSetOwned = setsList[0];
            var ownedTheme = themesList.First(theme => theme.Theme == testSetOwned.Theme).ToTheme();
            var ownedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetOwned.Theme && subtheme.Subtheme == testSetOwned.Subtheme).ToSubtheme();
            ownedSubtheme.Theme = ownedTheme;

            var ownedSet = testSetOwned.ToSet();
            ownedSet.Theme = ownedTheme;
            ownedSet.Subtheme = ownedSubtheme;

            _themeRepository.AddOrUpdate(ownedTheme);
            _subthemeRepository.AddOrUpdate(ownedSubtheme);
            _setRepository.AddOrUpdate(ownedSet);

            _bricksetUserRepository.AddOrUpdateSet(testUser, new BricksetUserSet
            {
                SetId = ownedSet.SetId,
                Owned = true,
                QuantityOwned = 2
            });

            _bricksetUserRepository.UpdateUserSynchronizationTimestamp(testUser, DateTimeOffset.Now.AddSeconds(-1));

            var bricksetUserSet = new BricksetUserSet
            {
                SetId = ownedSet.SetId,
                Owned = true,
                QuantityOwned = 1
            };

            _bricksetUserRepository.AddOrUpdateSet(testUser, bricksetUserSet);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets>());

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets>());

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, testUser, userHash);

            var user = _bricksetUserRepository.Get(testUser);

            await bricksetApiService.Received().SetCollection(Arg.Any<ParameterSetCollection>());
            user.Sets.Should().NotBeEmpty();
            user.Sets.Count.Should().Be(1);
            user.Sets[0].SetId.Should().Be(bricksetUserSet.SetId);
            user.Sets[0].QuantityOwned.Should().Be(bricksetUserSet.QuantityOwned);
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetPrimaryUser_SynchronizationTimestampSetAndHasSetsToUpdateAndHasNewRemoteSets_UpdatesRemoteCollectionAndAddRemoteSetToLocalExceptAlreadyLocalSets()
        {
            var apiKey = "APIKEY";
            var userHash = "USERHASH";
            var testUser = "TESTUSER";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Primary, testUser);

            var themesList = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));

            var testSetOwned = setsList[0];
            var ownedTheme = themesList.First(theme => theme.Theme == testSetOwned.Theme).ToTheme();
            var ownedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetOwned.Theme && subtheme.Subtheme == testSetOwned.Subtheme).ToSubtheme();
            ownedSubtheme.Theme = ownedTheme;

            var ownedSet = testSetOwned.ToSet();
            ownedSet.Theme = ownedTheme;
            ownedSet.Subtheme = ownedSubtheme;

            var testSetWanted = setsList[1];
            testSetWanted.Wanted = true;

            var wantedTheme = themesList.First(theme => theme.Theme == testSetWanted.Theme).ToTheme();
            var wantedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetWanted.Theme && subtheme.Subtheme == testSetWanted.Subtheme).ToSubtheme();
            wantedSubtheme.Theme = wantedTheme;

            var wantedSet = testSetWanted.ToSet();
            wantedSet.Theme = wantedTheme;
            wantedSet.Subtheme = wantedSubtheme;

            _themeRepository.AddOrUpdate(ownedTheme);
            _subthemeRepository.AddOrUpdate(ownedSubtheme);
            _setRepository.AddOrUpdate(ownedSet);

            _themeRepository.AddOrUpdate(wantedTheme);
            _subthemeRepository.AddOrUpdate(wantedSubtheme);
            _setRepository.AddOrUpdate(wantedSet);

            _bricksetUserRepository.AddOrUpdateSet(testUser, new BricksetUserSet
            {
                SetId = ownedSet.SetId,
                Owned = true,
                QuantityOwned = 2
            });

            _bricksetUserRepository.UpdateUserSynchronizationTimestamp(testUser, DateTimeOffset.Now.AddSeconds(-1));

            var bricksetUserSet = new BricksetUserSet
            {
                SetId = ownedSet.SetId,
                Owned = true,
                QuantityOwned = 1
            };

            _bricksetUserRepository.AddOrUpdateSet(testUser, bricksetUserSet);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets>());

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets> { testSetWanted });

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, testUser, userHash);

            var user = _bricksetUserRepository.Get(testUser);

            await bricksetApiService.Received().SetCollection(Arg.Any<ParameterSetCollection>());
            user.Sets.Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == bricksetUserSet.SetId && userSet.Owned == bricksetUserSet.Owned && userSet.QuantityOwned == bricksetUserSet.QuantityOwned).Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == testSetWanted.SetId && userSet.Wanted == testSetWanted.Wanted).Should().NotBeEmpty();
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetFriend_DoesNotHaveRemoteSets_DoesNotUpdateLocalSetsAndUpdatesUserSynchronizationTimestamp()
        {
            var testUser = "TESTFRIEND";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Friend, testUser);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets>());

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets>());

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(string.Empty, testUser, string.Empty);

            var user = _bricksetUserRepository.Get(testUser);

            user.Sets.Should().BeEmpty();
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public async Task SynchronizeBricksetFriend_HasRemoteSets_UpdatesLocalSetsAndUpdatesUserSynchronizationTimestamp()
        {
            var apiKey = "APIKEY";
            var userHash = "USERHASH";
            var testUser = "TESTFRIEND";
            _bricksetUserRepository.Add(BricksetUserTypeEnum.Friend, testUser);

            var themesList = JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var subthemesList = JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));

            var testSetOwned = setsList[0];
            testSetOwned.Owned = true;
            testSetOwned.QtyOwned = 2;

            var ownedTheme = themesList.First(theme => theme.Theme == testSetOwned.Theme).ToTheme();
            var ownedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetOwned.Theme && subtheme.Subtheme == testSetOwned.Subtheme).ToSubtheme();
            ownedSubtheme.Theme = ownedTheme;

            var ownedSet = testSetOwned.ToSet();
            ownedSet.Theme = ownedTheme;
            ownedSet.Subtheme = ownedSubtheme;

            var testSetWanted = setsList[1];
            testSetWanted.Wanted = true;

            var wantedTheme = themesList.First(theme => theme.Theme == testSetWanted.Theme).ToTheme();
            var wantedSubtheme = subthemesList.First(subtheme => subtheme.Theme == testSetWanted.Theme && subtheme.Subtheme == testSetWanted.Subtheme).ToSubtheme();
            wantedSubtheme.Theme = wantedTheme;

            var wantedSet = testSetWanted.ToSet();
            wantedSet.Theme = wantedTheme;
            wantedSet.Subtheme = wantedSubtheme;

            _themeRepository.AddOrUpdate(ownedTheme);
            _subthemeRepository.AddOrUpdate(ownedSubtheme);
            _setRepository.AddOrUpdate(ownedSet);

            _themeRepository.AddOrUpdate(wantedTheme);
            _subthemeRepository.AddOrUpdate(wantedSubtheme);
            _setRepository.AddOrUpdate(wantedSet);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Owned == "1"))
                .Returns(new List<Sets> { testSetOwned });

            bricksetApiService
                .GetSets(Arg.Is<ParameterSets>(parameter => parameter.Wanted == "1"))
                .Returns(new List<Sets> { testSetWanted });

            var userSynchronizer = CreateTarget(bricksetApiService);

            await userSynchronizer.SynchronizeBricksetPrimaryUser(apiKey, testUser, userHash);

            var user = _bricksetUserRepository.Get(testUser);

            user.Sets.Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == testSetOwned.SetId && userSet.Owned == testSetOwned.Owned && userSet.QuantityOwned == testSetOwned.QtyOwned).Should().NotBeEmpty();
            user.Sets.Where(userSet => userSet.SetId == testSetWanted.SetId && userSet.Wanted == testSetWanted.Wanted).Should().NotBeEmpty();
            user.UserSynchronizationTimestamp.HasValue.Should().BeTrue();
        }

        private UserSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService = bricksetApiService ?? Substitute.For<IBricksetApiService>();

            return new UserSynchronizer(bricksetApiService, _bricksetUserRepository, Substitute.For<IMessageHub>());
        }
    }
}
