using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient("http://localhost:6163");
        private User user = new User();

        public HomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.user = (User)e.Parameter;
            this.DataContext = this.user;
            getAssignedPatients();
        }

        private async void getAssignedPatients()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<List<Patient>>("Patient", HttpMethod.Get, null);
                if (resultJson != null)
                {
                    PatientList.ItemsSource = resultJson;
                }
            }
            catch
            {
                var message = "There was an error retrieving your profile";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        private void navToPatientPage(object sender, TappedRoutedEventArgs e)
        {
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            PatientScreenData psd = new PatientScreenData();
            psd.Patient = new PatientDetail();
            psd.Patient.PatientId = Convert.ToInt32(id);
            psd.User = this.user;
            this.Frame.Navigate(typeof(PatientPage), psd);
        }
    }
}
