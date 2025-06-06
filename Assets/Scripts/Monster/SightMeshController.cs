#define DebugVisualize // I don't want to have have to tie this to a SerializeField variable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SightMeshController : MonoBehaviour
{
    MeshFilter meshFilter;
    [SerializeField] Light2D light2D;
    [SerializeField] Transform origin;
    [SerializeField] [Range(0, 360)] float fov = 90;
    [SerializeField] [Range(0, 10)] float visionRange = 5;
    [SerializeField] [Range(0,50)] int rayCount = 16;
    [SerializeField] LayerMask visionLayers;
    [SerializeField] PolygonCollider2D polyCollider;
    [SerializeField] Sprite baseSprite;

    Sprite visionSprite = null;

    SortedList<float, Vector3> raycasts = new SortedList<float, Vector3>();

    public float LookDirection { get => lookDirection; set => lookDirection = value; }
    [SerializeField] private float lookDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        light2D.pointLightInnerRadius = 0;
        light2D.pointLightOuterAngle = fov;
        light2D.pointLightOuterRadius = visionRange;
    }

    private void LateUpdate()
    {
        float angle = lookDirection - fov / 2;
        float angleIncrease = fov / rayCount;

        raycasts.Clear();
        for (int i = 0; i <= rayCount; i++)
        {
            Vector2 vectorAngle = GetVectorFromAngle(angle);
#if (DebugVisualize)
            Debug.DrawRay(origin.position, vectorAngle, Color.green);
            Debug.DrawLine(origin.position, origin.position + (Vector3)(vectorAngle * visionRange), Color.green);
#endif
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin.position, vectorAngle, visionRange, visionLayers);
            if (raycastHit2D.collider == null && raycasts.ContainsKey(angle) == false)
            {
                Vector2 rayEndPoint = vectorAngle * visionRange;
                raycasts.Add(angle, rayEndPoint);
                RaycastOpenArea((Vector2)origin.position + rayEndPoint, vectorAngle);
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

#if (DebugVisualize)
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
#endif

        // create mesh
        Vector2[] points = new Vector2[sortedRaycasts.Count + 2];
        points[0] = Vector2.zero;
        for (int i = 0; i < sortedRaycasts.Count; i++)
        {
            points[i + 1] = sortedRaycasts[i];
        }
        points[sortedRaycasts.Count + 1] = Vector2.zero;
        polyCollider.points = points;
        Mesh mesh = polyCollider.CreateMesh(false, false);
        meshFilter.mesh = mesh;

        /*
        * TITLE : �Is it possible to convert meshes to sprites?� function code
        * AUTHOR : kacperszkola365
        * DATE : 2/25/2025
        * AVAILABIILTY : https://discussions.unity.com/t/is-it-possible-to-convert-meshes-to-sprites/198984/3
        */
        if (visionSprite == null)
        {
            Texture2D texture = baseSprite.texture;
            visionSprite = Sprite.Create(
                texture, // texture
                new Rect(0.0f, 0.0f, texture.width, texture.height), // section of texture
                new Vector2(.5f, .5f), // pivot
                baseSprite.pixelsPerUnit, // pixelsPerUnit
                (uint)(visionRange * 2 * baseSprite.pixelsPerUnit) // size (more or less)
                );
            GetComponent<SpriteMask>().sprite = visionSprite;
        }

        var copiedVerticies = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            copiedVerticies[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y);
        }

        for (int i = 0; i < copiedVerticies.Length; ++i)
            copiedVerticies[i] = (copiedVerticies[i] * visionSprite.pixelsPerUnit) + visionSprite.pivot;
        
        var copiedTriangels = new ushort[mesh.triangles.Length];
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            copiedTriangels[i] = (ushort)mesh.triangles[i];
        }
        visionSprite.OverrideGeometry(copiedVerticies, copiedTriangels);

        light2D.transform.localRotation = Quaternion.Euler(0, 0, lookDirection + 180);
    }

    // While we are doing a very similar thing to this video, almost everything needed to be changed causing the only thing actually used are these two methods
    /*
    * TITLE : �FieldOfView.cs� source code
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
        // we want to Floor the point of collision to the bottom corner
        // A value of 5.86 should be floored to 5, however due to the nature of colliders, values like 4.99996 should be floored to 5 and not 4

        float RoundAmount = 1000f; 
        Vector2 bottomLeftCorner = new Vector2(
            Mathf.Floor(Mathf.Round(originalRaycast.point.x * RoundAmount) / RoundAmount),
            Mathf.Floor(Mathf.Round(originalRaycast.point.y * RoundAmount) / RoundAmount)
            );


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

    private void RaycastOpenArea(Vector2 endOfRay, Vector2 rayAngle)
    {
        Vector2 perpendicularAngle = Vector2.Perpendicular(rayAngle);

        RaycastHit2D raycastHit2D = Physics2D.Raycast(endOfRay, perpendicularAngle, LengthBetweenEndPoints, visionLayers);
        if (raycastHit2D.collider != null)
        {
#if (DebugVisualize)
            Debug.DrawRay(endOfRay, perpendicularAngle, Color.cyan);
#endif
            AddAdditionalRaycasts(raycastHit2D.point);
        }

        raycastHit2D = Physics2D.Raycast(endOfRay, -perpendicularAngle, LengthBetweenEndPoints, visionLayers);
        if (raycastHit2D.collider != null)
        {
#if (DebugVisualize)
            Debug.DrawRay(endOfRay, -perpendicularAngle, Color.cyan);
#endif
            AddAdditionalRaycasts(raycastHit2D.point);
        }
    }

    private void AddAdditionalRaycasts(Vector2 raycastTo)
    {
        float startAngle = LookDirection - fov / 2;
        Vector3 relativeDirection = (Vector3)raycastTo - origin.position;
        float angle = GetAngleFromVectorFloat(relativeDirection);

        // convert angle to 0-360 (for example 400 -> 40)
        if (angle > startAngle - fov + 360)
        {
            angle -= 360;
        }

        if (angle > startAngle || angle < startAngle - fov || raycasts.ContainsKey(angle))
        {
            return;
        }



        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin.position, GetVectorFromAngle(angle), visionRange, visionLayers);
        if (raycastHit2D.collider == null)
        {
            raycasts.Add(angle, GetVectorFromAngle(angle) * visionRange);
#if (DebugVisualize)
            //Debug.DrawRay(origin.position, GetVectorFromAngle(angle), Color.yellow);
            Debug.DrawLine(origin.position, origin.position + GetVectorFromAngle(angle) * visionRange, Color.yellow);
#endif
        }
        else
        {
            raycasts.Add(angle, raycastHit2D.point - (Vector2)origin.position);
#if (DebugVisualize)
            //Debug.DrawRay(origin.position, GetVectorFromAngle(angle), Color.yellow);
            Debug.DrawLine(origin.position, raycastHit2D.point, Color.yellow);
#endif
        }
    }

    private float LengthBetweenEndPoints // c^2 = a^2+b^2-2abcos(c), but since a=b -> c^2 = 2a^2 - 2a^2 * cos(c)
    {
        get
        {
            float angle = fov / rayCount; // c
            float twoASquared = 2 * (visionRange * visionRange);
            return Mathf.Sqrt(twoASquared - twoASquared * Mathf.Cos(angle * Mathf.Deg2Rad));
        }
    }
}
