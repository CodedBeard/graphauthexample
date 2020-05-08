using System;
using System.Threading.Tasks;
using CodedBeard.GraphAuth.Interfaces;
using CodedBeard.GraphAuth.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace CodedBeard.GraphAuth 
{
    public class GraphAuthentication : IAuthentication
    {
        private readonly string _host;
        private readonly string _tenant;
        private readonly string _client_id;
        private readonly string _scope;
        private readonly string _client_secret;
        private readonly IMemoryCacheManager _memoryCacheManager;

        public GraphAuthentication(IOptions<GraphDetails> graph, IMemoryCacheManager memoryCacheManager)
        {
            var options = graph.Value;
            _host = options.Host;
            _tenant = options.Tenant;
            _client_id = options.Client_Id;
            _client_secret = options.Client_Secret;
            _scope = options.Scope;
            _memoryCacheManager = memoryCacheManager;
        }
        public async Task<AccessTokenResult> GetAccessToken()
        {
            var cacheKey = "_GraphToken";

            var graphToken = _memoryCacheManager.Get<AccessTokenResult>(cacheKey);
            if (!string.IsNullOrEmpty(graphToken?.Access_Token))
            {
                return graphToken;
            }

            var clientCredentials = new ClientCredential(_client_secret);
            var app = new ConfidentialClientApplication(_client_id, $"{_host}/{_tenant}", "https://graph.microsoft.com", clientCredentials, null, new TokenCache());

            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator
            var scopes = new string[] { _scope };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClientAsync(scopes);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
            }
            var token = new AccessTokenResult
            {
                Access_Token = result.AccessToken,
                Expires_In = result.ExpiresOn.ToString(),
            };

            var cacheTime = result.ExpiresOn - DateTime.UtcNow;
            //cacheTime = TimeSpan.FromMinutes(5);
            _memoryCacheManager.Set(cacheKey, token, cacheTime);

            return token;
        }
    }
}
