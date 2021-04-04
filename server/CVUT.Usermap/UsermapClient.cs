
using CVUT.Auth;
using CVUT.Usermap.Model;

using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CVUT.Usermap
{
    public class UsermapClient : IUserContextService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public UsermapClient(HttpClient client, IMapper mapper, ZuulClient zuul, IOptions<UsermapClientOptions> options)
        {
            _mapper = mapper;
            _client = new FluentClient(new Uri("https://kosapi.fit.cvut.cz/usermap/v1"), client);
            var coordinator = new AccessTokenCoordinator(zuul, options.Value.ClientId, options.Value.ClientSecret);
            _client.SetRequestCoordinator(coordinator);
        }

        public Task<User> Get(string username, string token) =>
            _client
                .GetAsync($"people/{username}")
                .WithBearerAuthentication(token)
                .As<User>();

        public Task<User> Get(string username) =>
            _client
                .GetAsync($"people/{username}")
                .As<User>();

        public async Task<List<UserInfoDTO>> Suggest(string term, int limit = 10)
        {
            var users = await _client
                .GetAsync($"people")
                .WithArgument("query", $"name==\"{term}\"")
                .WithArgument("limit", limit)
                .AsArray<User>();

            // try find by username
            try
            {
                var user = await Get(term);
                return _mapper.Map<List<UserInfoDTO>>(users.Take(limit - 1).Prepend(user));
            }
            catch (ApiException ex) when (ex.Status == HttpStatusCode.NotFound)
            {
                // not found by username
                return _mapper.Map<List<UserInfoDTO>>(users);
            }
        }
    }
}
