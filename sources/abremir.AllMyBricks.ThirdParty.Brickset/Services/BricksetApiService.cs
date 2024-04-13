using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.ThirdParty.Brickset.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Enumerations;
using abremir.AllMyBricks.ThirdParty.Brickset.Extensions;
using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;
using Flurl;
using Flurl.Http;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Services
{
    public class BricksetApiService : IBricksetApiService
    {
        public async Task<bool> CheckKey(ParameterApiKey checkKeyParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultCheckKey, ParameterApiKey>(checkKeyParameters).ConfigureAwait(false)).Status is ResultStatus.Success;
        }

        public async Task<string> Login(ParameterLogin loginParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultLogin, ParameterLogin>(loginParameters).ConfigureAwait(false)).Hash;
        }

        public async Task<bool> CheckUserHash(ParameterApiKeyUserHash checkUserHashParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultCheckUserHash, ParameterApiKeyUserHash>(checkUserHashParameters).ConfigureAwait(false)).Status is ResultStatus.Success;
        }

        public async Task<IEnumerable<Themes>> GetThemes(ParameterApiKey getThemesParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetThemes, ParameterApiKey>(getThemesParameters).ConfigureAwait(false)).Themes;
        }

        public async Task<IEnumerable<Subthemes>> GetSubthemes(ParameterTheme getSubthemesParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetSubthemes, ParameterTheme>(getSubthemesParameters).ConfigureAwait(false)).Subthemes;
        }

        public async Task<IEnumerable<Years>> GetYears(ParameterTheme getYearsParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetYears, ParameterTheme>(getYearsParameters).ConfigureAwait(false)).Years;
        }

        public async Task<IEnumerable<Sets>> GetSets(GetSetsParameters getSetsParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetSets, ParameterSets>(getSetsParameters.ToParameterSets()).ConfigureAwait(false)).Sets;
        }

        public async Task<IEnumerable<Instructions>> GetInstructions(ParameterSetId getInstructionsParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetInstructions, ParameterSetId>(getInstructionsParameters).ConfigureAwait(false)).Instructions;
        }

        public async Task<IEnumerable<SetImage>> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultGetAdditionalImages, ParameterSetId>(getAdditionalImagesParameters).ConfigureAwait(false)).AdditionalImages;
        }

        public async Task<bool> SetCollection(SetCollectionParameters setCollectionParameters)
        {
            return (await BricksetHttpPostUrlEncodeAsync<ResultSetCollection, ParameterSetCollection>(setCollectionParameters.ToParameterSetCollection()).ConfigureAwait(false)).Status is ResultStatus.Success;
        }

        private static async Task<T> BricksetHttpPostUrlEncodeAsync<T, U>(U parameters) where T : ResultBase where U : ParameterApiKey
        {
            var requestResult = await Constants.BricksetApiUrl
                .WithSettings((settings) => settings.JsonSerializer = BricksetJsonSerializer.JsonSerializer)
                .AppendPathSegment(typeof(T).GetDescription())
                .PostUrlEncodedAsync(parameters)
                .ReceiveJson<T>()
                .ConfigureAwait(false);

            if (requestResult.Status is ResultStatus.Error)
            {
                throw new BricksetRequestException(requestResult.Message);
            }

            return requestResult;
        }
    }
}
