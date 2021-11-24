using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface ITypesRepository
    {
        Task<IEnumerable<MeterType>> GetMeterTypesAsync();
        Task<IEnumerable<MeterManufacturer>> GetMeterManufacturersAsync();
        Task<IEnumerable<MeterModel>> GetMeterModelsAsync();
    }
}
