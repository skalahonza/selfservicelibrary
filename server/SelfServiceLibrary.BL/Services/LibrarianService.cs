
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
    public class LibrarianService
    {
        private readonly MongoDbContext _dbContext;
        private readonly IMapper _mapper;

        public LibrarianService(MongoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private Task<UpdateResult> AddRole(string username, Role role) =>
            _dbContext
                .Users
                .UpdateOneAsync(
                    x => x.Username == username,
                    Builders<User>.Update.AddToSet(x => x.Roles, role),
                    new UpdateOptions { IsUpsert = true });

        private Task<UpdateResult> RemoveRole(string username, Role role) =>
            _dbContext
                .Users
                .UpdateOneAsync(
                    x => x.Username == username,
                    Builders<User>.Update.Pull(x => x.Roles, role));

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
