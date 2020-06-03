using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;

namespace AzureCloudStorageRepository
{
    public class TableStorageRepository<T> : ITableStorageRepository<T> where T : ITableEntity, new()
    {
        public string TableName { get; set; }
        private CloudTableClient TableClient { get; set; }
        public TableStorageRepository(CloudTableClient tableClient)
        {
            TableClient = tableClient;
            TableName = typeof(T).Name;
        }

        public ITableStorageRepository<T> OverrideTableName(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public ITableStorageRepository<T> ResetTableName()
        {
            TableName = typeof(T).Name;
            return this;
        }


        public async Task<IEnumerable<T>> GetAsync()
        {
            var table = TableClient.GetTableReference(TableName);

            var entities = new List<T>();

            if (table.Exists())
            {
                TableContinuationToken token = null;
                do
                {
                    var queryResult = await table.CreateQuery<T>().ExecuteSegmentedAsync(token);
                    entities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);
            }

            return entities;
        }

        public async Task<IEnumerable<T>> GetAsync(Predicate<T> func)
        {
            var table = TableClient.GetTableReference(TableName);
            var entities = new List<T>();

            if (table.Exists())
            {
                TableContinuationToken token = null;
                do
                {
                    var queryResult = await table.CreateQuery<T>().Where(o => func(o)).AsTableQuery().ExecuteSegmentedAsync(token);
                    entities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);
            }

            return entities;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var table = TableClient.GetTableReference(TableName);
            if (table.Exists())
            {
                var retrieveOperation = TableOperation.Retrieve<T>(TableName, id);
                var result = await table.ExecuteAsync(retrieveOperation);

                if ((result.Result is T resultEntity)) return resultEntity;
            }
            return default;

        }

        public async Task<T> InsertAsync(T entity)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var insertOrMergeOperation = TableOperation.Insert(entity);

            var a = await table.ExecuteAsync(insertOrMergeOperation);
            return (T)a.Result;
        }

        public async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var batch = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batch.Insert(entity);
            }
            var a = await table.ExecuteBatchAsync(batch);
            return a.Select(o => (T) o.Result);


        }

        public async Task<T> InsertOrReplaceAsync(T entity)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);

            var a = await table.ExecuteAsync(insertOrMergeOperation);
            return (T)a.Result;
        }

        public async Task<IEnumerable<T>> InsertOrReplaceAsync(IEnumerable<T> entities)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var batch = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batch.InsertOrReplace(entity);
            }
            var a = await table.ExecuteBatchAsync(batch);
            return a.Select(o => (T)o.Result);


        }

        public async Task<T> InsertOrMergeAsync(T entity)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            var a = await table.ExecuteAsync(insertOrMergeOperation);
            return (T)a.Result;
        }

        public async Task<IEnumerable<T>> InsertOrMergeAsync(IEnumerable<T> entities)
        {
            var table = TableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var batch = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batch.InsertOrMerge(entity);
            }
            var a = await table.ExecuteBatchAsync(batch);
            return a.Select(o => (T)o.Result);


        }

        public async Task DeleteAsync(string id, bool deleteTableIfEmpty = false)
        {
            var table = TableClient.GetTableReference(TableName);
            var entity = await GetByIdAsync(id);

            if (table.Exists())
            {
                var deleteOperation = TableOperation.Delete(entity);

                await table.ExecuteAsync(deleteOperation);
                if (deleteTableIfEmpty) await table.DeleteIfEmpty();
            }
        }

        public async Task DeleteAsync(Predicate<T> func, bool deleteTableIfEmpty = false)
        {
            var table = TableClient.GetTableReference(TableName);
            var entities = await GetAsync(func);

            if (table.Exists())
            {
                var batch = new TableBatchOperation();
                foreach (var tableEntity in entities)
                {
                    batch.Delete(tableEntity);
                }

                await table.ExecuteBatchAsync(batch);
                if(deleteTableIfEmpty) await table.DeleteIfEmpty();

            }
        }
    }
}
