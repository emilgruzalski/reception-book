using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class GuestService : IGuestService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public GuestService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<GuestDto> guests, MetaData metaData)> GetAllGuestsAsync(
        GuestParameters guestParameters, bool trackChanges)
    {
        var guestsWithMetaData = await _repository.Guest.GetAllGuestsAsync(guestParameters, trackChanges);

        var guestsDto = _mapper.Map<IEnumerable<GuestDto>>(guestsWithMetaData);

        return (guests: guestsDto, metaData: guestsWithMetaData.MetaData);
    }

    public async Task<GuestDto> GetGuestAsync(Guid guestId, bool trackChanges)
    {
        var guest = await GetGuestAndCheckIfItExists(guestId, trackChanges);

        return _mapper.Map<GuestDto>(guest);
    }

    public async Task<GuestDto> CreateGuestAsync(GuestForCreationDto guestForCreation)
    {
        var guest = _mapper.Map<Guest>(guestForCreation);
        guest.CreatedAt = DateTime.UtcNow;

        _repository.Guest.CreateGuest(guest);
        await _repository.SaveAsync();

        return _mapper.Map<GuestDto>(guest);
    }

    public async Task UpdateGuestAsync(Guid guestId, GuestForUpdateDto guestForUpdate, bool trackChanges)
    {
        var guest = await GetGuestAndCheckIfItExists(guestId, trackChanges);

        _mapper.Map(guestForUpdate, guest);
        await _repository.SaveAsync();
    }

    public async Task DeleteGuestAsync(Guid guestId, bool trackChanges)
    {
        var guest = await GetGuestAndCheckIfItExists(guestId, trackChanges);

        if (await _repository.Reservation.AnyForGuestAsync(guestId))
            throw new GuestHasReservationsBadRequestException(guestId);

        _repository.Guest.DeleteGuest(guest);
        await _repository.SaveAsync();
    }

    private async Task<Guest> GetGuestAndCheckIfItExists(Guid guestId, bool trackChanges)
    {
        var guest = await _repository.Guest.GetGuestAsync(guestId, trackChanges);
        if (guest is null)
            throw new GuestNotFoundException(guestId);

        return guest;
    }
}
