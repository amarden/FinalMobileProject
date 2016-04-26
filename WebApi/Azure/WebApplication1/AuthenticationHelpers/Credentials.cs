using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace Azure.AuthenticationHelpers
{
    /// <summary>
    /// A helper class that gets information from the api caller and then gets their credentials from the appropriate provider 
    /// </summary>
    public class Credentials
    {
        private MobileAppSettingsDictionary ConfigSettings;
        private IPrincipal User;
        HttpRequestMessage Request;

        //Set our class variables with api information
        public Credentials(IPrincipal User, MobileAppSettingsDictionary config, HttpRequestMessage Request)
        {
            this.ConfigSettings = config;
            this.User = User;
            this.Request = Request;
        }

        /// <summary>
        /// Determines provider type and then sends request to provider to get information on user, returns user information
        /// </summary>
        /// <returns></returns>
        public async Task<UserSubmit> GetUserInfo()
        {
            var userSubmit = new UserSubmit();
            TwitterCredentials twitterCredentials = await User.GetAppServiceIdentityAsync<TwitterCredentials>(Request);
            userSubmit = await getCredentials(twitterCredentials);
            return userSubmit;
        }

        /// <summary>
        /// Gets user information from Twitter
        /// </summary>
        /// <param name="twitterCredentials"></param>
        /// <returns>User information in form of User Submit class</returns>
        private async Task<UserSubmit> getCredentials(TwitterCredentials twitterCredentials)
        {
            UserSubmit userInfo = new UserSubmit();
            userInfo.Provider = "Twitter";
            if (twitterCredentials == null)
            {
                return userInfo;
            }


            // Extract twitter consumer secret and key
            string twitterConsumerSecret = ConfigSettings["WEBSITE_AUTH_TWITTER_CONSUMER_SECRET"];
            string twitterConsumerKey = ConfigSettings["WEBSITE_AUTH_TWITTER_CONSUMER_KEY"];

            // Note: The twitterCredentials.UserId maps to a screen name in twitter.
            // Find out more about this twitter API here: https://dev.twitter.com/rest/reference/get/users/show
            Uri twitterUri = new Uri("https://api.twitter.com/1.1/users/show.json?screen_name=" + twitterCredentials.UserId);

            // Create an HTTP Client with the OAuthMessageHander to support OAuth
            // Authentication to the twitter IDP
            HttpClient httpClient = new HttpClient(
                new OAuthMessageHandler(twitterConsumerKey,
                                        twitterConsumerSecret,
                                        twitterCredentials.AccessToken,
                                        twitterCredentials.AccessTokenSecret,
                                        new HttpClientHandler()));

            // Setup the request criteria, here using the HttpMethod.Get
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, twitterUri);

            // Request json results
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Make the call to the twitter IDP to get the information
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            // If the service responded with the information we requested parse the results
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                userInfo = JsonConvert.DeserializeObject<UserSubmit>(result);
                userInfo.Provider = "Twitter";
            }

            return userInfo;
        }
    }
}
