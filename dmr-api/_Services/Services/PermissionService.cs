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
    public class PermissionService : IPermissionService
    {

        private readonly IPermissionRepository _repoPermission;

        private readonly IMapper _mapper;
        private readonly IActionRepository _repoAction;
        private readonly IModuleRepository _repoModule;
        private readonly IRoleRepository _repoRole;
        private readonly IFunctionSystemRepository _repoFunctionSystem;
        private readonly IActionInFunctionSystemRepository _repoActionInFunctionSystem;
        private readonly IUserRoleRepository _repoUserRole;
        private readonly MapperConfiguration _configMapper;

        public PermissionService(
            IPermissionRepository repoPermission,
            IMapper mapper,
            IActionRepository repoAction,
            IModuleRepository repoModule,
            IRoleRepository repoRole,
            IFunctionSystemRepository repoFunctionSystem,
            IActionInFunctionSystemRepository repoActionInFunctionSystem,
            IUserRoleRepository repoUserRole,
            MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoAction = repoAction;
            _repoModule = repoModule;
            _repoRole = repoRole;
            _repoFunctionSystem = repoFunctionSystem;
            _repoActionInFunctionSystem = repoActionInFunctionSystem;
            _repoUserRole = repoUserRole;
            _repoPermission = repoPermission;
        }

        public async Task<bool> Add(Permission model)
        {
            var Permission = _mapper.Map<Permission>(model);

            _repoPermission.Add(Permission);
            return await _repoPermission.SaveAll();
        }



        public async Task<bool> Delete(object id)
        {
            var Permission = _repoPermission.FindById(id);
            _repoPermission.Remove(Permission);
            return await _repoPermission.SaveAll();
        }

        public async Task<bool> Update(Permission model)
        {
            var Permission = _mapper.Map<Permission>(model);
            _repoPermission.Update(Permission);
            return await _repoPermission.SaveAll();
        }


        public Permission GetById(object id) => _repoPermission.FindById(id);

        public async Task<List<FunctionSystem>> GetAllFunction()
        {
            var functions = await _repoFunctionSystem.FindAll().Include(x=>x.Module).ToListAsync();
            return  functions;
        }

        public async Task<List<Module>> GetAllModule()
        => await _repoModule.FindAll().OrderBy(x => x.Sequence).ToListAsync();

        public Task<ResponseDetail<object>> GetAllFunctionByPermision()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Permission>> GetAllAsync() => await _repoPermission.FindAll().ToListAsync();

        public Task<PagedList<Permission>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<Permission>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDetail<object>> UpdateModule(Module module)
        {
            _repoModule.Update(module);
            try
            {
                var result = await _repoModule.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }

        }

        public async Task<ResponseDetail<object>> DeleteModule(int moduleID)
        {
            var module = await _repoModule.FindAll(x=> x.ID == moduleID).FirstOrDefaultAsync();
            _repoModule.Remove(module);
            try
            {
                var result = await _repoModule.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> AddModule(Module module)
        {
            module.CreatedTime = DateTime.Now;
            _repoModule.Add(module);
            try
            {
                var result = await _repoModule.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> UpdateFunction(FunctionSystem module)
        {
            _repoFunctionSystem.Update(module);
            try
            {
                var result = await _repoFunctionSystem.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> DeleteFunction(int functionID)
        {
            var module = await _repoFunctionSystem.FindAll(x => x.ID == functionID).FirstOrDefaultAsync();
            _repoFunctionSystem.Remove(module);
            try
            {
                var result = await _repoFunctionSystem.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }

        }

        public async Task<ResponseDetail<object>> AddFunction(FunctionSystem module)
        {
            _repoFunctionSystem.Add(module);
            try
            {
                var result = await _repoFunctionSystem.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> UpdateAction(Models.Action action)
        {
            _repoAction.Update(action);
            try
            {
                var result = await _repoAction.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> DeleteAction(int actionID)
        {
            var action = await _repoAction.FindAll(x => x.ID == actionID).FirstOrDefaultAsync();
            _repoAction.Remove(action);
            try
            {
                var result = await _repoAction.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async Task<ResponseDetail<object>> AddAction(Models.Action action)
        {
            _repoAction.Add(action);
            try
            {
                var result = await _repoAction.SaveAll();
                return new ResponseDetail<object> { Status = result };
            }
            catch (System.Exception ex)
            {
                return new ResponseDetail<object> { Message = ex.Message };
            }
        }

        public async  Task<List<Models.Action>> GetAllAction() => await _repoAction.FindAll().ToListAsync();

        public async Task<IEnumerable<HierarchyNode<FunctionSystem>>> GetFunctionsAsTreeView()
        {
            var model = (await _repoFunctionSystem.FindAll().Include(x=> x.Module).Include(x=>x.Function).OrderBy(x => x.ID).OrderBy(x=> x.ModuleID).ThenBy(x=> x.Sequence).ToListAsync()).AsHierarchy(x => x.ID, y => y.ParentID);
            return model;
        }

        public async Task<object> GetMenuByUserPermission(int userId)
        {
            var roles = await _repoUserRole.FindAll(x=> x.UserID == userId).Select(x=> x.RoleID).ToArrayAsync();
            var query = from f in _repoFunctionSystem.FindAll().Include(x=> x.Module)
                        join p in _repoPermission.FindAll()
                            on f.ID equals p.FunctionSystemID
                        join r in _repoRole.FindAll() on p.RoleID equals r.ID
                        join a in _repoAction.FindAll()
                            on p.ActionID equals a.ID
                        where roles.Contains(r.ID) && a.Code == "VIEW"
                        select new 
                        {
                            Id = f.ID,
                            Name = f.Name,
                            Url = f.Url,
                            ParentId = f.ParentID,
                            SortOrder = f.Sequence,
                            Module= f.Module,
                            ModuleId = f.ModuleID
                        };
            var data = await query.Distinct()
                .OrderBy(x => x.ParentId)
                .ThenBy(x => x.SortOrder)
                .ToListAsync();
            return data.GroupBy(x=> x.Module).Select( x=> new {
                Module = x.Key.Name,
                Children = x,
                HasChildren = x.Any()
            });
        }
    }
}