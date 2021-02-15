
using Pathoschild.Http.Client;

using SelfServiceLibrary.Domain.Entities;
using SelfServiceLibrary.Service.Interfaces;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CVUT.Usermap
{
    public class UsermapClient : IUserProvider
    {
        private readonly IClient _client;

        public UsermapClient(HttpClient client) =>
            _client = new FluentClient(new Uri("https://kosapi.fit.cvut.cz/usermap/v1"), client);

        public Task<User> Get(string username) =>
            _client
                .GetAsync($"people/{username}")
                .As<User>();
    }
}
