using DMR_API.DTO;
using DMR_API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API._Services.Interface
{
    public interface IBottomFactoryService
    {
        Task<ToDoListForReturnDto> ToDoList(int buildingID);
        Task<ToDoListForReturnDto> DoneList(int buildingID);
        Task<ToDoListForReturnDto> UndoneList(int buildingID);

        Task<DispatchListForReturnDto> DispatchList(int buildingID);
        Task<ToDoListForReturnDto> DelayList(int buildingID);
        Task<ToDoListForReturnDto> EVA_UVList(int buildingID);

        Task<ResponseDetail<object>> ScanQRCode(ScanQRCodeParams printParams);
        Task<ResponseDetail<object>> GenerateScanByNumber(GenerateSubpackageParams obj);
        Task<ResponseDetail<object>> AddDispatch(AddDispatchParams bfparams);


    }
}
