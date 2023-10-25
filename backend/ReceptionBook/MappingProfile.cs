using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace ReceptionBook;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomDto>();
        CreateMap<RoomForCreationDto, Room>();
        CreateMap<RoomForUpdateDto, Room>();
    }
}
