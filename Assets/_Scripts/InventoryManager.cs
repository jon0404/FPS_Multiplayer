using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : NetworkBehaviour
{
    [SerializeField] InputActionReference next;
    [SerializeField] InventoryManager player;
    [SerializeField] InputActionReference previous;
    [SerializeField] List<GameObject> items = new();

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void Update()
    {

        if (player.next) 
            PreviousItem();
        if (player.previous) 
            NextItem();
    }
    private void NextItem()
    {
        
    }
    private void PreviousItem()
    {
        
    }
}
