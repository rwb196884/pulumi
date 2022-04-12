using Pulumi;
using System.Net;
using Resources = Pulumi.AzureNative.Resources;
using Sql = Pulumi.AzureNative.Sql;

namespace rwb196884
{
    public class MyStack : Stack
    {
        private const string PulumiProject = "rwb196884";

        private static string GetExternalIP()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            Pulumi.Log.Info($"GetExternalIP is {externalIpString}.");
            return IPAddress.Parse(externalIpString).ToString(); // Parse it to check that it's valid.
        }

        public MyStack()
        {
            // Read the yml file for the stack.
            Config config = new Pulumi.Config();

            string applicationName = config.Require("ApplicationName");
            string environmentName = config.Require("EnvironmentName");
            //string loc = config.Require("azure-native:location");
            // It seems that we can only read configuration values that start with 'rwb196884' and to do so we must omit the prefix.

            Pulumi.Log.Info($"ApplicationName is {applicationName}.");
            Pulumi.Log.Info($"EnvironmentName is {environmentName}.");

            string ae = $"{applicationName}-{environmentName}";

            // Create an Azure Resource Group
            Resources.ResourceGroup? resourceGroup = new Resources.ResourceGroup($"{PulumiProject}-{ae}-resource-group");
            // We'll do everything in here so it's easy to find in the Azure website and delete it when we're done.

            //string administratorLoginPassword = "SqlServerAdminPassword"; // This will fail because the password doesn't meet the policy. But the resource group is still created; pulumi does not tidy up after an error.
            string administratorLoginPassword = "Sql-Server-Admin-Password-123";
            Sql.Server sqlServer = new Sql.Server($"{PulumiProject}-{ae}-sql-server", new Sql.ServerArgs()
            {
                ResourceGroupName = resourceGroup.Name,
                AdministratorLogin = "rwb",
                AdministratorLoginPassword = administratorLoginPassword
            });

            string ip = GetExternalIP();
            Sql.FirewallRule firewallRule = new Sql.FirewallRule("rwb_2022-04-12", new Sql.FirewallRuleArgs()
            {
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
                StartIpAddress = ip,
                EndIpAddress = ip,

            });

            // Create a database.
            Sql.Database database = new Sql.Database($"{PulumiProject}-{ae}-sql-database", new Pulumi.AzureNative.Sql.DatabaseArgs()
            {
                ResourceGroupName = resourceGroup.Name,
                ServerName = sqlServer.Name,
            });


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