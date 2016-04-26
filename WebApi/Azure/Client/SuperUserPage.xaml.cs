using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class SuperUserPage : Page
    {
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient("http://localhost:6163");

        public class PatientCreate
        {
            public int number { get; set; }
        }

        public SuperUserPage()
        {
            this.InitializeComponent();
        }

        private async void createPatients(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var numberText = (TextBox)FindName("numberOfPatients");
            var number = numberText.Text;
            if (await isNumber(number))
            {
                try
                {
                    PatientCreate pc = new PatientCreate();
                    pc.number = Convert.ToInt32(number);
                    var data = JToken.FromObject(pc);
                    await MobileServiceDotNet.InvokeApiAsync("patient", data);
                    var message = number + " patients were created";
                    var dialog = new MessageDialog(message);
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                }
                catch
                {
                    var message = "There was a problem trying to create new patients";
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

        private async Task<bool> isNumber(string number)
        {
            int realNumber;
            if (Int32.TryParse(number, out realNumber))
            {
                return true;
            }
            else
            {
                var message = "You must enter a number";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
                return false;
            }
        }
    }
}
