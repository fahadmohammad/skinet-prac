using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;


namespace API.ThirdPartyAuth
{
    public class GoogleAuth : IGoogleAuth
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _googleAuth;
        public GoogleAuth(IConfiguration configuration)
        {
            _configuration = configuration;
            _googleAuth = _configuration.GetSection("Authentication:Google");
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _googleAuth["ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);

                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}