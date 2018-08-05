using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Extensions;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SubthemeSynchronizer : ISubthemeSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly ISubthemeRepository _subthemeRepository;

        public SubthemeSynchronizer(
            IBricksetApiService bricksetApiService,
            ISubthemeRepository subthemeRepository)
        {
            _bricksetApiService = bricksetApiService;
            _subthemeRepository = subthemeRepository;
        }

        public IEnumerable<Subtheme> Synchronize(string apiKey, Theme theme)
        {
            var subthemes = new List<Subtheme>();

            var getSubthemesParameters = new ParameterTheme
            {
                ApiKey = apiKey,
                Theme = theme.Name
            };

            foreach (var bricksetSubtheme in _bricksetApiService
                .GetSubthemes(getSubthemesParameters))
            {
                var subtheme = bricksetSubtheme.ToSubtheme();

                subthemes.Add(subtheme);

                subtheme.Theme = theme;

                _subthemeRepository.AddOrUpdate(subtheme);
            }

            return subthemes;
        }
    }
}