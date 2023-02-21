using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MongoDB.Driver;
using MongoDbGenericRepository;
using System;
using System.Threading.Tasks;

namespace Techgen.DAL.Abstract
{
    public interface IDataContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        IModel Model { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}
