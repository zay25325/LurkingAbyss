using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// spawns gizmo circles wherever noises happen
// TODO: add tag filtering for visualized noises

public class NoiseVisualizerController : MonoBehaviour
{
    [SerializeField] GameObject noiseCirclePrefab;
    [SerializeField] bool ShowNoises = true;
    private bool ShowingNoises = false;

    private void CreateNoiseCircle(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags) {
        var obj = Instantiate(noiseCirclePrefab, new Vector3(pos.x,pos.y,0), Quaternion.identity);
        var con = obj.GetComponent<NoiseVisualizer>();
        con.volume = volume;
        con.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.ShowNoises == true) {
            this.StartShowingNoises();
        } else {
            this.StopShowingNoises();
        }
    }

    private void StartShowingNoises() {
        if(this.ShowingNoises == false) {
            NoiseDetectionManager.Instance.NoiseEvent.AddListener(CreateNoiseCircle);
            this.ShowingNoises = true;
        }
    }

    private void StopShowingNoises() {
        if(this.ShowingNoises == true) {
            NoiseDetectionManager.Instance.NoiseEvent.RemoveListener(CreateNoiseCircle);
            this.ShowingNoises = false;
        }
    }
}
