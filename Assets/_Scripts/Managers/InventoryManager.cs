using System;
using PurrNet;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.UI;
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
}

public class InventoryManager : NetworkBehaviour
{
    [SerializeField] float hotbarDelay;
    [SerializeField] float hotbarDelay2;
    [SerializeField] float actualHotbarDelay;
    [SerializeField] float hotbarDelaySet = 0.5f;
    [SerializeField] float hotbarDelaySet2 = 0.15f;
    [SerializeField] bool spedUpHotbar = false;
    [SerializeField] InputActionReference next;
    [SerializeField] InputActionReference previous;
    [SerializeField] InputActionReference drop;
    [SerializeField] float dropForce = 10;
    [SerializeField] Player player;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] private InventorySlot[] invSlots = new InventorySlot[8];
    private GameObject equippedItem = null;
    [SerializeField] int activeItemSlot = 0;
    MainGameView ui;
    public int maxSlots = 8;
    
    void Awake()
    {
        maxSlots = invSlots.Length;
        for(int i = 0; i < maxSlots; i++)
        {
            invSlots[i] = new InventorySlot();
            Debug.Log($"{invSlots}");

        }
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        enabled = isOwner;
    }

    private void Start()
    {
        InstanceHandler.GetInstance<MainGameView>().Hotbar(maxSlots);
        maxSlots--;

    }

    private void Update()
    {
        if (actualHotbarDelay > 0f)
            actualHotbarDelay -= Time.deltaTime;

        if (previous.action.IsPressed() && actualHotbarDelay <= 0f)
            PreviousSlot();

        if (next.action.IsPressed() && actualHotbarDelay <= 0f)  
            NextSlot();

        if(next.action.WasReleasedThisFrame())
        {
            spedUpHotbar = false;
            actualHotbarDelay = 0f;
        }

        if(previous.action.WasReleasedThisFrame())
        {
            spedUpHotbar = false;
            actualHotbarDelay = 0f;
        }
        
        if(drop.action.WasPressedThisFrame())
            Drop();
    }

    private void NextSlot()
    {
        activeItemSlot++;
        if(activeItemSlot > maxSlots)
            activeItemSlot = 0;
        Equip(invSlots[activeItemSlot].itemData);
    }

    private void PreviousSlot()
    {
        activeItemSlot--;
        if(activeItemSlot < 0)
            activeItemSlot = maxSlots;
        Equip(invSlots[activeItemSlot].itemData);

    }

    private void Equip(ItemData _itemData)
    {
        InstanceHandler.GetInstance<MainGameView>().UpdateSelected(activeItemSlot);
        if(spedUpHotbar == false)
            actualHotbarDelay = hotbarDelaySet;
        if(spedUpHotbar == true)
            actualHotbarDelay = hotbarDelaySet2;
        spedUpHotbar = true;
        if(equippedItem != null)
        {         
            Destroy(equippedItem);
            equippedItem = null;
        }
        if(_itemData != null)
        {
            equippedItem = Instantiate(_itemData.itemPrefab, player.rotationMimic.transform);
            equippedItem.TryGetComponent<IInitialize>(out var iinitialize);
            iinitialize.Initialize(player);
        }
    }

    private void Drop()
    {
        if(equippedItem == null)
            return;
        InstanceHandler.GetInstance<MainGameView>().DeleteItemIcon(activeItemSlot);  
        Destroy(equippedItem);
        Vector3 spawnPosition = cam.transform.position + cam.transform.forward * 1f;

        var droppedItem = Instantiate(invSlots[activeItemSlot].itemData.pickup_itemPrefab, spawnPosition, cam.transform.rotation);
        droppedItem.GetComponent<ItemPickup>().itemData = invSlots[activeItemSlot].itemData;
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        rb.AddForce(cam.transform.forward * dropForce, ForceMode.Impulse);
        equippedItem = null;
        invSlots[activeItemSlot].itemData = null;

    }

    public void PickupItem(ItemData _itemData)
    {
        int firstFreeSlot = Array.FindIndex(invSlots, slot => slot.itemData == null);
        if(firstFreeSlot < 0)
            return;
        if(equippedItem == null)
        {
            invSlots[activeItemSlot].itemData = Instantiate(_itemData);
            Equip(invSlots[activeItemSlot].itemData);
            firstFreeSlot = activeItemSlot;
        }
        else
        {
            invSlots[firstFreeSlot].itemData = _itemData;
        }
        InstanceHandler.GetInstance<MainGameView>().UpdateItemIcon(_itemData.itemIcon, firstFreeSlot);
        
    }
    
    /*public void ToggleItem(GameObject item)
    {
        if(!item.activeSelf)
            item.SetActive(true);
        else
            item.SetActive(false);
    }*/
}
