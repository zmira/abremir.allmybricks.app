using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class ReferenceDataRepositoryTests : TestRepositoryBase
    {
        private static IReferenceDataRepository _referenceDataRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidCategoryReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Category>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidPackagingTypeReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<PackagingType>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidTagReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Tag>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidThemeGroupReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<ThemeGroup>(referenceDataValue, null);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenCategoryReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData, insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenPackaginTypeReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData, insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenTagReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData, insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenThemeGroupReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData, insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenCategoryReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData, exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenPackaginTypeReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData, exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenTagReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData, exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenThemeGroupReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData, exists: true);
        }

        private void GetOrAddTestHelper<T>(string referenceDataValue, T expectedReferenceData, bool insert = false, bool exists = false) where T : IReferenceData, new()
        {
            if (exists)
            {
                InsertData<T>(expectedReferenceData);
            }

            T referenceData = _referenceDataRepository.GetOrAdd<T>(referenceDataValue);

            referenceData?.GetType().Should().Be(typeof(T));
            referenceData?.Value.Should().Be(expectedReferenceData?.Value);

            if (insert || exists)
            {
                var referenceDataList = GetAllData<T>();

                referenceDataList.Should().ContainSingle();
                referenceDataList.First().Value.Should().Be(referenceDataValue);
            }
        }
    }
}