using HUX.Collections;
using UnityEngine;
using Zenject;

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

    public GameObject ListItemPrefab;

    // Use this for initialization
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

    // Update is called once per frame
    void Update()
    {

    }
}
