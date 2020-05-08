using CodedBeard.GraphAuth;
using System.Threading.Tasks;

namespace CodedBeard.GraphAuth.Interfaces 
{
    public interface IAuthentication
    {
        Task<AccessTokenResult> GetAccessToken();
    }
}