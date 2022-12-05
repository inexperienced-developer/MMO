using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public static Dictionary<string, Item> ItemDict { get; private set; }
    [SerializeField] private List<Item> m_Items;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        BuildItemDict();
    }

    private void BuildItemDict()
    {
        ItemDict = new();
        string idFmt = "000000";
        //Get all items

        foreach(var item in m_Items)
        {
            ItemDict.Add(item.Id.ToString(idFmt), item);
        }
    }
}
