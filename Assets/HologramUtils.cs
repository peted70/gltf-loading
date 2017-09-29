using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IHologram
{
    string Name { get; set; }
}

public interface ILogger
{
    void Log(object message);
}

public class UnityLogger : ILogger
{
    public void Log(object message)
    {
        Debug.Log(message);
    }
}

public interface IHologramCollection
{
    Task<IEnumerable<IHologram>> GetHologramsAsync();
}

public class Hologram : IHologram
{
    public string Name { get; set; }
}

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
