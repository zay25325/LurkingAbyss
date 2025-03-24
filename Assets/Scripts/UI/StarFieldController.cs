using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldController : MonoBehaviour
{
    [SerializeField] float starScale = 2f;
    [SerializeField] float starMoveSpeed = 2f;

    [SerializeField] float cameraEdge = 6f;
    float starFieldEdge { get => 32 * starScale / 2; } // 1024 (sprite size) / 32 (pixels per unit) = 32, /2 as the edge is half of the height

    List<GameObject> starFieldList = new List<GameObject>();
    [SerializeField] GameObject starFieldPrefab;

    // Start is called before the first frame update
    void Start()
    {
        starFieldList.Add(GameObject.Instantiate(starFieldPrefab, transform));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = starFieldList.Count-1; i >= 0; i--)
        {
            starFieldList[i].transform.position -= new Vector3(0, starMoveSpeed, 0) * Time.deltaTime;
            if (starFieldList[i].transform.position.y < -(starFieldEdge + cameraEdge))
            {
                Destroy(starFieldList[i]);
                starFieldList.RemoveAt(i);
            }
        }

        if (starFieldList.Count == 0 || starFieldList[0].transform.position.y < -starFieldEdge + cameraEdge*2)
        {
            starFieldList.Insert(0, GameObject.Instantiate(starFieldPrefab, new Vector3(0, starFieldEdge + cameraEdge * 2, 0), new Quaternion(), transform));
        }
    }
}
