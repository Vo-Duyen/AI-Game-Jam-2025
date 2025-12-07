using UnityEngine;
using Cinemachine;

public class CameraZoneSwitcher : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Kéo Polygon Collider của vùng này vào đây")]
    public PolygonCollider2D zoneConfiner;

    [Tooltip("Tag của nhân vật")]
    public string playerTag = "Player";

    private static CinemachineConfiner2D currentConfiner;

    private void Start()
    {
        if (zoneConfiner == null)
            zoneConfiner = GetComponent<PolygonCollider2D>();

        if (currentConfiner == null)
        {
            currentConfiner = FindObjectOfType<CinemachineConfiner2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player entered camera zone: " + gameObject.name);
            SwitchCameraBounds();
        }
    }

    private void SwitchCameraBounds()
    {
        if (currentConfiner != null && zoneConfiner != null)
        {
            currentConfiner.m_BoundingShape2D = zoneConfiner;

            currentConfiner.InvalidateCache();
        }
    }
}