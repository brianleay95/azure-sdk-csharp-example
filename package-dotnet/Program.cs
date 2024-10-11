using System;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using System.Threading.Tasks;

namespace AzureStorageExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Authenticate using DefaultAzureCredential
            var credential = new DefaultAzureCredential();

            // Define subscription ID
           string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

            // Create a Resource Management client
            var armClient = new ArmClient(credential, subscriptionId);

            // Define resource group and storage account names
            string resourceGroupName = "myResourceGroup";
            string storageAccountName = "mystorageaccount" + Guid.NewGuid().ToString().Substring(0, 8); // Ensure globally unique name
            string region = "eastus";

            // Create a new resource group
            SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
            ResourceGroupData resourceGroupData = new ResourceGroupData(new AzureLocation("eastus")); // Use AzureLocation properly
            var resourceGroupLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, resourceGroupData);
            ResourceGroupResource resourceGroup = resourceGroupLro.Value;

            Console.WriteLine($"Resource group '{resourceGroupName}' created.");

            // Create the storage account
            StorageAccountCreateOrUpdateContent storageAccountCreateParams = new StorageAccountCreateOrUpdateContent(
                new StorageSku(StorageSkuName.StandardLrs), // Standard Locally Redundant Storage (LRS)
                StorageKind.StorageV2,
                new AzureLocation(region) // Use AzureLocation properly
            )
            {
                AccessTier = StorageAccountAccessTier.Hot // Use StorageAccountAccessTier instead of AccessTier
            };

            var storageAccountLro = await resourceGroup.GetStorageAccounts().CreateOrUpdateAsync(WaitUntil.Completed, storageAccountName, storageAccountCreateParams);
            StorageAccountResource storageAccount = storageAccountLro.Value;

            Console.WriteLine($"Storage account '{storageAccountName}' created with blob storage support.");
        }
    }
}
