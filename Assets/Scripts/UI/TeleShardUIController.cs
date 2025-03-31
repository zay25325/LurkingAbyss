using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleShardUIController : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] PlayerStats stats;
    [SerializeField] List<Sprite> teleSprites = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        stats.UpdateTeleShardsUI.AddListener(UpdateUI);
    }

    void UpdateUI(int shardCount)
    {
        img.sprite = teleSprites[Mathf.Clamp(shardCount, 0, teleSprites.Count)];
    }
}
