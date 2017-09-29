using HUX.Collections;
using UnityEngine;
using Zenject;

public class ObjectData : MonoBehaviour
{
    [Inject]
    public void Construct(IHologramCollection hologramCatalog, ILogger log)
    {
        HologramCatalog = hologramCatalog;
        this.log = log;
    }

    IHologramCollection HologramCatalog;
    private ILogger log;

    public GameObject ListItemPrefab;

    // Use this for initialization
    async void Start()
    {
        var res = await HologramCatalog.GetHologramsAsync().ConfigureAwait(false);
        var collection = gameObject.GetComponent<ObjectCollection>();
        foreach (var r in res)
        {
            var obj = new ObjectCollection.CollectionNode()
            {
                Name = r.Name,
            };

            var cube = Instantiate(ListItemPrefab);
            cube.transform.parent = collection.transform;
            obj.transform = cube.transform;

            collection.NodeList.Add(obj);
            log.Log(r.Name);
        }
        log.Log("DONE");
        collection.UpdateCollection();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
