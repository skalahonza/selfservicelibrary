
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Extensions;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.DAL;
using SelfServiceLibrary.DAL.Entities;
using SelfServiceLibrary.DAL.Enums;

namespace SelfServiceLibrary.BL.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> AddRole(string username, Role role) =>
            await _dbContext
                .Users
                .UpdateOneAsync(
                    x => x.Username == username,
                    Builders<User>.Update.AddToSet(x => x.Roles, role),
                    new UpdateOptions { IsUpsert = true }) switch
            {
                { MatchedCount: 1, ModifiedCount: 0 } => false, // user was already in a role
                _ => true
            };

        public async Task<bool> RemoveRole(string username, Role role) =>
            await _dbContext
                .Users
                .UpdateOneAsync(
                    x => x.Username == username,
                    Builders<User>.Update.Pull(x => x.Roles, role)) switch
            {
                { MatchedCount: 1, ModifiedCount: 0 } => false, // user was not in a role
                _ => true
            };

        public Task<List<UserListDTO>> GetAll() =>
            _dbContext
                .Users
                .AsQueryable()
                .OrderBy(x => x.Username)
                .ProjectTo<User, UserListDTO>(_mapper)
                .ToListAsync();

        public Task<List<UserListDTO>> GetAll(Role role) =>
            _dbContext
                .Users
                .AsQueryable()
                .Where(x => x.Roles.Contains(role))
                .ProjectTo<User, UserListDTO>(_mapper)
                .ToListAsync();

        public Task<bool> IsInRole(string username, Role role) =>
            _dbContext
                .Users
                .Find(x => x.Username == username && x.Roles.Contains(role))
                .AnyAsync();

        public Task<HashSet<Role>> GetRoles(string username) =>
            _dbContext
                .Users
                .Find(x => x.Username == username)
                .Project(x => x.Roles)
                .FirstOrDefaultAsync();
    }
}
