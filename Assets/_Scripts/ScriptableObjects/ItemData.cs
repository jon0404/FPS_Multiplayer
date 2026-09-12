using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData object", menuName = "ItemData object")]
public class ItemData : ScriptableObject
{
    public GameObject pickup_itemPrefab;
    public GameObject itemPrefab;
    public Image itemIcon;
    
}

 
