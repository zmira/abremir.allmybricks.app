using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Extensions;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Services
{
    public class BricksetApiService : IBricksetApiService
    {
        public async Task<bool> CheckKey(ParameterApiKey checkKeyParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultCheckKey, ResultCheckKey, ParameterApiKey>(checkKeyParameters)).Value.Equals(Constants.ResponseOk);
        }

        public async Task<string> Login(ParameterLogin loginParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultLogin, ResultLogin, ParameterLogin>(loginParameters)).Value;
        }

        public async Task<string> CheckUserHash(ParameterUserHash checkUserHashParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultCheckUserHash, ResultCheckUserHash, ParameterUserHash>(checkUserHashParameters)).Value;
        }

        public async Task<IEnumerable<Themes>> GetThemes(ParameterApiKey getThemesParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfThemes, IEnumerable<Themes>, ParameterApiKey>(getThemesParameters);
        }

        public async Task<IEnumerable<Subthemes>> GetSubthemes(ParameterTheme getSubthemesParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfSubthemes, IEnumerable<Subthemes>, ParameterTheme>(getSubthemesParameters);
        }

        public async Task<IEnumerable<Years>> GetYears(ParameterTheme getYearsParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfYears, IEnumerable<Years>, ParameterTheme>(getYearsParameters);
        }

        public async Task<IEnumerable<Sets>> GetSets(ParameterSets getSetsParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfSets, IEnumerable<Sets>, ParameterSets>(getSetsParameters);
        }

        public async Task<Sets> GetSet(ParameterUserHashSetId getSetParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ArrayOfSet, IEnumerable<Sets>, ParameterUserHashSetId>(getSetParameters)).FirstOrDefault();
        }

        public async Task<IEnumerable<Sets>> GetRecentlyUpdatedSets(ParameterMinutesAgo getRecentlyUpdatedSetsParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfRecentlyUpdatedSets, IEnumerable<Sets>, ParameterMinutesAgo>(getRecentlyUpdatedSetsParameters);
        }

        public async Task<IEnumerable<Instructions>> GetInstructions(ParameterSetId getInstructionsParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfInstructions, IEnumerable<Instructions>, ParameterSetId>(getInstructionsParameters);
        }

        public async Task<IEnumerable<AdditionalImages>> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfAdditionalImages, IEnumerable<AdditionalImages>, ParameterSetId>(getAdditionalImagesParameters);
        }

        public async Task<IEnumerable<Reviews>> GetReviews(ParameterSetId getReviewsParameters)
        {
            return await BricksetHttpPostUrlEncodeAsync<ArrayOfReviews, IEnumerable<Reviews>, ParameterSetId>(getReviewsParameters);
        }

        private async Task<U> BricksetHttpPostUrlEncodeAsync<T, U, V>(V parameters) where T: class where U : class where V : class
        {
            return await Constants.BricksetApiUrl.AppendPathSegment(typeof(T).GetDescription()).PostUrlEncodedAsync(parameters).ReceiveXml<T>() as U;
        }
    }
}
