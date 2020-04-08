namespace abremir.AllMyBricks.ThirdParty.Brickset.Models.Parameters
{
    public class ParameterLogin : ParameterApiKey
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
