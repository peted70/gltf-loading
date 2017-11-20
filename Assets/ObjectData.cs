using System;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.Collections;
using UnityEngine;
using Zenject;
using GLTF;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using UnityGLTF;
using HoloToolkit.Unity.InputModule;

#if !UNITY_EDITOR
using System.Net.Http;
#endif

public class ObjectData : MonoBehaviour
{
    private static string ModelString =
        "Z2xURgIAAACk1gEAQAgAAEpTT057ImFzc2V0Ijp7ImdlbmVyYXRvciI6IkNPTExBREEyR0xURiIsInZlcnNpb24iOiIyLjAifSwic2NlbmUiOjAsInNjZW5lcyI6W3sibm9kZXMiOlswXX1dLCJub2RlcyI6W3siY2hpbGRyZW4iOlsyLDFdLCJtYXRyaXgiOlswLjAwOTk5OTk5OTc3NjQ4MjU4MiwwLjAsMC4wLDAuMCwwLjAsMC4wMDk5OTk5OTk3NzY0ODI1ODIsMC4wLDAuMCwwLjAsMC4wLDAuMDA5OTk5OTk5Nzc2NDgyNTgyLDAuMCwwLjAsMC4wLDAuMCwxLjBdfSx7Im1hdHJpeCI6Wy0wLjcyODk2ODY3OTkwNDkzNzcsMC4wLC0wLjY4NDU0NzA2NjY4ODUzNzYsMC4wLC0wLjQyNTIwNDkwMjg4NzM0NDM4LDAuNzgzNjkzNDMyODA3OTIyNCwwLjQ1Mjc5NzI5MzY2MzAyNDksMC4wLDAuNTM2NDc1MDYyMzcwMzAwMywwLjYyMTE0NzgxMTQxMjgxMTMsLTAuNTcxMjg3OTg5NjE2Mzk0LDAuMCw0MDAuMTEzMDA2NTkxNzk2OSw0NjMuMjY0MDA3NTY4MzU5NCwtNDMxLjA3ODAzMzQ0NzI2NTYsMS4wXSwiY2FtZXJhIjowfSx7Im1lc2giOjB9XSwiY2FtZXJhcyI6W3sicGVyc3BlY3RpdmUiOnsiYXNwZWN0UmF0aW8iOjEuNSwieWZvdiI6MC42NjA1OTI1NTU5OTk3NTU5LCJ6ZmFyIjoxMDAwMC4wLCJ6bmVhciI6MS4wfSwidHlwZSI6InBlcnNwZWN0aXZlIn1dLCJtZXNoZXMiOlt7InByaW1pdGl2ZXMiOlt7ImF0dHJpYnV0ZXMiOnsiTk9STUFMIjoxLCJQT1NJVElPTiI6MiwiVEVYQ09PUkRfMCI6M30sImluZGljZXMiOjAsIm1vZGUiOjQsIm1hdGVyaWFsIjowfV0sIm5hbWUiOiJMT0Qzc3BTaGFwZSJ9XSwiYWNjZXNzb3JzIjpbeyJidWZmZXJWaWV3IjowLCJieXRlT2Zmc2V0IjowLCJjb21wb25lbnRUeXBlIjo1MTIzLCJjb3VudCI6MTI2MzYsIm1heCI6WzIzOThdLCJtaW4iOlswXSwidHlwZSI6IlNDQUxBUiJ9LHsiYnVmZmVyVmlldyI6MSwiYnl0ZU9mZnNldCI6MCwiY29tcG9uZW50VHlwZSI6NTEyNiwiY291bnQiOjIzOTksIm1heCI6WzAuOTk5NTk4OTc5OTQ5OTUxMiwwLjk5OTU4MDk3OTM0NzIyOSwwLjk5ODQzNTk3NDEyMTA5MzhdLCJtaW4iOlstMC45OTkwODM5OTU4MTkwOTE4LC0xLjAsLTAuOTk5ODMxOTc0NTA2Mzc4Ml0sInR5cGUiOiJWRUMzIn0seyJidWZmZXJWaWV3IjoxLCJieXRlT2Zmc2V0IjoyODc4OCwiY29tcG9uZW50VHlwZSI6NTEyNiwiY291bnQiOjIzOTksIm1heCI6Wzk2LjE3OTkwMTEyMzA0Njg4LDE2My45NzAwMDEyMjA3MDMxMyw1My45MjUxOTc2MDEzMTgzNl0sIm1pbiI6Wy02OS4yOTg1MDAwNjEwMzUxNiw5LjkyOTM2OTkyNjQ1MjYzNywtNjEuMzI4MTk3NDc5MjQ4MDVdLCJ0eXBlIjoiVkVDMyJ9LHsiYnVmZmVyVmlldyI6MiwiYnl0ZU9mZnNldCI6MCwiY29tcG9uZW50VHlwZSI6NTEyNiwiY291bnQiOjIzOTksIm1heCI6WzAuOTgzMzQ1OTg1NDEyNTk3NywwLjk4MDAzNjk3Mzk1MzI0NzFdLCJtaW4iOlswLjAyNjQwOTAwMDE1ODMwOTkzOCwwLjAxOTk2MzAyNjA0Njc1MjkzXSwidHlwZSI6IlZFQzIifV0sIm1hdGVyaWFscyI6W3sicGJyTWV0YWxsaWNSb3VnaG5lc3MiOnsiYmFzZUNvbG9yVGV4dHVyZSI6eyJpbmRleCI6MH0sIm1ldGFsbGljRmFjdG9yIjowLjB9LCJlbWlzc2l2ZUZhY3RvciI6WzAuMCwwLjAsMC4wXSwibmFtZSI6ImJsaW5uMy1meCJ9XSwidGV4dHVyZXMiOlt7InNhbXBsZXIiOjAsInNvdXJjZSI6MH1dLCJpbWFnZXMiOlt7ImJ1ZmZlclZpZXciOjMsIm1pbWVUeXBlIjoiaW1hZ2UvcG5nIn1dLCJzYW1wbGVycyI6W3sibWFnRmlsdGVyIjo5NzI5LCJtaW5GaWx0ZXIiOjk5ODYsIndyYXBTIjoxMDQ5Nywid3JhcFQiOjEwNDk3fV0sImJ1ZmZlclZpZXdzIjpbeyJidWZmZXIiOjAsImJ5dGVPZmZzZXQiOjc2NzY4LCJieXRlTGVuZ3RoIjoyNTI3MiwidGFyZ2V0IjozNDk2M30seyJidWZmZXIiOjAsImJ5dGVPZmZzZXQiOjAsImJ5dGVMZW5ndGgiOjU3NTc2LCJieXRlU3RyaWRlIjoxMiwidGFyZ2V0IjozNDk2Mn0seyJidWZmZXIiOjAsImJ5dGVPZmZzZXQiOjU3NTc2LCJieXRlTGVuZ3RoIjoxOTE5MiwiYnl0ZVN0cmlkZSI6OCwidGFyZ2V0IjozNDk2Mn0seyJidWZmZXIiOjAsImJ5dGVPZmZzZXQiOjEwMjA0MCwiYnl0ZUxlbmd0aCI6MTYzMDJ9XSwiYnVmZmVycyI6W3siYnl0ZUxlbmd0aCI6MTE4MzQyfV19ICBIzgEAQklOADm4RL7qP2+/j1KZPslUgb0UXn6/K0y/PXuD770r22u/mNu9PiXKHr2TNX6/y2jkPeSerr0Acn6/t3qOPRo0hL7lK3G/BTJbPihInL6EuXG/ad/8PfX30r12bn6/cCAkPYP44L3+Yn6/bti2PCLGo77uCXK/Cd97PXe6cz5UN0+/DWwJP8WKyj4RUVS/EALKPibfpD7kMSe/uHUvPzQPDD9dcCq/WOIBP/7UaD7ncHG/+FJ4PqXdGD6WB22/6q2xPqD/nr47F1K/NZX1Pv95Or6MSkq/gc0VP1ck1r40ZVe/pzuvPmuC0L51BCy/ZFseP/p9C79GPza/y9biPvT+b77eOx6/vhZAP8TQ8r4pe1u/EOhMPnzt8b4AG2C/ebHQPZfjGb8pJUC/on2MPk/qF7/2mkq/xCIWPoaR/j4na1i/lulHPtyDkD5+GXS/US/YPYogLj9Juy2/l+ONPv0Vyj6lEgK/XfpDPxrcJj/p7wG/7kEQP+hq8z4Q58G+G0hLPwCNOj94erW+BAEWP/608b6VCQe/hdE0P01nj779+um+7x5YP1ZEIb8G8Ba/x2gBPwGJBr+BPc6+Ctc/P2YyML+5iPe+6nYKPxH/sL5B06q+14RgP5EoTD/QtAC/RMOqPmptXj9FEaq+OPO7PgN8Rz+gUlW+8FEXPw6GDj9T622+YitMP/WFHD/kE7K9DFlJP2FUTj/zyJ+9mzcWPw2pZj/oT0u+dXfFPlq3aT8v+5W9JonNPuJZFj+mRKI+oKY+PxFVID8b9OU9E35FPwJ/RD/isZc+8YERP76+Tj8FNNE9jrAUP+7sZz8L68Y9XfvSPvN0Xj8O2os+VkbTPpnzvD4KDyo/c2cmP20e/z7MmgA/4uY0P9WXFT8aFBk/CHIMP4lfMT9dFus+1lQOPyklUD+9itQ+vvrQPgJJQD/43gM/UWbTPgD7SD6oVUQ//WgcP+I+ij6PMTc/JeskPwMEwz7RHjs/avYQPwBw9D6IDyw/VOQQP+BlKj+NJB0/QFHZPrZiDz8bgzY/lgTYPpOmEb4sgWQ/7QzbPi9p7L2+S00/3A0WP/sjPL6tvmY/6s3IPtRGFb7UuVI/1H0MP3myS778wXA/zR2NPj6xLr5iZWQ/0SPWPpTaq72p3Eg/MUEdP82U1r23QFY/nYUJPz/7kb0wR0M/I4ckP1iqG75ZwnY/2NZfPpc7M72E134/+bqsPa97q735SXU=";

