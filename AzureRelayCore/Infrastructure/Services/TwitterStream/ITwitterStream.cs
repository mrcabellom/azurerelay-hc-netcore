using System.IO;
using System.Threading.Tasks;

namespace AzureRelayCore.Infrastructure.Services.TwitterStream
{
    public interface ITwitterStream
    {
        void StopTwitterStream();
        Task StartTwitterStreamAsync(Stream hyConnection);
    }
}
