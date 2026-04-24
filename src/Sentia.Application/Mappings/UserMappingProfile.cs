using AutoMapper;
using Sentia.Application.Features.Users.Dtos;
using Sentia.Domain.Entities;

namespace Sentia.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
