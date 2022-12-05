using Firebase.Firestore;
using Firebase.Extensions;
using InexperiencedDeveloper.Core;
using System.Collections.Generic;
using System;
using InexperiencedDeveloper.Utils.Log;
using System.Linq;
using System.Threading.Tasks;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using UnityEngine.TextCore.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace InexperiencedDeveloper.Firebase
{
    public class DBManager : Singleton<DBManager>
    {
        private FirebaseFirestore m_DB;

        private List<string> m_SubCollections = new List<string>() { "Appearance", "Inventory", };

        protected override void Awake()
        {
            base.Awake();
            m_DB = FirebaseFirestore.DefaultInstance;
        }

        public async Task<List<CharacterAppearanceData>> GetCharacters(string email)
        {
            List<CharacterAppearanceData> result = new List<CharacterAppearanceData>();
            Query charactersQuery = m_DB.Collection("users").Document(email).Collection("Characters");
            QuerySnapshot charactersQuerySnap = await charactersQuery.GetSnapshotAsync();
            if(charactersQuerySnap != null)
            {
                foreach (DocumentSnapshot character in charactersQuerySnap.Documents)
                {
                    Dictionary<string, object> characterData = character.ToDictionary();
                    //characterData.TryGetValue("race", out object raceObj);
                    byte level = ObjToByte(characterData, "level");
                    ushort totalLevel = (ushort)ObjToByte(characterData, "totalLevel");
                    DocumentReference appearanceRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(character.Id).Collection("Appearance").Document("Physical");
                    DocumentSnapshot appearanceSnap = await appearanceRef.GetSnapshotAsync();
                    if(appearanceSnap != null)
                    {
                        Dictionary<string, object> appearanceDict = appearanceSnap.ToDictionary();
                        byte skinColor = ObjToByte(appearanceDict, "skinColor");
                        byte hairColor = ObjToByte(appearanceDict, "hairColor");
                        byte hairStyle = ObjToByte(appearanceDict, "hairStyle");
                        byte facialHairStyle = ObjToByte(appearanceDict, "facialHairStyle");
                        byte eyebrowStyle = ObjToByte(appearanceDict, "eyebrowStyle");
                        byte eyeColor = ObjToByte(appearanceDict, "eyeColor");
                        byte appearanceByte = ObjToByte(appearanceDict, "appearanceByte");
                        CharacterAppearanceData data = new CharacterAppearanceData(character.Id, level, totalLevel,
                            skinColor, hairColor, hairStyle, facialHairStyle, eyebrowStyle, eyeColor, appearanceByte);
                        result.Add(data);
                    }
                }
            }
            return result;
        }

        public async Task<bool> CheckCharacterExists(string n)
        {
            IDLogger.Log(n);
            DocumentReference charsRef = m_DB.Collection("characters").Document(n[0].ToString().ToUpper());
            DocumentSnapshot charSnap = await charsRef?.GetSnapshotAsync();
            if(charSnap != null)
            {
                Dictionary<string, object> charDict = charSnap.ToDictionary();
                if (charDict == null || charDict.Count <= 0) return false;
                foreach (var k in charDict.Keys.ToArray())
                {
                    if (k.ToLower() == n.ToLower())
                        return true;
                }
            }
            return false;
        }

        public async Task<List<CharacterAppearanceData>> AddNewCharToDB(string email, CharacterAppearanceData data)
        {
            Dictionary<string, object> generalDict = new();
            Dictionary<string, object> appearanceDict = new();
            Dictionary<string, object> bagDict = new();
            Dictionary<string, object> inventoryDict = new();
            CreateDataDict(data, ref generalDict, ref appearanceDict, ref bagDict, ref inventoryDict);
            await m_DB.Collection("users").Document(email).Collection("Characters").Document(data.Name).SetAsync(generalDict);
            await m_DB.Collection("users").Document(email).Collection("Characters").Document(data.Name).Collection("Appearance").Document("Physical").SetAsync(appearanceDict);
            await m_DB.Collection("users").Document(email).Collection("Characters").Document(data.Name).Collection("Inventory").Document("Bags").SetAsync(bagDict);
            await m_DB.Collection("users").Document(email).Collection("Characters").Document(data.Name).Collection("Inventory").Document("Items").SetAsync(inventoryDict);
            Dictionary<string, object> globalChar = new Dictionary<string, object>
            {
                    { data.Name, 0 }
            };
            await m_DB.Collection("characters").Document(data.Name[0].ToString().ToUpper()).SetAsync(globalChar, SetOptions.MergeAll);
            return await GetCharacters(email);
        }

        private void CreateDataDict(CharacterAppearanceData data, ref Dictionary<string, object> generalDict, ref Dictionary<string, object> appearanceDict,
            ref Dictionary<string, object> bagDict, ref Dictionary<string, object> inventoryDict)
        {
            generalDict = new();
            generalDict.Add("level", data.Level);
            generalDict.Add("totalLevel", data.TotalLevel);
            appearanceDict = new();
            byte appearanceByte = Utilities.BoolsToByte(new bool[3] { data.PantsOn, data.ShirtOn, data.BootsOn });
            appearanceDict.Add("appearanceByte", appearanceByte);
            appearanceDict.Add("eyeColor", data.EyeColor);
            appearanceDict.Add("eyebrowStyle", data.EyebrowStyle);
            appearanceDict.Add("facialHairStyle", data.FacialHairStyle);
            appearanceDict.Add("hairColor", data.HairColor);
            appearanceDict.Add("hairStyle", data.HairStyle);
            appearanceDict.Add("skinColor", data.SkinColor);
            bagDict = new();
            //First bags
            bagDict.Add("Bag1", "000003"); // Item # 000003: Weathered Bag
            //Then fill
            inventoryDict = new();
            inventoryDict.Add("0", "000000"); // Item # 000000: Home Stone (Hearthstone)
        }

        public async Task<List<CharacterAppearanceData>> DeleteCharFromDB(string email, string n)
        {
            //Go into character and get all sub-collections
            bool successful = false;
            foreach(var c in m_SubCollections)
            {
                try
                {
                    Query subCollectionQuery = m_DB.Collection("users").Document(email).Collection("Characters").Document(n).Collection(c);
                    QuerySnapshot subCollectionQuerySnap = await subCollectionQuery.GetSnapshotAsync();
                    if (subCollectionQuerySnap != null)
                    {
                        //Delete each subcollection
                        foreach (var doc in subCollectionQuerySnap.Documents)
                        {
                            DocumentReference docRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(n).Collection(c).Document(doc.Id);
                            await docRef.DeleteAsync();
                        }
                    }
                } catch (Exception e)
                {
                    IDLogger.LogError($"Failed removing character {n} with Exception: {e}");
                    return null;
                }
            }
            //Delete character info
            try
            {
                DocumentReference charRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(n);
                await charRef.DeleteAsync();
                Dictionary<string, object>  delete = new Dictionary<string, object> { { n, FieldValue.Delete } };
                await m_DB.Collection("characters").Document(n[0].ToString().ToUpper()).SetAsync(delete, SetOptions.MergeAll);
                successful = true;
            } catch (Exception e)
            {
                IDLogger.LogError($"Failed removing character {n} with Exception: {e}");
                return null;
            }
            return successful ? await GetCharacters(email) : null;
        }

        public async Task<List<List<float>>> GetSpawnDataFromDB(string email, string n)
        {
            try
            {

                DocumentReference charRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(n);
                DocumentSnapshot charSnap = await charRef?.GetSnapshotAsync();
                List<List<float>> data = new List<List<float>>();
                if (charSnap != null)
                {
                    List<float> lastPos = new();
                    List<float> lastRot = new();
                    List<float> gearList = new();
                    Dictionary<string, object> set = new Dictionary<string, object>();
                    Dictionary<string, object> charDict = charSnap.ToDictionary();
                    if (charDict == null || charDict.Count <= 0) return null;
                    //Get last pos
                    if (charDict.ContainsKey("lastPos"))
                    {

                        List<object> c = (List<object>)charDict["lastPos"];
                        foreach(var item in c)
                        {
                            float i = float.Parse(item.ToString());
                            lastPos.Add(i);
                            IDLogger.LogWarning($"Added {i} to lastPos: {lastPos.Count}");
                        }
                    }
                    else
                    {
                        //If doesn't exist spawn at original pos and save to dict
                        Vector3 pos = GameManager.Instance.SpawnPoint.position;
                        lastPos = new() { pos.x, pos.y, pos.z };

                        //Add geographic area as last digit
                        lastPos.Add(ConstGeoAreas.STARTING_AREA);
                        set = new();
                        set.Add("lastPos", lastPos);
                        await charRef.SetAsync(set, SetOptions.MergeAll);
                    }
                    //Get last rot
                    if (charDict.ContainsKey("lastRot"))
                    {
                        List<object> c = (List<object>)charDict["lastRot"];
                        foreach (var item in c)
                        {
                            float i = float.Parse(item.ToString());
                            lastRot.Add(i);
                        }
                    }
                    else
                    {
                        //If doesn't exist spawn at original rot and save to dict
                        Quaternion rot = GameManager.Instance.SpawnPoint.rotation;
                        lastRot = new() { rot.x, rot.y, rot.z, rot.w };
                        set = new();
                        set.Add("lastRot", lastRot);
                        await charRef.SetAsync(set, SetOptions.MergeAll);
                    }
                    //Get Gear Data
                    if (charDict.ContainsKey("gearList"))
                    {
                        List<object> c = (List<object>)charDict["gearList"];
                        foreach(var item in c)
                        {
                            float i = float.Parse(item.ToString());
                            gearList.Add(i);
                        }
                    }
                    else
                    {
                        gearList = new List<float>();
                        set = new();
                        set.Add("gearList", gearList);
                        await charRef.SetAsync(set, SetOptions.MergeAll);
                    }

                    data = new List<List<float>>();
                    data.Add(lastPos);
                    data.Add(lastRot);
                    data.Add(gearList);
                }
                DocumentReference bagRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(n).Collection("Inventory").Document("Bags");
                DocumentReference inventoryRef = m_DB.Collection("users").Document(email).Collection("Characters").Document(n).Collection("Inventory").Document("Items");
                DocumentSnapshot bagSnap = await bagRef.GetSnapshotAsync();
                DocumentSnapshot inventorySnap = await inventoryRef.GetSnapshotAsync();
                if(bagSnap != null)
                {
                    Dictionary<string, object> bagDict = bagSnap.ToDictionary();
                    List<float> bagList = new();
                    foreach (var key in bagDict.Keys)
                    {
                        float i = float.Parse(bagDict[key].ToString());
                        bagList.Add(i);
                    }
                    data.Add(bagList);
                }
                if (inventorySnap != null)
                {
                    Dictionary<string, object> inventoryDict = inventorySnap.ToDictionary();
                    List<float> inventoryList = new();
                    foreach (var key in inventoryDict.Keys)
                    {
                        float i = float.Parse(inventoryDict[key].ToString());
                        inventoryList.Add(i);
                    }
                    data.Add(inventoryList);
                }
                return data;
            }
            catch (Exception e)
            {
                IDLogger.LogError($"Failed to get Spawn Data from DB: {e}");
                throw new Exception("Can't complete getting data");
            }
        }

        public async Task<Dictionary<ushort, string>> GetItemDictionary()
        {
            Dictionary<ushort, string> data = new Dictionary<ushort, string>();
            DocumentReference itemRef = m_DB.Collection("data").Document("items");
            DocumentSnapshot itemSnap = await itemRef.GetSnapshotAsync();
            if(itemSnap != null)
            {
                Dictionary<string, object> items = itemSnap.ToDictionary();
                if (items == null || items.Count <= 0) { throw new ArgumentException("Can't find any items", "NoItemInDB"); }
                foreach (var k in items.Keys.ToArray())
                {
                    ushort id = ushort.Parse(k);
                    data.Add(id, (string)items[k]);
                }
            }
            return data;
        }
        
        #region Helpers
        public async Task AddItemsToDB(List<Item> items)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach(var item in items)
            {
                // Item String Format: "{ItemName}`{SlotNum}`{HasAbility}-{Ability1}-{Ability2}"
                string itemString = $"{item.ItemName}`{(ushort)item.Slot}";
                if(item.GetType() == typeof(EquippableItem))
                {
                    EquippableItem e = (EquippableItem)item;
                    itemString += $"`{e.HasAbility}";
                }
                data.Add(item.Id.ToString(), itemString);
            }
            DocumentReference itemRef = m_DB.Collection("data").Document("items");
            await itemRef.SetAsync(data, SetOptions.MergeAll);
        }
        #endregion

        private byte ObjToByte(Dictionary<string, object> dict, string key)
        {
            dict.TryGetValue(key, out object obj);
            byte b = byte.Parse(obj.ToString());
            return b;
        }
    }
}

