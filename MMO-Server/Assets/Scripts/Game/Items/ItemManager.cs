using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils.Log;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : Singleton<ItemManager>
{
    public static Dictionary<string, Item> Items {get; private set;} = new Dictionary<string, Item>();
    [SerializeField] private AssetLabelReference m_ItemLabelReference;
    private int attempts = 0;

    private void Start()
    {
        Addressables.LoadAssetsAsync<Item>(m_ItemLabelReference, null).Completed += BuildDict;
    }

    private void BuildDict(AsyncOperationHandle<IList<Item>> obj)
    {
        if(obj.Result == null)
        {
            IDLogger.LogError($"Item list is null");
            if (attempts < 5)
            {
                attempts++;
                Addressables.LoadAssetsAsync<Item>(m_ItemLabelReference, null).Completed += BuildDict;
                return;
            }
            return;
        }
        IDLogger.Log($"Building dict");
        List<Item> items = obj.Result.ToList();
        foreach(var item in items)
        {
            Items.Add(item.Id.ToString(Constants.ITEM_ID_FORMAT), item);
        }
    }

    //public static async Task InitAsync()
    //{
    //    //Get Item List from DB
    //    try
    //    {
    //        m_Items = await DBManager.Instance.GetItemDictionary();
    //    } catch (System.Exception e)
    //    {
    //        IDLogger.LogError($"Failed building item list with Exception: {e}");
    //    }
    //}


    // Item String Format: "{ItemName}`{SlotNum}`{HasAbility}-{Ability1}-{Ability2}"
    //public static async Task<string> GetItemString(ushort id)
    //{
    //    try
    //    {
    //        if (m_Items == null || m_Items.Count <= 0) await Init();
    //        if (m_Items.ContainsKey(id))
    //            return m_Items[id];
    //        else
    //            throw new System.Exception("Search dictionary, but can't find anything");
    //    }
    //    catch (System.Exception e)
    //    {
    //        IDLogger.LogError($"Failed building item list with Exception: {e}");
    //        throw new System.Exception("Can't find item in dictionary");
    //    }
    //}
}
