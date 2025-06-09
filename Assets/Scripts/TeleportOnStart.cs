using UnityEngine;

public class TeleportOnStart : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;

    void Start()
    {
        transform.position = targetPosition;
    }
}