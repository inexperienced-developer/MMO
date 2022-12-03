using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Hearthstone")]
public class Hearthstone : Item, IUseable
{
    public void Use()
    {
        IDLogger.Log("Going home");
    }
}
