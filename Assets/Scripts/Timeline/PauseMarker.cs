using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PauseMarker : Marker, INotification, INotificationOptionProvider, IComparable<PauseMarker>
{
    public PropertyName id { get; set; }

    public NotificationFlags flags => NotificationFlags.Retroactive;

    public int CompareTo(PauseMarker other)
    {
        return this.time.CompareTo(other.time);
    }
}
