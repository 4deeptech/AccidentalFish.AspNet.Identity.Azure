using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.AspNet.Identity.Azure
{
    public class TableUserLogin : TableEntity
    {
        public TableUserLogin()
        {
            
        }

        public TableUserLogin(string userId, string loginProvider, string providerKey)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            //DMF RowKey cannot have certain characters in it!!!
            ProviderKey = providerKey.Replace("/", "").Replace("\\", "").Replace("#", "").Replace("?", "");

            SetPartitionAndRowKey();
        }

        public void SetPartitionAndRowKey()
        {
            PartitionKey = UserId;
            //DMF RowKey cannot have certain characters in it!!!
            RowKey = ProviderKey;
        }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string UserId { get; set; }
    }
}
