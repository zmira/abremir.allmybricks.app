using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;
using System.Linq;

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
                var theme = bricksetTheme.ToTheme();

                var getYearsParameters = new ParameterTheme
                {
                    ApiKey = apiKey,
                    Theme = bricksetTheme.Theme
                };

                theme.SetCountPerYear = _bricksetApiService
                    .GetYears(getYearsParameters)
                    .ToYearSetCount()
                    .ToList();

                themeList.Add(theme);

                _themeRepository.AddOrUpdate(theme);
            }

            return themeList;
        }
    }
}