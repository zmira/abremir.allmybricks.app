using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
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
    public class SubthemeSynchronizer : ISubthemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly ISubthemeRepository _subthemeRepository;
        private readonly IMessageHub _messageHub;

        public SubthemeSynchronizer(
            IBricksetApiService bricksetApiService,
            ISubthemeRepository subthemeRepository,
            IMessageHub messageHub)
        {
            _bricksetApiService = bricksetApiService;
            _subthemeRepository = subthemeRepository;
            _messageHub = messageHub;
        }

        public async Task<IEnumerable<Subtheme>> Synchronize(string apiKey, Theme theme)
        {
            _messageHub.Publish(new SubthemeSynchronizerStart());

            var subthemes = new List<Subtheme>();

            try
            {
                var getSubthemesParameters = new ParameterTheme
                {
                    ApiKey = apiKey,
                    Theme = theme.Name
                };

                var bricksetSubthemes = (await _bricksetApiService.GetSubthemes(getSubthemesParameters)).ToList();

                _messageHub.Publish(new SubthemesAcquired { Theme = theme.Name, Count = bricksetSubthemes.Count });

                foreach (var bricksetSubtheme in bricksetSubthemes)
                {
                    _messageHub.Publish(new SynchronizingSubthemeStart { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });

                    try
                    {
                        var subtheme = bricksetSubtheme.ToSubtheme();

                        subthemes.Add(subtheme);

                        subtheme.Theme = theme;

                        _subthemeRepository.AddOrUpdate(subtheme);
                    }
                    catch (Exception ex)
                    {
                        _messageHub.Publish(new SynchronizingSubthemeException { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme, Exception = ex });
                    }

                    _messageHub.Publish(new SynchronizingSubthemeEnd { Theme = theme.Name, Subtheme = bricksetSubtheme.Subtheme });
                }
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SubthemeSynchronizerException { Theme = theme.Name, Exception = ex });
            }

            _messageHub.Publish(new SubthemeSynchronizerEnd());

            return subthemes;
        }
    }
}
