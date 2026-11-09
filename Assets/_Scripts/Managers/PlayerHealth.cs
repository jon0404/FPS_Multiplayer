using System;
using PurrNet;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    // Shared health value for the owner and remote clients.
    [SerializeField] private SyncVar<int> health = new(initialValue: 100);
    [SerializeField] private int selfLayer, otherLayer;

    public Action<PlayerID> onDeath_Server;

    // Exposes the current health value without allowing direct mutation.
    public int Health => health.value;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursive(gameObject, actualLayer);

        if (isOwner)
        {
            var mainGameView = InstanceHandler.GetInstance<MainGameView>();
            if (mainGameView != null)
                mainGameView.UpdateHealth(health.value);

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
        var mainGameView = InstanceHandler.GetInstance<MainGameView>();
        if (mainGameView != null)
            mainGameView.UpdateHealth(newHealth);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        if (obj == null)
            return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    [ServerRpc(requireOwnership: false)]
    public void ChangeHealth(int amount)
    {
        health.value += amount;

        // The player is eliminated when health reaches zero or below.
        if (health.value <= 0)
        {
            onDeath_Server?.Invoke(owner.Value);
            Destroy(gameObject);
        }
    }
}
