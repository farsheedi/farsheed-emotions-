using Emotions.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Emotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTable : ContentPage
    {



        public AzureTable()
        {
            InitializeComponent();

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<FarsheedEmotionsModel> EmotionsInformation = await AzureManager.AzureManagerInstance.GetEmotionsInformation();

            EmotionsList.ItemsSource = EmotionsInformation;
        }

    }
}