using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
   public interface ITypesService
   {
       Task<IEnumerable<MeterType>> GetMeterTypesAsync();
       Task<IEnumerable<MeterManufacturer>> GetMeterManufacturersAsync();
       Task<IEnumerable<MeterModel>> GetMeterModelsAsync();
   }
}
