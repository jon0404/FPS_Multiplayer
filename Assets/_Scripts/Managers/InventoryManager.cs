using System;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[System.Serializable]
public class InventorySlot
{
    public GameObject itemPrefab;
}

public class InventoryManager : NetworkBehaviour
{
    [SerializeField] float hotbarDelay;
    [SerializeField] float hotbarDelay2;
    [SerializeField] float actualHotbarDelay;
    [SerializeField] float hotbarDelaySet = 0.5f;
    [SerializeField] float hotbarDelaySet2 = 0.15f;
    [SerializeField] bool spedUpHotbar;
    [SerializeField] InputActionReference next;
    [SerializeField] InputActionReference previous;
    [SerializeField] InputActionReference drop;
    [SerializeField] GameObject player;
    [SerializeField] private InventorySlot[] invSlots = new InventorySlot[8];
    private GameObject equippedItem = null;
    [SerializeField] int activeItemSlot = 0;
    MainGameView ui;
    int maxSlots;
    
    void Awake()
    {
    

    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        enabled = isOwner;
    }

    private void Start()
    {
        maxSlots = invSlots.Length;
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
        Equip(invSlots[activeItemSlot].itemPrefab);
    }

    private void PreviousSlot()
    {
        activeItemSlot--;
        if(activeItemSlot < 0)
            activeItemSlot = maxSlots;
        Equip(invSlots[activeItemSlot].itemPrefab);

    }

    private void Equip(GameObject _itemPrefab)
    {
        if(spedUpHotbar == false)
            actualHotbarDelay = hotbarDelaySet;
        if(spedUpHotbar == true)
            actualHotbarDelay = hotbarDelaySet2;
        spedUpHotbar = true;
        InstanceHandler.GetInstance<MainGameView>().UpdateSelected(activeItemSlot);
        Debug.Log($"{activeItemSlot}");
        if(equippedItem != null)
        {         
            Destroy(equippedItem);
            equippedItem = null;
        }
        if(_itemPrefab != null)
        {
            equippedItem = Instantiate(_itemPrefab, transform.position, transform.rotation, transform);
            Debug.Log($"SPAWNED: {equippedItem.name} | ID: {equippedItem.GetEntityId()}");
            Debug.Log($"{equippedItem.name} equipped");
        }
    }

    private void Drop()
    {
        if(equippedItem == null)
            return;
        InstanceHandler.GetInstance<MainGameView>().DeleteItemIcon(activeItemSlot);  
        Debug.Log($"DROPPING: {equippedItem.name} | ID: {equippedItem.GetEntityId()}");

        Destroy(equippedItem);
        equippedItem = null;
        Debug.Log($"{equippedItem} still here?");
        invSlots[activeItemSlot].itemPrefab = null;

    }

    public void PickupItem(GameObject _itemPrefab)
    {
        int firstFreeSlot = Array.FindIndex(invSlots, slot => slot.itemPrefab == null);
        Debug.Log($"{firstFreeSlot} Inventory slot ");    
        if(firstFreeSlot < 0)
            return;
        if(equippedItem == null)
        {
            invSlots[activeItemSlot].itemPrefab = _itemPrefab;
            Equip(invSlots[activeItemSlot].itemPrefab);
            Debug.Log($"{invSlots[activeItemSlot].itemPrefab} equipped itemPrefab");
            firstFreeSlot = activeItemSlot;
        }
        else
        {
            invSlots[firstFreeSlot].itemPrefab = _itemPrefab;
            Debug.Log($"Slot {firstFreeSlot} now contains: {invSlots[firstFreeSlot].itemPrefab}");
            Debug.Log($"{firstFreeSlot} set new item to that and didnt equip");
        }
        Item _icon = _itemPrefab.GetComponent<Item>();   
        InstanceHandler.GetInstance<MainGameView>().UpdateItemIcon(_icon, firstFreeSlot);
        
    }
    
    /*public void ToggleItem(GameObject item)
    {
        if(!item.activeSelf)
            item.SetActive(true);
        else
            item.SetActive(false);
    }*/
}
