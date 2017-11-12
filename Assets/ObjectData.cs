﻿using System;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.Collections;
using UnityEngine;
using Zenject;
using GLTF;
using System.Threading.Tasks;
using System.Collections.Generic;
//using System.Net.Http;

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
            var obj = new CollectionNode()
            {
                Name = r.Name,
            };

            var cube = Instantiate(ListItemPrefab);

            var button = cube.GetComponentInChildren<Button>();
            button.OnButtonPressed += OnButtonPressed;

            // Set up properties on the instantiated prefab..
            var data = cube.GetComponentInChildren<ListItemData>();
            data.Name = r.Name;
            data.Uri = r.Uri.ToString();

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

    private async void OnButtonPressed(GameObject obj)
    {
        var data = obj.GetComponent<ListItemData>();
        if (data == null)
            return;
        var uri = data.Uri;

        var bytes = new byte[10];// await DownloadFileAsync(uri);

        // Download the file from the Uri and load the model into the scene
        var loader = new GLTFLoader(bytes, gameObject.transform.parent);
        StartCoroutine(loader.Load());
    }

    //private async Task<byte[]> DownloadFileAsync(string uri)
    //{
    //    var httpClient = new HttpClient();
    //    var resp = await httpClient.GetAsync(uri);
    //    var ba = await resp.Content.ReadAsByteArrayAsync();
    //    return ba;
    //}
}
