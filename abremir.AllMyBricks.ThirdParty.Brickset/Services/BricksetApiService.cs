using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Extensions;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Xml;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Services
{
    public class BricksetApiService : IBricksetApiService
    {
        public bool CheckKey(ParameterApiKey checkKeyParameters)
        {
            return GetBricksetData<ResultCheckKey, ResultCheckKey, ParameterApiKey>(checkKeyParameters).Value.Equals(Constants.ResponseOk);
        }

        public string Login(ParameterLogin loginParameters)
        {
            return GetBricksetData<ResultLogin, ResultLogin, ParameterLogin>(loginParameters).Value;
        }

        public string CheckUserHash(ParameterUserHash checkUserHashParameters)
        {
            return GetBricksetData<ResultCheckUserHash, ResultCheckUserHash, ParameterUserHash>(checkUserHashParameters).Value;
        }

        public IEnumerable<Themes> GetThemes(ParameterApiKey getThemesParameters)
        {
            return GetBricksetData<ArrayOfThemes, IEnumerable<Themes>, ParameterApiKey>(getThemesParameters);
        }

        public IEnumerable<Subthemes> GetSubthemes(ParameterTheme getSubthemesParameters)
        {
            return GetBricksetData<ArrayOfSubthemes, IEnumerable<Subthemes>, ParameterTheme>(getSubthemesParameters);
        }

        public IEnumerable<Years> GetYears(ParameterTheme getYearsParameters)
        {
            return GetBricksetData<ArrayOfYears, IEnumerable<Years>, ParameterTheme>(getYearsParameters);
        }

        public IEnumerable<Sets> GetSets(ParameterSets getSetsParameters)
        {
            return GetBricksetData<ArrayOfSets, IEnumerable<Sets>, ParameterSets>(getSetsParameters);
        }

        public Sets GetSet(ParameterUserHashSetId getSetParameters)
        {
            return GetBricksetData<ArrayOfSet, IEnumerable<Sets>, ParameterUserHashSetId>(getSetParameters).FirstOrDefault();
        }

        public IEnumerable<Sets> GetRecentlyUpdatedSets(ParameterMinutesAgo getRecentlyUpdatedSetsParameters)
        {
            return GetBricksetData<ArrayOfRecentlyUpdatedSets, IEnumerable<Sets>, ParameterMinutesAgo>(getRecentlyUpdatedSetsParameters);
        }

        public IEnumerable<Instructions> GetInstructions(ParameterSetId getInstructionsParameters)
        {
            return GetBricksetData<ArrayOfInstructions, IEnumerable<Instructions>, ParameterSetId>(getInstructionsParameters);
        }

        public IEnumerable<AdditionalImages> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters)
        {
            return GetBricksetData<ArrayOfAdditionalImages, IEnumerable<AdditionalImages>, ParameterSetId>(getAdditionalImagesParameters);
        }

        public IEnumerable<Reviews> GetReviews(ParameterSetId getReviewsParameters)
        {
            return GetBricksetData<ArrayOfReviews, IEnumerable<Reviews>, ParameterSetId>(getReviewsParameters);
        }

        private U GetBricksetData<T, U, V>(V parameters) where T: class where U : class where V : class
        {
            return Constants.BricksetApiUrl.AppendPathSegment(typeof(T).GetDescription()).PostUrlEncodedAsync(parameters).ReceiveXml<T>().Result as U;
        }
    }
}
