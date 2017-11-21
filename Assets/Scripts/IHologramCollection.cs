using System.Collections.Generic;
using System.Threading.Tasks;
public interface IHologramCollection
{
    Task<IEnumerable<IHologram>> GetHologramsAsync();
}
