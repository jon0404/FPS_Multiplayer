using UnityEngine;

public class KPlayerCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
