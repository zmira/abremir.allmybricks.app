using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Interfaces
{
    public interface IBricksetApiService
    {
        Task<bool> CheckKey(ParameterApiKey checkKeyParameters);
        Task<string> Login(ParameterLogin loginParameters);
        Task<string> CheckUserHash(ParameterUserHash checkUserHashParameters);
        Task<IEnumerable<Themes>> GetThemes(ParameterApiKey getThemesParameters);
        Task<IEnumerable<Subthemes>> GetSubthemes(ParameterTheme getSubthemesParameters);
        Task<IEnumerable<Years>> GetYears(ParameterTheme getYearsParameters);
        Task<IEnumerable<Sets>> GetSets(ParameterSets getSetsParameters);
        Task<Sets> GetSet(ParameterUserHashSetId getSetParameters);
        Task<IEnumerable<Sets>> GetRecentlyUpdatedSets(ParameterMinutesAgo getRecentlyUpdatedSetsParameters);
        Task<IEnumerable<Instructions>> GetInstructions(ParameterSetId getInstructionsParameters);
        Task<IEnumerable<AdditionalImages>> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters);
        Task<IEnumerable<Reviews>> GetReviews(ParameterSetId getReviewsParameters);
        Task<string> SetCollection(ParameterSetCollection setCollectionParameters);
        Task<string> SetCollectionOwns(ParameterSetCollectionOwns setCollectionOwnsParameters);
        Task<string> SetCollectionWants(ParameterSetCollectionWants setCollectionWantsParameters);
        Task<string> SetCollectionQtyOwned(ParameterSetCollectionQtyOwned setCollectionQtyOwnedParameters);
    }
}
