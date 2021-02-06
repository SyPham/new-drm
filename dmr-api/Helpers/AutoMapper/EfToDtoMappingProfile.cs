using System;
using DMR_API.DTO;
using DMR_API.Models;
using AutoMapper;
using System.Linq;
using dmr_api.Models;
using System.Collections.Generic;

namespace DMR_API.Helpers.AutoMapper
{
    public class EfToDtoMappingProfile : Profile
    {
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public EfToDtoMappingProfile()
        {
            CreateMap<Dispatch, DispatchTodolistDto>();

            CreateMap<User, UserForDetailDto>();
            CreateMap<Glue, GlueDto>().ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));
            CreateMap<Glue, GlueCreateDto>().ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));
            CreateMap<Glue, GlueCreateDto1>()
                .ForMember(d => d.GlueID, o => o.MapFrom(s => s.ID))
                .ForMember(d => d.KindName, o => o.MapFrom(s => s.Kind == null ? string.Empty : s.Kind.Name))
                .ForMember(d => d.PartName, o => o.MapFrom(s => s.Part == null ? string.Empty : s.Part.Name))
                .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material == null ? string.Empty : s.Material.Name))
                .ForMember(d => d.Chemical, o => o.MapFrom(s => new GlueDto1 { ID = s.ID, Name = s.Name }))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));
            CreateMap<Ingredient, IngredientDto>()
                .ForMember(d => d.Supplier, o => o.MapFrom(x => x.Supplier.Name))
                .ForMember(d => d.GlueType, o => o.MapFrom(x => x.GlueType))
                .ForMember(d => d.VOC, o => o.MapFrom(x => x.VOC.ToDouble()))
                .ForMember(d => d.Unit, o => o.MapFrom(x => x.Unit.ToDouble()))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));
            CreateMap<Role, RoleDto>();
            CreateMap<LunchTime, LunchTimeDto>()
                .ForMember(x => x.Content, o => o.Ignore());
            CreateMap<Ingredient, IngredientDto1>()
                .ForMember(d => d.Supplier, o => o.MapFrom(x => x.Supplier.Name))
                .ForMember(d => d.GlueType, o => o.MapFrom(x => x.GlueType))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));/*.ForMember(d => d.Position, o => o.MapFrom(s => s.Position != null ? alpha[s.Position -1] : ""))*/;
            CreateMap<Ingredient, IngredientForImportExcelDto>().ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate.ToParseStringDateTime()));

            CreateMap<Line, LineDto>();
            CreateMap<Station, StationDto>()
                .ForMember(d => d.GlueName, o => o.MapFrom(x => x.Glue.GlueName.Name));
            CreateMap<Plan, PlanDto>()
                 .ForMember(d => d.Glues, o => o.MapFrom(x => x.BPFCEstablish.Glues.Where(x => x.isShow).Select(x => x.Name)))
                 .ForMember(d => d.StartTime, o => o.MapFrom(x => new TimeDto(x.StartWorkingTime)))
                 .ForMember(d => d.EndTime, o => o.MapFrom(x => new TimeDto(x.FinishWorkingTime)))
                 .ForMember(d => d.IsGenerate, o => o.MapFrom(x => x.ToDoList.Count > 0))
                 .ForMember(d => d.ModelName, o => o.MapFrom(x => x.BPFCEstablish.ModelName.Name))
                .ForMember(d => d.ModelNoName, o => o.MapFrom(x => x.BPFCEstablish.ModelNo.Name))
                .ForMember(d => d.ArticleName, o => o.MapFrom(x => x.BPFCEstablish.ArticleNo.Name))
                .ForMember(d => d.BuildingName, o => o.MapFrom(x => x.Building.Name))
                .ForMember(d => d.BPFCName, o => o.MapFrom(x => $"{x.BPFCEstablish.ModelName.Name} - {x.BPFCEstablish.ModelNo.Name} - {x.BPFCEstablish.ArticleNo.Name} - {x.BPFCEstablish.ArtProcess.Process.Name}"))
                .ForMember(d => d.ProcessName, o => o.MapFrom(x => x.BPFCEstablish.ArtProcess.Process.Name))
                .ForMember(d => d.ModelNameID, o => o.MapFrom(x => x.BPFCEstablish.ModelNameID))
                .ForMember(d => d.ModelNoID, o => o.MapFrom(x => x.BPFCEstablish.ModelNoID))
                .ForMember(d => d.ArticleNoID, o => o.MapFrom(x => x.BPFCEstablish.ArticleNoID))
                .ForMember(d => d.ArtProcessID, o => o.MapFrom(x => x.BPFCEstablish.ArtProcessID));
            CreateMap<ModelNo, ModelNoDto>();
            CreateMap<ArtProcess, ArtProcessDto>();
            CreateMap<Process, ProcessDto>();
            CreateMap<Kind, KindDto>();
            CreateMap<Part, PartDto>();
            CreateMap<Material, MaterialDto>();
            CreateMap<ModelName, ModelNameDto>();
            CreateMap<UserDetailDto, UserDetail>();
            CreateMap<Supplier, SuppilerDto>();
            CreateMap<ArticleNo, ArticleNoDto>();
            CreateMap<Building, BuildingDto>()
                .ForMember(d => d.ParentID, o => o.MapFrom(x => x.ParentID == 0 || x.ParentID == null ? null : x.ParentID))
                .ForMember(d => d.LunchTimeID, o => o.MapFrom(x => x.LunchTime == null ? 0 : x.LunchTime.ID))
                .ForMember(d => d.LunchTime, o => o.MapFrom(x => x.LunchTime == null ? "N/A" : $"{x.LunchTime.StartTime.Hour.ToString("D2")}:{x.LunchTime.StartTime.Minute.ToString("D2")} - {x.LunchTime.EndTime.Hour.ToString("D2")}:{x.LunchTime.EndTime.Minute.ToString("D2")}"));
            CreateMap<BuildingUser, BuildingUserDto>();
            CreateMap<Comment, CommentDto>();
            CreateMap<BPFCEstablish, BPFCEstablishDto>()
                .ForMember(d => d.DueDateStatus, o => o.MapFrom(x => x.ArtProcess.Process.Name == "ASY" ? false : DateTime.Now.Date > x.DueDate.Value.Date))
                .ForMember(d => d.ModelName, o => o.MapFrom(x => x.ModelName.Name))
                .ForMember(d => d.ModelNo, o => o.MapFrom(x => x.ModelNo.Name))
                .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.ArticleNo.Name))
                .ForMember(d => d.ArtProcess, o => o.MapFrom(x => x.ArtProcess.Process.Name));
            CreateMap<BPFCEstablish, BPFCStatusDto>()
              .ForMember(d => d.ModelName, o => o.MapFrom(x => x.ModelName.Name))
              .ForMember(d => d.ModelNo, o => o.MapFrom(x => x.ModelNo.Name))
              .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.ArticleNo.Name))
              .ForMember(d => d.ArtProcess, o => o.MapFrom(x => x.ArtProcess.Process.Name));
            CreateMap<BPFCEstablish, BPFCRecordDto>()
           .ForMember(d => d.ModelName, o => o.MapFrom(x => x.ModelName.Name))
           .ForMember(d => d.ModelNo, o => o.MapFrom(x => x.ModelNo.Name))
           .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.ArticleNo.Name))
           .ForMember(d => d.ArtProcess, o => o.MapFrom(x => x.ArtProcess.Process.Name));

            CreateMap<MixingInfo, MixingInfoDto>()
            .ForMember(d => d.ExpiredTime, o => o.MapFrom(x => x.ExpriedTime()))
             .ForMember(d => d.RealTotal, o => o.MapFrom(real => real.MixingInfoDetails.Sum(x => x.Amount)));

            CreateMap<MixingInfoForCreateDto, MixingInfo>();

            CreateMap<BPFCEstablish, BPFCEstablish>()
             .ForMember(d => d.ModelName, o => o.MapFrom(x => x.ModelName.Name))
             .ForMember(d => d.ModelNo, o => o.MapFrom(x => x.ModelNo.Name))
             .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.ArticleNo.Name))
             .ForMember(d => d.ArtProcess, o => o.MapFrom(x => x.ArtProcess.Process.Name));
            CreateMap<IngredientInfoDto, IngredientInfo>();
            CreateMap<IngredientInfoReportDto, IngredientInfoReport>();
            CreateMap<Stir, StirDTO>()
             .ForMember(d => d.GlueType, o => o.MapFrom(x => x.MixingInfo.Glue.GlueIngredients.FirstOrDefault(x => x.Position == "A").Ingredient.GlueType))
                ;
            CreateMap<Setting, SettingDTO>();
            CreateMap<PlanForCloneDto, Plan>();
            CreateMap<ScaleMachineDto, ScaleMachine>();
            CreateMap<ToDoList, ToDoListDto>();
            CreateMap<Glue, ConsumtionDto>()
           .ForMember(d => d.ModelName, o => o.MapFrom(x => x.BPFCEstablish.ModelName.Name))
           .ForMember(d => d.ModelNo, o => o.MapFrom(x => x.BPFCEstablish.ModelNo.Name))
           .ForMember(d => d.Std, o => o.MapFrom(x => x.Consumption.ToFloat()))
           .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.BPFCEstablish.ArticleNo.Name))
           .ForMember(d => d.ArticleNo, o => o.MapFrom(x => x.BPFCEstablish.ArticleNo.Name));
            CreateMap<MixingInfoDetailForAddDto, MixingInfoDetail>()
           .ForMember(d => d.MixingInfo, o => o.Ignore())
           .ForMember(d => d.Ingredient, o => o.Ignore());
            CreateMap<Mailing, MailingDto>();

        }

    }
}