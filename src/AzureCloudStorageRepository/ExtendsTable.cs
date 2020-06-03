using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace AzureCloudStorageRepository
{
    public static class ExtendsTable
    {
        public static async Task DeleteIfEmpty(this CloudTable me)
        {
            if (me.Exists())
            {
                var query = new TableQuery<TableEntity>().Take(1);
                var items = me.ExecuteQuery(query);
                if (!items.Any())
                {
                    await me.DeleteAsync();
                }
            }
        }

    }
}