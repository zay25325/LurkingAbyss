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

        public LogItem(string Name, string Description, ItemType itemType, bool isDiscovered /*, Image logItemImage*/)
        {
            this.Name = Name;
            this.Description = Description;
            this.itemType = itemType;
            this.isDiscovered = isDiscovered;
            //this.logItemImage = logItemImage;
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

        InitializeUI();
        #endregion
    }

    private bool InitializeLogItems()
    {
        try
        {
            #region Manual Logging
            // manually logging right now
            loggedItems.Add(new LogItem("Rock", "Small and hard. Mostly useless. May be able to attract attention when thrown.", ItemType.Item, true));
            loggedItems.Add(new LogItem("Andrew", "testing description", ItemType.Monster, true));
            loggedItems.Add(new LogItem("Second item", "Item 3", ItemType.Item, false));
            //SaveDiscoveredItems(); // update the file manually (testing)
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
                    LogItem item = new LogItem(name, description, itemType, isDiscovered);
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

    private void InitializeUI()
    {
        // tab buttons
        var itemsBtn = rootElement.Q<Button>("Items-Btn");
        var monstersBtn = rootElement.Q<Button>("Monsters-Btn");

        itemsBtn.clicked += () =>
        {
            currentItemType = ItemType.Item;
            DisplayItems();  // Add refresh call
        };

        monstersBtn.clicked += () =>
        {
            currentItemType = ItemType.Monster;
            DisplayItems();  // Add refresh call
        };

        DisplayItems();  // Initial display
    }

    private void DisplayItems()
    {
        var objectCards = rootElement.Q<VisualElement>("Object-Cards");
        objectCards.Clear();  // Clear existing items

        //Debug.Log($"Displaying items. Total items: {loggedItems.Count}, Current type: {currentItemType}");

        // loop through the items/monsters and display them
        foreach (var item in loggedItems)
        {
            // make sure the item is the correct type
            if (item.ItemType() != currentItemType)
                continue;

            var itemCard = new VisualElement();
            itemCard.AddToClassList("object-card");

            // Name container
            var nameContainer = new VisualElement();
            nameContainer.AddToClassList("name-container");
            var itemName = new Label(item.IsDiscovered() ? item.GetName() : "???");
            itemName.AddToClassList("name");
            nameContainer.Add(itemName);

            // Description container
            var descContainer = new VisualElement();
            descContainer.AddToClassList("description-container");
            var itemDesc = new Label(item.IsDiscovered() ? item.GetDesc() : "???");
            itemDesc.AddToClassList("description");
            descContainer.Add(itemDesc);

            // Add containers to card
            itemCard.Add(nameContainer);
            itemCard.Add(descContainer);

            // finally add to the screen
            objectCards.Add(itemCard);
            Image itemImage = new();
            //Debug.Log($"Added card for item: {item.GetName()}, Type: {item.ItemType()}");
        }
    }

    private bool CheckIsDiscovered()
    {

        throw new NotImplementedException();
        //return false;
    }

    private void DiscoverLogItem(Item item)
    {
        // check if the item is already discovered

        // if not, discover it, add it to both the list and the items.dat file
    }
}
