using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryManager inventoryManager { get; private set; }
    public CinemachineCamera cam;
    public GameObject rotationMimic;

    public PlayerController playerController { get; private set; }
    void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        //cam = GetComponentInChildren<CinemachineCamera>();
        playerController = GetComponent<PlayerController>();

    }
}
