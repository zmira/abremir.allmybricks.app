﻿using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
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

                var bricksetThemes = (await _bricksetApiService.GetThemes(getThemesParameters)).ToList();

                _messageHub.Publish(new ThemesAcquired { Count = bricksetThemes.Count });

                foreach (var bricksetTheme in bricksetThemes)
                {
                    _messageHub.Publish(new SynchronizingTheme { Theme = bricksetTheme.Theme });

                    try
                    {
                        var theme = bricksetTheme.ToTheme();

                        var getYearsParameters = new ParameterTheme
                        {
                            ApiKey = apiKey,
                            Theme = bricksetTheme.Theme
                        };

                        theme.SetCountPerYear = (await _bricksetApiService.GetYears(getYearsParameters))
                            .ToYearSetCountEnumerable()
                            .ToList();

                        themeList.Add(theme);

                        _themeRepository.AddOrUpdate(theme);
                    }
                    catch (Exception ex)
                    {
                        _messageHub.Publish(new SynchronizingThemeException { Theme = bricksetTheme.Theme, Exception = ex });
                    }

                    _messageHub.Publish(new SynchronizedTheme { Theme = bricksetTheme.Theme });
                }
            }
            catch(Exception ex)
            {
                _messageHub.Publish(new ThemeSynchronizerException { Exception = ex });
            }

            _messageHub.Publish(new ThemeSynchronizerEnd());

            return themeList;
        }
    }
}