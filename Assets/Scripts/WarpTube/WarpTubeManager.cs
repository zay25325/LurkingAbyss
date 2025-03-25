using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTubeManager : MonoBehaviour
{
    public static WarpTubeManager Instance;
    private Dictionary<int, List<WarpTubeController>> tubeGroups = new Dictionary<int, List<WarpTubeController>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterTube(WarpTubeController tube)
    {
        int id = tube.tubeID;

        if (!tubeGroups.ContainsKey(id))
        {
            tubeGroups[id] = new List<WarpTubeController>();
        }

        tubeGroups[id].Add(tube);

        // If we have exactly two tubes with the same ID, pair them
        if (tubeGroups[id].Count == 2)
        {
            tubeGroups[id][0].pairedTube = tubeGroups[id][1];
            tubeGroups[id][1].pairedTube = tubeGroups[id][0];
        }
    }
}
