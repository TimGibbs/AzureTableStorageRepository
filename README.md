# AzureTableStorageRepository
Generic repositiory for storing items in Azure table storage.
Stored items need to implement ITableEntity


Implementation
```
    builder.Services
      .AddTransient(o => 
        CloudStorageAccount.Parse(o.GetService<IConfiguration>()["AzureWebJobsStorage"]).CreateCloudTableClient());
    builder.Services
      .AddTransient(typeof(ITableStorageRepository<>), typeof(TableStorageRepository<>));
```
Items will be stored and retrieved from a table named after the class unless overwritten.
```
    var result = await _tableStorageRepository
                .OverrideTableName(item)
                .GetByIdAsync(id);
```
