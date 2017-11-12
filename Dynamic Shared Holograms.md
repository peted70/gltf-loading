# Dynamic Shared Holograms

I’ve decided to break this post up into a series as when I got into this I realised there are a number of natural sections with their own merit. So think of this as a trip through my thought process as I put together a sample app. 

1. *Dependency Injection*

    This may not seem absolutely necessary for a sample app but early in my career I learnt the hard way that anticipating change and coding against abstractions was almost always the best way to begin writing *any* software. I would argue that samples benefit the most from this since dependencies can be more easily understood and reconfigured and also the easier a sample is to keep up to date the less likely it is that the sample will become obsolete and potentially counter-productive to the reader. Of course there is a tradeoff here between obscuring the main point of the sample with extraneous code and reaping the benefits of DI.

    I had previously settled on using some kind of [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection) in my apps but since working with Unity I had fallen out of that practice. This post gives me a chance to revisit and after some short research I found [Zenject](https://github.com/modesttree/Zenject) and decided to give it a try.

    For this sample I wanted to have an online model catalogue so the first job for DI was to hide the apps data retrieval behind an interface and have that configured by Zenject. I started with a dummy implementation of the interface:

```cs
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
```
Note that I'm using a Task-based async interface; enabled by using the experimental .net 4.6 support in Unity.

    ![Unity .NET4.6 Support]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\Zenject SceneContext.PNG")

Then, adding Zenject to the project using the Asset store. Next a Zenject SceneContext needs to be added to the scene which is facilitated with a menu editor option:

   ![Zenject SceneContext]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\net46unity.PNG")    

Next, a Zenject Installer script is needed, again this can be added using an editor extension:

   ![Zenject Installer]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\Zenject Installer.PNG")    
     
The installer is the script where you can configure the dependencies for the app. I added code to supply an IHologramCollection implemented by the concrete class HologramCatalog with a view to replacing this later on with a real cloud catalogue.

```cs
public class Installer : MonoInstaller<Installer>
{
    public override void InstallBindings()
    {
        Container.Bind<IHologram>()
                 .To<Hologram>()
                 .AsTransient();
        Container.Bind<IHologramCollection>()
                 .To<HologramCatalog>()
                 .AsSingle();
        Container.Bind<ILogger>()
                 .To<UnityLogger>()
                 .AsSingle();
        Container.Bind<ObjectData>()
                 .AsTransient();
    }
}
```
Now I can create a Monobehaviour-derived class to handle data retrieval. It's necessary to declare a function to inject the interfaces here, amd decorate with the Inject attribute. (Note, I'm also injecting an ILogger interface here).

```cs
public class ObjectData : MonoBehaviour
{
    [Inject]
    public void Construct(IHologramCollection hologramCatalog, ILogger log)
    {
        _hologramCatalog = hologramCatalog;
        _log = log;
    }

    private IHologramCollection _hologramCatalog;
    private ILogger _log;
```

Now, with the help of async/await we can call the interface and action the data items returned:

