using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThemeSynchronizer : IThemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly IThemeRepository _themeRepository;

        public event EventHandler ThemeSynchronizerStart;
        public event EventHandler ThemeSynchronizerEnd;
        public event EventHandler<int> ThemesAcquired;
        public event EventHandler<string> SynchronizingTheme;
        public event EventHandler<string> SynchronizedTheme;

        public ThemeSynchronizer(
            IBricksetApiService bricksetService,
            IThemeRepository themeRepository)
        {
            _bricksetApiService = bricksetService;
            _themeRepository = themeRepository;
        }

        public IEnumerable<Theme> Synchronize(string apiKey)
        {
            ThemeSynchronizerStart?.Invoke(this, null);

            var themeList = new List<Theme>();

            var getThemesParameters = new ParameterApiKey
            {
                ApiKey = apiKey
            };

            var bricksetThemes = _bricksetApiService.GetThemes(getThemesParameters).ToList();

            ThemesAcquired?.Invoke(this, bricksetThemes.Count);

            foreach (var bricksetTheme in bricksetThemes)
            {
                SynchronizingTheme?.Invoke(this, bricksetTheme.Theme);

                var theme = bricksetTheme.ToTheme();

                var getYearsParameters = new ParameterTheme
                {
                    ApiKey = apiKey,
                    Theme = bricksetTheme.Theme
                };

                theme.SetCountPerYear = _bricksetApiService
                    .GetYears(getYearsParameters)
                    .ToYearSetCountEnumerable()
                    .ToList();

                themeList.Add(theme);

                _themeRepository.AddOrUpdate(theme);

                SynchronizedTheme?.Invoke(this, bricksetTheme.Theme);
            }

            ThemeSynchronizerEnd?.Invoke(this, null);

            return themeList;
        }
    }
}