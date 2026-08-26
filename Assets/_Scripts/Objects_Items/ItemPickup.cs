using System;
using PurrNet;
using UnityEngine;

public class ItemPickup : NetworkBehaviour, IInteractable
{
    [SerializeField] GameObject itemPrefab;
    
    
    public void Interact(GameObject interactor)
    {
        var invManager = interactor.GetComponent<InventoryManager>();
        invManager.PickupItem(itemPrefab);
        //Destroy(this.gameObject);
    }
}
