using dmr_api.Models;
using DMR_API._Repositories.Interface;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using DMR_API.Helpers;
using DMR_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API._Services.Services
{
    public class BottomFactoryService : IBottomFactoryService
    {
        private readonly IToDoListRepository _repoToDoList;
        private readonly IHttpContextAccessor _accessor;
        private readonly IJWTService _jwtService;
        private readonly IBuildingRepository _repoBuilding;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IBuildingUserRepository _repoBuildingUser;
        private readonly IDispatchRepository _repoDispatch;
        private readonly IMixingInfoRepository _mixingInfoRepository;
        private readonly ISubpackageRepository _subpackageRepository;
        private readonly IDispatchListRepository _repoDispatchList;
        private readonly IDispatchListDetailRepository _repoDispatchListDetail;

        public BottomFactoryService(
            IToDoListRepository repoToDoList,
            IHttpContextAccessor accessor,
            IJWTService jwtService,
             IBuildingRepository repoBuilding,
            IUserRoleRepository userRoleRepository,
            IIngredientRepository ingredientRepository,
            IBuildingUserRepository repoBuildingUser,
               IDispatchRepository repoDispatch,
               IMixingInfoRepository mixingInfoRepository,
               ISubpackageRepository subpackageRepository,
            IDispatchListRepository repoDispatchList,
            IDispatchListDetailRepository repoDispatchListDetail
            )
        {
            _repoToDoList = repoToDoList;
            _accessor = accessor;
            _jwtService = jwtService;
            _repoBuilding = repoBuilding;
            _userRoleRepository = userRoleRepository;
            _ingredientRepository = ingredientRepository;
            _repoBuildingUser = repoBuildingUser;
            _repoDispatch = repoDispatch;
            _mixingInfoRepository = mixingInfoRepository;
            _subpackageRepository = subpackageRepository;
            _repoDispatchList = repoDispatchList;
            _repoDispatchListDetail = repoDispatchListDetail;
        }

        public async Task<ResponseDetail<object>> AddDispatch(AddDispatchParams bfparams)
        {
            var userID = _jwtService.GetUserID();
            var subpackage = await _subpackageRepository.FindAll(x => x.MixingInfoID == bfparams.MixingInfoID).OrderByDescending(x => x.CreatedTime).FirstOrDefaultAsync();
            if (subpackage == null)
            {
                return new ResponseDetail<object>("", false, "Vui lòng chia keo trước!");
            }
            // Chon Default
            if (bfparams.Option == "Default")
            {
                var dispatch = await _repoDispatch.FindAll(x => x.MixingInfoID).OrderByDescending(x => x.CreatedTime).ToListAsync();
                if (dispatch.Count == 0)
                {
                    var dispatchModel = new Dispatch
                    {
                        CreatedTime = DateTime.Now,
                        CreatedBy = userID,
                        RemainingAmount = subpackage.Amount,
                        MixingInfoID = bfparams.MixingInfoID,
                    };
                    _repoDispatch.Add(dispatchModel);
                    await _repoDispatch.SaveAll();
                    return new ResponseDetail<object>("", false, "Vui lòng chia keo trước!");
                }
                else
                {
                    var dispatchModel = new Dispatch
                    {
                        CreatedTime = DateTime.Now,
                        CreatedBy = userID,
                        RemainingAmount = dispatch.First().Amount,
                        MixingInfoID = bfparams.MixingInfoID,
                    };
                    _repoDispatch.Add(dispatchModel);
                    await _repoDispatch.SaveAll();
                    return new ResponseDetail<object>("", true, "Tạo Thành Công!");
                }

            }
            else // Chon Reset
            {
                var dispatchModel = new Dispatch
                {
                    CreatedTime = DateTime.Now,
                    CreatedBy = userID,
                    RemainingAmount = subpackage.Amount,
                    MixingInfoID = bfparams.MixingInfoID,
                };
                _repoDispatch.Add(dispatchModel);
                await _repoDispatch.SaveAll();
                return new ResponseDetail<object>("", true, "Tạo Thành Công!");
            }
            // if (Dispatch == 0) RemainingAmount = SubpackageAmount
            // if (Disaptch !=0) RemainingAmount = LatestRemainingAmount

            // Chon Reset
            // GetLatestSubpackageAmount

            // DispatchAmount = GetLatestSubpackageAmount
        }

        public async Task<ToDoListForReturnDto> DelayList(int buildingID)
        {
            var userID = _jwtService.GetUserID();
            var role = await _userRoleRepository.FindAll(x => x.UserID == userID).FirstOrDefaultAsync();
            var currentTime = DateTime.Now.ToLocalTime();
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
            var currentDate = currentTime.Date;
            var value = currentTime.ToString("tt", CultureInfo.InvariantCulture);
            var morning = "AM";
            var start = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 00, 00);
            var model = await _repoToDoList.FindAll(x =>
                   x.IsDelete == false
                   && x.EstimatedStartTime.Date == currentDate
                   && x.EstimatedFinishTime.Date == currentDate
                   && x.GlueName.Contains(" + ")
                   && x.BuildingID == buildingID)
               .ToListAsync();
            if (role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var lines = await _repoBuildingUser.FindAll().Include(x => x.Building).Where(x => x.Building.ParentID == buildingID).Select(x => x.BuildingID).ToListAsync();
                model = model.Where(x => lines.Contains(x.LineID)).ToList();
            }
            else
            {
                var lines = await _repoBuilding.FindAll().Where(x => x.ParentID == buildingID).Select(x => x.ID).ToListAsync();
                model = model.Where(x => lines.Contains(x.LineID)).ToList();
            }
            // map dto
            var result = MapToTodolistDto(model);

            // filter by Middle of the day
            if (value == morning)
                result = result.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

            // caculate

            var total = result.Count();
            var doneTotal = result.Where(x => x.PrintTime != null).Count();
            var todoTotal = result.Where(x => x.PrintTime is null
                                            && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay
                                         ).Count();

            var delayTotal = result.Where(x => x.PrintTime is null
                                                && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay
                                                ).Count();

            // decentralization
            //if (role.RoleID == (int)Enums.Role.Worker)
            //{
            //    var data = result.Where(x => x.PrintTime is null
            //                                 && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay
            //                             ).ToList();
            //    var response = new ToDoListForReturnDto();
            //    response.TodoDetail(data, doneTotal, todoTotal, delayTotal, total);
            //    return response;
            //}
            if (role.RoleID == (int)Enums.Role.Admin || role.RoleID == (int)Enums.Role.Supervisor || role.RoleID == (int)Enums.Role.Staff || role.RoleID == (int)Enums.Role.Worker || role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var response = new ToDoListForReturnDto();
                var data = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).ToList();
                var dispatchListModel = await _repoDispatchList.FindAll(x =>
                                                                x.IsDelete == false
                                                                && x.EstimatedStartTime.Date == currentDate
                                                                && x.EstimatedFinishTime.Date == currentDate
                                                                && x.BuildingID == buildingID)
                                                                 .ToListAsync();
                //if (role.RoleID == (int)Enums.Role.Dispatcher)
                //{
                //    dispatchListModel = dispatchListModel.Where(x => lines.Contains(x.LineID)).ToList();
                //}
                // map to dto
                var dispatchList = MapToDispatchListDto(dispatchListModel);
                if (value == morning)
                    dispatchList = dispatchList.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                var dispatchTotal = dispatchList.Count();
                var dispatchListResult = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).ToList();
                var delayDispatchTotal = dispatchListResult.Count();
                var dispatchListDoneTotal = dispatchList.Where(x => x.FinishDispatchingTime != null).Count();

                var todoDispatchTotal = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
                var recaculatetotal = todoTotal + delayTotal + doneTotal + todoDispatchTotal + delayDispatchTotal + dispatchListDoneTotal;

                doneTotal = doneTotal + dispatchListDoneTotal;
                response.TodoDetail(data, doneTotal, todoTotal, delayTotal, recaculatetotal);

                response.DispatcherDetail(data, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);
                return response;
            }
            return new ToDoListForReturnDto(result.Where(x => x.PrintTime is null).ToList(), doneTotal, todoTotal, delayTotal, total);
        }

        public async Task<DispatchListForReturnDto> DispatchList(int buildingID)
        {
            var userID = _jwtService.GetUserID();
            var role = await _userRoleRepository.FindAll(x => x.UserID == userID).FirstOrDefaultAsync();

            try
            {
                var currentTime = DateTime.Now.ToLocalTime();
                var currentDate = currentTime.Date;
                var value = currentTime.ToString("tt", CultureInfo.InvariantCulture);
                var morning = "AM";
                var start = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 00, 00);
                var model = await _repoDispatchList.FindAll(x =>
                       x.IsDelete == false
                       && x.EstimatedStartTime.Date == currentDate
                       && x.EstimatedFinishTime.Date == currentDate
                       && x.BuildingID == buildingID)
                   .ToListAsync();
                if (role.RoleID == (int)Enums.Role.Dispatcher)
                {
                    var lines = await _repoBuildingUser.FindAll().Include(x => x.Building).Where(x => x.Building.ParentID == buildingID).Select(x => x.BuildingID).ToListAsync();
                    model = model.Where(x => lines.Contains(x.LineID)).ToList();
                }
                else
                {
                    var lines = await _repoBuilding.FindAll().Where(x => x.ParentID == buildingID).Select(x => x.ID).ToListAsync();
                    model = model.Where(x => lines.Contains(x.LineID)).ToList();
                }
                // map to dto
                var dispatchList = MapToDispatchListDto(model);

                // filter by Middle of the day
                if (value == morning)
                    dispatchList = dispatchList.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                // Caculate
                var dispatchTotal = dispatchList.Count();

                var dispatchListResult = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).ToList();
                var todoDispatchTotal = dispatchListResult.Count();

                var delayDispatchTotal = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();
                var dispatchListDoneTotal = dispatchList.Where(x => x.FinishDispatchingTime != null).Count();

                // decentralization
                //if (role.RoleID == (int)Enums.Role.Dispatcher)
                //{
                //    var todoModel = await _repoToDoList.FindAll(x =>
                //                                         x.IsDelete == false
                //                                         && x.EstimatedStartTime.Date == currentDate
                //                                         && x.EstimatedFinishTime.Date == currentDate
                //                                         && x.GlueName.Contains(" + ")
                //                                         && x.BuildingID == buildingID)
                //                   .ToListAsync();

                //    // map dto
                //    var todoResult = MapToTodolistDto(todoModel);

                //    // filter by Middle of the day
                //    if (value == morning)
                //        todoResult = todoResult.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                //    // caculate doneTodolist
                //    var total = todoResult.Count();
                //    var data = todoResult.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).ToList();
                //    var todoTotal = data.Count();
                //    var delayTotal = todoResult.Where(x => x.PrintTime is null
                //                                        && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();
                //    var doneTotal = todoResult.Where(x => x.PrintTime != null).Count();

                //    var response = new DispatchListForReturnDto();
                //    response.DoneTotal = dispatchListDoneTotal;
                //    response.DispatcherDetail(dispatchListResult, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);
                //    return response;
                //}
                if (role.RoleID == (int)Enums.Role.Admin || role.RoleID == (int)Enums.Role.Supervisor || role.RoleID == (int)Enums.Role.Staff || role.RoleID == (int)Enums.Role.Worker || role.RoleID == (int)Enums.Role.Dispatcher)
                {
                    var response = new DispatchListForReturnDto();
                    response.DispatcherDetail(dispatchListResult, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);
                    var todoModel = await _repoToDoList.FindAll(x =>
                      x.IsDelete == false
                      && x.EstimatedStartTime.Date == currentDate
                      && x.EstimatedFinishTime.Date == currentDate
                      && x.GlueName.Contains(" + ")
                      && x.BuildingID == buildingID)
                  .ToListAsync();

                    // map dto
                    var todoResult = MapToTodolistDto(todoModel);

                    // filter by Middle of the day
                    if (value == morning)
                        todoResult = todoResult.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                    // caculate
                    var total = todoResult.Count();
                    var data = todoResult.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).ToList();
                    var todoTotal = data.Count();
                    var delayTotal = todoResult.Where(x => x.PrintTime is null
                                                        && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();


                    var doneTotal = todoResult.Where(x => x.PrintTime != null).Count();
                    total = doneTotal + delayTotal + todoTotal + dispatchListDoneTotal + todoDispatchTotal + delayDispatchTotal;
                    doneTotal = doneTotal + dispatchListDoneTotal;
                    response.TodoDetail(dispatchListResult, doneTotal, todoTotal, delayTotal, total);

                    return response;
                }
                return new DispatchListForReturnDto(dispatchListResult, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ToDoListForReturnDto> DoneList(int buildingID)
        {
            var userID = _jwtService.GetUserID();
            var role = await _userRoleRepository.FindAll(x => x.UserID == userID).FirstOrDefaultAsync();
            var currentTime = DateTime.Now.ToLocalTime();
            var currentDate = currentTime.Date;
            var value = currentTime.ToString("tt", CultureInfo.InvariantCulture);
            var morning = "AM";
            var start = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 00, 00);
            var model = await _repoToDoList.FindAll(x =>
                  x.IsDelete == false
                  && x.EstimatedStartTime.Date == currentDate
                  && x.EstimatedFinishTime.Date == currentDate
                  && x.GlueName.Contains(" + ")
                  && x.BuildingID == buildingID)
               .ToListAsync();

            var result = MapToTodolistDto(model);
            // filter by middle of the day
            if (value == morning)
                result = result.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

            // dispatch
            // map to dto
            var dispatchListModel = await _repoDispatchList.FindAll(x =>
              x.IsDelete == false
              && x.EstimatedStartTime.Date == currentDate
              && x.EstimatedFinishTime.Date == currentDate
              && x.BuildingID == buildingID)
           .ToListAsync();
            if (role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var lines = await _repoBuildingUser.FindAll().Include(x => x.Building).Where(x => x.Building.ParentID == buildingID).Select(x => x.BuildingID).ToListAsync();
                dispatchListModel = dispatchListModel.Where(x => lines.Contains(x.LineID)).ToList();
            }
            else
            {
                var lines = await _repoBuilding.FindAll().Where(x => x.ParentID == buildingID).Select(x => x.ID).ToListAsync();
                dispatchListModel = dispatchListModel.Where(x => lines.Contains(x.LineID)).ToList();
            }
            // map to dto
            var dispatchListResult = MapDispatchToToDoListDto(dispatchListModel);

            // filter by middle of the day
            if (value == morning)
                dispatchListResult = dispatchListResult.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

            // decentralzation
            if (role.RoleID == (int)Enums.Role.Admin || role.RoleID == (int)Enums.Role.Supervisor || role.RoleID == (int)Enums.Role.Staff || role.RoleID == (int)Enums.Role.Worker || role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var dispatchTotal = dispatchListResult.Count();
                var dispatchListDoneResult = dispatchListResult.Where(x => x.FinishDispatchingTime != null).ToList();
                var delayDispatchTotal = dispatchListResult.Where(x => x.FinishDispatchingTime == null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();
                var todoDispatchTotal = dispatchListResult.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
                var doneDispatchTotal = dispatchListDoneResult.Count();

                var doneList = result.ToList();

                // tinh tong ca 2 danh sach hoan thanh
                var total = doneList.Count();

                var todoTotal = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
                var delayTotal = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();
                var doneTotal = result.Where(x => x.PrintTime != null).Count();

                var response = new ToDoListForReturnDto();
                int jobTypeOfTodo = 1, jobTypeOfDispatch = 2;
                var data = doneList.Concat(dispatchListResult).Where(x => x.PrintTime != null && x.JobType == jobTypeOfTodo || x.FinishDispatchingTime != null && x.JobType == jobTypeOfDispatch).ToList();
                var recaculatetotal = todoTotal + delayTotal + doneTotal + todoDispatchTotal + delayDispatchTotal + doneDispatchTotal;
                response.TodoDetail(data, doneTotal + doneDispatchTotal, todoTotal, delayTotal, recaculatetotal);
                response.DispatcherDetail(data, 0, todoDispatchTotal, delayDispatchTotal, dispatchTotal);

                return response;
            }
            //if (role.RoleID == (int)Enums.Role.Worker)
            //{
            //    var doneListForWorker = result.Where(x => x.PrintTime != null).ToList();
            //    var doneTotalForWorker = doneListForWorker.Count;

            //    var totalForWorker = result.Count();
            //    var todoTotalForWorker = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay && x.GlueName.Contains(" + ")).Count();
            //    var delayTotalForWorker = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay && x.GlueName.Contains(" + ")).Count();

            //    return new ToDoListForReturnDto(doneListForWorker, doneTotalForWorker, todoTotalForWorker, delayTotalForWorker, totalForWorker);
            //}
            //if (role.RoleID == (int)Enums.Role.Dispatcher)
            //{
            //    var doneListForDispatcher = dispatchListResult.Where(x => x.FinishDispatchingTime != null).ToList();
            //    var doneTotalForDispatcher = doneListForDispatcher.Count;

            //    var totalForDispatcher = dispatchListResult.Count();
            //    var todoTotalForDispatcher = dispatchListResult.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
            //    var delayTotalForDispatcher = dispatchListResult.Where(x => x.FinishDispatchingTime is null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).Count();
            //    var resoponse = new ToDoListForReturnDto();
            //    resoponse.DoneTotal = doneTotalForDispatcher;
            //    resoponse.DispatcherDetail(doneListForDispatcher, doneTotalForDispatcher, todoTotalForDispatcher, delayTotalForDispatcher, totalForDispatcher);
            //    return resoponse;
            //}
            return new ToDoListForReturnDto(null, 0, 0, 0, 0);
        }

        public async Task<ToDoListForReturnDto> EVA_UVList(int buildingID)
        {
            var userID = _jwtService.GetUserID();
            var role = await _userRoleRepository.FindAll(x => x.UserID == userID).FirstOrDefaultAsync();
            var currentTime = DateTime.Now.ToLocalTime();
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
            var currentDate = currentTime.Date;
            var value = currentTime.ToString("tt", CultureInfo.InvariantCulture);
            var morning = "AM";
            var start = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 00, 00);
            var model = await _repoToDoList.FindAll(x =>
                   x.IsDelete == false
                   && x.EstimatedStartTime.Date == currentDate
                   && x.EstimatedFinishTime.Date == currentDate
                   && x.GlueName.Contains(" + ")
                   && x.BuildingID == buildingID)
               .ToListAsync();
            if (role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var lines = await _repoBuildingUser.FindAll().Include(x => x.Building).Where(x => x.Building.ParentID == buildingID).Select(x => x.BuildingID).ToListAsync();
                model = model.Where(x => lines.Contains(x.LineID)).ToList();
            }
            else
            {
                var lines = await _repoBuilding.FindAll().Where(x => x.ParentID == buildingID).Select(x => x.ID).ToListAsync();
                model = model.Where(x => lines.Contains(x.LineID)).ToList();
            }
            // map dto
            var result = MapToTodolistDto(model);

            // filter by Middle of the day
            if (value == morning)
                result = result.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

            // caculate

            var total = result.Count();
            var doneTotal = result.Where(x => x.PrintTime != null).Count();
            var todoTotal = result.Where(x => x.PrintTime is null
                                            && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay
                                         ).Count();

            var delayTotal = result.Where(x => x.PrintTime is null
                                                && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay
                                                ).Count();

            // decentralization
            //if (role.RoleID == (int)Enums.Role.Worker)
            //{
            //    var data = result.Where(x => x.PrintTime is null
            //                                 && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay
            //                             ).ToList();
            //    var response = new ToDoListForReturnDto();
            //    response.TodoDetail(data, doneTotal, todoTotal, delayTotal, total);
            //    return response;
            //}
            if (role.RoleID == (int)Enums.Role.Admin || role.RoleID == (int)Enums.Role.Supervisor || role.RoleID == (int)Enums.Role.Staff || role.RoleID == (int)Enums.Role.Worker || role.RoleID == (int)Enums.Role.Dispatcher)
            {
                var response = new ToDoListForReturnDto();
                var data = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).ToList();
                var dispatchListModel = await _repoDispatchList.FindAll(x =>
                                                                x.IsDelete == false
                                                                && x.EstimatedStartTime.Date == currentDate
                                                                && x.EstimatedFinishTime.Date == currentDate
                                                                && x.BuildingID == buildingID)
                                                                 .ToListAsync();
                //if (role.RoleID == (int)Enums.Role.Dispatcher)
                //{
                //    dispatchListModel = dispatchListModel.Where(x => lines.Contains(x.LineID)).ToList();
                //}
                // map to dto
                var dispatchList = MapToDispatchListDto(dispatchListModel);
                if (value == morning)
                    dispatchList = dispatchList.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                var dispatchTotal = dispatchList.Count();
                var dispatchListResult = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).ToList();
                var delayDispatchTotal = dispatchListResult.Count();
                var dispatchListDoneTotal = dispatchList.Where(x => x.FinishDispatchingTime != null).Count();

                var todoDispatchTotal = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
                var recaculatetotal = todoTotal + delayTotal + doneTotal + todoDispatchTotal + delayDispatchTotal + dispatchListDoneTotal;

                doneTotal = doneTotal + dispatchListDoneTotal;
                response.TodoDetail(data, doneTotal, todoTotal, delayTotal, recaculatetotal);

                response.DispatcherDetail(data, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);
                return response;
            }
            return new ToDoListForReturnDto(result.Where(x => x.PrintTime is null).ToList(), doneTotal, todoTotal, delayTotal, total);
        }

        public async Task<ResponseDetail<object>> GenerateScanByNumber(GenerateSubpackageParams obj)
        {
            var userID = _jwtService.GetUserID();
            var subpackageList = new List<Subpackage>();
            for (int i = obj.Can - 1; i >= 0; i--)
            {
                var subpackage = new Subpackage
                {
                    CreatedBy = userID,
                    CreatedTime = DateTime.Now,
                    Amount = obj.AmountOfChemical / obj.Can // 15 / 5 = 3
                };
                subpackageList.Add(subpackage);
            }
            _subpackageRepository.AddRange(subpackageList);
            await _subpackageRepository.SaveAll();
            return new ResponseDetail<object>("", true, "Thành công!");
        }

        public async Task<ResponseDetail<object>> ScanQRCode(ScanQRCodeParams scanQRCodeParams)
        {
            var mixBy = _jwtService.GetUserID();

            var ingredient = await _ingredientRepository.FindAll(x => x.MaterialNO == scanQRCodeParams.PartNO).FirstOrDefaultAsync();
            if (ingredient == null) return new ResponseDetail<object>("", false, "Không tìm thấy hóa chất này! Vui lòng thử lại!");
            var mixingInfo = new MixingInfo
            {
                GlueID = scanQRCodeParams.GlueID,
                GlueName = scanQRCodeParams.GlueName,
                BuildingID = scanQRCodeParams.BuildingID,
                MixBy = mixBy,
                EstimatedFinishTime = scanQRCodeParams.EstimatedFinishTime,
                EstimatedStartTime = scanQRCodeParams.EstimatedStartTime

            };
            _mixingInfoRepository.Add(mixingInfo);
            await _mixingInfoRepository.SaveAll();
            return new ResponseDetail<object>("", true, "Thành Công!");

        }

        public Task<ToDoListForReturnDto> UndoneList(int buildingID)
        {
            throw new NotImplementedException();
        }

        // Helper for donelist
        List<ToDoListDto> MapDispatchToToDoListDto(List<DispatchList> model)
        {
            var dispatchlist = new List<ToDoListDto>();

            var groupByDispatchListModel = model.GroupBy(x => new { x.EstimatedStartTime, x.EstimatedFinishTime, x.GlueNameID });
            foreach (var todo in groupByDispatchListModel)
            {
                var item = todo.FirstOrDefault();
                var lineList = todo.Select(x => x.LineName).OrderBy(x => x).ToList();
                var deliveredAmount = todo.Sum(x => x.DeliveredAmount);

                var itemDispatch = new ToDoListDto();
                itemDispatch.ID = item.ID;
                itemDispatch.JobType = 2;
                itemDispatch.PlanID = item.PlanID;
                itemDispatch.MixingInfoID = item.MixingInfoID;
                itemDispatch.GlueID = item.GlueID;
                itemDispatch.LineID = item.LineID;
                itemDispatch.LineName = item.LineName;
                itemDispatch.GlueName = item.GlueName;
                itemDispatch.GlueNameID = item.GlueNameID;

                itemDispatch.Supplier = item.Supplier;
                itemDispatch.Status = item.FinishDispatchingTime.HasValue == false ? false : item.FinishDispatchingTime.Value.ToRemoveSecond() <= item.EstimatedFinishTime;

                itemDispatch.StartMixingTime = null;
                itemDispatch.FinishMixingTime = null;

                itemDispatch.StartStirTime = null;
                itemDispatch.FinishStirTime = null;

                itemDispatch.StartDispatchingTime = item.StartDispatchingTime;
                itemDispatch.FinishDispatchingTime = item.FinishDispatchingTime;

                itemDispatch.PrintTime = item.PrintTime;

                itemDispatch.MixedConsumption = 0;
                itemDispatch.DeliveredConsumption = deliveredAmount;
                itemDispatch.StandardConsumption = 0;
                itemDispatch.DeliveredAmount = Math.Round(deliveredAmount, 3);

                itemDispatch.DispatchTime = item.CreatedTime;
                itemDispatch.EstimatedStartTime = item.EstimatedStartTime;
                itemDispatch.EstimatedFinishTime = item.EstimatedFinishTime;

                itemDispatch.AbnormalStatus = item.AbnormalStatus;

                itemDispatch.LineNames = lineList;
                itemDispatch.BuildingID = item.BuildingID;
                dispatchlist.Add(itemDispatch);
            }
            var modelTempDispatchList = dispatchlist.OrderBy(x => x.EstimatedStartTime)
                                                     .GroupBy(x => new { x.EstimatedStartTime, x.EstimatedFinishTime }).ToList();
            var dispatchListResult = new List<ToDoListDto>();

            foreach (var item in modelTempDispatchList)
            {
                dispatchListResult.AddRange(item.OrderByDescending(x => x.GlueName));
            }
            return dispatchListResult;
        }

        // Helper for delay, todo
        List<ToDoListDto> MapToTodolistDto(List<ToDoList> model)
        {
            var groupBy = model.GroupBy(x => new { x.EstimatedStartTime, x.EstimatedFinishTime, x.GlueNameID, x.PrintTime });
            var todolist = new List<ToDoListDto>();
            foreach (var todo in groupBy)
            {
                var item = todo.FirstOrDefault();
                var lineList = todo.Select(x => x.LineName).DistinctBy(x => x).OrderBy(x => x).ToList();
                var stdTotal = todo.Select(x => x.StandardConsumption).Sum();
                var stddeliver = todo.Select(x => x.DeliveredConsumption).Sum();

                var itemTodolist = new ToDoListDto();
                itemTodolist.ID = item.ID;
                itemTodolist.JobType = 1;
                itemTodolist.PlanID = item.PlanID;
                itemTodolist.MixingInfoID = item.MixingInfoID;
                itemTodolist.GlueID = item.GlueID;
                itemTodolist.LineID = item.LineID;
                itemTodolist.LineName = item.LineName;
                itemTodolist.GlueName = item.GlueName;
                itemTodolist.GlueNameID = item.GlueNameID;
                itemTodolist.Supplier = item.Supplier;
                itemTodolist.Status = item.Status;

                itemTodolist.StartMixingTime = item.StartMixingTime;
                itemTodolist.FinishMixingTime = item.FinishMixingTime;

                itemTodolist.StartStirTime = item.StartStirTime;
                itemTodolist.FinishStirTime = item.FinishStirTime;

                itemTodolist.StartDispatchingTime = item.StartDispatchingTime;
                itemTodolist.FinishDispatchingTime = item.FinishDispatchingTime;

                itemTodolist.PrintTime = item.PrintTime;

                itemTodolist.MixedConsumption = Math.Round(item.MixedConsumption, 2);
                itemTodolist.DeliveredConsumption = Math.Round(stddeliver, 2);
                itemTodolist.StandardConsumption = Math.Round(stdTotal, 2);

                itemTodolist.EstimatedStartTime = item.EstimatedStartTime;
                itemTodolist.EstimatedFinishTime = item.EstimatedFinishTime;

                itemTodolist.AbnormalStatus = item.AbnormalStatus;
                itemTodolist.IsDelete = item.IsDelete;

                itemTodolist.LineNames = lineList;
                itemTodolist.BuildingID = item.BuildingID;
                todolist.Add(itemTodolist);
            }

            // GroupBy period and then by glueName
            var modelTemp = todolist.OrderBy(x => x.EstimatedStartTime).GroupBy(x => new { x.EstimatedStartTime, x.EstimatedFinishTime }).ToList();
            var result = new List<ToDoListDto>();
            foreach (var item in modelTemp)
            {
                result.AddRange(item.OrderByDescending(x => x.GlueName));
            }

            return result;
        }

        // Helper for dispatchlist, dispatchlist delay
        List<DispatchListDto> MapToDispatchListDto(List<DispatchList> model)
        {
            var currentTime = DateTime.Now.ToLocalTime();
            var currentDate = currentTime.Date;
            var groupBy = model.GroupBy(x => new { x.EstimatedStartTime, x.EstimatedFinishTime, x.GlueNameID });
            var dispatchlist = new List<DispatchListDto>();
            foreach (var todo in groupBy)
            {
                var item = todo.FirstOrDefault();
                var lineList = todo.Select(x => x.LineName).DistinctBy(x => x).OrderBy(x => x).ToList();

                var itemTodolist = new DispatchListDto();
                itemTodolist.ID = item.ID;
                itemTodolist.PlanID = item.PlanID;
                itemTodolist.MixingInfoID = item.MixingInfoID;
                itemTodolist.GlueID = item.GlueID;
                itemTodolist.LineID = item.LineID;
                itemTodolist.LineName = item.LineName;
                itemTodolist.GlueName = item.GlueName;
                itemTodolist.GlueNameID = item.GlueNameID;
                itemTodolist.Supplier = item.Supplier;
                itemTodolist.ColorCode = item.ColorCode;

                itemTodolist.StartDispatchingTime = item.StartDispatchingTime;
                itemTodolist.FinishDispatchingTime = item.FinishDispatchingTime;

                itemTodolist.PrintTime = item.PrintTime;

                itemTodolist.DeliveredAmount = item.DeliveredAmount;

                itemTodolist.EstimatedStartTime = item.EstimatedStartTime;
                itemTodolist.EstimatedFinishTime = item.EstimatedFinishTime;

                itemTodolist.FinishTimeOfPeriod = item.FinishTimeOfPeriod;
                itemTodolist.StartTimeOfPeriod = item.StartTimeOfPeriod;
                itemTodolist.IsDelete = item.IsDelete;

                itemTodolist.LineNames = lineList;
                itemTodolist.BuildingID = item.BuildingID;
                dispatchlist.Add(itemTodolist);
            }
            var modelTemp = dispatchlist.Where(x => x.EstimatedFinishTime.Date == currentDate)
                .OrderBy(x => x.EstimatedStartTime).GroupBy(x => new { x.GlueNameID, x.EstimatedStartTime, x.EstimatedFinishTime }).ToList();
            var result = new List<DispatchListDto>();
            foreach (var item in modelTemp)
            {
                result.AddRange(item.OrderByDescending(x => x.GlueName));
            }
            return result;
        }

        public async Task<ToDoListForReturnDto> ToDoList(int buildingID)
        {
            try
            {
                var userID = _jwtService.GetUserID();
                var role = await _userRoleRepository.FindAll(x => x.UserID == userID).FirstOrDefaultAsync();

                var currentTime = DateTime.Now.ToLocalTime();
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
                var currentDate = currentTime.Date;
                var value = currentTime.ToString("tt", CultureInfo.InvariantCulture);
                var morning = "AM";
                var start = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 00, 00);
                var model = await _repoToDoList.FindAll(x =>
                       x.IsDelete == false
                       && x.EstimatedStartTime.Date == currentDate
                       && x.EstimatedFinishTime.Date == currentDate
                       && x.GlueName.Contains(" + ")
                       && x.BuildingID == buildingID)
                   .ToListAsync();
                //var model2 = await _repoToDoList.FindAll(x =>
                //       x.IsDelete == false
                //       && x.EstimatedStartTime.Date == currentDate
                //       && x.EstimatedFinishTime.Date == currentDate
                //       && x.GlueName.Contains(" + ")
                //       && x.AbnormalStatus == true
                //       && x.BuildingID == buildingID)
                //   .ToListAsync();
                if (role.RoleID == (int)Enums.Role.Dispatcher)
                {
                    var lines = await _repoBuildingUser.FindAll().Include(x => x.Building).Where(x => x.Building.ParentID == buildingID).Select(x => x.BuildingID).ToListAsync();
                    model = model.Where(x => lines.Contains(x.LineID)).ToList();
                }
                else
                {
                    var lines = await _repoBuilding.FindAll().Where(x => x.ParentID == buildingID).Select(x => x.ID).ToListAsync();
                    model = model.Where(x => lines.Contains(x.LineID)).ToList();
                }
                // map dto
                var result = MapToTodolistDto(model);

                // filter by Middle of the day
                if (value == morning)
                    result = result.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();

                // caculate
                var total = result.Count();
                var data = result.Where(x => x.PrintTime is null && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).ToList();
                var todoTotal = data.Count();
                var delayTotal = result.Where(x => x.PrintTime is null
                                                 && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay
                                                 ).Count();
                var doneTotal = result.Where(x => x.PrintTime != null).Count();

                //if (role.RoleID == (int)Enums.Role.Worker)
                //{
                //    var response = new ToDoListForReturnDto();
                //    response.TodoDetail(data, doneTotal, todoTotal, delayTotal, total);
                //    return response;
                //}
                if (role.RoleID == (int)Enums.Role.Admin || role.RoleID == (int)Enums.Role.Supervisor || role.RoleID == (int)Enums.Role.Staff || role.RoleID == (int)Enums.Role.Worker || role.RoleID == (int)Enums.Role.Dispatcher)
                {

                    var dispatchListModel = await _repoDispatchList.FindAll(x =>
                                                               x.IsDelete == false
                                                               && x.EstimatedStartTime.Date == currentDate
                                                               && x.EstimatedFinishTime.Date == currentDate
                                                               && x.BuildingID == buildingID)
                                                               .ToListAsync();
                    //if (role.RoleID == (int)Enums.Role.Dispatcher)
                    //{
                    //    dispatchListModel = dispatchListModel.Where(x => lines.Contains(x.LineID)).ToList();
                    //}
                    // map to dto
                    var dispatchList = MapToDispatchListDto(dispatchListModel);
                    if (value == morning)
                        dispatchList = dispatchList.Where(x => x.EstimatedStartTime.TimeOfDay <= start.TimeOfDay).ToList();
                    var response = new ToDoListForReturnDto();

                    var dispatchTotal = dispatchList.Count();
                    var dispatchListDoneTotal = dispatchList.Where(x => x.FinishDispatchingTime != null).Count();
                    var dispatchListResult = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedFinishTime.TimeOfDay < currentTime.TimeOfDay).ToList();
                    var delayDispatchTotal = dispatchListResult.Count();
                    var todoDispatchTotal = dispatchList.Where(x => x.FinishDispatchingTime == null && x.EstimatedStartTime.TimeOfDay <= currentTime.TimeOfDay && x.EstimatedFinishTime.TimeOfDay >= currentTime.TimeOfDay).Count();
                    var recaculatetotal = todoTotal + delayTotal + doneTotal + todoDispatchTotal + delayDispatchTotal + dispatchListDoneTotal;

                    doneTotal = doneTotal + dispatchListDoneTotal;
                    response.TodoDetail(data, doneTotal, todoTotal, delayTotal, recaculatetotal);

                    response.DispatcherDetail(data, dispatchListDoneTotal, todoDispatchTotal, delayDispatchTotal, dispatchTotal);

                    return response;
                }
                return new ToDoListForReturnDto(data, doneTotal, todoTotal, delayTotal, total);

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
