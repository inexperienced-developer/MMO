using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class ItemManager : Singleton<ItemManager>
{
    public static Dictionary<string, Item> ItemDict { get; private set; } = new Dictionary<string, Item>();

    [SerializeField] private AssetLabelReference m_ItemLabelReference;


    protected override void Awake()
    {
        base.Awake();
        Addressables.LoadAssetsAsync<Item>(m_ItemLabelReference, null).Completed += BuildDict;
    }

    private void BuildDict(AsyncOperationHandle<IList<Item>> obj)
    {
        IDLogger.Log($"Building dict");
        List<Item> items = obj.Result.ToList();
        foreach (var item in items)
        {
            ItemDict.Add(item.Id.ToString(Constants.ITEM_ID_FORMAT), item);
        }
    }
}
