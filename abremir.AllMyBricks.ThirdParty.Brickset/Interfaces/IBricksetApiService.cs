using System.Collections.Generic;
using System.Threading.Tasks;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Interfaces
{
    public interface IBricksetApiService
    {
        Task<bool> CheckKey(ParameterApiKey checkKeyParameters);
        Task<string> Login(ParameterLogin loginParameters);
        Task<bool> CheckUserHash(ParameterApiKeyUserHash checkUserHashParameters);
        Task<IEnumerable<Themes>> GetThemes(ParameterApiKey getThemesParameters);
        Task<IEnumerable<Subthemes>> GetSubthemes(ParameterTheme getSubthemesParameters);
        Task<IEnumerable<Years>> GetYears(ParameterTheme getYearsParameters);
        Task<IEnumerable<Sets>> GetSets(GetSetsParameters getSetsParameters);
        Task<IEnumerable<Instructions>> GetInstructions(ParameterSetId getInstructionsParameters);
        Task<IEnumerable<SetImage>> GetAdditionalImages(ParameterSetId getAdditionalImagesParameters);
        Task<bool> SetCollection(SetCollectionParameters setCollectionParameters);
    }
}
