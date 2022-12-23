using Microsoft.AspNetCore.Mvc;
using Techgen.Domain.Entity;

namespace Techgen.DAL.Interfaces
{
    public interface IUserRepository
    {   
        public Task<List<User>> GetAll();
        public Task<User> Get(string id);
        public Task Update(string id, User user);
        public Task Create(User user);
        public Task Delete(string id);
        public Task<User> FindUser(string userEmail);
    }
}

