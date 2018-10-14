using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
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
        private readonly IDataSynchronizerEventManager _dataSynchronizerEventHandler;

        public ThemeSynchronizer(
            IBricksetApiService bricksetService,
            IThemeRepository themeRepository,
            IDataSynchronizerEventManager dataSynchronizerEventHandler)
        {
            _bricksetApiService = bricksetService;
            _themeRepository = themeRepository;
            _dataSynchronizerEventHandler = dataSynchronizerEventHandler;
        }

        public IEnumerable<Theme> Synchronize(string apiKey)
        {
            _dataSynchronizerEventHandler.Raise(new ThemeSynchronizerStart());

            var themeList = new List<Theme>();

            try
            {
                var getThemesParameters = new ParameterApiKey
                {
                    ApiKey = apiKey
                };

                var bricksetThemes = _bricksetApiService.GetThemes(getThemesParameters).ToList();

                _dataSynchronizerEventHandler.Raise(new ThemesAcquired { Count = bricksetThemes.Count });

                foreach (var bricksetTheme in bricksetThemes)
                {
                    _dataSynchronizerEventHandler.Raise(new SynchronizingTheme { Name = bricksetTheme.Theme });

                    try
                    {
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
                    }
                    catch (Exception ex)
                    {
                        _dataSynchronizerEventHandler.Raise(new SynchronizingThemeException { Name = bricksetTheme.Theme, Exception = ex });
                    }

                    _dataSynchronizerEventHandler.Raise(new SynchronizedTheme { Name = bricksetTheme.Theme });
                }
            }
            catch(Exception ex)
            {
                _dataSynchronizerEventHandler.Raise(new ThemeSynchronizerException { Exception = ex });
            }

            _dataSynchronizerEventHandler.Raise(new ThemeSynchronizerEnd());

            return themeList;
        }
    }
}