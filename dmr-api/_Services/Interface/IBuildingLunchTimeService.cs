using DMR_API.DTO;
using DMR_API.Helpers;
using DMR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API._Services.Interface
{
    public interface IBuildingLunchTimeService 
    {
        Task<List<Period>> GetPeriodByLunchTime( int lunchTimeID);
        Task<ResponseDetail<object>> UpdatePeriod(Period period);

    }
}
