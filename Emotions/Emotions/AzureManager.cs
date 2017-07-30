using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Emotions.DataModels;

namespace Emotions
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<FarsheedEmotionsModel> FarsheedEmotionsTable;
      

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://farsheedemotions.azurewebsites.net");
            this.FarsheedEmotionsTable = this.client.GetTable<FarsheedEmotionsModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<FarsheedEmotionsModel>> GetEmotionsInformation()
        {
            return await this.FarsheedEmotionsTable.ToListAsync();
        }

        public async Task PostEmotionsInformation(FarsheedEmotionsModel farsheedEmotionsModel)
        {
            await this.FarsheedEmotionsTable.InsertAsync(farsheedEmotionsModel);
        }
    }
}
