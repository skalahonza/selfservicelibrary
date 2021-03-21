
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.Domain.Enums;
using SelfServiceLibrary.Persistence;
using SelfServiceLibrary.Persistence.Entities;
using SelfServiceLibrary.Service.DTO.User;
using SelfServiceLibrary.Service.Extensions;
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Service.Services
{
    public class AdminService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public AdminService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private Task<UpdateResult> AddRole(string username, Role role) =>
            _dbContext
                .Users
                .UpdateOneAsync(x => x.Username == username, Builders<User>.Update.AddToSet(x => x.Roles, role));

        private Task<UpdateResult> RemoveRole(string username, Role role) =>
            _dbContext
                .Users
                .UpdateOneAsync(x => x.Username == username, Builders<User>.Update.Pull(x => x.Roles, role));

        public Task<List<UserListDTO>> GetAll() =>
            _dbContext
                .Users
                .AsQueryable()
                .Where(x => x.Roles.Contains(Role.Librarian))
                .ProjectTo<User, UserListDTO>(_mapper)
                .ToListAsync();

        public Task<bool> IsLibrarian(string username) =>
            _dbContext
                .Users
                .Find(x => x.Username == username && x.Roles.Contains(Role.Librarian))
                .AnyAsync();

        public Task AddLibrarian(string username)
        {
            // TODO handle user not found
            return AddRole(username, Role.Librarian);
        }

        public Task RemoveLibrarian(string username)
        {
            // TODO handle user not found
            return RemoveRole(username, Role.Librarian);
        }
    }
}
