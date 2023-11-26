using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using abremir.AllMyBricks.Onboarding.Shared.Configuration;
using abremir.AllMyBricks.Onboarding.Shared.Extensions;
using abremir.AllMyBricks.Onboarding.Shared.Models;

namespace abremir.AllMyBricks.Onboarding.Shared.Security
{
    public class HmacDelegatingHandler : DelegatingHandler
    {
        public HmacDelegatingHandler(HttpMessageHandler next) : base(next) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content is null)
            {
                return null;
            }

            var apiKeyRequest = JsonSerializer.Deserialize<ApiKeyRequest>(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            var appId = apiKeyRequest.DeviceIdentification.DeviceHash;
            var apiKey = apiKeyRequest.RegistrationHash;

            var requestUri = Uri.EscapeDataString(request.RequestUri.ToString());
            var requestHttpMethod = request.Method.Method;
            var nonce = Guid.NewGuid().ToString("N");
            var requestTimestamp = DateTime.UtcNow.TotalSecondsFromEpochStart().ToString();

            var requestContentHash = SHA256Hash.ComputeHash(await request.Content.ReadAsStreamAsync().ConfigureAwait(false));
            var requestContentBase64String = Convert.ToBase64String(requestContentHash);

            var signatureRawData = appId + requestHttpMethod + requestUri + requestTimestamp + nonce + requestContentBase64String;

            var secretKeyByteArray = Encoding.UTF8.GetBytes(apiKey);

            var signature = Encoding.UTF8.GetBytes(signatureRawData);

            using var hmac = new HMACSHA256(secretKeyByteArray);

            var signatureBytes = hmac.ComputeHash(signature);
            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.HmacAuthenticationScheme, $"{appId}:{requestSignatureBase64String}:{nonce}:{requestTimestamp}");

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
