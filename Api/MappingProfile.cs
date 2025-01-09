using System.Globalization;
using Api.Dtos;
using Api.Dtos.Group;
using Api.Dtos.Table;
using AutoMapper;
using Domain.Entities;

namespace Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed))
            .ForMember(dest => dest.GroupIds, opt => opt.MapFrom(src => src.Groups.Select(g => g.Id)))
            .ForMember(dest => dest.GroupAsAdminIds, opt => opt.MapFrom(src => src.GroupsAsAdmin.Select(g => g.Id)));

        CreateMap<Group, GroupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Admin))
            .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.Users.Select(u => u.Id)))
            .ForMember(dest => dest.TableIds, opt => opt.MapFrom(src => src.Tables.Select(t => t.Id)));

        CreateMap<Table, TableDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime))
            .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
            .ForMember(dest => dest.StudentColumn, opt => opt.MapFrom(src => src.StudentColumn))
            .ForMember(dest => dest.HeaderRow, opt => opt.MapFrom(src => src.HeaderRow))
            .ForMember(dest => dest.AdditionalData, opt => opt.MapFrom(src => src.AdditionalData))
            .ForMember(dest => dest.ListToSearch, opt => opt.MapFrom(src => src.ListToSearch));

        CreateMap<UserDiff, UserDiffDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Table, opt => opt.MapFrom(src => src.Table))
            .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime))
            .ForMember(dest => dest.Diff,
                opt => opt.MapFrom(src =>
                    src.Diff.ToDictionary(key => key.Key,
                        val => double.Parse(val.Value, CultureInfo.InvariantCulture))));

        CreateMap<Table, MinimalTableDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));

        CreateMap<Group, MinimalGroupDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TableIds, opt => opt.MapFrom(src => src.Tables.Select(t => t.Id)));
    }
}