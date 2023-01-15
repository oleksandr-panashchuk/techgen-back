using Humanizer;
using MongoDB.Driver;
using System.Linq;
using System.Reflection;
using Techgen.DAL.Abstract;
using Techgen.Domain.Extentions;

namespace Techgen.DAL
{
    public class DataContext : IDataContext
    {
        /// <summary>
        /// The IMongoClient from the official MongoDB driver
        /// </summary>
        public IMongoClient Client { get; }

        /// <summary>
        /// The IMongoDatabase from the official MongoDB driver
        /// </summary>
        public IMongoDatabase Database { get; }


        /// <summary>
        /// The constructor of the MongoDataContext, it needs an object implementing <see cref="IMongoDatabase"/>.
        /// </summary>
        /// <param name="mongoDatabase">An object implementing IMongoDatabase</param>
        public DataContext(IMongoDatabase mongoDatabase)
        {
            // Avoid legacy UUID representation: use Binary 0x04 subtype.
            InitializeGuidRepresentation();
            Database = mongoDatabase;
            Client = Database.Client;
        }

        /// <summary>
        /// Returns a collection for a document type. Also handles document types with a partition key.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The optional value of the partition key.</param>
        public virtual IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null)
        {
            return Database.GetCollection<TDocument>(GetCollectionName<TDocument>(partitionKey));
        }

        /// <summary>
        /// Drops a collection, use very carefully.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        public virtual void DropCollection<TDocument>(string partitionKey = null)
        {
            Database.DropCollection(GetCollectionName<TDocument>(partitionKey));
        }

        /// <summary>
        /// Sets the Guid representation of the MongoDB Driver.
        /// </summary>
        /// <param name="guidRepresentation">The new value of the GuidRepresentation</param>
        public virtual void SetGuidRepresentation(MongoDB.Bson.GuidRepresentation guidRepresentation)
        {
            MongoDefaults.GuidRepresentation = guidRepresentation;
        }

        /// <summary>
        /// Extracts the CollectionName attribute from the entity type, if any.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The name of the collection in which the TDocument is stored.</returns>
        protected virtual string GetAttributeCollectionName<TDocument>()
        {
            return (typeof(TDocument).GetTypeInfo()
                                     .GetCustomAttributes(typeof(BsonCollectionAttribute))
                                     .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName;
        }

        /// <summary>
        /// Initialize the Guid representation of the MongoDB Driver.
        /// Override this method to change the default GuidRepresentation.
        /// </summary>
        protected virtual void InitializeGuidRepresentation()
        {
            // by default, avoid legacy UUID representation: use Binary 0x04 subtype.
            MongoDefaults.GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard;
        }

        /// <summary>
        /// Given the document type and the partition key, returns the name of the collection it belongs to.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="partitionKey">The value of the partition key.</param>
        /// <returns>The name of the collection.</returns>
        protected virtual string GetCollectionName<TDocument>(string partitionKey)
        {
            var collectionName = GetAttributeCollectionName<TDocument>() ?? Pluralize<TDocument>();
            if (string.IsNullOrEmpty(partitionKey))
            {
                return collectionName;
            }
            return $"{partitionKey}-{collectionName}";
        }

        /// <summary>
        /// Very naively pluralizes a TDocument type name.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The pluralized document name.</returns>
        protected virtual string Pluralize<TDocument>()
        {
            return (typeof(TDocument).Name.Pluralize()).Camelize();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).                  
                    this.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DataContext()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
