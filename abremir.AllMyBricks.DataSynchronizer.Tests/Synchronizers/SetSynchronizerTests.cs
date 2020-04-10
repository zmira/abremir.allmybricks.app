using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NFluent;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore RCS1163 // Unused parameter.
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task SynchronizeForThemeAndSubtheme_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<GetSetsParameters>())
                .Returns(Enumerable.Empty<Sets>());

            var setSynchronizer = CreateTarget(bricksetApiService);

            await setSynchronizer.Synchronize(string.Empty, new Theme { Name = string.Empty, YearFrom = 0, YearTo = 1 }, new Subtheme { Name = string.Empty });

            Check.That(_setRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task SynchronizeForThemeAndSubtheme_BricksetApiServiceReturnsListOfSets_AllSetsAreSaved()
        {
            var themesList = JsonConvert.DeserializeObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = JsonConvert.DeserializeObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));
            var subthemesList = JsonConvert.DeserializeObject<List<Subthemes>>(GetResultFileFromResource(Constants.JsonFileGetSubthemes));
            var setsList = JsonConvert.DeserializeObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetSets));
            var testSet = setsList.First(set => set.SetId == Constants.TestSetId);
            var additionalImagesList = JsonConvert.DeserializeObject<List<SetImage>>(GetResultFileFromResource(Constants.JsonFileGetAdditionalImages));
            var instructionsList = JsonConvert.DeserializeObject<List<Instructions>>(GetResultFileFromResource(Constants.JsonFileGetInstructions));
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
                .GetSets(Arg.Any<GetSetsParameters>())
                .Returns(ret => setsList.Where(setFromList => setFromList.Year == ((GetSetsParameters)ret.Args()[0]).Year
                    && setFromList.Theme == ((GetSetsParameters)ret.Args()[0]).Theme
                    && setFromList.Subtheme == ((GetSetsParameters)ret.Args()[0]).Subtheme));
            bricksetApiService
                .GetAdditionalImages(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(additionalImagesList);
            bricksetApiService
                .GetInstructions(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(instructionsList);

            var setSynchronizer = CreateTarget(bricksetApiService);

            await setSynchronizer.Synchronize(string.Empty, theme, subtheme);

            var expectedSets = setsList.Where(bricksetSets => bricksetSets.Subtheme == subtheme.Name).ToList();

            Check.That(_setRepository.All()).CountIs(expectedSets.Count);
            var persistedSet = _setRepository.Get(testSet.SetId);
            Check.That(persistedSet.Images).CountIs(additionalImagesList.Count);
            Check.That(persistedSet.Instructions).CountIs(instructionsList.Count);
        }

        [TestMethod]
        public async Task SynchronizeForRecentlyUpdated_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<GetSetsParameters>())
                .Returns(Enumerable.Empty<Sets>());

            var setSynchronizer = CreateTarget(bricksetApiService);

            await setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now.Date);

            Check.That(_setRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task SynchronizeForRecentlyUpdated_BricksetApiServiceReturnsListOfSets_AllSetsAreSaved()
        {
            var themesList = JsonConvert.DeserializeObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var theme = testTheme.ToTheme();
            var recentlyUpdatedSetsList = JsonConvert.DeserializeObject<List<Sets>>(GetResultFileFromResource(Constants.JsonFileGetRecentlyUpdatedSets));

            _themeRepository.AddOrUpdate(theme);

            var subtheme = new Subtheme
            {
                Name = "",
                Theme = theme,
                YearFrom = theme.YearFrom,
                YearTo = theme.YearTo
            };

            _subthemeRepository.AddOrUpdate(subtheme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<GetSetsParameters>())
                .Returns(recentlyUpdatedSetsList);

            var setSynchronizer = CreateTarget(bricksetApiService);

            await setSynchronizer.Synchronize(string.Empty, DateTimeOffset.Now);

            Check.That(_setRepository.All()).CountIs(recentlyUpdatedSetsList.Count);
        }

        private SetSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService = bricksetApiService ?? Substitute.For<IBricksetApiService>();

            var thumbnailSynchronizer = Substitute.For<IThumbnailSynchronizer>();

            return new SetSynchronizer(bricksetApiService, _setRepository, _referenceDataRepository, _themeRepository, _subthemeRepository, thumbnailSynchronizer, Substitute.For<IMessageHub>());
        }
    }
}
