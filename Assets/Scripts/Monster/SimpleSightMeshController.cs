using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SimpleSightMeshController : MonoBehaviour
{
    MeshFilter meshFilter;
    [SerializeField] Light2D light2D;
    [SerializeField] Transform origin;
    [SerializeField] [Range(0, 360)] float fov = 90;
    [SerializeField] [Range(0, 10)] float visionRange = 5;
    [SerializeField] [Range(0, 50)] int rayCount = 5;
    [SerializeField] LayerMask visionLayers;
    [SerializeField] PolygonCollider2D polyCollider;

    SortedList<float, Vector3> raycasts = new SortedList<float, Vector3>();

    public float LookDirection { get => lookDirection; set => lookDirection = value; }
    [SerializeField] private float lookDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        light2D.pointLightInnerRadius = 0;
        light2D.pointLightOuterAngle = fov;
        light2D.pointLightOuterRadius = visionRange;

        meshFilter = GetComponent<MeshFilter>();
        float angle = fov / 2;
        float angleIncrease = fov / rayCount;

        raycasts.Clear();
        for (int i = 0; i <= rayCount; i++)
        {
            Vector2 vectorAngle = GetVectorFromAngle(angle);
            Vector2 rayEndPoint = vectorAngle * visionRange;
            raycasts.Add(angle, rayEndPoint);

            angle -= angleIncrease;
        }

        List<Vector3> sortedRaycasts = new List<Vector3>(raycasts.Values);
        sortedRaycasts.Reverse();

        // create mesh
        Vector2[] points = new Vector2[sortedRaycasts.Count + 2];
        points[0] = Vector2.zero;
        for (int i = 0; i < sortedRaycasts.Count; i++)
        {
            points[i + 1] = sortedRaycasts[i];
        }
        points[sortedRaycasts.Count + 1] = Vector2.zero;
        polyCollider.points = points;
        meshFilter.mesh = polyCollider.CreateMesh(false, false);
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, 0, lookDirection-90);
    }

    // While we are doing a very similar thing to this video, almost everything needed to be changed causing the only thing actually used are these two methods
    /*
    * TITLE : “FieldOfView.cs” source code
    * AUTHOR : Code Monkey
    * DATE : 1/22/2025
    * AVAILABIILTY : https://www.youtube.com/watch?v=CSeUMTaNFYk
    */
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }
    // End Citation
}