```cs
    async void Start()
    {
        var res = await _hologramCatalog.GetHologramsAsync();
        foreach (var r in res)
        {
            // Populate a menu with a representation of the
            // 3D models..
        }
    }
```
2. *Model Loading*

    The 3D industry has been lacking a ubiquitous, standard file format for scene description. This is the motivation behind the creation of the GLTF specification (see [link](https://github.com/KhronosGroup/glTF)) by the Khronos Group. Significantly, Microsoft announced support for the standard so we'll see it incorporated in products moving forwards. You can read through the spec at the above link and also find up to date links to various tools, validators and import/exporters in various different environments and languages. 
    
    My starting point for this was the Unity GLTF loader here [link](https://github.com/KhronosGroup/UnityGLTF "Unity GLTF Loader"). The project here contains code for the GLTF components and some example scenes, note also that the code is dependent on JSON.NET. I briefly thought about isolating this dependency but decided it wasn't worth the effort for this sample. So my first step was to export to a custom package just the items I needed to be able to import a 3D scene.

    ![export GLTF code]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\export GLTF Code.PNG")

    ### GLTFLoader.cs ###

    GLTFLoader.cs is essentially the top-level object which you can configure and call it's Load function to initiate the import.

    ```cs
    var loader = new GLTFLoader(Url, gameObject.transform);

    // Set up a mapping to shaders
	loader.SetShaderForMaterialType(GLTFLoader.MaterialType.PbrMetallicRoughness, GLTFStandard);
    
    // etc.

    loader.Multithreaded = Multithreaded;
	loader.MaximumLod = MaximumLod;
	
    // Load the file..
    loader.Load();
    ```

    The GLTFLoader class uses UnityWebRequest to make a request to a url to retrieve the GLTF format file, downloads it and parses it into memory and then creates a Unity scene representation from it. Although I would have like to have a progress update throughout those operations to report to the user and may have liked to use a different network stack I'll go with this for now.

3. *Cloud Model Catalogue*

In order to make this actually usable we'll need a real catalogue to retrieve the 3D models from. I'm not going to create a Web API at this point but we can simulate one using Azure Storage:

I first added the Github repo (created by my colleague at Microsoft Dave Douglas) from here https://github.com/Unity3dAzure/StorageServices as a git submodule by navigating a git command prompt to my Assets folder and typing:

*git submodule add https://github.com/Unity3dAzure/StorageServices.git*

I then followed the instructions to create an Azure Storage Account and associated Blob Container from [here](www.deadlyfingers.net/azure/unity3d-and-azure-blob-storage/). My intention is to create a read-only access so I will upload 3D gltf files using the Azure portal interface:

    ![Azure Portal Blob Container]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\azure blob container.PNG")

N.B. I will use binary GLTF files (.glb) with embedded resources for convenience.

So, I got hold of some sample models from here [Sample GLTF Models](https://github.com/KhronosGroup/glTF-Sample-Models) and uploaded them to my blob container.

    ![Azure Portal Blob Container]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\azure blob upload.PNG")

When I returned back to my code it slowly dawned on me that the Azure Storage access code uses co-routines, which in turn 'requires' a **MonoBehaviour**-derived object which kinda breaks the repository pattern architecture I have been striving to create. It seems to me that a MonoBehaviour is really a construct of visual elements and here I am tying it to my data. At this point I could have implemented my own **StartCoroutine** function but I came to the conclusion that this code should be asynchronous and therefore shouldn't really be in a co-routine at all. The upshot of this is that I decided to use the experimental .NET 4.6 framework support inside Unity, load up the **Azure Storage SDK** and use it directly. This way I could keep the implementation of **IHologramCollection** completely seperated from the rest of the code.

    ![.NET 4.6 Support in Unity]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\net46 unity support.PNG")

This decision didn't come completely free of challenges though. The first was that Unity doesn't support [Nuget](https://www.nuget.org/),that is the package manager that works with Visual Studio and Unity doesn't have it's own package management system either. Ordinarily I would use Nuget to manage downloading the **Azure Storage SDK** and manage the versioning, update and dependencies for me. I had to do this manually so used the Nuget.org website to work out what the dependencies and versions were and I used a blank Visual Studio project to pull those down to my computer.

 ![Azure Storage SDK Nuget]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\Azure Storage SDK dependencies.PNG")

 I copied out the DLLs into the Plugins folder of my Unity project and then was able to write code to access the SDK from my scripts. 

 <example code here>

 I was expecting that this would all just work as I expected now but any attempt to make an asynchronous call using the Unity Editor resulted in a faulted Task being returned. 

 ![TrustFailure Exception]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\faulted exception.png")        

As you can see the 'authentication or decryption has failed'. After some research [this](https://stackoverflow.com/questions/3674692/mono-webclient-invalid-ssl-certificates) Stackoverflow answer alerted me to the following 'The main reason is that Mono, unlike Microsoft's .NET implementation, does not include trusted root certificates, so all certificate validation will fail by default.' It is possbile to hook into the certificate validation process and since this is just a sample, I can allow all requests through quite simply by exectuing the following code:

```cs
ServicePointManager.ServerCertificateValidationCallback = 
    (s, c, ch, pe) => true;
```
Although this would only cause a problem in the Unity Editor, since on the HoloLens we would not be using Mono, it is nevertheless nice to have all of this work in the Editor so we can more easily define the user experience there.

So let's review the final code for the Cloud Catalogue starting with the repository interface:

```cs
public interface IHologramCollection
{
    Task<IEnumerable<IHologram>> GetHologramsAsync();
}
```
Which we implement using the Azure Storage SDK like this:

```cs
public class AzureHologramCollection : IHologramCollection
{
    private CloudBlobClient _serviceClient;
    private CloudBlobContainer _container;

    public AzureHologramCollection()
    {
        var accountName = Environment.GetEnvironmentVariable("HOLOGRAMCOLLECTION_ACCOUNTNAME");
        var accountKey = Environment.GetEnvironmentVariable("HOLOGRAMCOLLECTION_ACCOUNTKEY");

        ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, pe) => true;

        string connectionString = 
            string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net",
            accountName, accountKey);
        var account = CloudStorageAccount.Parse(connectionString);
        _serviceClient = account.CreateCloudBlobClient();
        _container = _serviceClient.GetContainerReference("models");
    }

    public async Task<IEnumerable<IHologram>> GetHologramsAsync()
    {
        var results = await _container.ListBlobsSegmentedAsync(null);
        var list = results.Results;

        return list.Select(item =>
            {
                var blob = (CloudBlockBlob)item;
                return new Hologram
                {
                    Name = blob.Name,
                    Uri = blob.Uri,
                };
            });
    }
}
```
First note that the account name and account key which I removed from the connection string which is accessbile via the [Azure Portal](http://portal.azure.com) is supplied via environment variables. It isn't good practice to add the keys to source control especially on Github in a public repo.

4. *Interaction Design & User Experience*
Even in a simple app like this there are quite a few considerations for how to make this a good user experience. In the first instance we want to represent a collection of models from which we can select models and pull them out into our environment. We should free ourselves to think in terms of 3D and not be constrained to a flat 2D UI and interaction paradigms of that nature. In order to help out with this I'm going to leverage the guidance and implementation from the [Mixed Reality Design Lab](https://github.com/Microsoft/MRDesignLabs_Unity).
Initially we'll look at the interactible objects and the collections from the Design Labs:
 ![Interactible Objects]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\InteractibleObject_Hero.jpg")
![Collections]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\ObjectCollection_Hero.jpg")
The Desgin Labs tools themselves have been seperated out from the main repo to allow them to be added easily to your own projects see [MRDesign Labs Tools](https://github.com/Microsoft/MRDesignLabs_Unity_tools). I used the Git submodule approach to add this repo to my project.
![HUX imported into project]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\HUX in project.PNG")
The HUX folder now sits alongside the Mixed Reality Toolkit in my project. So, now I have some tools, a blank 3D canvas and a whole heap of questions about the best way to represent this. Here are some of the questions I had in mind:
* How to represent 'loading'?
* Where to position the collection?
* How to represent the models as 'thumbnails'?
* How would this translate from a single-user experience to multi-user?
And many more. To avoid getting too caught up in the details I'll take the approach of prototyping some simple options and iterate on those. So, initially I'm setting up this:
* App starts and calls the online catalogue
* Use the items retrieved from the catalogue to initialise a Design Labs ObjectCollection
* Use a simple 3D primitive with some text to represent the items
* Implement interactions to load the 3D models into the users environment and allow manipulation (position, scale, etc.)

To kick this off I dragged a HoloLens prefab from the HUX into my scene
 
![HUX HoloLens prefab]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\HUXHoloLensPrefab.PNG")

This prefab comes with quite a few components:
* a camera
* managers for handling audio, focus, interaction, manipulation and input
* a visualisation of the HoloLens device itself along with frustum, clipping planes, etc.

Next, I added an empty GameObject which I named 'Collection' and I added a HUX ObjectCollection component to it:

![HUX Object Collection]("C:\Users\pdaukin\Pictures\Blogs\Dynamic Shared Holograms\HUXObjectCollection.png")

This component allows me to layout a collection of elements using some pre-defined layouts; cylinder, plane, surface and scatter. I chose cylinder for now and I have controls to be able to define cell sizes, columns, rows, etc. So let's see how to load up the data and insert a 3d item representation into the ObjectCollection...

 I created a script to do that - let's look at the Start() method here:

 ```cs
    async void Start()
    {
        var res = await _hologramCatalog.GetHologramsAsync();
        var collection = gameObject.GetComponent<ObjectCollection>();
        foreach (var r in res)
        {
            var obj = new ObjectCollection.CollectionNode()
            {
                Name = r.Name,
            };

            var cube = Instantiate(ListItemPrefab);

            // Set up properties on the instantiated prefab..
            var text = cube.GetComponentInChildren<TextMesh>();
            text.text = r.Name;
     
            cube.transform.parent = collection.transform;
            obj.transform = cube.transform;

            collection.NodeList.Add(obj);
            _log.Log(r.Name);
        }
        _log.Log("DONE");
        collection.UpdateCollection();
    }
 ```
The dependencies, _hologramCatalog and _log are injected using the dependency injection container, Zenject (See previous post). Note that I'm using async/await made available to me by the Unity .NET 4.6 support. I load the data with a call to GetHologramsAsync and for each item returned I instantiate a prefab which is a public field and so can be set in the Unity editor. I create a CollectionNode from the data passed back in each catalog item which I can use to add to the ObjectCollection. Finally, I make a call to UpdateCollection to apply the changes. If I run this now I get something like the following:

<insert screenshot here>         
 
Without any kind of feedback or response from the elements when you gaze at them or when you activate them this all feels broken so let's see how the HUX can help with that. 

5. *Sharing*



