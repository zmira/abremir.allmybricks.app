using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class BricksetUserRepositoryTests : DataTestsBase
    {
        private static IBricksetUserRepository _bricksetUserRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _bricksetUserRepository = new BricksetUserRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Add_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.Add(BricksetUserType.None, username);

            Check.That(bricksetUser).IsNull();
        }

        [TestMethod]
        public void Add_BricksetUserAlreadyExists_ReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Add(bricksetUserUnderTest.UserType, bricksetUserUnderTest.BricksetUsername);

            Check.That(bricksetUser.BricksetUsername).IsEqualTo(bricksetUserUnderTest.BricksetUsername);
        }

        [TestMethod]
        public void Add_BricksetUserDoesNotExist_InsertsAndReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            const string newUsername = "new username";

            var bricksetUser = _bricksetUserRepository.Add(bricksetUserUnderTest.UserType, newUsername);

            Check.That(bricksetUser.BricksetUsername).IsEqualTo(newUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Get_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.Get(username);

            Check.That(bricksetUser).IsNull();
        }

        [TestMethod]
        public void Get_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Get("username");

            Check.That(bricksetUser).IsNull();
        }

        [TestMethod]
        public void Get_BricksetUserExists_ReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.Get(bricksetUserUnderTest.BricksetUsername);

            Check.That(bricksetUser.BricksetUsername).IsEqualTo(bricksetUserUnderTest.BricksetUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Exists_InvalidUsername_ReturnsFalse(string username)
        {
            var bricksetUserExists = _bricksetUserRepository.Exists(username);

            Check.That(bricksetUserExists).IsFalse();
        }

        [TestMethod]
        public void Exists_ValidUsernameAndDoesNotExist_ReturnsFalse()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUserExists = _bricksetUserRepository.Exists(Guid.NewGuid().ToString());

            Check.That(bricksetUserExists).IsFalse();
        }

        [TestMethod]
        public void Exists_ValidUsernameAndExists_ReturnsTrue()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUserExists = _bricksetUserRepository.Exists(bricksetUserUnderTest.BricksetUsername);

            Check.That(bricksetUserExists).IsTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void Remove_InvalidUsername_ReturnsFalse(string username)
        {
            var removedUser = _bricksetUserRepository.Remove(username);

            Check.That(removedUser).IsFalse();
        }

        [TestMethod]
        public void Remove_ValidUsernameAndDoesNotExist_ReturnsFalse()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var removedUser = _bricksetUserRepository.Remove(Guid.NewGuid().ToString());

            Check.That(removedUser).IsFalse();
        }

        [TestMethod]
        public void Remove_ValidUsernameAndExists_ReturnsTrueAndRemovesUser()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();
            string bricksetUsernameUnderTest = bricksetUserUnderTest.BricksetUsername;

            InsertData(bricksetUserUnderTest);

            var removedUser = _bricksetUserRepository.Remove(bricksetUsernameUnderTest);

            Check.That(removedUser).IsTrue();
            Check.That(_bricksetUserRepository.Exists(bricksetUsernameUnderTest)).IsFalse();
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

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_NullBricksetUserSet_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, null);

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_InvalidBricksetUserSet_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, new BricksetUserSet { Set = new Set { SetId = 0 } });

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserDoesNotExist_ReturnsNull()
        {
            var set = ModelsSetup.GetSetUnderTest();

            InsertData(set);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet("username", new BricksetUserSet { Set = new Set { SetId = set.SetId } });

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void AddOrUpdateSet_BricksetUserSetLinkedSetDoesNotExist_ReturnsNull()
        {
            var set = ModelsSetup.GetSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            set = InsertData(set);
            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, new BricksetUserSet { Set = new Set { SetId = set.SetId + 1 } });

            Check.That(bricksetUserSet).IsNull();
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

            Check.That(bricksetUserSet).HasFieldsWithSameValues(bricksetUserSetUnderTest);
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

            Check.That(bricksetUserSet).HasFieldsWithSameValues(bricksetUserSetUnderTest);
            Check.That(bricksetUserSet.Wanted).IsFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void GetSet_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet(username, 1);

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void GetSet_InvalidSetId_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet(bricksetUser.BricksetUsername, 0);

            Check.That(bricksetUserSet).IsNull();
        }

        [TestMethod]
        public void GetSet_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var bricksetUserSet = _bricksetUserRepository.GetSet("user name", 1);

            Check.That(bricksetUserSet).IsNull();
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

            Check.That(bricksetUserSet).IsNull();
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

            Check.That(bricksetUserSet.Set.SetId).IsEqualTo(bricksetUserSetUnderTest.Set.SetId);
        }

        [TestMethod]
        public void GetAllUsernames_BricksetUserTypeDoesNotExist_ReturnsEmptyList()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var usernameList = _bricksetUserRepository.GetAllUsernames(BricksetUserType.Friend);

            Check.That(usernameList).IsEmpty();
        }

        [TestMethod]
        public void GetAllUsernames_BricksetUserTypeExists_ReturnsListOfUsernames()
        {
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUser);

            var usernameList = _bricksetUserRepository.GetAllUsernames(bricksetUser.UserType);

            Check.That(usernameList).Not.IsEmpty();
            Check.That(usernameList.First()).IsEqualTo(bricksetUser.BricksetUsername);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(ModelsSetup.StringEmpty)]
        public void UpdateUserSynchronizationTimestamp_InvalidUsername_ReturnsNull(string username)
        {
            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(username, DateTimeOffset.Now);

            Check.That(bricksetUser).IsNull();
        }

        [TestMethod]
        public void UpdateUserSynchronizationTimestamp_BricksetUserDoesNotExist_ReturnsNull()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(Guid.NewGuid().ToString(), DateTimeOffset.Now);

            Check.That(bricksetUser).IsNull();
        }

        [TestMethod]
        public void UpdateUserSynchronizationTimestamp_BricksetUserExists_UpdatesUserSynchronizationTimestampAndReturnsModel()
        {
            var bricksetUserUnderTest = ModelsSetup.GetBricksetUserUnderTest();

            InsertData(bricksetUserUnderTest);

            var synchronizationTimestamp = DateTimeOffset.Now.AddHours(-2);

            var bricksetUser = _bricksetUserRepository.UpdateUserSynchronizationTimestamp(bricksetUserUnderTest.BricksetUsername, synchronizationTimestamp);

            Check.That(bricksetUser).IsNotNull();
            Check.That(bricksetUser.UserSynchronizationTimestamp).IsEqualTo(synchronizationTimestamp);
        }

        [TestMethod]
        public void GetWantedSets_BricksetUserDoesNotExist_ReturnsEmptyList()
        {
            var bricksetUserSetList = _bricksetUserRepository.GetWantedSets(string.Empty);

            Check.That(bricksetUserSetList).IsEmpty();
        }

        [TestMethod]
        public void GetWantedSets_BricksetUserExists_ReturnsListOfWantedSets()
        {
            var wantedSet = ModelsSetup.GetSetUnderTest();
            var ownedSet = ModelsSetup.GetSecondSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            wantedSet = InsertData(wantedSet);
            ownedSet = InsertData(ownedSet);
            InsertData(bricksetUser);

            var bricksetUserSetWanted = new BricksetUserSet
            {
                Set = wantedSet,
                Wanted = true
            };

            var bricksetUserSetOwned = new BricksetUserSet
            {
                Set = ownedSet,
                Owned = true
            };

            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetWanted);
            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetOwned);

            var bricksetUserSetList = _bricksetUserRepository.GetWantedSets(bricksetUser.BricksetUsername).ToList();

            Check.That(bricksetUserSetList).CountIs(1);
            Check.That(bricksetUserSetList.Select(bricksetUserSet => bricksetUserSet.Set.SetId)).Contains(bricksetUserSetWanted.Set.SetId);
        }

        [TestMethod]
        public void GetOwnedSets_BricksetUserDoesNotExist_ReturnsEmptyList()
        {
            var bricksetUserSetList = _bricksetUserRepository.GetWantedSets(string.Empty);

            Check.That(bricksetUserSetList).IsEmpty();
        }

        [TestMethod]
        public void GetOwnedSets_BricksetUserExists_ReturnsListOfOwnedSets()
        {
            var wantedSet = ModelsSetup.GetSetUnderTest();
            var ownedSet = ModelsSetup.GetSecondSetUnderTest();
            var bricksetUser = ModelsSetup.GetBricksetUserUnderTest();

            wantedSet = InsertData(wantedSet);
            ownedSet = InsertData(ownedSet);
            InsertData(bricksetUser);

            var bricksetUserSetWanted = new BricksetUserSet
            {
                Set = wantedSet,
                Wanted = true
            };

            var bricksetUserSetOwned = new BricksetUserSet
            {
                Set = ownedSet,
                Owned = true
            };

            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetWanted);
            _bricksetUserRepository.AddOrUpdateSet(bricksetUser.BricksetUsername, bricksetUserSetOwned);

            var bricksetUserSetList = _bricksetUserRepository.GetOwnedSets(bricksetUser.BricksetUsername).ToList();

            Check.That(bricksetUserSetList).CountIs(1);
            Check.That(bricksetUserSetList.Select(bricksetUserSet => bricksetUserSet.Set.SetId)).Contains(bricksetUserSetOwned.Set.SetId);
        }
    }
}
