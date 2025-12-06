using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    List<List<RecordFrameData>> _recordedData;

    List<TimeControlled> timeObjects = new List<TimeControlled>();

    private void FixedUpdate()
    {
        bool _pause = Input.GetKeyDown(KeyCode.UpArrow);
        bool _back = Input.GetKeyDown(KeyCode.LeftArrow);
        bool _forward = Input.GetKeyDown(KeyCode.RightArrow);

        if (_pause)
        {

        }
        else if (_back)
        {
        }
        else if (_forward)
        {
            foreach (TimeControlled timeObject in timeObjects)
            {
                timeObject.TimeUpdate();
            }
        }
    }
}

[System.Serializable]
public class RecordFrameData
{
    public Vector2 _position;
    public Vector2 _velocity;
}
