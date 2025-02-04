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
            this.isDiscovered = false;
            this.itemType = itemType;
        }

        public string GetName() { return Name; }
        public string GetDesc() { return Description; }
        public bool IsDiscovered() { return isDiscovered; }
        public ItemType ItemType() { return itemType; }

    }

    // list so we can store the info of the logged items
    private List<LogItem> loggedItems = new();
    private ItemType currentItemType = ItemType.Item;

    // Visual Unity UI Toolkit variabels
    private VisualElement rootElement;
    private VisualElement container;

    private string dataFilePath;


    void OnEnable()
    {
        #region UI Toolkit
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        var BackButton = rootElement.Q<Button>("BackBtn");
        BackButton.clicked += () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        };

        container = rootElement.Q<VisualElement>("Logs-Container");
        var monstersTab = rootElement.Q<Button>("MonstersTab");
        #endregion


        #region Data Initialization
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

        #region Data Saving
        dataFilePath = "Assets\\items.dat";
        if (File.Exists(dataFilePath))
        {
            if (!LoadDiscoveredItems())
                Debug.Log("Cannot load discovered items/monsters");
        }
        else { SaveDiscoveredItems(); }

        #endregion
    }

    private bool InitializeLogItems()
    {
        try
        {
            // manually logging right now
            loggedItems.Add(new LogItem("Rock", "Small and hard. Mostly useless. May be able to attract attention when thrown.", ItemType.Item));
            loggedItems.Add(new LogItem("Bruh", "Small and hard", ItemType.Monster));
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
        try
        {
            if (File.Exists(dataFilePath))
            {
                using var write = new FileStream(dataFilePath, FileMode.Open);
                using (var read = new BinaryReader(write))
                {
                    int itemCount = read.ReadInt32();

                    Debug.Log($"Loading {itemCount} discovered items");
                    for (int i = 0; i < itemCount; i++)
                    {
                        string name = read.ReadString();
                        string description = read.ReadString();
                        ItemType itemType = (ItemType)read.ReadInt32();
                        bool isDiscovered = read.ReadBoolean();
                    };
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception error)
        {
            Debug.Log(error.Message);
            return false;
        }
    }

    private bool SaveDiscoveredItems()
    {
        try
        {
            using var write = new FileStream(dataFilePath, FileMode.Create);
            using (var binaryWriter = new BinaryWriter(write))
            {
                binaryWriter.Write(loggedItems.Count);
                foreach (var item in loggedItems)
                {
                    binaryWriter.Write(item.GetName());
                    binaryWriter.Write(item.GetDesc());
                    binaryWriter.Write(item.IsDiscovered());
                    binaryWriter.Write((int)item.ItemType());
                }
            }
            return true;
        }
        catch (Exception error)
        {
            Debug.Log(error.Message);
            return false;
        }
    }

    private void DiscoverLogItem(string LogName, ItemType LogType)
    {

    }


    private void ShowItems()
    {
        
    }

    private void ShowMonsters()
    {

    }
}
