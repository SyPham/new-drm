using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMR_API._Repositories.Interface;
using DMR_API._Services.Interface;
using DMR_API.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DMR_API.SignalrHub
{
    public class ECHub : Hub
    {

        static HashSet<string> CurrentConnections = new HashSet<string>();
        private readonly static ConnectionMapping<string> _connections =
         new ConnectionMapping<string>();
        private readonly IToDoListService _todoService;
        private readonly IMailExtension _emailService;

        public ECHub(
            IToDoListService todoService,
            IMailExtension emailService

            )
        {
            _todoService = todoService;
            _emailService = emailService;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task Welcom(string scalingMachineID, string message, string unit)
        {
            await Clients.All.SendAsync("Welcom", scalingMachineID, message, unit);
        }
        public async Task JoinHub(int machineID)
        {

        }
        public async Task SendMail(string scalingMachineID)
        {

            var file = await _todoService.ExportExcelToDoListWholeBuilding();
            var subject = "Mixing Room Report";
            var fileName = $"mixingRoomReport{DateTime.Now.ToString("MMddyyyy")}.xlsx";
            var message = "Please refer to the Mixing Room Report";
            var mailList = new List<string> {
                        "mel.kuo@shc.ssbshoes.com",
                        "maithoa.tran@shc.ssbshoes.com",
                        "andy.wu@shc.ssbshoes.com",
                        "sin.chen@shc.ssbshoes.com",
                        "leo.doan@shc.ssbshoes.com",
                        "heidy.amos@shc.ssbshoes.com",
                        "bonding.team@shc.ssbshoes.com",
                        "Ian.Ho@shc.ssbshoes.com",
                        "swook.lu@shc.ssbshoes.com",
                        "damaris.li@shc.ssbshoes.com",
                        "peter.tran@shc.ssbshoes.com"
                };
            if (file != null || file.Length > 0)
            {
                await _emailService.SendEmailWithAttactExcelFileAsync(mailList, subject, message, fileName, file);
            }
        }
        public async Task CheckOnline()
        {
            await Clients.All.SendAsync("Online", CurrentConnections.Count);
        }
        public async Task Todolist(int buildingID)
        {
            await Clients.All.SendAsync("ReceiveTodolist", buildingID);
        }
        public async Task CreatePlan()
        {
            await Clients.All.SendAsync("ReceiveCreatePlan");
        }
        public override async Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            CurrentConnections.Add(id);
            await Clients.All.SendAsync("Online", CurrentConnections.Count);
            _connections.Add(id, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = CurrentConnections.FirstOrDefault(x => x == Context.ConnectionId);
            if (connection != null)
            {
                CurrentConnections.Remove(connection);
                _connections.Remove(connection, Context.ConnectionId);
            }
            await Clients.All.SendAsync("Online", CurrentConnections.Count);
            await base.OnDisconnectedAsync(exception);
        }

        //return list of all active connections
        public List<string> GetAllActiveConnections()
        {
            return CurrentConnections.ToList();
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}