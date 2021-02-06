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
    public class BuildingService : IBuildingService
    {

        private readonly IBuildingRepository _repoBuilding;
        private readonly IPeriodRepository _repoPeriod;
        private readonly ILunchTimeRepository _repoLunchTime;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public BuildingService(
            IBuildingRepository repoBuilding,
            IPeriodRepository repoPeriod,
            ILunchTimeRepository repoLunchTime,
            IMapper mapper, MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoBuilding = repoBuilding;
            _repoPeriod = repoPeriod;
            _repoLunchTime = repoLunchTime;
        }

        public async Task<bool> Add(BuildingDto model)
        {
            var building = _mapper.Map<Building>(model);
            _repoBuilding.Add(building);
            return await _repoBuilding.SaveAll();
        }

        public async Task<PagedList<BuildingDto>> GetWithPaginations(PaginationParams param)
        {
            var lists = _repoBuilding.FindAll().ProjectTo<BuildingDto>(_configMapper).OrderByDescending(x => x.ID);
            return await PagedList<BuildingDto>.CreateAsync(lists, param.PageNumber, param.PageSize);
        }
        public async Task<PagedList<BuildingDto>> Search(PaginationParams param, object text)
        {
            var lists = _repoBuilding.FindAll().ProjectTo<BuildingDto>(_configMapper)
            .Where(x => x.Name.Contains(text.ToString()))
            .OrderByDescending(x => x.ID);
            return await PagedList<BuildingDto>.CreateAsync(lists, param.PageNumber, param.PageSize);
        }
        public async Task<bool> Delete(object id)
        {
            var Building = _repoBuilding.FindById(id);
            _repoBuilding.Remove(Building);
            return await _repoBuilding.SaveAll();
        }

        public async Task<bool> Update(BuildingDto model)
        {
            var building = _mapper.Map<Building>(model);
            _repoBuilding.Update(building);
            return await _repoBuilding.SaveAll();
        }

        public async Task<List<BuildingDto>> GetAllAsync()
        {
            return await _repoBuilding.FindAll().ProjectTo<BuildingDto>(_configMapper).OrderByDescending(x => x.ID).ToListAsync();
        }

        public BuildingDto GetById(object id)
        {
            return _mapper.Map<Building, BuildingDto>(_repoBuilding.FindById(id));
        }

        public async Task<IEnumerable<HierarchyNode<BuildingDto>>> GetAllAsTreeView()
        {
            var lists = (await _repoBuilding.FindAll().Include(x => x.LunchTime).ProjectTo<BuildingDto>(_configMapper).OrderBy(x => x.Name).ToListAsync()).AsHierarchy(x => x.ID, y => y.ParentID);
            return lists;
        }

        public async Task<List<BuildingDto>> GetBuildings()
        {
            return await _repoBuilding.FindAll().Where(x => x.Level != 5).ProjectTo<BuildingDto>(_configMapper).OrderBy(x => x.Level).ToListAsync();

        }

        public async Task<object> CreateMainBuilding(BuildingDto buildingDto)
        {
            if (buildingDto.ID == 0)
            {
                var item = _mapper.Map<Building>(buildingDto);
                item.Level = 1;
                //item.ParentID = null;
                _repoBuilding.Add(item);
                try
                {
                    return new { status = await _repoBuilding.SaveAll(), building = item };
                }
                catch (Exception)
                {
                    return new { status = false };
                }

            }
            else
            {
                var item = _repoBuilding.FindById(buildingDto.ID);
                item.Name = buildingDto.Name;
                _repoBuilding.Update(item);
                try
                {
                    return new { status = await _repoBuilding.SaveAll(), building = item };
                }
                catch (Exception)
                {
                    return new { status = false };
                }

            }


        }

        public async Task<object> CreateSubBuilding(BuildingDto buildingDto)
        {
            var item = _mapper.Map<Building>(buildingDto);

            //Level cha tang len 1 va gan parentid cho subtask
            var itemParent = _repoBuilding.FindById(buildingDto.ParentID);
            item.Level = itemParent.Level + 1;
            item.ParentID = buildingDto.ParentID;
            _repoBuilding.Add(item);

            try
            {
                return new { status = await _repoBuilding.SaveAll(), building = item };
            }
            catch (Exception)
            {
                return new { status = false };
            }
        }

        public async Task<object> GetBuildingsForSetting() => await _repoBuilding.FindAll().Where(x => x.Level == 2).Select(x => new { x.ID, x.Name }).OrderBy(x => x.Name).ToListAsync();

        public async Task<bool> AddOrUpdateLunchTime(LunchTimeDto lunchTimeDto)
        {
            var ct = DateTime.Now;

            using var transaction = new TransactionScopeAsync().Create();
            {
                try
                {
                    lunchTimeDto.EndTime = lunchTimeDto.EndTime.ToLocalTime();
                    lunchTimeDto.StartTime = lunchTimeDto.StartTime.ToLocalTime();
                    var lunchTime = _mapper.Map<LunchTime>(lunchTimeDto);
                    var item = await _repoLunchTime.FindAll(x => x.BuildingID == lunchTime.BuildingID).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        _repoLunchTime.Add(lunchTime);
                        await _repoLunchTime.SaveAll();

                        // add period
                        var periodList = new List<Period>()
                {
                    {new Period
                    {
                        LunchTimeID = lunchTime.ID,
                        Sequence = 1,
                        StartTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),
                        EndTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),

                    }},
                     {new Period
                    {
                          LunchTimeID = lunchTime.ID,
                        Sequence = 2,
                        StartTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),
                        EndTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),

                    }},
                      {new Period
                    {
                           LunchTimeID = lunchTime.ID,
                        Sequence = 3,
                        StartTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),
                        EndTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 30,00),

                    }},
                       {new Period
                    {
                            LunchTimeID = lunchTime.ID,
                        Sequence = 4,
                        StartTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 0,00),
                        EndTime = new DateTime(ct.Year,ct.Month,ct.Day,0, 00,00),

                    }},
                        {new Period
                    {
                             LunchTimeID = lunchTime.ID,
                        Sequence = 5,
                        StartTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 0,00),
                        EndTime = new DateTime(ct.Year,ct.Month,ct.Day, 0, 00,00),

                    }}
                };
                        _repoPeriod.AddRange(periodList);
                        await _repoPeriod.SaveAll();
                        transaction.Complete();
                        return true;
                    }
                    else
                    {

                        if (lunchTimeDto.Content == "N/A")
                        {
                            _repoLunchTime.Remove(lunchTime);
                             await _repoLunchTime.SaveAll();

                            _repoPeriod.RemoveMultiple(_repoPeriod.FindAll(x => x.LunchTimeID == lunchTime.ID).ToList());
                            await _repoPeriod.SaveAll();
                        }
                        else
                        {
                            item.BuildingID = lunchTimeDto.BuildingID;
                            item.EndTime = lunchTimeDto.EndTime.ToLocalTime();
                            item.StartTime = lunchTimeDto.StartTime.ToLocalTime();
                            _repoLunchTime.Update(item);
                             await _repoLunchTime.SaveAll();
                            var periodList = _repoPeriod.FindAll(x => x.LunchTimeID == lunchTime.ID).ToList();
                            periodList.ForEach(item =>
                            {
                                item.StartTime = new DateTime(ct.Year, ct.Month, ct.Day, 0, 0, 00);
                                item.EndTime = new DateTime(ct.Year, ct.Month, ct.Day, 0, 00, 00);
                            });
                            _repoPeriod.UpdateRange(periodList);
                            await _repoPeriod.SaveAll();
                        }
                        transaction.Complete();
                        return true;
                    }
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    return false;
                }
              
            }
           
        }
    }
}