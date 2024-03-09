using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NFluent;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class FullSetSynchronizerTests : DataSynchronizerTestsBase
    {
        private static ISetRepository _setRepository;
        private static IReferenceDataRepository _referenceDataRepository;
        private static IThemeRepository _themeRepository;
        private static ISubthemeRepository _subthemeRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
            _referenceDataRepository = new ReferenceDataRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public async Task Synchronize_InsightsRepositoryReturnsDataSynchronizationTimestamp_Returns()
        {
            var insightsRepository = Substitute.For<IInsightsRepository>();
            insightsRepository.GetDataSynchronizationTimestamp().Returns(DateTimeOffset.Now);
            var onboardingService = Substitute.For<IOnboardingService>();
            var bricksetApiService = Substitute.For<IBricksetApiService>();

            var partialSetSynchronizer = CreateTarget(insightsRepository, onboardingService, bricksetApiService);

            await partialSetSynchronizer.Synchronize().ConfigureAwait(false);

            await onboardingService.DidNotReceive().GetBricksetApiKey();
            await bricksetApiService.DidNotReceive().GetSets(Arg.Any<GetSetsParameters>());
        }

        [TestMethod]
        public void Synchronize_OnboardingServiceReturnsEmptyBricksetApiKey_ThrowsException()
        {
            var onboardingService = Substitute.For<IOnboardingService>();
            onboardingService.GetBricksetApiKey().Returns(string.Empty);

            var partialSetSynchronizer = CreateTarget(onboardingService: onboardingService);

            Check.That(partialSetSynchronizer.Synchronize().ConfigureAwait(false)).Throws<Exception>();
        }

        [TestMethod]
        public async Task Synchronize_ThemeRepositoryReturnsEmptyListOfThemes_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();

            var subthemeSynchronizer = CreateTarget(bricksetApiService: bricksetApiService);

            await subthemeSynchronizer.Synchronize().ConfigureAwait(false);

            await bricksetApiService.DidNotReceive().GetSets(Arg.Any<GetSetsParameters>());
            Check.That(_setRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsEmptyListOfSets_NothingIsSaved()
        {
            var themesList = JsonConvert.DeserializeObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var testTheme = themesList.First(themes => themes.Theme == Constants.TestThemeArchitecture);
            var yearsList = JsonConvert.DeserializeObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));

            var theme = testTheme.ToTheme();
            theme.SetCountPerYear = yearsList.ToYearSetCountEnumerable().ToList();

            _themeRepository.AddOrUpdate(theme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetSets(Arg.Any<GetSetsParameters>())
                .Returns([]);

            var setSynchronizer = CreateTarget(bricksetApiService: bricksetApiService);

            await setSynchronizer.Synchronize().ConfigureAwait(false);

            await bricksetApiService.Received().GetSets(Arg.Any<GetSetsParameters>());
            Check.That(_setRepository.All()).IsEmpty();
        }

        [TestMethod]
        public async Task Synchronize_BricksetApiServiceReturnsListOfSets_AllSetsAreSaved()
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
                .Returns(setsList);
            bricksetApiService
                .GetAdditionalImages(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(additionalImagesList);
            bricksetApiService
                .GetInstructions(Arg.Is<ParameterSetId>(parameter => parameter.SetID == testSet.SetId))
                .Returns(instructionsList);

            var setSynchronizer = CreateTarget(bricksetApiService: bricksetApiService);

            await setSynchronizer.Synchronize().ConfigureAwait(false);

            Check.That(_setRepository.All()).CountIs(setsList.Count);
            var persistedSet = _setRepository.Get(testSet.SetId);
            Check.That(persistedSet.Images).CountIs(additionalImagesList.Count);
            Check.That(persistedSet.Instructions).CountIs(instructionsList.Count);
        }

        private FullSetSynchronizer CreateTarget(
            IInsightsRepository insightsRepository = null,
            IOnboardingService onboardingService = null,
            IBricksetApiService bricksetApiService = null)
        {
            if (insightsRepository is null)
            {
                insightsRepository = Substitute.For<IInsightsRepository>();
                insightsRepository.GetDataSynchronizationTimestamp().Returns(default(DateTimeOffset?));
            }

            if (onboardingService is null)
            {
                onboardingService = Substitute.For<IOnboardingService>();
                onboardingService.GetBricksetApiKey().Returns("brickset-api-key");
            }

            bricksetApiService ??= Substitute.For<IBricksetApiService>();

            var thumbnailSynchronizer = Substitute.For<IThumbnailSynchronizer>();

            return new FullSetSynchronizer(
                insightsRepository,
                onboardingService,
                bricksetApiService,
                _setRepository,
                _referenceDataRepository,
                _themeRepository,
                _subthemeRepository,
                thumbnailSynchronizer,
                Substitute.For<IMessageHub>());
        }
    }
}
