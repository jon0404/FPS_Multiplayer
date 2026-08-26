using UnityEngine;
using System.Collections.Generic;
using TMPro;
using PurrNet;
using UnityEngine.UI;
[System.Serializable]
public class ItemArray
{
    public GameObject itemObject;
}

public class MainGameView : View
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image hotbar_icon_empty;
    [SerializeField] private Image hotbar_icon_selected; 
    [SerializeField] private Transform emptySlots;
    [SerializeField] private Transform itemIcons;
    [SerializeField] private int itemsSlotId;
    [SerializeField] private ItemArray[] itemArray = new ItemArray[8];
    private void Awake()
    {
        itemArray[0] = new ItemArray();
        itemArray[1] = new ItemArray();
        itemArray[2] = new ItemArray();
        itemArray[3] = new ItemArray();
        itemArray[4] = new ItemArray();
        itemArray[5] = new ItemArray();
        itemArray[6] = new ItemArray();
        itemArray[7] = new ItemArray();
        InstanceHandler.RegisterInstance(this);
        var rectTrans = hotbar_icon_selected.GetComponent<RectTransform>();
    }
    

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<MainGameView>();
    }

    public override void OnShow()
    {
        
    }

    public override void OnHide()
    {
        
    }

    public void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }

    public void CreateSelected()
    {

    }

    public void UpdateItemIcon(Item item, int slot)
    {
        var icon = Instantiate(item.icon, itemIcons);
        itemArray[slot].itemObject = icon;
        Vector3 position = new Vector3 (-300, -450, 0);
        var rectTrans = icon.GetComponent<RectTransform>();
        position.x += 90 * (slot);
        rectTrans.anchoredPosition3D = position;

    }

    public void DeleteItemIcon(int slot)
    {
        Destroy(itemArray[slot].itemObject);
    }
    public void UpdateSelected(int slot)
    {
        Debug.Log($"{slot}");
            var rectTrans = hotbar_icon_selected.GetComponent<RectTransform>();
            Vector3 position = new Vector3 (-300, -450, 0);
            position.x += 90 * (slot);
            Vector3 pos = rectTrans.anchoredPosition3D;
            pos.x = position.x;
            rectTrans.anchoredPosition3D = pos;

        Debug.Log($"{position}");
    }

    public void Hotbar(int hotbarSlot)
    {
        Vector3 position = new Vector3(-300, -450, 0);
        while(hotbarSlot > 0)
        {
            Image iconEmpty = Instantiate(hotbar_icon_empty, emptySlots);
            var rectTrans = iconEmpty.GetComponent<RectTransform>();
            rectTrans.anchoredPosition3D = position;
            position.x += 90;
            hotbarSlot--;
        }
    }
}
