using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PurrNet;

public class RoundObjectManager : MonoBehaviour
{
    private static RoundObjectManager instance;
    private readonly HashSet<RoundResettable> resettableObjects = new();
    private readonly HashSet<GameObject> spawnedObjects = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private static RoundObjectManager GetOrCreate()
    {
        if (instance != null)
            return instance;

        instance = FindFirstObjectByType<RoundObjectManager>();
        if (instance == null)
            instance = new GameObject(nameof(RoundObjectManager)).AddComponent<RoundObjectManager>();

        return instance;
    }

    internal static void Register(RoundResettable obj)
    {
        if (obj != null)
            GetOrCreate().resettableObjects.Add(obj);
    }

    internal static void Unregister(RoundResettable obj)
    {
        if (instance != null && obj != null)
            instance.resettableObjects.Remove(obj);
    }

    public static void ResetRegisteredObjects(bool asServer)
    {
        GetOrCreate().ResetRound(asServer);
    }

    public void Register(GameObject obj)
    {
        if (obj == null)
            return;

        spawnedObjects.Add(obj);
    }

    public bool Unregister(GameObject obj)
    {
        if (obj == null)
            return false;

        return spawnedObjects.Remove(obj);
    }

    public void ResetRound(bool asServer = true)
    {
        foreach (var obj in resettableObjects.ToArray())
        {
            if (obj != null)
                obj.ResetForRound(asServer);
        }

        foreach (var obj in spawnedObjects.ToArray())
        {
            if (obj == null)
                continue;

            if (obj.TryGetComponent<NetworkIdentity>(out var identity) && identity.isSpawned)
            {
                if (asServer)
                    identity.Despawn();
            }
            else
            {
                Destroy(obj);
            }
        }

        spawnedObjects.Clear();
    }
}