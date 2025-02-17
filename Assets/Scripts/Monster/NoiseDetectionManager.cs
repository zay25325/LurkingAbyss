using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EntityInfo;

public class NoiseDetectionManager
{
    // Singleton pattern
    static NoiseDetectionManager instance;
    public static NoiseDetectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NoiseDetectionManager();
            }
            return instance;
        }
    }

    public UnityEvent<Vector2, float, List<EntityTags>> NoiseEvent = new UnityEvent<Vector2, float, List<EntityTags>>();
}
