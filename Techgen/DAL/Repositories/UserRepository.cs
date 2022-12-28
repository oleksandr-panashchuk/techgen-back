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
        private readonly IMongoCollection<ApplicationUser> users;

        public UserRepository(IMongoDatabase mongoDatabase)
        {
            users = mongoDatabase.GetCollection<ApplicationUser>("Users");
        }

        public async Task<List<ApplicationUser>> GetAll() =>
            await users.Find(_ => true).ToListAsync();

        public List<string> GetAllRecovery() =>
            users.AsQueryable().Select(x => x.RecoveryCode).ToList();
    }
}
