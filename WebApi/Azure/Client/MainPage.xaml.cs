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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        public class ProviderType
        {
            public string role { get; set; }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void login(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<User>("login", HttpMethod.Get, null);
                if (resultJson != null)
                {
                    if (resultJson.Role == "Nurse")
                    {
                        this.Frame.Navigate(typeof(NursePage), resultJson);
                    }
                    else if(resultJson.Role == "SuperUser")
                    {
                        this.Frame.Navigate(typeof(SuperUserPage));
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(HomePage), resultJson);
                    }
                }
            }
            catch (MobileServiceInvalidOperationException)
            {
                var message = "You have not registered or logged in. Please do so";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            catch
            {
                var message = "There was an error retrieving your profile";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }

        private async void register(object sender, TappedRoutedEventArgs e)
        {
            var btn = (Button)sender;
            var role = btn.Content;
            ProviderType pt = new ProviderType();
            pt.role = role.ToString();
            try
            {
                await MobileServiceDotNet.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
                var data = JToken.FromObject(pt);
                await MobileServiceDotNet.InvokeApiAsync("registration", data);
            }
            catch
            {
                var message = "There was a problem during registration";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }
    }
}
