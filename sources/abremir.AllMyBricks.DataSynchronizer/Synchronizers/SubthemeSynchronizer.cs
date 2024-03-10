using System;
using System.Linq;
using System.Threading.Tasks;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.Onboarding.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SubthemeSynchronizer : ISubthemeSynchronizer
    {
        private readonly IOnboardingService _onboardingService;
        private readonly IBricksetApiService _bricksetApiService;
        private readonly IThemeRepository _themeRepository;
        private readonly ISubthemeRepository _subthemeRepository;
        private readonly IMessageHub _messageHub;

        public SubthemeSynchronizer(
            IOnboardingService onboardingService,
            IBricksetApiService bricksetApiService,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository,
            IMessageHub messageHub)
        {
            _onboardingService = onboardingService;
            _bricksetApiService = bricksetApiService;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
            _messageHub = messageHub;
        }

        public async Task Synchronize()
        {
            _messageHub.Publish(new SubthemeSynchronizerStart());

            var apiKey = await _onboardingService.GetBricksetApiKey().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var exception = new Exception("Invalid Brickset API key");
                _messageHub.Publish(new ThemeSynchronizerException { Exception = exception });

                throw exception;
            }

            try
            {
                var themes = await _themeRepository.All().ConfigureAwait(false);

                _messageHub.Publish(new ThemesAcquired { Count = themes.Count() });

                foreach (var theme in themes)
                {
                    var getSubthemesParameters = new ParameterTheme
                    {
                        ApiKey = apiKey,
                        Theme = theme.Name
                    };

                    var bricksetSubthemes = (await _bricksetApiService.GetSubthemes(getSubthemesParameters).ConfigureAwait(false)).ToList();

                    _messageHub.Publish(new SubthemesAcquired { Theme = theme.Name, Count = bricksetSubthemes.Count });

                    foreach (var bricksetSubtheme in bricksetSubthemes)
                    {
                        _messageHub.Publish(new SynchronizingSubthemeStart { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });

                        var subtheme = bricksetSubtheme.ToSubtheme();

                        subtheme.Theme = theme;

                        var persistedSubtheme = await _subthemeRepository.Get(subtheme.Theme.Name, subtheme.Name).ConfigureAwait(false);

                        if (persistedSubtheme != null)
                        {
                            subtheme.Id = persistedSubtheme.Id;
                        }

                        await _subthemeRepository.AddOrUpdate(subtheme).ConfigureAwait(false);

                        _messageHub.Publish(new SynchronizingSubthemeEnd { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });
                    }
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SubthemeSynchronizerException { Exception = ex });

                throw;
            }

            _messageHub.Publish(new SubthemeSynchronizerEnd());
        }
    }
}
