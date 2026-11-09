using PurrNet;
using UnityEngine;

public class ItemPickup : NetworkBehaviour, IInteractable
{
    // The pickup keeps its data so the inventory can convert it into an item slot.
    public ItemData itemData;

    private void Awake()
    {
        if (!TryGetComponent<RoundResettable>(out _))
            gameObject.AddComponent<RoundResettable>();
    }

    public void Interact(GameObject interactor)
    {
        if (interactor == null || itemData == null)
            return;

        var invManager = interactor.GetComponent<InventoryManager>();
        if (invManager == null)
            return;

        invManager.PickupItem(itemData);
        gameObject.SetActive(false);
    }
}
