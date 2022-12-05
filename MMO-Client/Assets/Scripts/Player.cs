using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ushort Id { get; protected set; }
    public bool IsLocal { get; protected set; }
    protected string m_Email;
    public string Email => m_Email;
    public PlayerControls Controls { get; protected set; }
    public Inventory Inventory { get; protected set; }
    public bool DEBUG = false;

    protected virtual void OnDestroy()
    {
        PlayerManager.RemovePlayerFromList(this);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void Init(ushort id, string email = "")
    {
        Id = id;
        m_Email = email;
        if (Id == NetworkManager.Instance.Client.Id)
        {
            IsLocal = true;
        }
        else
        {
            IsLocal = false;
        }
    }

    public void SetInventory(List<ushort> bagIDs, List<ushort> itemIDs)
    {
        string idFmt = "000000";
        List<Bag> bags = new List<Bag>();
        foreach (var id in bagIDs)
        {
            if (ItemManager.ItemDict.TryGetValue(id.ToString(idFmt), out Item item))
            {
                BagScriptableObj bag = (BagScriptableObj)item;
                Bag b = new Bag(bag.Id, bag.name, (byte)bag.NumSlots, bag.Categories, bag.Icon);
                bags.Add(b);
            }
        }
        foreach (var id in itemIDs)
        {
            int bagIndex = 0;
            if (ItemManager.ItemDict.TryGetValue(id.ToString(idFmt), out Item item))
            {
                if (bags[bagIndex].IsFull) bagIndex++;
                if(bagIndex < bags.Count)
                    bags[bagIndex].AddItem(item);
            }
        }
        if(Inventory == null) Inventory = GetComponent<Inventory>() == null ? gameObject.AddComponent<Inventory>() : GetComponent<Inventory>();
        Inventory.SetBags(bags);
    }
}
