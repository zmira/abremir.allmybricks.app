using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Interfaces
{
    public interface IBricksetApiService
    {
        bool CheckKey(ParameterApiKey checkKeyParameters);
        string Login(ParameterLogin loginParameters);
        string CheckUserHash(ParameterUserHash checkUserHashParameters);
        IEnumerable<Themes> GetThemes(ParameterApiKey getThemesParameters);
        IEnumerable<Subthemes> GetSubthemes(ParameterTheme getSubthemesParameters);
        IEnumerable<Years> GetYears(ParameterTheme getYearsParameters);
        IEnumerable<Sets> GetSets(ParameterSets getSetsParameters);
        Sets GetSet(ParameterUserHashSetId getSetParameters);
        IEnumerable<Sets> GetRecentlyUpdatedSets(ParameterMinutesAgo getRecentlyUpdatedSetsParameters);
        IEnumerable<Instructions> GetInstructions(ParameterSetId getInstructionsParameters);
        IEnumerable<AdditionalImages> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters);
        IEnumerable<Reviews> GetReviews(ParameterSetId getReviewsParameters);
    }
}