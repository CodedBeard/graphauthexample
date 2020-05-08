using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CodedBeard.GraphAuth.Interfaces;

namespace CodedBeard.GraphAuth
{
    public class GraphClient : IGraphClient
    {

        private readonly IAuthentication _authentication;
        private readonly ILogger<IGraphClient> _logger;

        public GraphClient(IAuthentication authentication, ILogger<IGraphClient> logger)
        {
            _authentication = authentication;
            _logger = logger;
        }

        public async Task<IGraphServiceClient> GetGraphClient()
        {
            var token = await _authentication.GetAccessToken();
            var graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", token.Access_Token);

                return Task.FromResult(0);
            }));
            return graphServiceClient;
        }
    }
}
