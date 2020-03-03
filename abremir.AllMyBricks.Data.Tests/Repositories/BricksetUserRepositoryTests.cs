using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class BricksetUserRepositoryTests : DataTestsBase
    {
        private static IBricksetUserRepository _bricksetUserRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _bricksetUserRepository = new BricksetUserRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Add_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.Add(BricksetUserTypeEnum.None, username);

            bricksetUser.Should().BeNull();
        }

        [TestMethod]
        public void Add_BricksetUserAlreadyExists_ReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Add(bricksetUserUnderTest.UserType, bricksetUserUnderTest.BricksetUsername);

            bricksetUser.BricksetUsername.Should().Be(bricksetUserUnderTest.BricksetUsername);
        }

        [TestMethod]
        public void Add_BricksetUserDoesNotExist_InsertsAndReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            const string newUsername = "new username";

            var bricksetUser = _bricksetUserRepository.Add(bricksetUserUnderTest.UserType, newUsername);

            bricksetUser.BricksetUsername.Should().Be(newUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Get_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.Get(username);

            bricksetUser.Should().BeNull();
        }

        [TestMethod]
        public void Get_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Get("username");

            bricksetUser.Should().BeNull();
        }

        [TestMethod]
        public void Get_BricksetUserExists_ReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Get(bricksetUserUnderTest.BricksetUsername);

            bricksetUser.BricksetUsername.Should().Be(bricksetUserUnderTest.BricksetUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Exists_InvalidUsername_ReturnsFalse(string username)
        {
            var bricksetUserExists = _bricksetUserRepository.Exists(username);

            bricksetUserExists.Should().BeFalse();
        }

        [TestMethod]
        public void Exists_ValidUsernameAndDoesNotExist_ReturnsFalse()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUserExists = _bricksetUserRepository.Exists(Guid.NewGuid().ToString());

            bricksetUserExists.Should().BeFalse();
        }

        [TestMethod]
        public void Exists_ValidUsernameAndExists_ReturnsTrue()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUserExists = _bricksetUserRepository.Exists(bricksetUserUnderTest.BricksetUsername);

            bricksetUserExists.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Remove_InvalidUsername_ReturnsFalse(string username)
        {
            var removedUser = _bricksetUserRepository.Remove(username);

            removedUser.Should().BeFalse();
        }

        [TestMethod]
        public void Remove_ValidUsernameAndDoesNotExist_ReturnsFalse()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var removedUser = _bricksetUserRepository.Remove(Guid.NewGuid().ToString());

            removedUser.Should().BeFalse();
        }

        [TestMethod]
        public void Remove_ValidUsernameAndExists_ReturnsTrueAndRemovesUser()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();
            string bricksetUsernameUnderTest = bricksetUserUnderTest.BricksetUsername;

            InsertData(bricksetUserUnderTest);

            var removedUser = _bricksetUserRepository.Remove(bricksetUsernameUnderTest);

            removedUser.Should().BeTrue();
            _bricksetUserRepository.Exists(bricksetUsernameUnderTest).Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void AddOrUpdateSet_InvalidUsername_ReturnsNull(string username)
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(username, new BricksetUserSet { Set = new Set { SetId = set.SetId } });

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_NullBricksetUserSet_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, null);

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_InvalidBricksetUserSet_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, new BricksetUserSet { Set = new Set { SetId = 0 } });

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserDoesNotExist_ReturnsNull()
        {
            var set = ModelsSetup.GetSetUnderTest();

            InsertData(set);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet("username", new BricksetUserSet { Set = new Set { SetId = set.SetId } });

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserSetLinkedSetDoesNotExist_ReturnsNull()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, new BricksetUserSet { Set = new Set { SetId = set.SetId + 1 } });

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserSetDoesNotExist_InsertsAndReturnsModel()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSetUnderTest = new BricksetUserSet
            {
                Set = set,
                Wanted = false,
                Owned = true,
                QuantityOwned = 10
            };

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest);

            bricksetUserSet.Should().BeEquivalentTo(bricksetUserSetUnderTest);
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserSetExists_UpdatesAndReturnsModel()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSetUnderTest = new BricksetUserSet
            {
                Set = set,
                Wanted = true
            };

            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest);

            bricksetUserSetUnderTest.Wanted = false;
            bricksetUserSetUnderTest.Owned = true;
            bricksetUserSetUnderTest.QuantityOwned = 2;

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest);

            bricksetUserSet.Should().BeEquivalentTo(bricksetUserSetUnderTest);
            bricksetUserSet.Wanted.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GetSet_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet(username, 1);

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void GetSet_InvalidSetId_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet(bricksetUser.BricksetUsername, 0);

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void GetSet_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet("user name", 1);

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void GetSet_BricksetUserDoesNotHaveSetId_ReturnsNull()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSetUnderTest = new BricksetUserSet
            {
                Set = set,
                Wanted = true
            };

            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest);

            var bricksetUserSet = _bricksetUserRepository.GetSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest.Set.SetId + 1);

            bricksetUserSet.Should().BeNull();
        }

        [TestMethod]
        public void GetSet_BricksetUserHasSetId_ReturnsModel()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSetUnderTest = new BricksetUserSet
            {
                Set = set,
                Wanted = true
            };

            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest);

            var bricksetUserSet = _bricksetUserRepository.GetSet(bricksetUser.BricksetUsername, bricksetUserSetUnderTest.Set.SetId);

            bricksetUserSet.Set.SetId.Should().Be(bricksetUserSetUnderTest.Set.SetId);
        }

        [TestMethod]
        public void GetAllUsernames_BricksetUserTypeDoesNotExist_ReturnsEmptyList()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var usernameList = _bricksetUserRepository.GetAllUsernames(BricksetUserTypeEnum.Friend);

            usernameList.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllUsernames_BricksetUserTypeExists_ReturnsListOfUsernames()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var usernameList = _bricksetUserRepository.GetAllUsernames(bricksetUser.UserType);

            usernameList.Should().NotBeEmpty();
            usernameList.First().Should().Be(bricksetUser.BricksetUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void UpdateUserSynchronizationTimestamp_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(username, DateTimeOffset.Now);

            bricksetUser.Should().BeNull();
        }

        [TestMethod]
        public void UpdateUserSynchronizationTimestamp_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(Guid.NewGuid().ToString(), DateTimeOffset.Now);

            bricksetUser.Should().BeNull();
        }

        [TestMethod]
        public void UpdateUserSynchronizationTimestamp_BricksetUserExists_UpdatesUserSynchronizationTimestampAndReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var synchronizationTimestamp = DateTimeOffset.Now.AddHours(-2);

            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(bricksetUserUnderTest.BricksetUsername, synchronizationTimestamp);

            bricksetUser.Should().NotBeNull();
            bricksetUser.UserSynchronizationTimestamp.Should().Be(synchronizationTimestamp);
        }
    }
}
