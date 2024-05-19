using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class ThemeSanitizerTests : DataSynchronizerTestsBase
    {
        private static ThemeRepository _themeRepository;
        private static SubthemeRepository _subthemeRepository;
        private static SetRepository _setRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
            _subthemeRepository = new SubthemeRepository(MemoryRepositoryService);
            _setRepository = new SetRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void Synchronize_OnboardingServiceReturnsEmptyBricksetApiKey_ThrowsException()
        {
            var onboardingService = Substitute.For<IOnboardingService>();
            onboardingService.GetBricksetApiKey().Returns(string.Empty);

            var themeSanitizer = CreateTarget(onboardingService: onboardingService);

            Check.That(themeSanitizer.Synchronize()).Throws<Exception>();
        }

        [TestMethod]
        public async Task Synchronize_ActualNumberOfSetsMatchesExpectedNumberOfSets_DoesNothing()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();

            var themeSanitizer = CreateTarget(bricksetApiService: bricksetApiService);

            await themeSanitizer.Synchronize();

            await bricksetApiService.DidNotReceiveWithAnyArgs().GetThemes(default);
        }

        [TestMethod]
        public async Task Synchronize_ActualNumberOfSetsNotEqualToExpectedNumberOfSets_InvokesBricksetApiService()
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020 };
            await _themeRepository.AddOrUpdate(theme);

            var bricksetApiService = Substitute.For<IBricksetApiService>();

            var themeSanitizer = CreateTarget(bricksetApiService: bricksetApiService);

            await themeSanitizer.Synchronize();

            await bricksetApiService.ReceivedWithAnyArgs().GetThemes(default);
        }

        [TestMethod]
        public async Task Synchronize_ExistingThemesNoLongerPresentInBrickset_DeletesThemes()
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020 };
            await _themeRepository.AddOrUpdate(theme);

            var themeSanitizer = CreateTarget();

            await themeSanitizer.Synchronize();

            Check.That(await _themeRepository.Count()).Is(0);
        }

        [TestMethod]
        public async Task Synchronize_ThemeToDeleteAlsoHasSubthemes_DeletesSubthemes()
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020, SubthemeCount = 1 };
            await _themeRepository.AddOrUpdate(theme);

            var subtheme = new Subtheme { Id = 1, Name = "subtheme", Theme = theme, SetCount = 1, YearFrom = 2020, YearTo = 2020 };
            await _subthemeRepository.AddOrUpdate(subtheme);

            var themeSanitizer = CreateTarget();

            await themeSanitizer.Synchronize();

            Check.That(await _subthemeRepository.Count()).Is(0);
        }

        [TestMethod]
        public async Task Synchronize_ThemeToDeleteAlsoHasSets_DeletesSets()
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020, SubthemeCount = 1 };
            await _themeRepository.AddOrUpdate(theme);

            var theme2 = new Theme { Id = 2, Name = "theme-2", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020 };
            await _themeRepository.AddOrUpdate(theme2);

            var subtheme = new Subtheme { Id = 1, Name = "subtheme", Theme = theme, SetCount = 1, YearFrom = 2020, YearTo = 2020 };
            await _subthemeRepository.AddOrUpdate(subtheme);

            var set = new Set { SetId = 1, Name = "set", Theme = theme, Subtheme = subtheme, Year = 2020 };
            await _setRepository.AddOrUpdate(set);
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService.GetThemes(Arg.Any<ParameterApiKey>()).Returns([new Themes { Theme = theme2.Name, SetCount = theme2.SetCount, SubthemeCount = theme2.SubthemeCount, YearFrom = theme2.YearFrom, YearTo = theme2.YearTo }]);

            var themeSanitizer = CreateTarget(bricksetApiService: bricksetApiService);

            await themeSanitizer.Synchronize();

            Check.That(await _setRepository.Count()).Is(0);
        }

        private static ThemeSanitizer CreateTarget(
            IOnboardingService onboardingService = null,
            IBricksetApiService bricksetApiService = null,
            IBricksetUserRepository bricksetUserRepository = null)
        {
            if (onboardingService is null)
            {
                onboardingService = Substitute.For<IOnboardingService>();
                onboardingService.GetBricksetApiKey().Returns("brickset-api-key");
            }

            bricksetApiService ??= Substitute.For<IBricksetApiService>();
            bricksetUserRepository ??= Substitute.For<IBricksetUserRepository>();

            return new ThemeSanitizer(
                Substitute.For<IInsightsRepository>(),
                onboardingService,
                bricksetApiService,
                _setRepository,
                new ReferenceDataRepository(MemoryRepositoryService),
                _themeRepository,
                _subthemeRepository,
                bricksetUserRepository,
                Substitute.For<IThumbnailSynchronizer>(),
                Substitute.For<IMessageHub>());
        }
    }
}
