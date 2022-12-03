using InexperiencedDeveloper.Firebase;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemHelper : MonoBehaviour
{
    public List<Item> ItemsToAdd = new List<Item>();
    public List<EquippableItem> EquippableItemsToAdd = new List<EquippableItem>();
    [ContextMenu("Add Item to DB")]
    public async Task AddItemToDB()
    {
        try
        {
            foreach(var item in EquippableItemsToAdd)
            {
                ItemsToAdd.Add(item);
            }
            await DBManager.Instance.AddItemsToDB(ItemsToAdd);
            ItemsToAdd.Clear();
            EquippableItemsToAdd.Clear();
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Failed building item list with Exception: {e}");
        }
    }
}
