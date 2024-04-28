using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Enumerations;
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
    public class SetSanitizerTests : DataSynchronizerTestsBase
    {
        private static ISetRepository _setRepository;
        private static IThemeRepository _themeRepository;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _setRepository = new SetRepository(MemoryRepositoryService);
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void Synchronize_OnboardingServiceReturnsEmptyBricksetApiKey_ThrowsException()
        {
            var onboardingService = Substitute.For<IOnboardingService>();
            onboardingService.GetBricksetApiKey().Returns(string.Empty);

            var setSanitizer = CreateTarget(onboardingService: onboardingService);

            Check.That(setSanitizer.Synchronize()).Throws<Exception>();
        }

        [TestMethod]
        public async Task Synchronize_ActualNumberOfSetsMatchesExpectedNumberOfSets_DoesNothing()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();

            var setSanitizer = CreateTarget(bricksetApiService: bricksetApiService);

            await setSanitizer.Synchronize();

            await bricksetApiService.DidNotReceiveWithAnyArgs().GetSets(default);
        }

        [TestMethod]
        public async Task Synchronize_ActualNumberOfSetsGreaterThanExpectedNumberOfSets_DeletesExtraSets()
        {
            var testContext = await CreateTestContextWithActualNumberOfSetsGreaterThanExpectedNumberOfSets();

            await testContext.Sanitizer.Synchronize();

            Check.That(await _setRepository.Count()).Is(1);
            Check.That(await _setRepository.Get(testContext.Set2.SetId)).IsNull();
            Check.That(await _setRepository.Get(testContext.Set1.SetId)).IsNotNull();
            Check.That((await _setRepository.Get(testContext.Set1.SetId)).Pieces).IsEqualTo(555);
        }

        [TestMethod]
        public async Task Synchronize_ActualNumberOfSetsLessThanExpectedNumberOfSets_AddsMissingSets()
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 2, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 2 }], YearFrom = 2020, YearTo = 2020 };
            theme = await _themeRepository.AddOrUpdate(theme);

            var set = new Set { SetId = 1, Name = "set 1", Theme = theme, Year = 2020 };
            await _setRepository.AddOrUpdate(set);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService.GetSets(Arg.Any<GetSetsParameters>()).Returns([
                new Sets { SetId = 2, Name = "set 2", Theme = theme.Name },
                new Sets { SetId = 3, Name = "set 3", Theme = theme.Name },
            ]);

            var setSanitizer = CreateTarget(bricksetApiService: bricksetApiService);

            await setSanitizer.Synchronize();

            Check.That(await _setRepository.Count()).Is(2);
            Check.That(await _setRepository.Get(set.SetId)).IsNull();
            Check.That(await _setRepository.Get(2)).IsNotNull();
            Check.That(await _setRepository.Get(3)).IsNotNull();
        }

        [TestMethod]
        public async Task Synchronize_ThereAreNoBricksetUsers_DoesNotInvokeBricksetUserRepository()
        {
            var bricksetUserRepository = Substitute.For<IBricksetUserRepository>();
            bricksetUserRepository.GetAllUsernames(Arg.Any<BricksetUserType>()).Returns(
                _ => [],
                _ => []
            );

            var testContext = await CreateTestContextWithActualNumberOfSetsGreaterThanExpectedNumberOfSets(bricksetUserRepository);

            await testContext.Sanitizer.Synchronize();

            await bricksetUserRepository.DidNotReceive().RemoveSets(Arg.Any<string>(), Arg.Any<List<long>>());
        }

        [TestMethod]
        public async Task Synchronize_ThereAreBricksetUsers_InvokesBricksetUserRepository()
        {
            var bricksetUserRepository = Substitute.For<IBricksetUserRepository>();
            bricksetUserRepository.GetAllUsernames(Arg.Any<BricksetUserType>()).Returns(
                _ => ["primary"],
                _ => ["friend"]
            );

            var testContext = await CreateTestContextWithActualNumberOfSetsGreaterThanExpectedNumberOfSets(bricksetUserRepository);

            await testContext.Sanitizer.Synchronize();

            await bricksetUserRepository.Received(2).RemoveSets(Arg.Any<string>(), Arg.Any<List<long>>());
        }

        private async Task<(SetSanitizer Sanitizer, Set Set1, Set Set2)> CreateTestContextWithActualNumberOfSetsGreaterThanExpectedNumberOfSets(IBricksetUserRepository bricksetUserRepository = null)
        {
            var theme = new Theme { Id = 1, Name = "theme", SetCount = 1, SetCountPerYear = [new YearSetCount { Year = 2020, SetCount = 1 }], YearFrom = 2020, YearTo = 2020 };
            await _themeRepository.AddOrUpdate(theme);

            var set1 = new Set { SetId = 1, Name = "set 1", Theme = theme, Year = 2020 };
            var set2 = new Set { SetId = 2, Name = "set 2", Theme = theme, Year = 2020 };
            await _setRepository.AddOrUpdate(set1);
            await _setRepository.AddOrUpdate(set2);

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService.GetSets(Arg.Any<GetSetsParameters>()).Returns([new Sets { SetId = (int)set1.SetId, Name = set1.Name, Theme = theme.Name, Pieces = 555 }]);

            return (CreateTarget(bricksetApiService: bricksetApiService, bricksetUserRepository: bricksetUserRepository), set1, set2);
        }

        private static SetSanitizer CreateTarget(
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

            return new SetSanitizer(
                Substitute.For<IInsightsRepository>(),
                onboardingService,
                bricksetApiService,
                _setRepository,
                new ReferenceDataRepository(MemoryRepositoryService),
                _themeRepository,
                new SubthemeRepository(MemoryRepositoryService),
                bricksetUserRepository,
                Substitute.For<IThumbnailSynchronizer>(),
                Substitute.For<IMessageHub>());
        }
    }
}
