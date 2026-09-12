using System;
using System.Runtime.CompilerServices;
using PurrNet;
using UnityEditor;
using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
}
public interface IItem
{
    void ToggleItem(GameObject item);
}
public interface IInitialize
{
    void Initialize(Player player);
}