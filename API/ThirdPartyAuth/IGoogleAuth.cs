using System.Threading.Tasks;
using API.Dtos;
using Google.Apis.Auth;


namespace API.ThirdPartyAuth
{
    public interface IGoogleAuth
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth);
    }
}