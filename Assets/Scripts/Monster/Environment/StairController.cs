using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{
    public void TriggerLevelTransision()
    {
        LevelTransitionManager.Instance.NextLevel();
    }
}
