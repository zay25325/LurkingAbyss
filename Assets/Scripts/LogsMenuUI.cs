using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;


//enum ItemType
//{
//    Item,
//    Monster
//}

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
        private string logImagePath { get; set; }

        public LogItem(string Name, string Description, ItemType itemType, bool isDiscovered, string logImagePath)
        {
            this.Name = Name;
            this.Description = Description;
            this.itemType = itemType;
            this.isDiscovered = isDiscovered;
            this.logImagePath = logImagePath;
        }

        public string GetName() { return Name; }
        public string GetDesc() { return Description; }
        public bool IsDiscovered() { return isDiscovered; }
        public ItemType ItemType() { return itemType; }
        public bool SetDiscovery(bool discovered = true) { return isDiscovered = discovered; }
        public string GetImageFilePath() { return logImagePath; }

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
    {// manually logging right now
        try
        {
            if (loggedItems.Count == 0)
            {
                AddItems();
                SaveDiscoveredItems();
            }
            return true;
        }
        catch (Exception error)
        {
            Debug.Log(error.Message);
            return false;
        }
    }

    private void AddItems()
    {
        // Basics
        loggedItems.Add(new LogItem("Gun", "Harms monsters at a distance, makes a lot of noise", ItemType.Item, false, "Assets/Sprites/gun.png"));
        loggedItems.Add(new LogItem("Noise Maker", "Can be placed, makes noise after a timed fuse", ItemType.Item, false, "Assets/Sprites/noise_maker.png"));
        loggedItems.Add(new LogItem("Dash Booster", "Allows the player a quick burst of movement", ItemType.Item, false, "Assets/Sprites/dash_booster.png"));
        loggedItems.Add(new LogItem("Bear Trap", "Holds a monster in place for a time", ItemType.Item, false, "Assets/Sprites/bear_trap.png"));
        loggedItems.Add(new LogItem("Battery", "Takes an item slot, but lets you charge another item", ItemType.Item, false, "Assets/Sprites/battery.png"));
        loggedItems.Add(new LogItem("Rock", "Can be thrown, making noise on-hit or end of flight. Deals minor damage.", ItemType.Item, true, "Assets/Sprites/1_Stone(1).png"));
        loggedItems.Add(new LogItem("C4/Bomb", "Destroys walls, or other static obstructions. Timed fuse, placable", ItemType.Item, false, "Assets/Sprites/c4_bomb.png"));
        loggedItems.Add(new LogItem("Grenade", "Destroys walls, or other static obstructions. Throwable", ItemType.Item, false, "Assets/Sprites/grenade.png"));
        loggedItems.Add(new LogItem("Lamp", "Increases player's passive vision", ItemType.Item, false, "Assets/Sprites/lamp.png"));
        loggedItems.Add(new LogItem("Reflector", "Can reflect a projectile. Charge depletes upon use NOT when reflecting a projectile", ItemType.Item, false, "Assets/Sprites/reflector.png"));

        // Anomalies
        loggedItems.Add(new LogItem("Flamethrower", "Spews flame where the player is looking, can harm multiple enemies", ItemType.Item, false, "Assets/Sprites/flamethrower.png"));
        loggedItems.Add(new LogItem("Horned Helmet", "Gains the ability to charge and destroy walls", ItemType.Item, false, "Assets/Sprites/horned_helmet.png"));
        loggedItems.Add(new LogItem("Revivor", "Upon taking lethal damage, revive without any shield charge", ItemType.Item, false, "Assets/Sprites/revivor.png"));
        loggedItems.Add(new LogItem("Mobile Shield Generator", "Portable version of a generator that can restore shield health to player within a level", ItemType.Item, false, "Assets/Sprites/mobile_shield_generator.png"));
        loggedItems.Add(new LogItem("Warper", "Grants the player to do a short range teleport based on where the player is facing", ItemType.Item, false, "Assets/Sprites/warper.png"));
        loggedItems.Add(new LogItem("Invisible Belt", "Players can briefly go invisible for X amount of seconds. Moves slowly", ItemType.Item, false, "Assets/Sprites/invisible_belt.png"));
        loggedItems.Add(new LogItem("Grappler", "Grappling device to swing the player for quick movement", ItemType.Item, false, "Assets/Sprites/grappler.png"));
        loggedItems.Add(new LogItem("Mimic Helmet", "Transform into an object to hide from monsters. Can not move", ItemType.Item, false, "Assets/Sprites/mimic_helmet.png"));
        loggedItems.Add(new LogItem("Graviton Surge Plate", "When this trap activates it acts as a black hole", ItemType.Item, false, "Assets/Sprites/graviton_surge_plate.png"));
        loggedItems.Add(new LogItem("Stun Plate", "Pressure plate placed on a tile which stuns the monster who stepped on it and those close to its vicinity", ItemType.Item, false, "Assets/Sprites/stun_plate.png"));
        loggedItems.Add(new LogItem("Teleport Plate", "Pressure plate placed on a tile that teleports a target that steps on it", ItemType.Item, false, "Assets/Sprites/teleport_plate.png"));
        loggedItems.Add(new LogItem("Slow Grenade", "Slows enemies", ItemType.Item, false, "Assets/Sprites/slow_grenade.png"));
        loggedItems.Add(new LogItem("Stun Grenade", "Stuns enemies", ItemType.Item, false, "Assets/Sprites/stun_grenade.png"));
        loggedItems.Add(new LogItem("Warp Grenade", "Teleports enemies", ItemType.Item, false, "Assets/Sprites/warp_grenade.png"));

        // Gifts of the Abyss
        loggedItems.Add(new LogItem("Blackened Heart", "The Blackened Heart replaces the protagonist's heart and acts as a fuel source to provide constant low power to the shield attached to the player. Shield health can slowly regenerate over time, but loses ability to refill shield health at generators.", ItemType.Item, false, "Assets/Sprites/blackened_heart.png"));
        loggedItems.Add(new LogItem("Eldritch Eyes", "The Eldritch eyes give the protagonist to see anew. Unlocks a permanent map showcasing monster locations, but the eyes constantly make noise, attracting monster attention to player location.", ItemType.Item, false, "Assets/Sprites/eldritch_eyes.png"));
        loggedItems.Add(new LogItem("Eldritch Horns", "The horns fuse to the protagonist's head and grant them the ability to charge and destroy walls permanently (with cooldown). Not using the charge ability in time causes the monster within to briefly take over and charge at a random location.", ItemType.Item, false, "Assets/Sprites/eldritch_horns.png"));
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
                    string logImagePath = reader.ReadString();

                    // create the item and add to the list
                    LogItem item = new LogItem(name, description, itemType, isDiscovered, logImagePath);
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
                writer.Write(item.GetImageFilePath());
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

            var imageContainer = new VisualElement();
            imageContainer.AddToClassList("image-container");

            var itemImage = new Image();
            itemImage.AddToClassList("item-image");

            if (item.IsDiscovered())
            {
                string imagePath = item.GetImageFilePath();
                if (!string.IsNullOrEmpty(imagePath))
                {
                    Texture2D texture = Resources.Load<Texture2D>(imagePath);
                    if (texture != null)
                    {
                        itemImage.image = texture;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not load image for {item.GetName()} at path: {imagePath}");
                        
                        //itemImage.image = Resources.Load<Texture2D>("DefaultItem");
                    }
                }
            }
            else
            {
                //itemImage.image = Resources.Load<Texture2D>("UnknownItem");
            }
            imageContainer.Add(itemImage);

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
            itemCard.Add(imageContainer);
            itemCard.Add(nameContainer);
            itemCard.Add(descContainer);

            // finally add to the screen
            objectCards.Add(itemCard);
            //Debug.Log($"Added card for item: {item.GetName()}, Type: {item.ItemType()}");
        }
    }

    // attempting front of back contents for cards on items
    //private void DisplayItems()
    //{
    //    var objectCards = rootElement.Q<VisualElement>("Object-Cards");
    //    objectCards.Clear();  // clear existing items or monsters, no duplicates

    //    foreach (var item in loggedItems)
    //    {
    //        if (item.ItemType() != currentItemType)
    //            continue;

    //        // main card container
    //        var itemCard = new VisualElement();
    //        itemCard.AddToClassList("object-card");

    //        // front side of the card
    //        var cardFront = new VisualElement();
    //        cardFront.AddToClassList("card-front");

    //        // attach the info onto the card
    //        var nameContainer = new VisualElement();
    //        nameContainer.AddToClassList("name-container");
    //        var itemName = new Label(item.IsDiscovered() ? item.GetName() : "???");
    //        itemName.AddToClassList("name");
    //        nameContainer.Add(itemName);
    //        cardFront.Add(nameContainer);

    //        // back side of the card
    //        var cardBack = new VisualElement();
    //        cardBack.AddToClassList("card-back");

    //        var descContainer = new VisualElement();
    //        descContainer.AddToClassList("description-container");
    //        var itemDesc = new Label(item.IsDiscovered() ? item.GetDesc() : "???");
    //        itemDesc.AddToClassList("description");
    //        descContainer.Add(itemDesc);
    //        cardBack.Add(descContainer);

    //        // add both sides to the card
    //        itemCard.Add(cardFront);
    //        itemCard.Add(cardBack);

    //        //// Add hover handlers
    //        //itemCard.RegisterCallback<MouseEnterEvent>((evt) => 
    //        //{
    //        //    itemCard.AddToClassList("flipped");
    //        //});

    //        //itemCard.RegisterCallback<MouseLeaveEvent>((evt) => 
    //        //{
    //        //    itemCard.RemoveFromClassList("flipped");
    //        //});

    //        objectCards.Add(itemCard);

    //        // for adding sprites later
    //        //Image itemImage = new();
    //        //Debug.Log($"Added card for item: {item.GetName()}, Type: {item.ItemType()}");
    //    }
    //}

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