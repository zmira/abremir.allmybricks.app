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
    public class SubthemeSynchronizer : ISubthemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly ISubthemeRepository _subthemeRepository;

        public event EventHandler SubthemeSynchronizerStart;
        public event EventHandler SubthemeSynchronizerEnd;
        public event EventHandler<int> SubthemesAcquired;
        public event EventHandler<string> SynchronizingSubtheme;
        public event EventHandler<string> SynchronizedSubtheme;

        public SubthemeSynchronizer(
            IBricksetApiService bricksetApiService,
            ISubthemeRepository subthemeRepository)
        {
            _bricksetApiService = bricksetApiService;
            _subthemeRepository = subthemeRepository;
        }

        public IEnumerable<Subtheme> Synchronize(string apiKey, Theme theme)
        {
            SubthemeSynchronizerStart?.Invoke(this, null);

            var subthemes = new List<Subtheme>();

            var getSubthemesParameters = new ParameterTheme
            {
                ApiKey = apiKey,
                Theme = theme.Name
            };

            var bricksetSubthemes = _bricksetApiService.GetSubthemes(getSubthemesParameters).ToList();

            SubthemesAcquired?.Invoke(this, bricksetSubthemes.Count);

            foreach (var bricksetSubtheme in bricksetSubthemes)
            {
                SynchronizingSubtheme?.Invoke(this, bricksetSubtheme.Subtheme);

                var subtheme = bricksetSubtheme.ToSubtheme();

                subthemes.Add(subtheme);

                subtheme.Theme = theme;

                _subthemeRepository.AddOrUpdate(subtheme);

                SynchronizedSubtheme?.Invoke(this, bricksetSubtheme.Subtheme);
            }

            SubthemeSynchronizerEnd?.Invoke(this, null);

            return subthemes;
        }
    }
}