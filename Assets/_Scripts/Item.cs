using PurrNet;
using UnityEngine;

public class Item : NetworkBehaviour, IInteractable
{
    [SerializeField] private string itemName = "null";
    [SerializeField] GameObject item;
    [SerializeField] GameObject itemPrefab;
    
    public void Interact()
    {
        Debug.Log("pickup item");
        Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
