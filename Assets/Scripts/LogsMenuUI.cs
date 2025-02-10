using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


enum ItemType
{
    Item,
    Monster
}


public class LogsMenuUI : MonoBehaviour
{
    private enum ItemType
    {
        Item,
        Monster
    }

    private class LogItem
    {
        private string Name { get; set; }
        private string Description { get; set; }
        private bool isDiscovered { get; set; }
        private ItemType itemType { get; set; }

        public LogItem(string Name, string Description, ItemType itemType)
        {
            this.Name = Name;
            this.Description = Description;
            isDiscovered = false;
            this.itemType = itemType;
        }

        public string GetName() { return Name; }
        public string GetDesc() { return Description; }
        public bool IsDiscovered() { return isDiscovered; }
        public ItemType ItemType() { return itemType; }

        public bool SetDiscovery() { return !isDiscovered; }

    } 

    // list so we can store the info of the logged items
    private List<LogItem> loggedItems = new();
    private ItemType currentItemType = ItemType.Item;

    // Visual Unity UI Toolkit variabels
    private VisualElement rootElement;
    private VisualElement container;

    // File Saving Data
    private string dataFilePath;


    void OnEnable()
    {
        #region UI Toolkit
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        var BackButton = rootElement.Q<Button>("BackBtn");
        BackButton.clicked += () =>
        {
            SaveDiscoveredItems();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        };

        container = rootElement.Q<VisualElement>("Logs-Container");
        var monstersTab = rootElement.Q<Button>("MonstersTab");
        #endregion

        #region Data Initialization
        dataFilePath = "Assets\\items.dat";
        if (!InitializeLogItems())
            Debug.Log("Failed to Initialize Log items");

        //switch (currentItemType)
        //{
        //    case ItemType.Item:
        //        ShowItems();
        //        break;
        //    case ItemType.Monster:
        //        ShowMonsters();
        //        break;
        //}

        #endregion

        //#region Data Saving
        //dataFilePath = "Assets\\items.dat";

        //if (!LoadDiscoveredItems()) 
        //    Debug.Log("Cannot load discovered items/monsters");
        //else { SaveDiscoveredItems(); }

        //#endregion
    }

    private bool InitializeLogItems()
    {
        try
        {
            #region Manual Logging
            // manually logging right now
            //loggedItems.Add(new LogItem("Rock", "Small and hard. Mostly useless. May be able to attract attention when thrown.", ItemType.Item));
            //loggedItems.Add(new LogItem("Andrew", "testing description", ItemType.Monster));
            //loggedItems.Add(new LogItem("Second item", "Item 3", ItemType.Item));
            #endregion

            if (!LoadDiscoveredItems())
                Debug.Log("Cannot load discovered items/monsters");
            else { SaveDiscoveredItems(); }
            return true;
        }
        catch (Exception error)
        {
            Debug.Log(error.Message);
            return false;
        }
    }

    private bool LoadDiscoveredItems()
    {
        if (File.Exists(dataFilePath))
        {
            try
            {
                using var fileStream = new FileStream(dataFilePath, FileMode.Open);
                using var reader = new BinaryReader(fileStream);
                int itemCount = reader.ReadInt32();

                Debug.Log($"Loading {itemCount} discovered items");

                // clear so we dont have duplicates
                loggedItems.Clear();

                for (int i = 0; i < itemCount; i++)
                {
                    // take the item contents
                    string name = reader.ReadString();
                    string description = reader.ReadString();
                    bool isDiscovered = reader.ReadBoolean();
                    ItemType itemType = (ItemType)reader.ReadInt32();

                    // create the item and add to the list
                    LogItem item = new LogItem(name, description, itemType);
                    if (isDiscovered)
                        item.SetDiscovery();
                    loggedItems.Add(item);

                    Debug.Log($"Loaded item {i + 1}: {name}, {description}, {itemType}, {isDiscovered}");
                }
                return true;
            }
            catch (Exception error)
            {
                Debug.Log($"Data file is corrupted, Creating a new file || Unity Error: {error.Message}");
                return InitializeAndSaveNewFile();
            }
        }
        else
        {
            Debug.Log("No existing data file found, creating new one");
            return InitializeAndSaveNewFile();
        }
    }

    private bool InitializeAndSaveNewFile()
    {
        try
        {
            InitializeLogItems();
            return SaveDiscoveredItems();
        }
        catch (Exception error)
        {
            Debug.LogError($"Error initializing new file: {error.Message}");
            return false;
        }
    }

    private bool SaveDiscoveredItems()
    {
        try
        {
            using var fileStream = new FileStream(dataFilePath, FileMode.Create);
            using var writer = new BinaryWriter(fileStream);

            writer.Write(loggedItems.Count);
            foreach (var item in loggedItems)
            {
                writer.Write(item.GetName());
                writer.Write(item.GetDesc());
                writer.Write(item.IsDiscovered());
                writer.Write((int)item.ItemType());
            }
            return true;
        }
        catch (Exception error)
        {
            Debug.LogError($"Error saving items: {error.Message}");
            return false;
        }
    }

    private void DiscoverLogItem(Item item)
    {
        
    }

    private void ShowItemDetails(LogItem item)
    {
        if (item.IsDiscovered())
        {
            // reveal the text of the item/monsters
        }
        else
        {
            // set the text to ???
        }
    }
}
