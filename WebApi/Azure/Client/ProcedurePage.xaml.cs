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
    public sealed partial class ProcedurePage : Page
    {
        private PatientScreenData screenData = new PatientScreenData();
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        public ProcedurePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            populateProcedureList();
            populatePatientProcedures(false);
        }

        private async void populateProcedureList()
        {
            MasterList.ItemsSource = await MobileServiceDotNet.InvokeApiAsync<List<ProcedureCode>>("procedurecode", HttpMethod.Get, null);
        }

        private async void populatePatientProcedures(bool requery)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
            List<ViewPatientProcedure> patientProcedures = await MobileServiceDotNet.InvokeApiAsync<List<ViewPatientProcedure>>("procedurecode", HttpMethod.Get, parameters);
            patientProcedures.ForEach(x => x.ShowRules.userRole = this.screenData.User.Role);
            ProcedureList.ItemsSource = patientProcedures;
        }

        private async void addProcedure(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var assignment = createAssignment(id);
            try
            {
                var data = JToken.FromObject(assignment);
                await MobileServiceDotNet.InvokeApiAsync("procedurecode", data);
                populatePatientProcedures(true);
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

        private void navToChat(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatPage), this.screenData);
        }

        private void navToProvider(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProviderPage), this.screenData);
        }

        private async void completeProcedure(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientProcedureId"] = id.ToString() };
                await MobileServiceDotNet.InvokeApiAsync("procedurecode", HttpMethod.Put, parameters);
                populatePatientProcedures(true);
            }
            catch
            {
                var message = "There was an error while trying to complete a procedure";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }
    }
}
