using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

public class Installer : MonoInstaller<Installer>
{
    public override void InstallBindings()
    {
        Container.Bind<IHologram>().To<Hologram>().AsTransient();
        Container.Bind<IHologramCollection>().To<HologramCatalog>().AsSingle();
        Container.Bind<ILogger>().To<UnityLogger>().AsSingle();
        Container.Bind<ObjectData>().AsTransient();
    }
}

