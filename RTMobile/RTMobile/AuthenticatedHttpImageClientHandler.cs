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

        public AuthenticatedHttpImageClientHandler()
        {
           
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue ("Basic",
		    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{CrossSettings.Current.GetValueOrDefault("login", String.Empty)}:{CrossSettings.Current.GetValueOrDefault("password", string.Empty)}")));
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
