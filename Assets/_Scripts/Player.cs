using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager { get; private set; }
    public CinemachineCamera cam;
    public GameObject rotationMimic;

    public PlayerController playerController { get; private set; }

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        playerController = GetComponent<PlayerController>();

        if (cam == null)
            cam = GetComponentInChildren<CinemachineCamera>();

        if (rotationMimic == null)
            rotationMimic = gameObject;
    }
}
