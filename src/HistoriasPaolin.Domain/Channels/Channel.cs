using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class Channel : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsMadeForKids { get; set; } = true;
    public string DefaultAspectRatio { get; set; } = "16:9";
    public int DefaultVideoDurationSeconds { get; set; } = 180;
    public string DefaultPublicationPrivacy { get; set; } = ChannelPublicationPrivacy.Private;
    public ICollection<ChannelBrand> Brands { get; set; } = new List<ChannelBrand>();
    public ICollection<EditorialStrategy> EditorialStrategies { get; set; } = new List<EditorialStrategy>();

    public ChannelBrand? ActiveBrand => Brands.SingleOrDefault(brand => brand.IsActive);

    public void SetBrand(ChannelBrand brand)
    {
        if (brand.IsActive)
        {
            foreach (var existingBrand in Brands.Where(existingBrand => existingBrand.Id != brand.Id))
            {
                existingBrand.IsActive = false;
            }
        }

        if (brand.ChannelId == Guid.Empty)
        {
            brand.ChannelId = Id;
        }

        var existing = Brands.SingleOrDefault(existingBrand => existingBrand.Id == brand.Id);
        if (existing is null)
        {
            Brands.Add(brand);
        }
    }

    public bool HasActiveEditorialStrategy(Guid? exceptId = null) =>
        EditorialStrategies.Any(strategy => strategy.IsActive && (!exceptId.HasValue || strategy.Id != exceptId.Value));
}
