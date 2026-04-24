using AutoMapper;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Domain.Entities;

namespace Sentia.Application.Mappings;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageDto>();
    }
}
