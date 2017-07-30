using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Emotions.Model;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Emotions.DataModels;

namespace Emotions
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });


            await MakePredictionRequest(file);

            await countEmotions();

            file.Dispose();
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }


        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "334b9559ecee4b7fb3f42f877694f513");

            string url = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                
                response = await client.PostAsync(url, content);
           


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();


                    List<ResponseModel> responseModel = JsonConvert.DeserializeObject<List<ResponseModel>>(responseString);

                    ResponseModel your_face = responseModel[0];

                    if(responseModel.Count > 1)
                    {
                        await DisplayAlert("More than 1 face?", "Sorry, only the first face recognized will be scanned.", "OK");
                    }



                    double anger = your_face.scores.anger;
                    double contempt = your_face.scores.contempt;
                    double disgust = your_face.scores.disgust;
                    double fear = your_face.scores.fear;
                    double happiness = your_face.scores.happiness;
                    double neutral = your_face.scores.neutral;
                    double sadness = your_face.scores.sadness;
                    double surprise = your_face.scores.surprise;

                    double[] emotion_list = new double[] { anger, contempt, disgust, fear, happiness, neutral, sadness, surprise };

                    double maxEmotion = emotion_list.Max();     //getting max emotion

                    string your_emotion = " ";                        //final result to print.

                  

                    if (maxEmotion == anger)
                    {
                        your_emotion = "You are angry >:(";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == contempt)
                    {
                        your_emotion = "You are contempt.";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == disgust)
                    {
                        your_emotion = "You are disgusted.";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == fear)
                    {
                        your_emotion = "You are scared.";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == happiness)
                    {
                        your_emotion = "You are happy :D";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == neutral)
                    {
                        your_emotion = "You are neutral :|";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == sadness)
                    {
                        your_emotion = "You are sad :(";
                        TagLabel.Text = your_emotion;
                    }

                    if (maxEmotion == surprise)
                    {
                        your_emotion = "You are surprised!";
                        TagLabel.Text = your_emotion;
                    }

                
                        FarsheedEmotionsModel table_emote = new FarsheedEmotionsModel
                        {
                            Emotion = your_emotion
                        };

                        await AzureManager.AzureManagerInstance.PostEmotionsInformation(table_emote);
               


                    //------------------------------------------------------------------------------------------

                 

                    //Get rid of file once we have finished using it.
                    file.Dispose();
                }
                else
                {
                    await DisplayAlert("ERROR", "API Error", "OK");
                }
            }
        }

        private async Task countEmotions()
        {
           var emotion_count = await AzureManager.AzureManagerInstance.GetEmotionsInformation();

            int num_of_emotions = emotion_count.Count;

            await DisplayAlert("Number of Emotions", "You are person number " + num_of_emotions + " to use this app.", "OK");
        }
    }
}
