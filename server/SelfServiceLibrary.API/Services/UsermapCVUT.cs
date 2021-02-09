using Microsoft.AspNetCore.Http;

using Pathoschild.Http.Client;

using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.BL.Model;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SelfServiceLibrary.API.Services
{
    public class UsermapCVUT : IUserContextService
    {
        private readonly IClient _client;

        public UsermapCVUT(IHttpClientFactory factory, IHttpContextAccessor contextAccessor)
        {
            _client = new FluentClient(new Uri("https://kosapi.fit.cvut.cz/usermap/v1"), factory.CreateClient());
            _client.AddDefault(x => x.WithHeader("Authorization", contextAccessor.HttpContext?.Request.Headers["Authorization"]));
        }

        public Task<UserContext> GetInfo(string username) =>
            _client
                .GetAsync($"people/{username}")
                .As<UserContext>();
    }
}
