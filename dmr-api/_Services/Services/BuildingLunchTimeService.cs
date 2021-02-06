using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMR_API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DMR_API._Repositories.Interface;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using DMR_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DMR_API._Services.Services
{
    public class BuildingLunchTimeService : IBuildingLunchTimeService
    {
        private readonly IPeriodRepository _repoPeriod;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public BuildingLunchTimeService(IPeriodRepository repoPeriod,  IMapper mapper, MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoPeriod = repoPeriod;

        }

        public async Task<bool> Add(Period model)
        {
            var Line = _mapper.Map<Period>(model);
            _repoPeriod.Add(Line);
            return await _repoPeriod.SaveAll();
        }
        public async Task<bool> AddRangePeriod(Period model)
        {
            var Line = _mapper.Map<Period>(model);
            _repoPeriod.Add(Line);
            return await _repoPeriod.SaveAll();
        }

        //Cập nhật Period
        public async Task<bool> UpdatePeriod(Period model)
        {
            var period = _mapper.Map<Period>(model);
            _repoPeriod.Update(period);
            return await _repoPeriod.SaveAll();
        }
      

        public async Task<List<Period>> GetPeriodByLunchTime(int lunchTimeID)
        {
            return await _repoPeriod.FindAll(x => x.LunchTimeID == lunchTimeID).ToListAsync();
        }
    }
}