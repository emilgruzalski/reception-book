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

        CreateMap<Guest, GuestDto>();
        CreateMap<GuestForCreationDto, Guest>();
        CreateMap<GuestForUpdateDto, Guest>();

        CreateMap<Reservation, ReservationDto>()
            .ForMember(r => r.RoomNumber, opt => opt.MapFrom(x => x.Room!.Number))
            .ForMember(r => r.GuestFullName, opt => opt.MapFrom(x => $"{x.Guest!.FirstName} {x.Guest.LastName}"));
        CreateMap<ReservationForCreationDto, Reservation>();
        CreateMap<ReservationForUpdateDto, Reservation>();
    }
}
