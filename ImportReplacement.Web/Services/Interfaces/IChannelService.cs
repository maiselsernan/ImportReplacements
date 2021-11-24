using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
   public interface IChannelService
   {
      Task<IEnumerable<Channel>> GetChannelsAsync(long meterNumber);
   }
}
