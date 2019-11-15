using abremir.AllMyBricks.Core.Configuration;
using abremir.AllMyBricks.Core.Extensions;
using abremir.AllMyBricks.Core.Models;
using fastJSON;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Core.Security
{
    public class HmacDelegatingHandler : DelegatingHandler
    {
        public HmacDelegatingHandler(HttpMessageHandler next) : base(next) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = request.Content as StringContent;

            if(content == null)
            {
                return null;
            }

            var apiKeyRequest = JSON.ToObject<ApiKeyRequest>(await content.ReadAsStringAsync());
            var appId = apiKeyRequest.DeviceIdentification.DeviceHash;
            var apiKey = apiKeyRequest.RegistrationHash;

            var requestUri = UriHelper.Encode(request.RequestUri);
            var requestHttpMethod = request.Method.Method;
            var nonce = Guid.NewGuid().ToString("N");
            var requestContentBase64String = string.Empty;
            var requestTimestamp = DateTime.UtcNow.TotalSecondsFromEpochStart().ToString();

            if (request.Content != null)
            {
                var requestContentHash = SHA256Hash.ComputeHash(await request.Content.ReadAsStreamAsync().ConfigureAwait(false));
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            var signatureRawData = appId + requestHttpMethod + requestUri + requestTimestamp + nonce + requestContentBase64String;

            var secretKeyByteArray = Encoding.UTF8.GetBytes(apiKey);

            var signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (var hmac = new HMACSHA256(secretKeyByteArray))
            {
                var signatureBytes = hmac.ComputeHash(signature);
                var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.HmacAuthenticationScheme, $"{appId}:{requestSignatureBase64String}:{nonce}:{requestTimestamp}");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
