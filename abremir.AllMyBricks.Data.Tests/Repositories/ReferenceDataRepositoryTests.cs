using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Tests.Configuration;
using abremir.AllMyBricks.Data.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace abremir.AllMyBricks.Data.Tests.Repositories
{
    [TestClass]
    public class ReferenceDataRepositoryTests : DataTestsBase
    {
        private static IReferenceDataRepository _referenceDataRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GetOrAdd_InvalidCategoryReferenceDataValue_ReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Category>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GetOrAdd_InvalidPackagingTypeReferenceDataValue_ReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<PackagingType>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GetOrAdd_InvalidTagReferenceDataValue_ReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<Tag>(referenceDataValue, null);
        }

        [DataTestMethod]
        [DataRow(ModelsSetup.StringEmpty)]
        [DataRow(null)]
        public void GetOrAdd_InvalidThemeGroupReferenceDataValue_ReturnsNull(string referenceDataValue)
        {
            GetOrAddTestHelper<ThemeGroup>(referenceDataValue, null);
        }

        [TestMethod]
        public void GetOrAdd_CategoryReferenceDataDoesNotExist_InsertsReferenceData()
        {
            GetOrAddTestHelper<Category>(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData, insert: true);
        }

        [TestMethod]
        public void GetOrAdd_PackagingTypeReferenceDataDoesNotExist_InsertsReferenceData()
        {
            GetOrAddTestHelper<PackagingType>(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData, insert: true);
        }

        [TestMethod]
        public void GetOrAdd_TagReferenceDataDoesNotExist_InsertsReferenceData()
        {
            GetOrAddTestHelper<Tag>(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData, insert: true);
        }

        [TestMethod]
        public void GetOrAdd_ThemeGroupReferenceDataDoesNotExist_InsertsReferenceData()
        {
            GetOrAddTestHelper<ThemeGroup>(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData, insert: true);
        }

        [TestMethod]
        public void GetOrAdd_CategoryReferenceDataExists_DoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<Category>(ModelsSetup.CategoryReferenceDataValue, ModelsSetup.CategoryReferenceData, exists: true);
        }

        [TestMethod]
        public void GetOrAdd_PackagingTypeReferenceDataExists_DoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<PackagingType>(ModelsSetup.PackagingTypeReferenceDataValue, ModelsSetup.PackagingTypeReferenceData, exists: true);
        }

        [TestMethod]
        public void GetOrAdd_TagReferenceDataExists_DoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<Tag>(ModelsSetup.TagReferenceDataValue, ModelsSetup.TagReferenceData, exists: true);
        }

        [TestMethod]
        public void GetOrAdd_ThemeGroupReferenceDataExists_DoesNotInsertReferenceData()
        {
            GetOrAddTestHelper<ThemeGroup>(ModelsSetup.ThemeGroupReferenceDataValue, ModelsSetup.ThemeGroupReferenceData, exists: true);
        }

        private void GetOrAddTestHelper<T>(string referenceDataValue, T expectedReferenceData, bool insert = false, bool exists = false) where T : IReferenceData, new()
        {
            if (exists)
            {
                _referenceDataRepository.GetOrAdd<T>(expectedReferenceData.Value);
            }

            T referenceData = _referenceDataRepository.GetOrAdd<T>(referenceDataValue);

            if (!(expectedReferenceData is null))
            {
                Check.That(referenceData?.GetType()).IsEqualTo(typeof(T));
            }

            Check.That(referenceData?.Value).IsEqualTo(expectedReferenceData?.Value);

            if (insert || exists)
            {
                var referenceDataList = MemoryRepositoryService.GetRepository().Query<T>().ToList();

                Check.That(referenceDataList).CountIs(1);
                Check.That(referenceDataList[0].Value).IsEqualTo(referenceDataValue);
            }
        }
    }
}
