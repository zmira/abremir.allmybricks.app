using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class ThemeSynchronizer : IThemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly IThemeRepository _themeRepository;
        private readonly IMessageHub _messageHub;

        public ThemeSynchronizer(
            IBricksetApiService bricksetService,
            IThemeRepository themeRepository,
            IMessageHub messageHub)
        {
            _bricksetApiService = bricksetService;
            _themeRepository = themeRepository;
            _messageHub = messageHub;
        }

        public async Task<IEnumerable<Theme>> Synchronize(string apiKey)
        {
            _messageHub.Publish(new ThemeSynchronizerStart());

            var themeList = new List<Theme>();

            try
            {
                var getThemesParameters = new ParameterApiKey
                {
                    ApiKey = apiKey
                };

                var bricksetThemes = (await _bricksetApiService.GetThemes(getThemesParameters).ConfigureAwait(false)).ToList();

                _messageHub.Publish(new ThemesAcquired { Count = bricksetThemes.Count });

                foreach (var bricksetTheme in bricksetThemes)
                {
                    _messageHub.Publish(new SynchronizingThemeStart { Theme = bricksetTheme.Theme });

                    try
                    {
                        var theme = bricksetTheme.ToTheme();

                        var getYearsParameters = new ParameterTheme
                        {
                            ApiKey = apiKey,
                            Theme = bricksetTheme.Theme
                        };

                        theme.SetCountPerYear = (await _bricksetApiService.GetYears(getYearsParameters).ConfigureAwait(false))
                            .ToYearSetCountEnumerable()
                            .ToList();

                        var persistedTheme = _themeRepository.Get(theme.Name);

                        if (persistedTheme != null)
                        {
                            theme.Id = persistedTheme.Id;
                        }

                        _themeRepository.AddOrUpdate(theme);

                        themeList.Add(theme);
                    }
                    catch (Exception ex)
                    {
                        _messageHub.Publish(new SynchronizingThemeException { Theme = bricksetTheme.Theme, Exception = ex });
                    }

                    _messageHub.Publish(new SynchronizingThemeEnd { Theme = bricksetTheme.Theme });
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new ThemeSynchronizerException { Exception = ex });
            }

            _messageHub.Publish(new ThemeSynchronizerEnd());

            return themeList;
        }
    }
}
