using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DBU_Advising_Scheduler.TokenStorage;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System;

namespace DBU_Advising_Scheduler.Helpers
{
    public static class GraphHelper
    {
        // Load configuration settings from PrivateSettings.config
        private static string appId = ConfigurationManager.AppSettings["ida:AppId"];
        private static string appSecret = ConfigurationManager.AppSettings["ida:AppSecret"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static string graphScopes = ConfigurationManager.AppSettings["ida:AppScopes"];

        public static GraphServiceClient GetAuthenticatedClient()
        {
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var idClient = ConfidentialClientApplicationBuilder.Create(appId)
                            .WithRedirectUri(redirectUri)
                            .WithClientSecret(appSecret)
                            .Build();

                        var tokenStore = new SessionTokenStore(idClient.UserTokenCache,
                                HttpContext.Current, ClaimsPrincipal.Current);

                        var accounts = await idClient.GetAccountsAsync();

                // By calling this here, the token can be refreshed
                // if it's expired right before the Graph call is made
                var scopes = graphScopes.Split(' ');
                        var result = await idClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();

                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
        }

        public static async Task<User> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                    }));

            return await graphClient.Me.Request().GetAsync();
        }

        public static async Task<string> GetCoursesCalendarId()
        {
            GraphServiceClient graphClient = GraphHelper.GetAuthenticatedClient();

            // Get all calendars
            var calendars = await graphClient.Me.Calendars
                                .Request()
                                .GetAsync();


            // Find courses calendar if exists
            bool found = false;
            string id = "";

            foreach (var c in calendars)
            {
                if (c.Name == "Courses")
                {
                    found = true;
                    id = c.Id;
                }
            }

            // If not found, create calendar
            if (!found)
            {
                var calendar = new Calendar
                {
                    Name = "Courses"
                };
                var cal = await graphClient.Me.Calendars
                    .Request()
                    .AddAsync(calendar);

                id = cal.Id;
            }

            return id;
        }
    }
}