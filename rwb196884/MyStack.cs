using Pulumi;
using System;
using System.Diagnostics;

namespace rwb196884
{
    class MyStack : Stack
    {
        public MyStack()
        {
            Console.WriteLine("Hello World from Console.WriteLine."); // Outputs on `pulumi up` and into the log at https://app.pulumi.com/.../rwb196884/dev/updates/1.
            Debug.WriteLine("Hello World from Console.WriteLine."); // Doesn't do anything.
            Pulumi.Log.Info("Hello World from Pulumi.Log.Info."); // Outputs on `pulumi up` and into the log at https://app.pulumi.com/.../rwb196884/dev/updates/1


            //// Create an Azure Resource Group
            //var resourceGroup = new ResourceGroup("resourceGroup");

            //// Create an Azure resource (Storage Account)
            //var storageAccount = new StorageAccount("sa", new StorageAccountArgs
            //{
            //    ResourceGroupName = resourceGroup.Name,
            //    Sku = new SkuArgs
            //    {
            //        Name = SkuName.Standard_LRS
            //    },
            //    Kind = Kind.StorageV2
            //});

            //// Export the primary key of the Storage Account
            //this.PrimaryStorageKey = Output.Tuple(resourceGroup.Name, storageAccount.Name).Apply(names =>
            //    Output.CreateSecret(GetStorageAccountPrimaryKey(names.Item1, names.Item2)));
        }

        //[Output]
        //public Output<string> PrimaryStorageKey { get; set; }

        //private static async Task<string> GetStorageAccountPrimaryKey(string resourceGroupName, string accountName)
        //{
        //    var accountKeys = await ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs
        //    {
        //        ResourceGroupName = resourceGroupName,
        //        AccountName = accountName
        //    });
        //    return accountKeys.Keys[0].Value;
        //}
    }
}