    [Inject]
    public void Construct(IHologramCollection hologramCatalog, ILogger log)
    {
        _hologramCatalog = hologramCatalog;
        _log = log;
    }

    private IHologramCollection _hologramCatalog;
    private ILogger _log;

    public GameObject ListItemPrefab;
    public GameObject SceneRoot;
    public GameObject SceneParentPrefab;

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

    private void OnButtonPressed(GameObject obj)
    {
        var data = obj.GetComponent<ListItemData>();
        if (data == null)
            return;
        var uri = data.Uri;

        var go = GameObject.Instantiate(SceneParentPrefab);
        go.transform.parent = SceneRoot.transform;
        //go.transform.Translate(obj.transform.position);

        go.AddComponent<BoxCollider>();
        go.AddComponent<TapToPlace>().IsBeingPlaced = true;

        var gltfComponent = go.GetComponent<UnityGLTF.GLTFComponent>();
        gltfComponent.Url = uri;
        StartCoroutine(gltfComponent.Load());
    }

    private async Task<byte[]> DownloadFileAsync(string uri)
    {
#if !UNITY_EDITOR
        var httpClient = new HttpClient();
        var resp = await httpClient.GetAsync(uri);
        var ba = await resp.Content.ReadAsByteArrayAsync();

        var str = Convert.ToBase64String(ba);

        _log.Log("Binary string - " + str);
#else
        // Return a fixed model in byte array format..
        _log.Log("str length = " + ModelString.Length);
        var ba = Convert.FromBase64String(ModelString);
#endif
        return ba;
    }
}
