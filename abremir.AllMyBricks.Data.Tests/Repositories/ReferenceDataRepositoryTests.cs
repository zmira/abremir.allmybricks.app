using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Realms;
using System.Linq;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class ReferenceDataRepositoryTests : DataTestsBase
    {
        private static IReferenceDataRepository _referenceDataRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidCategoryReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Category, Managed.Category>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidPackagingTypeReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<PackagingType, Managed.PackagingType>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidTagReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Tag, Managed.Tag>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GivenGetOrAdd_WhenInvalidThemeGroupReferenceDataValue_ThenReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<ThemeGroup, Managed.ThemeGroup>(referenceDataValue, null);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenCategoryReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper<Category, Managed.Category>(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData.ToPlainObject(), insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenPackaginTypeReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper<PackagingType, Managed.PackagingType>(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData.ToPlainObject(), insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenTagReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper<Tag, Managed.Tag>(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData.ToPlainObject(), insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenThemeGroupReferenceDataDoesNotExist_ThenInsertsReferenceData()
        {
            GetOrAddTestHelper<ThemeGroup, Managed.ThemeGroup>(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData.ToPlainObject(), insert: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenCategoryReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<Category, Managed.Category>(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData.ToPlainObject(), exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenPackaginTypeReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<PackagingType, Managed.PackagingType>(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData.ToPlainObject(), exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenTagReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<Tag, Managed.Tag>(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData.ToPlainObject(), exists: true);
        }

        [TestMethod]
        public void GivenGetOrAdd_WhenThemeGroupReferenceDataExists_ThenDoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<ThemeGroup, Managed.ThemeGroup>(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData.ToPlainObject(), exists: true);
        }

        private void GetOrAddTestHelper<T, U>(string referenceDataValue, T expectedReferenceData, bool insert = false, bool exists = false) where T : IReferenceData where U : RealmObject, IReferenceData
        {
            if (exists)
            {
                _referenceDataRepository.GetOrAdd<T>(expectedReferenceData.Value);
            }

            T referenceData = _referenceDataRepository.GetOrAdd<T>(referenceDataValue);

            referenceData?.GetType().Should().Be(typeof(T));
            referenceData?.Value.Should().Be(expectedReferenceData?.Value);

            if (insert || exists)
            {
                var referenceDataList = MemoryRepositoryService.GetRepository().All<U>();

                referenceDataList.Should().ContainSingle();
                referenceDataList.First().Value.Should().Be(referenceDataValue);
            }
        }
    }
}
