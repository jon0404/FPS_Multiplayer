using System;
using PurrNet;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(initialValue: 100); //a syncvar object with reference variable health
    [SerializeField] private int selfLayer, otherLayer;

    public Action<PlayerID> onDeath_Server; //variable that holds a reference to a method that accepts PlayerHealth objects and returns void

    public int Health => health;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursive(gameObject, actualLayer);

        if (isOwner)
        {
            InstanceHandler.GetInstance<MainGameView>().UpdateHealth(health.value);
            health.onChanged += OnHealthChanged;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        health.onChanged -= OnHealthChanged;
    }
    
    private void OnHealthChanged(int newHealth)
    {
        InstanceHandler.GetInstance<MainGameView>().UpdateHealth(newHealth);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform) // enumerates the transform and gets the transforms of children (idk)
        {
            SetLayerRecursive(child.gameObject, layer);

        }
    }

    [ServerRpc(requireOwnership:false)]
    public void ChangeHealth(int amount)
    {
        health.value += amount;
        if(health <= 0)
        {
            onDeath_Server?.Invoke(owner.Value);
            Destroy(gameObject);
        }
    }
}
