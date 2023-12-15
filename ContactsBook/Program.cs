using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Threading.Tasks;

namespace ContactsBook
{
    class Program
    {
        static string connectionString = "DefaultEndpointsProtocol=https;AccountName=nazarstorage;AccountKey=PsA1JRcOoznKp/iFToKZWlF2Z4YuVURmdOEnt+HUjt0Tk4VjzDAR6rZvztU7uP9VTyV3Wj6pirGk+ASt5RBlrQ==;EndpointSuffix=core.windows.net";

        static string rowKey = "PhoneBbook";
        static string partitionKey = "2";

        public static async Task Main(string[] args)
        {
            await ProcessAsync();

            Console.WriteLine("program finished...");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            TableServiceClient client = new TableServiceClient(connectionString);
            var table = await CreateTable(client);
            InsertRecord(client, table);
            ReadRecord(client, table);
            DeleteTable(client, table);
        }

        private static void DeleteTable(TableServiceClient client, TableItem table)
        {
            client.DeleteTable(table.Name);
        }

        private static void ReadRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            Pageable<TableEntity> queryResultsFilter = table.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");
            foreach (TableEntity qEntity in queryResultsFilter)
            {
                Console.WriteLine($"{qEntity.GetString("LastName")} {qEntity.GetString("FirstName")}: {qEntity.GetString("PhonNumber")},{qEntity.GetString("Adress")};Photo:{qEntity.GetString("PhotoUrl")}");
            }
        }

        private static void InsertRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            var entity = new TableEntity(partitionKey, rowKey)
            {
                {"FirstName", "Nazar" },
                {"LastName", "Mikliush" },
                {"Adress", "Lviv" },
                {"PhonNumber","380931177011" },
                {"PhotoUrl", "https://nazarstorage.blob.core.windows.net/photo/img.jpg"},
            };
            table.AddEntity(entity);
        }

        private static async Task<TableItem> CreateTable(TableServiceClient client)
        {
            string tableName = "ContactBook";
            TableItem table = client.CreateTable(tableName);
            return table;
        }
    }
}
