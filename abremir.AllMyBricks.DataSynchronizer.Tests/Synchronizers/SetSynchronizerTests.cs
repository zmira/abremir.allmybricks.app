using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class SetSynchronizerTests : DataSynchronizerTestsBase
    {
        private static ISetRepository _setRepository;
        private static IReferenceDataRepository _referenceDataRepository;
        private static IThemeRepository _themeRepository;
        private static ISubthemeRepository _subthemeRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable RECS0154 // Parameter is never used
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void SynchronizeForThemeAndSubtheme_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<ParameterSets>())
                .Returns(Enumerable.Empty<Sets>());

            var setSynchronizer = CreateTarget(bricksetApiService);

            setSynchronizer.Synchronize(string.Empty, new Theme { Name = string.Empty, YearFrom = 0, YearTo = 1 }, new Subtheme { Name = string.Empty });

            _setRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public void SynchronizeForThemeAndSubtheme_BricksetApiServiceReturnsListOfSets_AllSetsAreSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = fastJSON.JSON.ToObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = fastJSON.JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));
            var testSet = setsList.First(set => set.SetId == Constants.TestSetId);
            var additionalImagesList = fastJSON.JSON.ToObject<List<AdditionalImages>>(GetResultFileFromResource(Constants.JsonFileGetAdditionalImages));
            var instructionsList = fastJSON.JSON.ToObject<List<Instructions>>(GetResultFileFromResource(Constants.JsonFileGetInstructions));
            var reviewsList = fastJSON.JSON.ToObject<List<Reviews>>(GetResultFileFromResource(Constants.JsonFileGetReviews));
            var testSubtheme = subthemesList.First(bricksetSubtheme => bricksetSubtheme.Subtheme == testSet.Subtheme);

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            foreach(var subthemeItem in subthemesList)
            {
                var subthemeTheme = subthemeItem.ToSubtheme();
                subthemeTheme.Theme = theme;

                _subthemeRepository.AddOrUpdate(subthemeTheme);
            }

            var subtheme = testSubtheme.ToSubtheme();
            subtheme.Theme = theme;

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<ParameterSets>())
                .Returns(ret => setsList.Where(setFromList => setFromList.Year == ((ParameterSets)ret.Args()[0]).Year
                    && setFromList.Theme == ((ParameterSets)ret.Args()[0]).Theme
                    && setFromList.Subtheme == ((ParameterSets)ret.Args()[0]).Subtheme));
            bricksetApiService
                .GetAdditionalImages(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(additionalImagesList);
            bricksetApiService
                .GetInstructions(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(instructionsList);
            bricksetApiService
                .GetReviews(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(reviewsList);

            var setSynchronizer = CreateTarget(bricksetApiService);

            setSynchronizer.Synchronize(string.Empty, theme, subtheme);

            var expectedSets = setsList.Where(bricksetSets => bricksetSets.Subtheme == subtheme.Name).ToList();

            _setRepository.All().Count().Should().Be(expectedSets.Count);
            var persistedSet = _setRepository.Get(testSet.SetId);
            persistedSet.Images.Count.Should().BeGreaterOrEqualTo(additionalImagesList.Count);
            persistedSet.Instructions.Count.Should().Be(instructionsList.Count);
            persistedSet.Reviews.Count.Should().Be(reviewsList.Count);
        }

        [TestMethod]
        public void SynchronizeForThemeAndSubtheme_RetrieveFullSetDataOnSynchronizationTrueButNullSet_NothingIsSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = fastJSON.JSON.ToObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = fastJSON.JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));
            var testSet = setsList.First(set => set.SetId == Constants.TestSetId);
            var additionalImagesList = fastJSON.JSON.ToObject<List<AdditionalImages>>(GetResultFileFromResource(Constants.JsonFileGetAdditionalImages));
            var instructionsList = fastJSON.JSON.ToObject<List<Instructions>>(GetResultFileFromResource(Constants.JsonFileGetInstructions));
            var reviewsList = fastJSON.JSON.ToObject<List<Reviews>>(GetResultFileFromResource(Constants.JsonFileGetReviews));
            var testSubtheme = subthemesList.First(bricksetSubtheme => bricksetSubtheme.Subtheme == testSet.Subtheme);

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            foreach (var subthemeItem in subthemesList)
            {
                var subthemeTheme = subthemeItem.ToSubtheme();
                subthemeTheme.Theme = theme;

                _subthemeRepository.AddOrUpdate(subthemeTheme);
            }

            var subtheme = testSubtheme.ToSubtheme();
            subtheme.Theme = theme;

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<ParameterSets>())
                .Returns(ret => setsList.Where(setFromList => setFromList.Year == ((ParameterSets)ret.Args()[0]).Year
                    && setFromList.Theme == ((ParameterSets)ret.Args()[0]).Theme
                    && setFromList.Subtheme == ((ParameterSets)ret.Args()[0]).Subtheme));
            bricksetApiService
                .GetAdditionalImages(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(additionalImagesList);
            bricksetApiService
                .GetInstructions(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(instructionsList);
            bricksetApiService
                .GetReviews(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(reviewsList);
            bricksetApiService
                .GetSet(Arg.Any<ParameterUserHashSetId>())
                .Returns((Sets)null);

            var preferencesService = Substitute.For<IPreferencesService>();
            preferencesService
                .RetrieveFullSetDataOnSynchronization
                .Returns(true);

            var setSynchronizer = CreateTarget(bricksetApiService, preferencesService);

            setSynchronizer.Synchronize(string.Empty, theme, subtheme);

            var expectedSets = setsList.Where(bricksetSets => bricksetSets.Subtheme == subtheme.Name).ToList();

            bricksetApiService.ReceivedWithAnyArgs().GetSet(null);
            _setRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public void SynchronizeForThemeAndSubtheme_RetrieveFullSetDataOnSynchronizationTrueAndSetNotNull_GetSetInvokedAndAllSetsAreSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = fastJSON.JSON.ToObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = fastJSON.JSON.ToObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));
            var testSet = setsList.First(set => set.SetId == Constants.TestSetId);
            var additionalImagesList = fastJSON.JSON.ToObject<List<AdditionalImages>>(GetResultFileFromResource(Constants.JsonFileGetAdditionalImages));
            var instructionsList = fastJSON.JSON.ToObject<List<Instructions>>(GetResultFileFromResource(Constants.JsonFileGetInstructions));
            var reviewsList = fastJSON.JSON.ToObject<List<Reviews>>(GetResultFileFromResource(Constants.JsonFileGetReviews));
            var testSubtheme = subthemesList.First(bricksetSubtheme => bricksetSubtheme.Subtheme == testSet.Subtheme);

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            foreach (var subthemeItem in subthemesList)
            {
                var subthemeTheme = subthemeItem.ToSubtheme();
                subthemeTheme.Theme = theme;

                _subthemeRepository.AddOrUpdate(subthemeTheme);
            }

            var subtheme = testSubtheme.ToSubtheme();
            subtheme.Theme = theme;

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<ParameterSets>())
                .Returns(ret => setsList.Where(setFromList => setFromList.Year == ((ParameterSets)ret.Args()[0]).Year
                    && setFromList.Theme == ((ParameterSets)ret.Args()[0]).Theme
                    && setFromList.Subtheme == ((ParameterSets)ret.Args()[0]).Subtheme));
            bricksetApiService
                .GetAdditionalImages(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(additionalImagesList);
            bricksetApiService
                .GetInstructions(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(instructionsList);
            bricksetApiService
                .GetReviews(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(reviewsList);
            bricksetApiService
                .GetSet(Arg.Any<ParameterUserHashSetId>())
                .Returns(ret => setsList.First(setFromList => setFromList.SetId == ((ParameterUserHashSetId)ret.Args()[0]).SetID));

            var preferencesService = Substitute.For<IPreferencesService>();
            preferencesService
                .RetrieveFullSetDataOnSynchronization
                .Returns(true);

            var setSynchronizer = CreateTarget(bricksetApiService, preferencesService);

            setSynchronizer.Synchronize(string.Empty, theme, subtheme);

            var expectedSets = setsList.Where(bricksetSets => bricksetSets.Subtheme == subtheme.Name).ToList();

            bricksetApiService.ReceivedWithAnyArgs().GetSet(null);
            _setRepository.All().Count().Should().Be(expectedSets.Count);
            var persistedSet = _setRepository.Get(testSet.SetId);
            persistedSet.Images.Count.Should().BeGreaterOrEqualTo(additionalImagesList.Count);
            persistedSet.Instructions.Count.Should().Be(instructionsList.Count);
            persistedSet.Reviews.Count.Should().Be(reviewsList.Count);
        }

        [TestMethod]
        public void SynchronizeForRecentlyUpdated_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetRecentlyUpdatedSets(Arg.Any<ParameterMinutesAgo>())
                .Returns(Enumerable.Empty<Sets>());

            var setSynchronizer = CreateTarget(bricksetApiService);

            setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now.Date);

            _setRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public void SynchronizeForRecentlyUpdated_BricksetApiServiceReturnsListOfSets_AllSetsAreSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeTechnic);
            var theme = testTheme.ToTheme();
            var recentlyUpdatedSetsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetRecentlyUpdatedSets));

            _themeRepository.AddOrUpdate(theme);

            var subtheme = new Subtheme
            {
                Name = "",
                Theme = theme
            };

            _subthemeRepository.AddOrUpdate(subtheme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetRecentlyUpdatedSets(Arg.Any<ParameterMinutesAgo>())
                .Returns(recentlyUpdatedSetsList);

            var setSynchronizer = CreateTarget(bricksetApiService);

            setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now);

            _setRepository.All().Count().Should().Be(recentlyUpdatedSetsList.Count);
        }

        [TestMethod]
        public void SynchronizeForRecentlyUpdated_RetrieveFullSetDataOnSynchronizationTrueButNullSet_GetSetInvokedButNothingSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeTechnic);
            var theme = testTheme.ToTheme();
            var recentlyUpdatedSetsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetRecentlyUpdatedSets));

            _themeRepository.AddOrUpdate(theme);

            var subtheme = new Subtheme
            {
                Name = "",
                Theme = theme
            };

            _subthemeRepository.AddOrUpdate(subtheme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetRecentlyUpdatedSets(Arg.Any<ParameterMinutesAgo>())
                .Returns(recentlyUpdatedSetsList);
            bricksetApiService
                .GetSet(Arg.Any<ParameterUserHashSetId>())
                .Returns((Sets)null);

            var preferencesService = Substitute.For<IPreferencesService>();
            preferencesService
                .RetrieveFullSetDataOnSynchronization
                .Returns(true);

            var setSynchronizer = CreateTarget(bricksetApiService, preferencesService);

            setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now);

            bricksetApiService.ReceivedWithAnyArgs().GetSet(null);
            _setRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public void SynchronizeForRecentlyUpdated_RetrieveFullSetDataOnSynchronizationTrueAndSetNotNull_GetSetInvokedAndAllSetsAreSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeTechnic);
            var theme = testTheme.ToTheme();
            var recentlyUpdatedSetsList = fastJSON.JSON.ToObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetRecentlyUpdatedSets));

            _themeRepository.AddOrUpdate(theme);

            var subtheme = new Subtheme
            {
                Name = "",
                Theme = theme
            };

            _subthemeRepository.AddOrUpdate(subtheme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetRecentlyUpdatedSets(Arg.Any<ParameterMinutesAgo>())
                .Returns(recentlyUpdatedSetsList);
            bricksetApiService
                .GetSet(Arg.Any<ParameterUserHashSetId>())
                .Returns(ret => recentlyUpdatedSetsList.First(setFromList => setFromList.SetId == ((ParameterUserHashSetId)ret.Args()[0]).SetID));

            var preferencesService = Substitute.For<IPreferencesService>();
            preferencesService
                .RetrieveFullSetDataOnSynchronization
                .Returns(true);

            var setSynchronizer = CreateTarget(bricksetApiService, preferencesService);

            setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now);

            bricksetApiService.ReceivedWithAnyArgs().GetSet(null);
            _setRepository.All().Count().Should().Be(recentlyUpdatedSetsList.Count);
        }

        private SetSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null, IPreferencesService preferencesService = null)
        {
            bricksetApiService = bricksetApiService ?? Substitute.For<IBricksetApiService>();
            preferencesService = preferencesService ?? Substitute.For<IPreferencesService>();

            var thumbnailSynchronizer = Substitute.For<IThumbnailSynchronizer>();

            return new SetSynchronizer(bricksetApiService, _setRepository, _referenceDataRepository, _themeRepository, _subthemeRepository, preferencesService, thumbnailSynchronizer);
        }
    }
}