using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GraphForge.Api.Auth
{
    public class AuthOptions
    {
        public string Issuer = "GraphForge";

        public string Audience = "GraphForgeClient";

        public string Key = "";
    }
}
