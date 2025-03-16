using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityComparer : IComparer<IInteractable>
{
    public int Compare(IInteractable x, IInteractable y)
    {
        return x.InteractPriority.CompareTo(y.InteractPriority);
    }
}
