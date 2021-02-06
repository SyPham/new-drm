using AutoMapper;
using AutoMapper.QueryableExtensions;
using DMR_API._Repositories.Interface;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using DMR_API.Helpers;
using DMR_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API._Services.Services
{
    public class BuildingUserService : IBuildingUserService
    {
        private readonly IBuildingUserRepository _buildingUserRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public BuildingUserService(IBuildingUserRepository buildingUserRepository,
            IBuildingRepository buildingRepository,
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _buildingUserRepository = buildingUserRepository;
            _buildingRepository = buildingRepository;
            _userRoleRepository = userRoleRepository;
        }

        public Task<bool> Add(BuildingUserDto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BuildingUserDto>> GetAllAsync()
        {
            return await _buildingUserRepository.FindAll().ProjectTo<BuildingUserDto>(_configMapper).ToListAsync();

        }

        //public async Task<object> GetBuildingByUserID(int userid)
        //{
        //    var model = await _buildingUserRepository.FindAll().FirstOrDefaultAsync(x => x.UserID == userid);
        //    if (model == null) return new Building();
        //    return _buildingRepository.FindById(model.BuildingID);
        //}

        public async Task<List<BuildingUserDto>> GetBuildingUserByBuildingID(int buildingID)
        {
            return await _buildingUserRepository.FindAll().Where(x => x.BuildingID == buildingID).ProjectTo<BuildingUserDto>(_configMapper).ToListAsync();
        }

        public BuildingUserDto GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<BuildingUserDto>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public async Task<object> MapBuildingUser(int userid, int buildingid)
        {
            var buildingLevel = 2;
            var item = await _buildingUserRepository.FindAll().Include(x => x.Building).Where(x => x.Building.Level == buildingLevel && x.UserID == userid).ToListAsync();
            if (item.Count == 0)
            {
                try
                {
                    _buildingUserRepository.Add(new BuildingUser
                    {
                        UserID = userid,
                        BuildingID = buildingid,
                        CreatedDate = DateTime.Now
                    });

                    await _buildingUserRepository.SaveAll();
                    return new
                    {
                        status = true,
                        message = "Mapping Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new
                    {
                        status = false,
                        message = "Failed on save!"
                    };
                }
            }
            else
            {

                try
                {
                    _buildingUserRepository.RemoveMultiple(item);
                    await _buildingUserRepository.SaveAll();
                    _buildingUserRepository.Add(new BuildingUser
                    {
                        UserID = userid,
                        BuildingID = buildingid,
                        CreatedDate = DateTime.Now
                    });
                    return new
                    {
                        status = await _buildingUserRepository.SaveAll(),
                        message = "Mapping Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new
                    {
                        status = false,
                        message = "Failed on save!"
                    };
                }
            }
        }


        public async Task<object> MappingUserWithBuilding(BuildingUserDto buildingUserDto)
        {
            var item = await _buildingUserRepository.FindAll().FirstOrDefaultAsync(x => x.UserID == buildingUserDto.UserID && x.BuildingID == buildingUserDto.BuildingID);
            if (item == null)
            {
                _buildingUserRepository.Add(new BuildingUser
                {
                    UserID = buildingUserDto.UserID,
                    BuildingID = buildingUserDto.BuildingID
                });
                try
                {
                    await _buildingUserRepository.SaveAll();
                    return new
                    {
                        status = true,
                        message = "Mapping Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new
                    {
                        status = false,
                        message = "Failed on save!"
                    };
                }
            }
            else
            {

                return new
                {
                    status = false,
                    message = "The User belonged with other building!"
                };
            }
        }


        public async Task<object> RemoveBuildingUser(BuildingUserDto buildingUserDto)
        {
            var item = await _buildingUserRepository.FindAll().FirstOrDefaultAsync(x => x.UserID == buildingUserDto.UserID && x.BuildingID == buildingUserDto.BuildingID);
            if (item != null)
            {
                _buildingUserRepository.Remove(item);
                try
                {
                    await _buildingUserRepository.SaveAll();
                    return new
                    {
                        status = true,
                        message = "Delete Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new
                    {
                        status = false,
                        message = "Failed on delete!"
                    };
                }
            }
            else
            {

                return new
                {
                    status = false,
                    message = ""
                };
            }
        }


        public Task<PagedList<BuildingUserDto>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(BuildingUserDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDetail<object>> RemoveLineUser(BuildingUserDto buildingUserDto)
        {
            var lineLevel = 3;
            var item = await _buildingUserRepository.FindAll().Include(x => x.Building).FirstOrDefaultAsync(x => x.Building.Level == lineLevel && x.UserID == buildingUserDto.UserID && x.BuildingID == buildingUserDto.BuildingID);
            if (item != null)
            {
                _buildingUserRepository.Remove(item);
                try
                {
                    await _buildingUserRepository.SaveAll();
                    return new ResponseDetail<object>()
                    {
                        Status = true,
                        Message = "Delete Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new ResponseDetail<object>()
                    {
                        Status = false,
                        Message = "Failed on delete!"
                    };
                }
            }
            else
            {

                return new ResponseDetail<object>()
                {
                    Status = false,
                    Message = ""
                };
            }
        }
        public async Task<ResponseDetail<object>> MapLineUser(int userid, int buildingid)
        {
            var lineLevel = 3;
            var item = await _buildingUserRepository.FindAll().Include(x => x.Building).FirstOrDefaultAsync(x => x.Building.Level == lineLevel && x.UserID == userid && x.BuildingID == buildingid);
            if (item != null)
            {
                return new ResponseDetail<object>
                {
                    Status = false,
                    Message = "Đã map chuyền này rồi!"
                };
            }
            try
            {
                _buildingUserRepository.Add(new BuildingUser
                {
                    UserID = userid,
                    BuildingID = buildingid,
                    CreatedDate = DateTime.Now
                });

                await _buildingUserRepository.SaveAll();
                return new ResponseDetail<object>
                {
                    Status = true,
                    Message = "Mapping Successfully!"
                };
            }
            catch (Exception)
            {
                return new ResponseDetail<object>
                {
                    Status = false,
                    Message = "Failed on save!"
                };
            }

        }

        public async Task<ResponseDetail<List<BuildingDto>>> GetLineByUserID(int userid, int buildingid)
        {
            var model = from a in _buildingRepository.FindAll(x => x.Level == 3 && x.ParentID == buildingid)
                        join b in _buildingUserRepository.FindAll(x => x.UserID == userid) on a.ID equals b.BuildingID into ab
                        from c in ab.DefaultIfEmpty()
                        select new BuildingDto
                        {
                            ID = a.ID,
                            Level = a.Level,
                            Status = c == null ? false : true,
                            Name = a.Name
                        };

            return new ResponseDetail<List<BuildingDto>>
            {
                Data = await model.ToListAsync(),
                Status = true
            };
        }

        public async Task<ResponseDetail<object>> RemoveMultipleBuildingUser(BuildingUserDto buildingUserDto)
        {
            var buildingLevel = 2;
            var item = await _buildingUserRepository.FindAll().Include(x => x.Building).FirstOrDefaultAsync(x => x.Building.Level == buildingLevel && x.UserID == buildingUserDto.UserID && x.BuildingID == buildingUserDto.BuildingID);
            if (item != null)
            {
                _buildingUserRepository.Remove(item);
                try
                {
                    await _buildingUserRepository.SaveAll();
                    return new ResponseDetail<object>()
                    {
                        Status = true,
                        Message = "Delete Successfully!"
                    };
                }
                catch (Exception)
                {
                    return new ResponseDetail<object>()
                    {
                        Status = false,
                        Message = "Failed on delete!"
                    };
                }
            }
            else
            {

                return new ResponseDetail<object>()
                {
                    Status = false,
                    Message = ""
                };
            }
        }
        public async Task<ResponseDetail<object>> MapMultipleBuildingUser(int userid, int buildingid)
        {
            var buildingLevel = 2;
            var item = await _buildingUserRepository.FindAll().Include(x => x.Building).FirstOrDefaultAsync(x => x.Building.Level == buildingLevel && x.UserID == userid && x.BuildingID == buildingid);
            if (item != null)
            {
                return new ResponseDetail<object>
                {
                    Status = false,
                    Message = "Đã map tào nhà này rồi!"
                };
            }
            try
            {
                _buildingUserRepository.Add(new BuildingUser
                {
                    UserID = userid,
                    BuildingID = buildingid,
                    CreatedDate = DateTime.Now
                });

                await _buildingUserRepository.SaveAll();
                return new ResponseDetail<object>
                {
                    Status = true,
                    Message = "Mapping Successfully!"
                };
            }
            catch (Exception)
            {
                return new ResponseDetail<object>
                {
                    Status = false,
                    Message = "Failed on save!"
                };
            }

        }

        public async Task<ResponseDetail<List<BuildingDto>>> GetBuildingByUserID(int userid)
        {
            var buildingLevel = 2;

            var model = from a in _buildingRepository.FindAll(x => x.Level == buildingLevel)
                        join b in _buildingUserRepository.FindAll(x => x.UserID == userid) on a.ID equals b.BuildingID into ab
                        from c in ab.DefaultIfEmpty()
                        select new BuildingDto
                        {
                            ID = a.ID,
                            Level = a.Level,
                            Status = c == null ? false : true,
                            Name = a.Name
                        };

            return new ResponseDetail<List<BuildingDto>>
            {
                Data = await model.ToListAsync(),
                Status = true
            };
        }

        public async Task<ResponseDetail<List<BuildingDto>>> GetBuildingUserByUserID(int userid)
        {
            var buildingLevel = 2;

            var role = await _userRoleRepository.FindAll(x => x.UserID == userid).FirstOrDefaultAsync();
            if (role.RoleID == (int)Enums.Role.Worker)
            {
               var data = await _buildingUserRepository.FindAll(x => x.UserID == userid).Include(x=> x.Building).Select(a=> new BuildingDto
               {
                   ID = a.Building.ID,
                   Level = a.Building.Level,
                   Name = a.Building.Name
               }).ToListAsync();
                return new ResponseDetail<List<BuildingDto>>
                {
                    Data = data,
                    Status = true
                };
            } 
            if (role.RoleID == (int)Enums.Role.Admin || role.ID == (int)Enums.Role.Supervisor || role.ID == (int)Enums.Role.Staff)
            {
                var data = await _buildingRepository.FindAll(x => x.Level == buildingLevel).Select(a => new BuildingDto
                {
                    ID = a.ID,
                    Level = a.Level,
                    Name = a.Name
                }).ToListAsync();
                return new ResponseDetail<List<BuildingDto>>
                {
                    Data = data,
                    Status = true
                };
            }
            if (role.RoleID == (int)Enums.Role.Dispatcher )
            {
                var data = await _buildingUserRepository.FindAll(x => x.UserID == userid)
                    .Include(x => x.Building)
                    .Where(x=> x.Building.Level == buildingLevel).Select(a => new BuildingDto
                {
                    ID = a.Building.ID,
                    Level = a.Building.Level,
                    Name = a.Building.Name
                }).ToListAsync();
                return new ResponseDetail<List<BuildingDto>>
                {
                    Data = data,
                    Status = true
                };
            }

            return new ResponseDetail<List<BuildingDto>>
            {
                Data = null,
                Status = false
            };
        }
    }
}
