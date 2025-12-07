using UnityEngine;

public class TimeControlled : MonoBehaviour
{
    private Vector2 _velocity;

    public SpriteRenderer MySpriteRenderer { get; private set; }

    protected virtual void Awake()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
        if (MySpriteRenderer == null) MySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public virtual void TimeUpdate()
    {

    }
}