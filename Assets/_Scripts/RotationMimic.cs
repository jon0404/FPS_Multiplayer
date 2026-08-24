using PurrNet;
using UnityEngine;

public class RotationMimic : NetworkBehaviour
{
    [SerializeField] private GameObject cameraMimic;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }
    void Update()
    {
        if (!cameraMimic)
            return;
        transform.rotation = cameraMimic.transform.rotation;
    }
}
