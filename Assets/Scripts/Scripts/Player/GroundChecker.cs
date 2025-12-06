using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool GroundChecked()
    {
        return Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Ground"));
    }
}
