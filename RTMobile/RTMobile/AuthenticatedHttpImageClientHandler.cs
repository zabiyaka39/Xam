using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using AVFoundation;
using System.Threading.Tasks;
using System.Threading;
using Plugin.Settings;
using RTMobile.issues;
using RTMobile.settings;
using Windows.Security.Authentication.Web;

namespace RTMobile
{
    public class AuthenticatedHttpImageClientHandler : HttpClientHandler
    {
        
        public string token;

        public AuthenticatedHttpImageClientHandler()
        {
            token = CrossSettings.Current.GetValueOrDefault("token", string.Empty);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(token);
            request.Headers.Add("Authorization", "Bearer " + token);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
