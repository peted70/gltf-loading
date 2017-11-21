using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class TapToPlaceWithEvent : TapToPlace
{
    public event EventHandler Placed;
    public class PlacedEventArgs : EventArgs {}

    protected override void StopPlacing()
    {
        base.StopPlacing();
        OnPlaced();
    }

    void OnPlaced()
    {
        var placed = Placed;
        if (placed != null)
            placed(this, new PlacedEventArgs());
    }
}
