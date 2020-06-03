using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureCloudStorageRepository
{
    public interface ITableStorageRepository<T>
    {
        Task<IEnumerable<T>> GetAsync();
        Task<IEnumerable<T>> GetAsync(Predicate<T> func);
        Task<T> GetByIdAsync(string id);
        Task<T> InsertAsync(T entity);
        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities);
        Task<T> InsertOrReplaceAsync(T entity);
        Task<IEnumerable<T>> InsertOrReplaceAsync(IEnumerable<T> entities);
        Task<T> InsertOrMergeAsync(T entity);
        Task<IEnumerable<T>> InsertOrMergeAsync(IEnumerable<T> entities);
        Task DeleteAsync(string id, bool deleteTableIfEmpty = false);
        Task DeleteAsync(Predicate<T> func, bool deleteTableIfEmpty = false);
        ITableStorageRepository<T> OverrideTableName(string tableName);
        ITableStorageRepository<T> ResetTableName();
    }
}