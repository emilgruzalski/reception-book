using Entities.Models;

namespace Shared.RequestFeatures;

public class RoomParameters : RequestParameters
{
    public RoomParameters() => OrderBy = "number";

    public RoomType? Type { get; set; }
    public RoomStatus? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinCapacity { get; set; }
    public string? SearchTerm { get; set; }

    public bool ValidPriceRange => !MinPrice.HasValue || !MaxPrice.HasValue || MaxPrice >= MinPrice;
}
