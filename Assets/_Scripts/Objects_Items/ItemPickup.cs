using System;
using PurrNet;
using UnityEngine;

public class ItemPickup : NetworkBehaviour, IInteractable
{
    public ItemData itemData;
    
    
    public void Interact(GameObject interactor)
    {
        var invManager = interactor.GetComponent<InventoryManager>();
        invManager.PickupItem(itemData);
        Destroy(this.gameObject);
    }
}
