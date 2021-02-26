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
        private readonly ILunchTimeRepository _repoLunchTime;
        private readonly IJWTService _jWTService;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public BuildingLunchTimeService(IPeriodRepository repoPeriod, 
            ILunchTimeRepository repoLunchTime, 
            IJWTService jWTService,
            IMapper mapper, 
            MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoPeriod = repoPeriod;
            _repoLunchTime = repoLunchTime;
            _jWTService = jWTService;
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
        public async Task<ResponseDetail<object>> UpdatePeriod(Period model)
        {
            var userID = _jWTService.GetUserID();
            var period = _mapper.Map<Period>(model);
            var lunchTime = await _repoLunchTime.FindAll(x => x.ID == model.LunchTimeID).FirstOrDefaultAsync();
            var startLunchTime = lunchTime.StartTime.TimeOfDay;
            var endLunchTime = lunchTime.EndTime.TimeOfDay;
            // lunchTime 12:30-13:30
            // 12:30 >= 12:30 and 13:00 <= 13:30
            if (period.StartTime.TimeOfDay >= startLunchTime && period.EndTime.TimeOfDay <= endLunchTime)
            {
                return new ResponseDetail<object>() { Status = false, Message = "Thời gian bắt đầu và thời gian kết thúc không được giao với giờ ăn trưa!" };
            }
            if (period.StartTime.TimeOfDay > period.EndTime.TimeOfDay)
            {
                return new ResponseDetail<object>() { Status = false, Message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc!" };
            }

            if ((period.StartTime.TimeOfDay - period.EndTime.TimeOfDay).TotalHours > 2.5)
            {
                return new ResponseDetail<object>() { Status = false, Message = "Thời gian bắt đầu và thời gian kết thúc phải nhỏ hơn hoặc bằng 2.5 giờ!" };
            }
            period.UpdatedBy = userID;
            period.UpdatedTime = DateTime.Now;
            _repoPeriod.Update(period);
            return new ResponseDetail<object>() { Status = await _repoPeriod.SaveAll() };
        }
      

        public async Task<List<Period>> GetPeriodByLunchTime(int lunchTimeID)
        {
            return await _repoPeriod.FindAll(x => x.LunchTimeID == lunchTimeID).ToListAsync();
        }
    }
}