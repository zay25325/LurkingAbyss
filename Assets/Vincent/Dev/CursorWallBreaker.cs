using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorWallBreaker : MonoBehaviour
{
    [SerializeField] LevelController levelManager;
    [SerializeField] Camera view;

    private Mouse mouse;

    private Vector3 cursorpos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        this.mouse = Mouse.current;
        if(Input.GetMouseButtonDown(0)) {
            this.OnClick();
        }
    }

    void OnClick() {
        cursorpos = view.ScreenToWorldPoint(Input.mousePosition);
        //cursorpos += this.transform.position;
        // do something
        var result = levelManager.BreakTileAt(cursorpos);
        Debug.Log($"Cursor clicked at {cursorpos}! {Input.mousePosition}");
        Debug.Log($"Broke Tile: {result}");
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0f,0f,1f,0.5f);
        Gizmos.DrawWireSphere(cursorpos,1);
    }
}
