using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SightMeshController : MonoBehaviour
{
    MeshFilter meshFilter;
    [SerializeField] Transform origin;
    [SerializeField] [Range(0, 360)] float fov = 90;
    [SerializeField] [Range(0, 10)] float visionRange = 5;
    [SerializeField] [Range(0,50)] int rayCount = 16;
    [SerializeField] LayerMask visionLayers;
    [SerializeField] PolygonCollider2D polyCollider;

    SortedList<float, Vector3> raycasts = new SortedList<float, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        transform.parent = null;
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0,0,1);
    }

    private void LateUpdate()
    {
        float angle = GetAngleFromVectorFloat(origin.up) - fov / 2;
        float angleIncrease = fov / rayCount;

        raycasts.Clear();
        for (int i = 0; i <= rayCount; i++)
        {
            Debug.DrawRay(origin.position, GetVectorFromAngle(angle), Color.green);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin.position, GetVectorFromAngle(angle), visionRange, visionLayers);
            if (raycastHit2D.collider == null)
            {
                raycasts.Add(angle, GetVectorFromAngle(angle) * visionRange);
            }
            else
            {
                if (raycasts.ContainsKey(angle) == false)
                {
                    raycasts.Add(angle, raycastHit2D.point - (Vector2)origin.position);
                    if (raycastHit2D.collider.tag == "Walls")
                    {
                        RaycastTilemapCorners(raycastHit2D);
                    }
                }
            }

            angle -= angleIncrease;
        }

        List<Vector3> sortedRaycasts = new List<Vector3>(raycasts.Values);
        sortedRaycasts.Reverse();

        for (int i = 0; i < sortedRaycasts.Count; i++)
        {
            if (i == 0)
            {
                Debug.DrawLine(origin.position, sortedRaycasts[i] + origin.position, Color.blue);
            }
            else if (i == sortedRaycasts.Count-1)
            {
                Debug.DrawLine(sortedRaycasts[i] + origin.position, origin.position, Color.blue);
            }
            else
            {
                Debug.DrawLine(sortedRaycasts[i] + origin.position, sortedRaycasts[i+1] + origin.position, Color.blue);
            }
        }


        // create mesh
        Vector2[] points = new Vector2[sortedRaycasts.Count + 2];
        points[0] = Vector2.zero;
        for (int i = 0; i < sortedRaycasts.Count; i++)
        {
            points[i + 1] = sortedRaycasts[i];
        }
        points[sortedRaycasts.Count + 1] = Vector2.zero;
        polyCollider.points = points;
        polyCollider.offset = origin.position;
        meshFilter.mesh = polyCollider.CreateMesh(false, false);
    }

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

    private void RaycastTilemapCorners(RaycastHit2D originalRaycast)
    {
        Vector2 bottomLeftCorner = new Vector2(Mathf.Floor(originalRaycast.point.x), Mathf.Floor(originalRaycast.point.y));
        float cornerOffset = 0.01f; // covering corner (1,1) we want to send out 2 raycasts to (1.01,1) (0.99,1)
        // since we are always using position relative to the bottom left corner, we do not care if the normal is positive or negative, only if we hit the side or the top/bottom
        Vector2 absNormal = new Vector2(Mathf.Abs(originalRaycast.normal.x), Mathf.Abs(originalRaycast.normal.y));

        // bottom left corner
        AddAdditionalRaycasts(bottomLeftCorner - new Vector2(absNormal.y, absNormal.x) * cornerOffset);
        AddAdditionalRaycasts(bottomLeftCorner + new Vector2(absNormal.y, absNormal.x) * cornerOffset);

        // opposite corner
        AddAdditionalRaycasts(bottomLeftCorner + new Vector2(absNormal.y, absNormal.x) * (1 - cornerOffset));
        AddAdditionalRaycasts(bottomLeftCorner + new Vector2(absNormal.y, absNormal.x) * (1 + cornerOffset));
    }

    private void AddAdditionalRaycasts(Vector2 raycastTo)
    {
        Vector3 relativeDirection = (Vector3)raycastTo - origin.position;
        float angle = GetAngleFromVectorFloat(relativeDirection);
        float startAngle = GetAngleFromVectorFloat(origin.up) - fov / 2;

        if (angle > startAngle - fov + 360)
        {
            angle -= 360;
        }

        if (angle > startAngle || angle < startAngle - fov || raycasts.ContainsKey(angle))
        {
            return;
        }

        Debug.DrawRay(origin.position, GetVectorFromAngle(angle), Color.yellow);

        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin.position, GetVectorFromAngle(angle), visionRange, visionLayers);
        if (raycastHit2D.collider == null)
        {
            raycasts.Add(angle, GetVectorFromAngle(angle) * visionRange);
        }
        else
        {
            raycasts.Add(angle, raycastHit2D.point - (Vector2)origin.position);
        }
    }
}
