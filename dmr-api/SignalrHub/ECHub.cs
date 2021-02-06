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
        private readonly static ConnectionMapping<string> _connectionsOnline =
        new ConnectionMapping<string>();
      
        private readonly IToDoListService _todoService;
        private readonly IMailingService _mailingService;
        private readonly IMailExtension _emailService;
        public ECHub(
            IToDoListService todoService,
            IMailingService mailingService,
            IMailExtension emailService

            )
        {
            _todoService = todoService;
            _mailingService = mailingService;
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
        public async Task WeighingScale(string scalingMachineID, string message, string unit, string building)
        {
            var groupName = building;
            await Clients.Group(groupName).SendAsync("ReceiveAmountWeighingScale", scalingMachineID, message, unit, building);
        }

        public async Task JoinGroup(string building)
        {
            var groupName = building;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var id = Context.ConnectionId;
            Console.WriteLine($"Client ID: {id} joined hub name {building}");
        }
        public async Task SendMail(string scalingMachineID)
        {

            var file = await _todoService.ExportExcelToDoListWholeBuilding();
            var subject = "Mixing Room Report";
            var fileName = $"mixingRoomReport{DateTime.Now.ToString("MMddyyyy")}.xlsx";
            var message = "Please refer to the Mixing Room Report";
            var mailList = new List<string>
            {
                //"mel.kuo@shc.ssbshoes.com",
                //"maithoa.tran@shc.ssbshoes.com",
                //"andy.wu@shc.ssbshoes.com",
                //"sin.chen@shc.ssbshoes.com",
                //"leo.doan@shc.ssbshoes.com",
                //"heidy.amos@shc.ssbshoes.com",
                //"bonding.team@shc.ssbshoes.com",
                //"Ian.Ho@shc.ssbshoes.com",
                //"swook.lu@shc.ssbshoes.com",
                //"damaris.li@shc.ssbshoes.com",
                //"peter.tran@shc.ssbshoes.com"
            };
            if (file != null || file.Length > 0)
            {
                await _emailService.SendEmailWithAttactExcelFileAsync(mailList, subject, message, fileName, file);
            }
        }
        public async Task AskMailing()
        {
            var mailingList = await _mailingService.GetAllAsync();
            await Clients.Group("Mailing").SendAsync("ReceiveMailing", mailingList);
        }
        public async Task Mailing()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Mailing");
            var mailingList = await _mailingService.GetAllAsync();
            await Clients.Group("Mailing").SendAsync("ReceiveMailing", mailingList);
        }
        public async Task CheckOnline(int userID)
        {
            string key = userID + "";
            var connectionID = _connectionsOnline.FindConnection(key);
            if (connectionID == null)
            {
                _connectionsOnline.Add(key, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Online");
                await Clients.Group("Online").SendAsync("Online", _connectionsOnline.Count);
            } else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Online");
                _connectionsOnline.Add(key, Context.ConnectionId);
                await Clients.Group("Online").SendAsync("Online", _connectionsOnline.Count);

            }
        }

        public async Task JoinReloadDispatch()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ReloadDispatch");
        }
        public async Task ReloadDispatch()
        {
            await Clients.Group("ReloadDispatch").SendAsync("ReloadDispatch");
        }
        public async Task JoinReloadTodo()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ReloadTodo");
        }
        public async Task ReloadTodo()
        {
            await Clients.Group("ReloadTodo").SendAsync("ReloadTodo");
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
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = CurrentConnections.FirstOrDefault(x => x == Context.ConnectionId);
            if (connection != null)
            {
                CurrentConnections.Remove(connection);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Online");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Mailing");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ReloadDispatch");
            }

            var key = _connectionsOnline.FindKeyByValue(Context.ConnectionId);
            if (key != null)
            {
                _connectionsOnline.RemoveKeyAndValue(key, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Online");
                await Clients.Group("Online").SendAsync("Online", _connectionsOnline.Count);
            }
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