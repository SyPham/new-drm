using AutoMapper;
using AutoMapper.QueryableExtensions;
using DMR_API._Repositories.Interface;
using DMR_API._Services.Interface;
using DMR_API.DTO;
using DMR_API.Helpers;
using DMR_API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMR_API._Services.Services
{
    public class MailingService : IMailingService
    {
        private readonly IMailingRepository _repoMailing;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        private readonly HttpClient client;
        public MailingService(IMailingRepository repoMailing, IMapper mapper, MapperConfiguration configMapper, IHttpClientFactory clientFactory)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoMailing = repoMailing;
            client = clientFactory.CreateClient("default");
        }

        public async Task<bool> Add(MailingDto model)
        {
            if (_repoMailing.FindAll(x => x.UserID == model.UserID).Any())
            {
                return false;
            }
            var item = _mapper.Map<Mailing>(model);
            _repoMailing.Add(item);
            return await _repoMailing.SaveAll();
        }

        public async Task<bool> Delete(object id)
        {
            if (!_repoMailing.FindAll(x => x.UserID == id.ToInt()).Any())
            {
                return false;
            }
            var item = _repoMailing.FindById(id);
            _repoMailing.Remove(item);
            return await _repoMailing.SaveAll();

        }

        public async Task<List<MailingDto>> GetAllAsync()
        {
            var response = await client.GetAsync($"Users/GetAll");
            string json = response.Content.ReadAsStringAsync().Result;
            var users = JsonConvert.DeserializeObject<List<UserDto>>(json);
            var model = await _repoMailing.FindAll().ProjectTo<MailingDto>(_configMapper).ToListAsync();
            var result = from a in users
                         join b in model on a.ID equals b.UserID
                         select new MailingDto
                         {
                             ID = b.ID,
                             UserID = b.UserID,
                             UserName = a.Username,
                             TimeSend = b.TimeSend,
                             Frequency = b.Frequency
                         };
            var groupBy = result.ToList().GroupBy(x => x.Frequency);
            var list = new List<MailingDto>();
            foreach (var item in groupBy)
            {
                list.Add(new MailingDto
                {
                    UserNames = item.Select(x => x.UserName).ToList(),
                    UserIDList = item.Select(x => x.UserID).ToList(),
                    TimeSend = item.First().TimeSend,
                    Frequency = item.Key
                });
            }

            return list;
        }

        public MailingDto GetById(object id)
        {
            return  _repoMailing.FindAll().ProjectTo<MailingDto>(_configMapper).FirstOrDefault();
        }

        public Task<PagedList<MailingDto>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<MailingDto>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(MailingDto model)
        {
            var item = _repoMailing.FindById(model.ID);
            if (item is null) return false;
            item.UserID = model.UserID;
            item.TimeSend = model.TimeSend;
            item.Email = model.Email;
            _repoMailing.Update(item);
            return await _repoMailing.SaveAll();
        }
    }
}
