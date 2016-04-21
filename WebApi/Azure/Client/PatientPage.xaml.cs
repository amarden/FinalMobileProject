using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientPage : Page
    {
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient("http://localhost:6163");
        private PatientScreenData screenData = new PatientScreenData();

        public PatientPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = screenData;
            if (screenData.Patient.Name == null)
            {
                getPatient(screenData.Patient.PatientId);
            }
        }

        private async void getPatient(int patientId)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"]=patientId.ToString() };
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<PatientDetail>("Patient", HttpMethod.Get, parameters);
                if (resultJson != null)
                {
                    this.screenData.Patient = resultJson;
                    BiometricList.ItemsSource = screenData.Patient.Biometrics;
                    this.DataContext = screenData;
                }
            }
            catch
            {
                var message = "There was an error retrieving this patient";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        private void navToProviders(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProviderPage), this.screenData);
        }

        private void navToProcedure(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProcedurePage), this.screenData);
        }

        private void navToChat(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatPage), this.screenData);
        }

        private async void discharge(object sender, TappedRoutedEventArgs e)
        {
            var parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
            await MobileServiceDotNet.InvokeApiAsync("Patient", HttpMethod.Put, parameters);
        }

        private void backToPatients(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HomePage), this.screenData.User);
        }
    }
}
