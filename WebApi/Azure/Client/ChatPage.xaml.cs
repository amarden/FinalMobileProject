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
    public sealed partial class ChatPage : Page
    {
        private PatientScreenData screenData = new PatientScreenData();
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient("http://localhost:6163");

        public ChatPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            populateChat();
        }

        private async void populateChat()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
            var chatLogs = await MobileServiceDotNet.InvokeApiAsync<List<ViewChatLog>>("chat", HttpMethod.Get, parameters);
            ChatLogList.ItemsSource = chatLogs;
        }

        private async void addMessage(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var messageElement = (TextBox)FindName("messageTextBox");
            var chatMessage = messageElement.Text;
            var chat = createMessage(chatMessage);
            messageElement.Text = "";
            try
            {
                var data = JToken.FromObject(chat);
                await MobileServiceDotNet.InvokeApiAsync("Chat", data);
                populateChat();
            }
            catch
            {
                var message = "There was an error while trying to add your message";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        private PatientChatLog createMessage(string message)
        {
            PatientChatLog chat = new PatientChatLog();
            chat.Created = DateTime.Now;
            chat.PatientId = this.screenData.Patient.PatientId;
            chat.ProviderId = this.screenData.User.Id;
            chat.Message = message;
            return chat;
        }

        private void navToProviders(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProviderPage), this.screenData);
        }

        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        private void navToNursePage(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NursePage), this.screenData.User);
        }
    }
}
