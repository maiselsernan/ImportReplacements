using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories
{
    public class TypesRepository : BaseRepository, ITypesRepository
    {
        public TypesRepository(SqlConnectionConfiguration sqlConfiguration) : base(sqlConfiguration)
        {
        }
       
        public async Task<IEnumerable<MeterType>> GetMeterTypesAsync()=> await QueryAsync<MeterType>(@"SELECT * FROM MeterTypes");
        public async Task<IEnumerable<MeterManufacturer>> GetMeterManufacturersAsync()=> await QueryAsync<MeterManufacturer>(@"SELECT * FROM MeterManufacturer");

        public async Task<IEnumerable<MeterModel>> GetMeterModelsAsync() =>
            await QueryAsync<MeterModel>(@"SELECT * FROM MeterModel");
    }
}
