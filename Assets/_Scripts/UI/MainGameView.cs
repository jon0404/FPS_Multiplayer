using UnityEngine;
using System;
using TMPro;
using PurrNet;
using UnityEngine.UI;
[System.Serializable]
public class HotbarSlots
{
    public Image emptyIcon;
    public RectTransform selectedIconTrans;
    public Image itemIcon;
    public RectTransform trans;
}
public class MainGameView : View
{

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image hotbar_icon_empty;
    [SerializeField] private Image hotbar_icon_selected; 
    [SerializeField] private Transform emptySlots;
    [SerializeField] private Transform itemIcons;
    [SerializeField] private int itemsSlotId;
    private HotbarSlots[] hotbarSlots;
    [SerializeField] private int maxSlots;
    private RectTransform selectedIconTrans;


    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);   
        selectedIconTrans = hotbar_icon_selected.GetComponent<RectTransform>();
    }

    public void Hotbar(int slotCount)
    {
        hotbarSlots = new HotbarSlots[slotCount];
        Vector3 position = new Vector3(-300, -450, 0);
        InstanceHandler.RegisterInstance(this);
        for(int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i] = new HotbarSlots();
            hotbarSlots[i].emptyIcon = Instantiate(hotbar_icon_empty, emptySlots);
            hotbarSlots[i].emptyIcon.rectTransform.anchoredPosition = position;
            position.x += 90;
            hotbarSlots[i].trans = hotbarSlots[i].emptyIcon.rectTransform;      
        }
        UpdateSelected(0);
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

    public void UpdateItemIcon(Image itemIcon, int slot)
    {
        hotbarSlots[slot].itemIcon = Instantiate(itemIcon, emptySlots);
        hotbarSlots[slot].itemIcon.rectTransform.anchoredPosition = hotbarSlots[slot].trans.anchoredPosition;


    }

    public void DeleteItemIcon(int slot)
    {
        Destroy(hotbarSlots[slot].itemIcon.gameObject);
    }
    public void UpdateSelected(int slot)
    {
            selectedIconTrans.anchoredPosition = hotbarSlots[slot].trans.anchoredPosition;
            Debug.Log($"{selectedIconTrans} highlight transform {hotbarSlots[slot].trans.anchoredPosition} slot transform");
    }
}
