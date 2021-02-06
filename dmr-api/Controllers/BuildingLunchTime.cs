using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using DMR_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMR_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BuildingLunchTimeController : ControllerBase
    {
        private readonly IBuildingLunchTimeService _buildingLunchTimeService;
        private readonly IBuildingService _buildingService;

        public BuildingLunchTimeController(IBuildingLunchTimeService buildingLunchTimeService, IBuildingService buildingService)
        {
            _buildingLunchTimeService = buildingLunchTimeService;
            _buildingService = buildingService;
        }
        // getall building
        [HttpGet]
        public async Task<IActionResult> GetAllBuildings()
        {
            var result = await _buildingService.GetBuildings();
            return Ok(result);
        }
        [HttpGet("{lunchTimeID}")]
        public async Task<IActionResult> GetAllPeriodByLunchTime(int lunchTimeID)
        {
            var result = await _buildingLunchTimeService.GetPeriodByLunchTime(lunchTimeID);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateLunchTime(LunchTimeDto create)
        {
            var status = await _buildingService.AddOrUpdateLunchTime(create);
            if (status) return NoContent();
            else
                throw new Exception("Creating or updating the lunchTime failed on save");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePeriod(Period update)
        {
            update.StartTime = update.StartTime.ToLocalTime();
            update.EndTime = update.EndTime.ToLocalTime();
            var status = await _buildingLunchTimeService.UpdatePeriod(update);
            if (status) return NoContent();
            else
                throw new Exception("Creating or updating the Period failed on save");
        }


    }
}
