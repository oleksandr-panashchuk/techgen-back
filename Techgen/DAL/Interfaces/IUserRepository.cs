using Microsoft.AspNetCore.Mvc;
using Techgen.Domain.Entity;

namespace Techgen.DAL.Interfaces
{
    public interface IUserRepository
    {   
        public Task<List<ApplicationUser>> GetAll();       
        public List<string> GetAllRecovery();
    }
}

