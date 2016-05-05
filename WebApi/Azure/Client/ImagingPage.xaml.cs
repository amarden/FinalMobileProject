using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagingPage : Page
    {
        private PatientScreenData screenData = new PatientScreenData();
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());
        private string imageType { get; set; }

        public ImagingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            populateImages();
        }

        private async void populateImages()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
            List<PatientImaging> patientImages = await MobileServiceDotNet.InvokeApiAsync<List<PatientImaging>>("patientimaging", HttpMethod.Get, parameters);
            ImageList.ItemsSource = patientImages;
        }

        private async void addImage(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.imageType == "" || this.imageType == null)
                {
                    throw new Exception();
                }
                FileOpenPicker fp = new FileOpenPicker(); // Adding filters for the file type to access.         
                fp.FileTypeFilter.Add(".jpeg");
                fp.FileTypeFilter.Add(".png");
                fp.FileTypeFilter.Add(".pf");
                fp.FileTypeFilter.Add(".jpg");
                // Using PickSingleFileAsync() will return a storage file which can be saved into an object of storage file class.          
                StorageFile sf = await fp.PickSingleFileAsync();
                // Adding bitmap image object to store the stream provided by the object of StorageFile defined above.BitmapImage bmp = new BitmapImage();           
                // Reading file as a stream and saving it in an object of IRandomAccess. 
                byte[] fileBytes = null;
                using (IRandomAccessStreamWithContentType stream = await sf.OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                }
                BitmapImage tempBitMap = new BitmapImage(new Uri(sf.Path));
                PatientImage.Source = tempBitMap;
                PatientImage.Visibility = Visibility.Visible;
                Stream fileStream = new MemoryStream(fileBytes);
                submitImage(fileBytes);
            }
            catch
            {
                var message = "Please choose image type before uploading an image";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }

        private async void submitImage(byte[] imageStream)
        {
            MyProgressBar.IsIndeterminate = true;
            var newImage = new SubmitImage();
            try
            {
                newImage.ImageStream = imageStream;
                newImage.PatientId = this.screenData.Patient.PatientId;
                newImage.ImageType = "MRI";
                var data = JToken.FromObject(newImage);
                await MobileServiceDotNet.InvokeApiAsync("patientimaging", data);
                this.imageType = "";
                populateImages();
            }
            catch
            {
                var message = "There was an error while trying to add a provider";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var obj = (RadioButton)sender;
            this.imageType = obj.Content.ToString();
        }

        private async void viewImage(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var data = new ImageNavScreenData() { BlobId = id, screenData = this.screenData };
            this.Frame.Navigate(typeof(ViewImage), data);
        }
    }
}
