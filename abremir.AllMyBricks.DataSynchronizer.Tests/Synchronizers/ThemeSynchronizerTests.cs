﻿using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using abremir.AllMyBricks.DataSynchronizer.Tests.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Tests.Shared;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Synchronizers
{
    [TestClass]
    public class ThemeSynchronizerTests : DataSynchronizerTestsBase
    {
        private static IThemeRepository _themeRepository;

        [ClassInitialize]
#pragma warning disable RCS1163 // Unused parameter.
        public static void ClassInitialize(TestContext testContext)
#pragma warning restore RCS1163 // Unused parameter.
        {
            _themeRepository = new ThemeRepository(MemoryRepositoryService);
        }

        [TestMethod]
        public void Synchronize_BricksetApiServiceReturnsEmptyList_NothingIsSaved()
        {
            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetThemes(Arg.Any<ParameterApiKey>())
                .Returns(Enumerable.Empty<Themes>());

            var themeSynchronizer = CreateTarget(bricksetApiService);

            var themes = themeSynchronizer.Synchronize(string.Empty);

            themes.Should().BeEmpty();
            _themeRepository.All().Should().BeEmpty();
        }

        [TestMethod]
        public void Synchronize_BricksetApiServiceReturnsListOfThemes_AllThemesAreSaved()
        {
            var themesList = fastJSON.JSON.ToObject<List<Themes>>(GetResultFileFromResource(Constants.JsonFileGetThemes));
            var yearsList = fastJSON.JSON.ToObject<List<Years>>(GetResultFileFromResource(Constants.JsonFileGetYears));

            var bricksetApiService = Substitute.For<IBricksetApiService>();
            bricksetApiService
                .GetThemes(Arg.Any<ParameterApiKey>())
                .Returns(themesList);
            bricksetApiService
                .GetYears(Arg.Is<ParameterTheme>(parameter => parameter.Theme == Constants.TestThemeArchitecture))
                .Returns(yearsList);

            var themeSynchronizer = CreateTarget(bricksetApiService);

            var themes = themeSynchronizer.Synchronize(string.Empty);

            themes.Count().Should().Be(themesList.Count);
            _themeRepository.All().Count().Should().Be(themesList.Count);
            _themeRepository.Get(Constants.TestThemeArchitecture).SetCountPerYear.Should().NotBeEmpty();
        }

        private ThemeSynchronizer CreateTarget(IBricksetApiService bricksetApiService = null)
        {
            bricksetApiService = bricksetApiService ?? Substitute.For<IBricksetApiService>();

            return new ThemeSynchronizer(bricksetApiService, _themeRepository);
        }
    }
}