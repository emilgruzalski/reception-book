using Entities.Models;

namespace Shared.RequestFeatures;

public class ReservationParameters : RequestParameters
{
    public ReservationStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? GuestId { get; set; }
    public string? SearchTerm { get; set; }

    public bool ValidDateRange => !FromDate.HasValue || !ToDate.HasValue || ToDate >= FromDate;
}
