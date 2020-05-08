using Microsoft.Graph;
using System.Threading.Tasks;

namespace CodedBeard.GraphAuth.Interfaces
{
    public interface IGraphClient
    {
        Task<IGraphServiceClient> GetGraphClient();
    }
}