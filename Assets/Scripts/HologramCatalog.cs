using System.Collections.Generic;
using System.Threading.Tasks;

public class HologramCatalog : IHologramCollection
{
    IEnumerable<IHologram> ctlg;

    public Task<IEnumerable<IHologram>> GetHologramsAsync()
    {
        ctlg =
            new List<IHologram>
            {
                new Hologram() { Name = "h1" },
                new Hologram() { Name = "h2" },
                new Hologram() { Name = "h3" },
                new Hologram() { Name = "h4" },
                new Hologram() { Name = "h5" },
                new Hologram() { Name = "h6" },
                new Hologram() { Name = "h7" },
                new Hologram() { Name = "h8" },
                new Hologram() { Name = "h9" },
            };

        return Task.FromResult(ctlg);
    }
}
