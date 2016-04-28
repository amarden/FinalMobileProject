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
            List<ViewPatientProcedure> patientProcedures = await MobileServiceDotNet.InvokeApiAsync<List<ViewPatientProcedure>>("patientimaging", HttpMethod.Get, parameters);
            //ImageList.ItemsSource = patientProcedures;
        }

        private async void addImage(object sender, RoutedEventArgs e)
        {
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

        

        private async void submitImage(byte[] imageStream)
        {
            MyProgressBar.IsIndeterminate = true;
            var newImage = new SubmitImage();
            //try
            //{
                newImage.ImageStream = imageStream;
                newImage.PatientId = this.screenData.Patient.PatientId;
                newImage.ImageType = "MRI";
                var data = JToken.FromObject(newImage);
                await MobileServiceDotNet.InvokeApiAsync("patientimaging", data);
                populateImages();
            //}
            //catch
            //{
                //var message = "There was an error while trying to add a provider";
                //var dialog = new MessageDialog(message);
                //dialog.Commands.Add(new UICommand("OK"));
                //await dialog.ShowAsync();
            //}
            //finally
            //{
            //    MyProgressBar.IsIndeterminate = false;
            //}
        }

        private PatientProcedure createAssignment(string id)
        {
            PatientProcedure pp = new PatientProcedure();
            pp.AssignedTime = DateTime.Now;
            pp.ProcedureCodeId = Convert.ToInt32(id);
            pp.PatientId = this.screenData.Patient.PatientId;
            pp.Completed = false;
            pp.CompletedTime = null;
            return pp;
        }

        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        private void test(object sender, TappedRoutedEventArgs e)
        {
            string la = "la";
            string test = "fa";
        }
    }
}
