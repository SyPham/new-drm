using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DMR_API.Helpers;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Tls;
using DMR_API.SignalrHub;
using Microsoft.AspNetCore.SignalR;

namespace DMR_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BottomFactoryController : ControllerBase
    {
        private readonly IBottomFactoryService _factoryService;
        private readonly IHubContext<ECHub> _hubContext;
        public BottomFactoryController(IBottomFactoryService factoryService, IHubContext<ECHub> hubContext)
        {
            _factoryService = factoryService;
            _hubContext = hubContext;
        }

       
        [HttpGet("{building}")]
        public async Task<IActionResult> ToDo(int building)
        {
            var batchs = await _factoryService.ToDoList(building);
            return Ok(batchs);
        }

      
        [HttpGet("{building}")]
        public async Task<IActionResult> DispatchList(int building)
        {
            var batchs = await _factoryService.DispatchList(building);
            return Ok(batchs);
        }
        [HttpGet("{building}")]
        public async Task<IActionResult> DispatchListDelay(int building)
        {
            var batchs = await _factoryService.DelayList(building);
            return Ok(batchs);
        }
        [HttpGet("{building}")]
        public async Task<IActionResult> Delay(int building)
        {
            var batchs = await _factoryService.UndoneList(building);
            return Ok(batchs);
        }
        [HttpGet("{building}")]
        public async Task<IActionResult> Done(int building)
        {
            var batchs = await _factoryService.DoneList(building);
            return Ok(batchs);
        }
        
        [HttpPost]
        public IActionResult ScanQRCode(ScanQRCodeParams scanQRCodeParams)
        {
            var batchs = _factoryService.ScanQRCode(scanQRCodeParams);
            return Ok(batchs);
        }

    }
}