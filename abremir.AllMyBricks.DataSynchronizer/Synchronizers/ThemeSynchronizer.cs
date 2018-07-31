using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThemeSynchronizer : IThemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly IThemeRepository _themeRepository;

        public ThemeSynchronizer(
            IBricksetApiService bricksetService,
            IThemeRepository themeRepository)
        {
            _bricksetApiService = bricksetService;
            _themeRepository = themeRepository;
        }

        public IEnumerable<Theme> Synchronize(string apiKey)
        {
            var themeList = new List<Theme>();

            var getThemesParameters = new ParameterApiKey
            {
                ApiKey = apiKey
            };

            foreach (var bricksetTheme in _bricksetApiService
                .GetThemes(getThemesParameters))
            {
                var theme = new Theme
                {
                    Name = bricksetTheme.Theme,
                    SetCount = (short)bricksetTheme.SetCount,
                    SubthemeCount = (short)bricksetTheme.SubthemeCount,
                    YearFrom = (short)bricksetTheme.YearFrom,
                    YearTo = (short)bricksetTheme.YearTo,
                };

                var getYearsParameters = new ParameterTheme
                {
                    ApiKey = apiKey,
                    Theme = bricksetTheme.Theme
                };

                foreach (var yearCount in _bricksetApiService
                    .GetYears(getYearsParameters))
                {
                    theme.SetCountPerYear.Add(new YearSetCount
                    {
                        Year = short.Parse(yearCount.Year),
                        SetCount = (short)yearCount.SetCount
                    });
                }

                themeList.Add(theme);

                _themeRepository.AddOrUpdate(theme);
            }

            return themeList;
        }
    }
}