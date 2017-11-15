using System;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.Collections;
using UnityEngine;
using Zenject;
using GLTF;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

#if !UNITY_EDITOR
using System.Net.Http;
#endif

public class ObjectData : MonoBehaviour
{
    private static string ModelString =
        @"Z2xURgIAAAD0qKsAjAcAAEpTT057ImFjY2Vzc29ycyI6W3siYnVmZmVyVmlldyI6NCwiY29tcG9uZW50VHlwZSI6NTEyNiwiY291bnQiOjM1NzUsInR5cGUiOiJWRUMyIiwibWF4IjpbMC45OTk5MDAzLC0wLjAyMjEzNzc2NDhdLCJtaW4iOlswLjAwMDY1ODU5OTMsLTAuOTk2NzczOTU4XX0seyJidWZmZXJWaWV3Ijo1LCJjb21wb25lbnRUeXBlIjo1MTI2LCJjb3VudCI6MzU3NSwidHlwZSI6IlZFQzMiLCJtYXgiOlsxLjAsMS4wLDAuOTk5OTc4Ml0sIm1pbiI6Wy0xLjAsLTEuMCwtMC45OTgwODIzXX0seyJidWZmZXJWaWV3Ijo2LCJjb21wb25lbnRUeXBlIjo1MTI2LCJjb3VudCI6MzU3NSwidHlwZSI6IlZFQzQiLCJtYXgiOlsxLjAsMC45OTk5OTc2LDEuMCwxLjBdLCJtaW4iOlstMC45OTkxMjg5LC0wLjk5OTkwNzg1MSwtMS4wLDEuMF19LHsiYnVmZmVyVmlldyI6NywiY29tcG9uZW50VHlwZSI6NTEyNiwiY291bnQiOjM1NzUsInR5cGUiOiJWRUMzIiwibWF4IjpbMC4wMDk5MjExNTQsMC4wMDk3NzE2MywwLjAxMDA3NjI0NTNdLCJtaW4iOlstMC4wMDk5MjExNTQsLTAuMDA5NzcxNjMsLTAuMDEwMDc2MjQ1M119LHsiYnVmZmVyVmlldyI6OCwiY29tcG9uZW50VHlwZSI6NTEyMywiY291bnQiOjE4MTA4LCJ0eXBlIjoiU0NBTEFSIiwibWF4IjpbMzU3NF0sIm1pbiI6WzBdfV0sImFzc2V0Ijp7ImdlbmVyYXRvciI6ImdsVEYgVG9vbHMgZm9yIFVuaXR5IiwidmVyc2lvbiI6IjIuMCJ9LCJidWZmZXJWaWV3cyI6W3siYnVmZmVyIjowLCJieXRlTGVuZ3RoIjozMjg1ODQ0fSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MzI4NTg0NCwiYnl0ZUxlbmd0aCI6NDc3NTUyOX0seyJidWZmZXIiOjAsImJ5dGVPZmZzZXQiOjgwNjEzNzMsImJ5dGVMZW5ndGgiOjI4NDU5MjN9LHsiYnVmZmVyIjowLCJieXRlT2Zmc2V0IjoxMDkwNzI5NiwiYnl0ZUxlbmd0aCI6MTMyODMzfSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MTEwNDAxMzIsImJ5dGVMZW5ndGgiOjI4NjAwfSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MTEwNjg3MzIsImJ5dGVMZW5ndGgiOjQyOTAwfSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MTExMTE2MzIsImJ5dGVMZW5ndGgiOjU3MjAwfSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MTExNjg4MzIsImJ5dGVMZW5ndGgiOjQyOTAwfSx7ImJ1ZmZlciI6MCwiYnl0ZU9mZnNldCI6MTEyMTE3MzIsImJ5dGVMZW5ndGgiOjM2MjE2fV0sImJ1ZmZlcnMiOlt7ImJ5dGVMZW5ndGgiOjExMjQ3OTQ4fV0sImltYWdlcyI6W3sibWltZVR5cGUiOiJpbWFnZS9wbmciLCJidWZmZXJWaWV3IjowfSx7Im1pbWVUeXBlIjoiaW1hZ2UvcG5nIiwiYnVmZmVyVmlldyI6MX0seyJtaW1lVHlwZSI6ImltYWdlL3BuZyIsImJ1ZmZlclZpZXciOjJ9LHsibWltZVR5cGUiOiJpbWFnZS9wbmciLCJidWZmZXJWaWV3IjozfV0sIm1lc2hlcyI6W3sicHJpbWl0aXZlcyI6W3siYXR0cmlidXRlcyI6eyJURVhDT09SRF8wIjowLCJOT1JNQUwiOjEsIlRBTkdFTlQiOjIsIlBPU0lUSU9OIjozfSwiaW5kaWNlcyI6NCwibWF0ZXJpYWwiOjB9XSwibmFtZSI6IkJvb21Cb3gifV0sIm1hdGVyaWFscyI6W3sicGJyTWV0YWxsaWNSb3VnaG5lc3MiOnsiYmFzZUNvbG9yVGV4dHVyZSI6eyJpbmRleCI6MH0sIm1ldGFsbGljUm91Z2huZXNzVGV4dHVyZSI6eyJpbmRleCI6MX19LCJub3JtYWxUZXh0dXJlIjp7ImluZGV4IjoyfSwib2NjbHVzaW9uVGV4dHVyZSI6eyJpbmRleCI6MX0sImVtaXNzaXZlRmFjdG9yIjpbMS4wLDEuMCwxLjBdLCJlbWlzc2l2ZVRleHR1cmUiOnsiaW5kZXgiOjN9LCJuYW1lIjoiQm9vbUJveF9NYXQifV0sIm5vZGVzIjpbeyJtZXNoIjowLCJuYW1lIjoiQm9vbUJveCJ9XSwic2NlbmUiOjAsInNjZW5lcyI6W3sibm9kZXMiOlswXX1dLCJ0ZXh0dXJlcyI6W3sic291cmNlIjowfSx7InNvdXJjZSI6MX0seyJzb3VyY2UiOjJ9LHsic291cmNlIjozfV19ICBMoasAQklOAIlQTkcNChoKAAAADUlIRFIAAAgAAAAIAAgCAAAAPcVEZwAAIABJREFUeAGUnWmWpMlxXWsCeoQANEFC5Mq4AWkJkrZA7YBLoBZG/SOaPEADPVXp3vfMLTyjCjxHnpUe5mbPng2ff0NEZlW9/vrrr1+d8fr1qw8fzuLTrx9ev3qD5cOr968G/foBRDzur1+//hAuFKgFQa6nCq2sCi4iLC/1HwxR/avgu3g51/uvZn6l9NLv06vXBCIUo7nV/coQi5gxC0zOFth6zdjaefmQ9FVgEnY1t8SYnoaFoGr0y9ZOXATm8RJGhAZKj/FtKomeNOW9GBpD0P//+Ku+04RTeXAfvvvuz7/97W/+6Z/+9zfffPPn776rkqPLTmI/fXiv4vWbN827DG9ipM+vX79ls70J4fsgky36N7jNNvKQSPPmjeUgqZBvDuYr9CwYr8C8BfsKNA2TtvosAii/KeXLJreTmUnpw/v3cfSAluK9AnymFObxAZzMzU2X95DpQlDzDHFj4luFuaF//Zpimd++efseTUBxTL4BAGNgefPmzSGpxnwN8OrVF1988e0f/vA//tf//MO//dsXX3xFVywqkZOBk+nMgOY1CZOkxHYtoy/RshZvPYr9Sg5k9+bnn39SmYF/mSXNGTGB4BZiaKVZZoWW3rITRBRQc2VzSsikcvKejr16bVJv1jPM9P9NzzsI4/DiLJygebGFybMHCF01GqvPq9ocLO0E8zBhMEppWMS34Zp+uCAJIlbRs1y9e0Mtu4ggREmE4/XIzbxsjxdhjlFoClY7A7j+2ZlCRnhchYrE8iKT9+9/prqkZDsMZVdfu4OTcXJLbfEjTRmSg68KZ4FBTZaTcpeNmDKSJEAO6/CtB43NTqCIotyZ49QQKTUaoyYQqydBUKJH8BTMlgCvk7aUUEaLVK1e4VFZ+e2GX/a9h0nUxBaeQhLObKHxlA+flOTvuZy6yu+5Nt0Okx1r7J5bopqEa0ORbxnbFDQgkpURE821soaQD2X0ckjkSA4ne330cOPViDnB0Hh1am5BHfuklriJZW6pl9dmIL5pTdiJAuD0MD5pnSewgd6netNpfZJaOhueghROXZsh4DDrXym0wsGj5YkFfS+VEEzqKtOQVppwxiVpwJw16aqcSSbxxYecl+lwKjRHTJM0AiPR6yyG5dhNwWsd0a/QdWqAhpBwnJDIprEhI/TYBLBs8yc7rF6/VRpCYnPxOnDlj1ILozgwnOxoQ2awWA5MSyqam1RRYgZhCuE3tGkdfwK7iqZNwFdkgKJGGCbqs1Ze1B966VzKoNH+mQ9ViTE57T4oTCJqjvtgJBAMGiFhe0pikEHfyV2AmPCGUzd9CzoFqYlKa+x9zRNLDoRPIqcUkB3mKLo5Vpy83X6akql4jwuLkHER8ZQ0tRYR+xtvRonbDFC20jJ1pYuNTchU1CgQ6hoNQk5DdwEHOtFtQdvlLVa";

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
            if (data != null)
            {
                data.Name = r.Name;
                data.Uri = r.Uri.ToString();
            }

            // Set up properties on the instantiated prefab..
            var text = cube.GetComponentInChildren<TextMesh>();
            if (text != null)
            {
                text.text = r.Name;
            }

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

        var bytes = await DownloadFileAsync(uri);

        // Download the file from the Uri and load the model into the scene
        var loader = new GLTFLoader(bytes, gameObject.transform.parent);
        StartCoroutine(loader.Load());
    }

    private async Task<byte[]> DownloadFileAsync(string uri)
    {
#if !UNITY_EDITOR
        var httpClient = new HttpClient();
        var resp = await httpClient.GetAsync(uri);
        var ba = await resp.Content.ReadAsByteArrayAsync();

        var str = Convert.ToBase64String(ba);

        Debug.Log("Binary string - " + str);
#else
        // Return a fixed model in byte array format..
        var ba = Convert.FromBase64String(ModelString);
#endif
        return ba;
    }
}
