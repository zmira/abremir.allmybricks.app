namespace abremir.AllMyBricks.ThirdParty.Brickset.Models
{
    public class ParameterLogin : ParameterApiKey
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}