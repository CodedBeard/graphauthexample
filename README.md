Set relevent settings in your own configuration, this library assumes aspcore DI is used, so startup would have:

```csharp
    //inside configure services, see example.appsettings.json in project
    services.Configure<GraphDetails>(Configuration.GetSection("GraphDetails"));
    services.AddSingleton<IAuthentication, GraphAuthentication>();
    services.AddSingleton<IGraphClient, GraphClient>();
```

then in the consuming class, inject the client, and comsume it in methods

```csharp
    public class SomeClass{
        private readonly IGraphClient _graphClient;

        public SomeClass(IGraphClient graphClient){
            _graphClient = graphClient;
        }

        public async Task SomeMethod(){
            var graphServiceClient = await _graphClient.GetGraphClient();
            // this calls the sharepoint part of graph, but could be anything.
            var graphSharePointSiteId = await graphServiceClient
                .Sites
                .GetByPath($"/sites/{siteName}", _sharePointHost)
                .Request()
                .GetAsync();

        }
    }
```