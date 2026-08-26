using UnityEngine;
using PurrNet;
public class Door : NetworkBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        Debug.Log("used door");
    }
}
