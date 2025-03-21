using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndWarpTimer : MonoBehaviour
{
    [SerializeField] GameObject baseObj;
    [SerializeField] GameObject lineObj;
    [SerializeField] GameObject menuObj;
    [SerializeField] float baseStart = 1f;
    [SerializeField] float lineStart = 1.85f;
    [SerializeField] float baseEnd = 6f;
    [SerializeField] float lineEnd = 6.5f;
    [SerializeField] float menuStart = 7.5f;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < baseStart && timer + Time.deltaTime >= baseStart)
        {
            baseObj.SetActive(true);
        }
        if (timer < lineStart && timer + Time.deltaTime >= lineStart)
        {
            lineObj.SetActive(true);
        }
        if (timer < baseEnd && timer + Time.deltaTime >= baseEnd)
        {
            baseObj.SetActive(false);
        }
        if (timer < lineEnd && timer + Time.deltaTime >= lineEnd)
        {
            lineObj.SetActive(false);
        }
        if (timer < menuStart && timer + Time.deltaTime >= menuStart)
        {
            menuObj.SetActive(true);
        }

        timer += Time.deltaTime;
    }
}
