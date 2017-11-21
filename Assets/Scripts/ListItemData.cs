using HoloToolkit.Examples.InteractiveElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItemData : MonoBehaviour
{
    public string Name;
    public string Uri;

    public void SetLoading(bool isLoading = true)
    {
        var children = gameObject.GetComponentsInChildren<Transform>(true);
        GameObject animation = null;
        foreach (var child in children)
        {
            if (child.gameObject.name == "LoaderAnimation")
            {
                animation = child.gameObject;
                break;
            }
        }
        if (animation == null)
        {
            Debug.Log("Error: Couldn't find animation component");
            return;
        }

        animation.SetActive(isLoading);
    }
}
