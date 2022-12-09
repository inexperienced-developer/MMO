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

    private int attempts = 0;

    protected void Start()
    {
        Addressables.LoadAssetsAsync<Item>(m_ItemLabelReference, null).Completed += BuildDict;
    }

    private void BuildDict(AsyncOperationHandle<IList<Item>> obj)
    {
        if(obj.Result == null)
        {
            IDLogger.LogError($"Item list is null");
            if(attempts < 5)
            {
                attempts++;
                Addressables.LoadAssetsAsync<Item>(m_ItemLabelReference, null).Completed += BuildDict;
                return;
            }
            return;
        }
        IDLogger.Log($"Building dict");
        List<Item> items = obj.Result.ToList();
        foreach (var item in items)
        {
            ItemDict.Add(item.Id.ToString(Constants.ITEM_ID_FORMAT), item);
        }
    }
}
