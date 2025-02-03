using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;


enum ItemType
{
    Item,
    Monster
}


public class LogsMenuUI : MonoBehaviour
{
    private class LogItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool isDiscovered { get; set; }
        public ItemType itemType { get; set; }

        private LogItem(string name, string description, ItemType itemType)
        {
            this.Name = name;
            this.Description = description;
            this.isDiscovered = false;
            this.itemType = itemType;
        }
    }

    // list so we can store the info of the logged items
    private List<LogItem> loggedItems = new();

    // Visual Unity UI Toolkit variabels
    private VisualElement rootElement;
    private VisualElement container;


    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        var BackButton = rootElement.Q<Button>("BackBtn");
        BackButton.clicked += () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        };

        container = rootElement.Q<VisualElement>("Logs-Container");



        InitializeLogItems();
    }

    private bool InitializeLogItems()
    {
        try
        {
            // log items here
            return true;
        }
        catch (System.Exception error)
        {
            Debug.Log(error.Message);
            return false;
        }
    }


    private void ShowDescription()
    {
        
    }
}
