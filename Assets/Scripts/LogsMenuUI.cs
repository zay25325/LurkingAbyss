using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor.Compilation;
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

        public LogItem(string name, string description, ItemType itemType)
        {
            this.Name = name;
            this.Description = description;
            this.isDiscovered = false;
            this.itemType = itemType;
        }
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

        switch (currentItemType)
        {
            case ItemType.Item:
                ShowItems();
                break;
            case ItemType.Monster:
                ShowMonsters();
                break;
        }
        #endregion

        #region Data Saving
        dataFilePath = "logs_data.dat";
        if (!LoadDiscoveredItems())
            Debug.Log("Unable to load discovered items");
        #endregion
    }

    private bool InitializeLogItems()
    {
        try
        {
            // manually logging right now
            loggedItems.Add(new LogItem("Rock", "Small and hard. Mostly useless. May be able to attract attention when thrown.", ItemType.Item));
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


                return true;
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


    private void ShowItems()
    {
        
    }

    private void ShowMonsters()
    {

    }
}
