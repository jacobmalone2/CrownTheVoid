using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public int InteractPriority { get; }
    public string InteractionPrompt { get; }
    public bool HasInteractedWith { get; }
    public bool IsItem {  get; }
    public bool IsKeyItem { get; }
    public bool Interact(Interactor interactor);
    public void ShowPopUp(string prompt);
}
