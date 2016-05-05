using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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
    public sealed partial class ProviderPage : Page
    {
        private PatientScreenData screenData = new PatientScreenData();
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());
        private List<ViewPatientProvider> providers;

        public ProviderPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            populateProviderList();
            populatePatientProvider(true);
        }

        private async void populateProviderList()
        {
            MasterList.ItemsSource = await MobileServiceDotNet.InvokeApiAsync<List<ViewProvider>>("provider", HttpMethod.Get, null);
        }

        private async void populatePatientProvider(bool requery)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
            this.providers = await MobileServiceDotNet.InvokeApiAsync<List<ViewPatientProvider>>("assignment", HttpMethod.Get, parameters);
            ProviderList.ItemsSource = providers;
        }

        private async void deleteProvider(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            try
            {
       
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientProviderId"] = id };
                await MobileServiceDotNet.InvokeApiAsync("assignment", HttpMethod.Delete, parameters);
                populatePatientProvider(true);
            }
            catch
            {
                var message = "There was an error while trying to delete a provider";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        private async void addProvider(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var assignment = createAssignment(id);
            try
            {
                if (this.providers.Any(x => x.ProviderId == Convert.ToInt32(id)))
                {
                    throw new InvalidDataException();
                }
                var data = JToken.FromObject(assignment);
                await MobileServiceDotNet.InvokeApiAsync("assignment", data);
                populatePatientProvider(true);

            }
            catch (InvalidDataException)
            {
                var message = "The provider you chose was already assigned";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
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

        private PatientProvider createAssignment(string id)
        {
            PatientProvider pp = new PatientProvider();
            pp.AssignedDate = DateTime.Now;
            pp.ProviderId = Convert.ToInt32(id);
            pp.PatientId = this.screenData.Patient.PatientId;
            pp.Active = true;
            return pp;
        }

        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        private void navToChat(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatPage), this.screenData);
        }
    }
}
