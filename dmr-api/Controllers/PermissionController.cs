﻿using System.Reflection;
using DMR_API._Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Module = DMR_API.Models.Module;
using DMR_API.Models;
using Action = DMR_API.Models.Action;

namespace DMR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("GetAllFunction")]
        public async Task<IActionResult> GetAllFunction()
        {
            var result = await _permissionService.GetAllFunction();
            return Ok(result);
        }
        [HttpGet("GetFunctionsAsTreeView")]
        public async Task<IActionResult> GetFunctionsAsTreeView()
        {
            var result = await _permissionService.GetFunctionsAsTreeView();
            return Ok(result);
        }
        [HttpGet("GetMenuByUserPermission/{userId}")]
        public async Task<IActionResult> GetMenuByUserPermission(int userId)
        {
            var result = await _permissionService.GetMenuByUserPermission(userId);
            return Ok(result);
        }
        [HttpGet("GetAllModule")]
        public async Task<IActionResult> GetAllModule()
        {
            var result = await _permissionService.GetAllModule();
            return Ok(result);
        }

        [HttpGet("GetAllAction")]
        public async Task<IActionResult> GetAllAction()
        {
            var result = await _permissionService.GetAllAction();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _permissionService.GetAllAsync();
            return Ok(result);
        }


        [HttpPost("CreateModule")]
        public async Task<IActionResult> CreateModule(Module create)
        {

            var result = await _permissionService.AddModule(create);
            if (result.Status)
            {
                return NoContent();
            }

            throw new Exception("Creating the Module failed on save");
        }

        [HttpPut("UpdateModule")]
        public async Task<IActionResult> UpdateModule(Module update)
        {
            var result = await _permissionService.UpdateModule(update);
            if (result.Status)
                return NoContent();
            return BadRequest($"Updating Module {update.ID} failed on save");
        }

        [HttpDelete("DeleteModule/{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var result = await _permissionService.DeleteModule(id);
             if(result.Status)
                return NoContent();
            throw new Exception("Error deleting the Module");
        }


        [HttpPost("CreateFunction")]
        public async Task<IActionResult> CreateFunction(FunctionSystem create)
        {
            var result = await _permissionService.AddFunction(create);
            if (result.Status)
            {
                return NoContent();
            }

            throw new Exception("Creating the Function failed on save");
        }

        [HttpPut("UpdateFunction")]
        public async Task<IActionResult> UpdateFunction(FunctionSystem update)
        {
            var result = await _permissionService.UpdateFunction(update);
            if (result.Status)
                return NoContent();
            return BadRequest($"Updating Function {update.ID} failed on save");
        }

        [HttpDelete("DeleteFunction/{id}")]
        public async Task<IActionResult> DeleteFunction(int id)
        {
            var result = await _permissionService.DeleteFunction(id);
            if (result.Status)
                return NoContent();
            throw new Exception("Error deleting the Function");
        }



        [HttpPost("CreateAction")]
        public async Task<IActionResult> CreateAction(Action create)
        {
            var result = await _permissionService.AddAction(create);
            if (result.Status)
            {
                return NoContent();
            }

            throw new Exception("Creating the Action failed on save");
        }

        [HttpPut("UpdateAction")]
        public async Task<IActionResult> UpdateAction(Action update)
        {
            var result = await _permissionService.UpdateAction(update);
            if (result.Status)
                return NoContent();
            return BadRequest($"Updating Action {update.ID} failed on save");
        }

        [HttpDelete("DeleteAction/{id}")]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var result = await _permissionService.DeleteAction(id);
            if (result.Status)
                return NoContent();
            throw new Exception("Error deleting the Action");
        }
    }
}

   