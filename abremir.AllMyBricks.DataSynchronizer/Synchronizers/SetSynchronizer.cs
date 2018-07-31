using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

namespace abremir.AllMyBricks.DataSynchronizer.Synchronizers
{
    public class SetSynchronizer : ISetSynchronizer
    {
        private readonly IBricksetApiService _bricksetApiService;
        private readonly ISetRepository _setRepository;
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly ISubthemeRepository _subthemeRepository;

        public SetSynchronizer(
            IBricksetApiService bricksetApiService,
            ISetRepository setRepository,
            IReferenceDataRepository referenceDataRepository,
            IThemeRepository themeRepository,
            ISubthemeRepository subthemeRepository)
        {
            _bricksetApiService = bricksetApiService;
            _setRepository = setRepository;
            _referenceDataRepository = referenceDataRepository;
            _themeRepository = themeRepository;
            _subthemeRepository = subthemeRepository;
        }

        public bool Synchronize(string apiKey, Theme theme, Subtheme subtheme)
        {
            for (var year = subtheme.YearFrom; year <= subtheme.YearTo; year++)
            {
                var getSetsParameters = new ParameterSets
                {
                    ApiKey = apiKey,
                    Theme = theme.Name,
                    Subtheme = subtheme.Name,
                    Year = year.ToString()
                };

                foreach (var bricksetSet in _bricksetApiService
                    .GetSets(getSetsParameters))
                {
                    _setRepository.AddOrUpdate(MapSet(theme, subtheme, bricksetSet));
                }
            }

            return true;
        }

        private Set MapSet(Theme theme, Subtheme subtheme, Sets bricksetSet)
        {
            var set = new Set
            {
                Theme = theme,
                Subtheme = subtheme,
                Category = _referenceDataRepository.GetOrAdd<Category>(bricksetSet.Category),
                PackagingType = _referenceDataRepository.GetOrAdd<PackagingType>(bricksetSet.PackagingType),
                ThemeGroup = _referenceDataRepository.GetOrAdd<ThemeGroup>(bricksetSet.ThemeGroup),
                SetId = bricksetSet.SetId,
                Number = bricksetSet.Number,
                Name = bricksetSet.Name,
                Description = bricksetSet.Description,
                Ean = bricksetSet.Ean,
                Upc = bricksetSet.Upc,
                NumberVariant = (byte)bricksetSet.NumberVariant,
                Year = string.IsNullOrWhiteSpace(bricksetSet.Year) ? (short?)null : short.Parse(bricksetSet.Year),
                Pieces = string.IsNullOrWhiteSpace(bricksetSet.Pieces) ? (short?)null : short.Parse(bricksetSet.Pieces),
                Minifigs = string.IsNullOrWhiteSpace(bricksetSet.Minifigs) ? (short?)null : short.Parse(bricksetSet.Minifigs),
                BricksetUrl = bricksetSet.BricksetUrl,
                Released = bricksetSet.Released,
                OwnedByTotal = (short)bricksetSet.OwnedByTotal,
                WantedByTotal = (short)bricksetSet.WantedByTotal,
                Rating = (float)bricksetSet.Rating,
                Availability = bricksetSet.Availability,
                AgeMin = string.IsNullOrWhiteSpace(bricksetSet.AgeMin) ? (byte?)null : byte.Parse(bricksetSet.AgeMin),
                AgeMax = string.IsNullOrWhiteSpace(bricksetSet.AgeMax) ? (byte?)null : byte.Parse(bricksetSet.AgeMax),
                Height = string.IsNullOrWhiteSpace(bricksetSet.Height) ? (short?)null : byte.Parse(bricksetSet.Height),
                Width = string.IsNullOrWhiteSpace(bricksetSet.Width) ? (short?)null : byte.Parse(bricksetSet.Width),
                Depth = string.IsNullOrWhiteSpace(bricksetSet.Depth) ? (short?)null : byte.Parse(bricksetSet.Depth),
                Weight = string.IsNullOrWhiteSpace(bricksetSet.Weight) ? (short?)null : byte.Parse(bricksetSet.Weight),
                Notes = bricksetSet.Notes,
                LastUpdated = bricksetSet.LastUpdated
            };

            return set;
        }
    }
}