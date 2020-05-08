using CodedBeard.GraphAuth.Interfaces;
using CodedBeard.GraphAuth.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CodedBeard.GraphAuth
{
    public class Authentication : IAuthentication
    {
        private readonly string _host;
        private readonly string _tenant;
        private readonly string _client_id;
        private readonly string _scope;
        private readonly string _client_secret;
        public Authentication(IOptions<GraphDetails> graph)
        {
            var options = graph.Value;
            _host = options.Host;
            _tenant = options.Tenant;
            _client_id = options.Client_Id;
            _client_secret = options.Client_Secret;
            _scope = options.Scope;
        }

        public async Task<AccessTokenResult> GetAccessToken()
        {
            var outgoingQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            outgoingQueryString.Add("client_id", _client_id);
            outgoingQueryString.Add("scope", _scope);
            //outgoingQueryString.Add("resource", _resource);
            outgoingQueryString.Add("client_secret", _client_secret);
            outgoingQueryString.Add("grant_type", "client_credentials");
            var postdata = outgoingQueryString.ToString();

            var url = $"{_host}/{_tenant}/oauth2/v2.0/token";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                streamWriter.Write(postdata);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var result = string.Empty;
            using (var httpResponse =  (HttpWebResponse) await httpWebRequest.GetResponseAsync())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            var accessTokenResult = JsonConvert.DeserializeObject<AccessTokenResult>(result);
            return JsonConvert.DeserializeObject<AccessTokenResult>(result);
        }
    }
}