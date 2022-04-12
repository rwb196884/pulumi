using Pulumi;
using System.Net;
using KeyVault = Pulumi.AzureNative.KeyVault;
using Resources = Pulumi.AzureNative.Resources;
using Sql = Pulumi.AzureNative.Sql;

namespace rwb196884
{
    class MyStack : Stack
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

            Output<string> administratorLoginPassword = config.RequireSecret("SqlServerAdminPassword");
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

            // Make a key vault.
            string tenantId = config.Require("AzureTenantId"); // Obtained from 
            string principalId = config.Require("AzurePrincipalId");

            KeyVault.Vault v = new KeyVault.Vault($"rwb-key-vault", new KeyVault.VaultArgs() // Name is 3 to 24 letters
            {
                ResourceGroupName = resourceGroup.Name,
                Properties = new KeyVault.Inputs.VaultPropertiesArgs()
                {
                    // https://www.pulumi.com/registry/packages/azure-native/api-docs/keyvault/vault/
                    AccessPolicies =
                {
                    new KeyVault.Inputs.AccessPolicyEntryArgs
                    {
                        ObjectId = principalId,
                        Permissions = new KeyVault.Inputs.PermissionsArgs
                        {
                            Certificates =
                                    {
                                        "get",
                                        "list",
                                        "delete",
                                        "create",
                                        "import",
                                        "update",
                                        "managecontacts",
                                        "getissuers",
                                        "listissuers",
                                        "setissuers",
                                        "deleteissuers",
                                        "manageissuers",
                                        "recover",
                                        "purge",
                                    },
                            Keys =
                                    {
                                        "encrypt",
                                        "decrypt",
                                        "wrapKey",
                                        "unwrapKey",
                                        "sign",
                                        "verify",
                                        "get",
                                        "list",
                                        "create",
                                        "update",
                                        "import",
                                        "delete",
                                        "backup",
                                        "restore",
                                        "recover",
                                        "purge",
                                    },
                            Secrets =
                                    {
                                        "get",
                                        "list",
                                        "set",
                                        "delete",
                                        "backup",
                                        "restore",
                                        "recover",
                                        "purge",
                                    },
                        },
                        TenantId = tenantId
                    }
                },
                    Sku = new KeyVault.Inputs.SkuArgs()
                    {
                        Family = KeyVault.SkuFamily.A,
                        Name = KeyVault.SkuName.Standard
                    },
                    TenantId = tenantId
                }
            });

            // Make the database conneciotn string and store it in the key vault.
            Output<string> cs = administratorLoginPassword.Apply(z => $"Server={sqlServer.FullyQualifiedDomainName}; Database={database.Name}, uid=rwb, pwd={z}");
            KeyVault.Secret s = new KeyVault.Secret($"{PulumiProject}-{ae}-key-vault-secret", new KeyVault.SecretArgs()
            {
                ResourceGroupName = resourceGroup.Name,
                VaultName = v.Name,
                SecretName = "ConnectionString",
                Properties = new KeyVault.Inputs.SecretPropertiesArgs() { Value = cs }
            });

            /* 
             * The value is:

            Server=Calling [ToString] on an [Output<T>] is not supported.

    To get the value of an Output<T> as an Output<string> consider:
    1. o.Apply(v => $"prefix{v}suffix")
    2. Output.Format($"prefix{hostname}suffix");

    See https://pulumi.io/help/outputs for more details.
    This function may throw in a future version of Pulumi.; Database=Calling [ToString] on an [Output<T>] is not supported.

    To get the value of an Output<T> as an Output<string> consider:
    1. o.Apply(v => $"prefix{v}suffix")
    2. Output.Format($"prefix{hostname}suffix");

    See https://pulumi.io/help/outputs for more details.
    This function may throw in a future version of Pulumi., uid=rwb, pwd=Sql-Server-Admin-Password

            */


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