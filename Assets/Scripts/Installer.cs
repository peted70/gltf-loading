using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Zenject;

public class Installer : MonoInstaller<Installer>
{
    public override void InstallBindings()
    {
        Debug.WriteLine("Here 1");
        Container.Bind<IHologram>().To<Hologram>().AsTransient();
        Container.Bind<IHologramCollection>().To<AzureHologramCollection>().AsSingle();
        Debug.WriteLine("Here 2");
        Container.Bind<ILogger>().To<UnityLogger>().AsSingle();
        Container.Bind<ObjectData>().AsTransient();
    }
}

