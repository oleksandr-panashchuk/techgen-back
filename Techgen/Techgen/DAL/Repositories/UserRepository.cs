using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Techgen.DAL.Interfaces;
using Techgen.Domain.DB;
using Techgen.Domain.Entity;

namespace Techgen.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> users;

        public UserRepository(IMongoDatabase mongoDatabase)
        {
            users = mongoDatabase.GetCollection<User>("Users");
        }

        public async Task Create(User user) =>      
            await users.InsertOneAsync(user);
         
        public async Task Delete(string id) =>
            await users.DeleteOneAsync(x => x.Id == id);

        public async Task<User> Get(string id) =>                 
            await users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User> FindUser(string userEmail) =>
            await users.Find(x => x.Email == userEmail).FirstOrDefaultAsync();

        public async Task<List<User>> GetAll() =>
            await users.Find(_ => true).ToListAsync();

        public async Task Update(string id, User user) =>
            await users.ReplaceOneAsync(x => x.Id == id, user);
       
    }
}
