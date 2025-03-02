/*
File: SpawnListManager.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/21/2025
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EntityInfo;
using Random = UnityEngine.Random;

/*
Name: SpawnListManager
Purpose: Given the number for each item type (monsters/items/environmental objects), a list will be generated for prefabs to instantiate into the level.
  This class will be responsible for handling special spawn logic like HiveMother/Swarmlings and warptubes 
*/
public class SpawnPoolManager : MonoBehaviour
{
    [SerializeField] SpawnPool ItemSpawnPool;
    [SerializeField] SpawnPool MonsterSpawnPool;
    [SerializeField] SpawnPool EnironmentSpawnPool;

    public List<GameObject> GenerateSpawnList(int itemCount, int monsterCount, int enironmentCount)
    {
        List<GameObject> prefabs = new List<GameObject>();

        AddRandomSelectionFromPool(prefabs, ItemSpawnPool, itemCount);
        AddRandomSelectionFromPool(prefabs, MonsterSpawnPool, monsterCount);
        AddRandomSelectionFromPool(prefabs, EnironmentSpawnPool, enironmentCount, true);

        return prefabs;
    }

    private void AddRandomSelectionFromPool(List<GameObject> addToList, SpawnPool spawnPool, int count, bool allowDuplicates = false)
    {
        List<EntityTags> keys = new List<EntityTags>(spawnPool.Keys());
        for (int i = 0; i < count; i++)
        {
            if (keys.Count == 0)
            {
                break; // failsafe in case you ask for too many
            }

            EntityTags key = keys[Random.Range(0, keys.Count)];
            foreach (SpawnEntryItem item in spawnPool[key])
            {
                addToList.AddRange(Enumerable.Repeat(item.Prefab, item.Count)); // add a prefab multiple times, like multiple swarmlings
            }

            if (allowDuplicates == false)
            {
                keys.Remove(key);  // Don't allow the same item/monster/environmentObject to be selected multiple times
            }
        }
    }

}



[Serializable]
public class SpawnPool 
{
    [SerializeField] public List<SpawnPoolEntry> Entries;

    private Dictionary<EntityTags, List<SpawnEntryItem>> rawPoolDictionary = null;
    private Dictionary<EntityTags, List<SpawnEntryItem>> poolDictionary
    {
        get 
        {
            if (rawPoolDictionary == null)
            {
                InitializePoolDictionary();
            }
            return rawPoolDictionary;
        }
    }

    private void InitializePoolDictionary()
    {
        rawPoolDictionary = new Dictionary<EntityTags, List<SpawnEntryItem>>();
        foreach (SpawnPoolEntry entry in Entries)
        {
            rawPoolDictionary.Add(entry.SpawnTag, entry.SpawnItems);
        }
    }

    // expose the dictionary
    public List<SpawnEntryItem> this[EntityTags tag]
    {
        get => poolDictionary[tag];
    }
    
    // Feel free to add more accessors as needed
    public bool ContainsKey(EntityTags tag)
    {
        return poolDictionary.ContainsKey(tag);
    }
    public Dictionary<EntityTags, List<SpawnEntryItem>>.KeyCollection Keys()
    {
        return poolDictionary.Keys;
    }
}

[Serializable]
public class SpawnPoolEntry
{
    [SerializeField] public EntityTags SpawnTag;
    [SerializeField] public List<SpawnEntryItem> SpawnItems;

}

[Serializable]
public class SpawnEntryItem
{
    [SerializeField] public GameObject Prefab;
    [SerializeField] public int Count = 1;
}


