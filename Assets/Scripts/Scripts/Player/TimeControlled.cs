using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControlled : MonoBehaviour
{
    private Vector2 _velocity;

    public Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public virtual void TimeUpdate()
    {

    }
}
