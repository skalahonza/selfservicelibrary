
using CVUT.Auth;

using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Retry;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CVUT.Usermap
{
    public class AccessTokenCoordinator : IRequestCoordinator
    {
        private readonly ZuulClient _zuul;
        private readonly string _clientId;
        private readonly string _clientSecret;

        private string? token;
        private DateTime? ttl;

        public AccessTokenCoordinator(ZuulClient zuul, string clientId, string clientSecret)
        {
            _zuul = zuul;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }


        /// <summary>Dispatch an HTTP request.</summary>
        /// <param name="request">The response message to validate.</param>
        /// <param name="send">Dispatcher that executes the request.</param>
        /// <returns>The final HTTP response.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(IRequest request, Func<IRequest, Task<HttpResponseMessage>> send)
        {
            if (string.IsNullOrEmpty(token) || ttl < DateTime.UtcNow)
            {
                var response = await _zuul.GetToken(_clientId, _clientSecret);
                token = response.AccessToken;
                ttl = DateTime.UtcNow.AddSeconds(response.ExpiresIn.GetValueOrDefault()).AddMinutes(-5); // five minutes before actual expiration
            }

            return await send(request.WithBearerAuthentication(token!));
        }
    }
}
