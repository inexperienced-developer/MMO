using InexperiencedDeveloper.Firebase;
using InexperiencedDeveloper.Utils.Log;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemManager
{
    private static Dictionary<ushort, string> m_Items = new Dictionary<ushort, string>();

    public static async Task Init()
    {
        //Get Item List from DB
        try
        {
            m_Items = await DBManager.Instance.GetItemDictionary();
        } catch (System.Exception e)
        {
            IDLogger.LogError($"Failed building item list with Exception: {e}");
        }
    }


    // Item String Format: "{ItemName}`{SlotNum}`{HasAbility}-{Ability1}-{Ability2}"
    public static async Task<string> GetItemString(ushort id)
    {
        try
        {
            if (m_Items == null || m_Items.Count <= 0) await Init();
            if (m_Items.ContainsKey(id))
                return m_Items[id];
            else
                throw new System.Exception("Search dictionary, but can't find anything");
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Failed building item list with Exception: {e}");
            throw new System.Exception("Can't find item in dictionary");
        }
    }
}
