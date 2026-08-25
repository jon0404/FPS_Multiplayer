using UnityEngine;
using PurrNet;
public class Door : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("used door");
    }
}
