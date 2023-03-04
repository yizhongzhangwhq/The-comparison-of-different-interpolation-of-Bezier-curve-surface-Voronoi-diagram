using System.Collections.Generic;
using UnityEngine;

public class BezierGUI : MonoBehaviour
{
    [Header("BezierCurve")]
    [SerializeField] private List<Transform> controlPoints;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float Line_Width = 0.1f;
    [SerializeField] private Color Line_Color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [SerializeField] private Color Curve_Color = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    private LineRenderer lineRenderer;
    private LineRenderer curveRenderer;
    private List<GameObject> pointGameObjects = new List<GameObject>();

    private void Start()
    {
        lineRenderer = CreateLineRenderer();
        curveRenderer = CreateLineRenderer();

        foreach (Transform controlPoint in controlPoints)
        {
            GameObject pointGameObject = Instantiate(pointPrefab, controlPoint.position, Quaternion.identity, transform);
            pointGameObject.name = "ControlPoint_" + pointGameObjects.Count;
            pointGameObjects.Add(pointGameObject);
        }
    }

    private void Update()
    {
        List<Vector2> pts = new List<Vector2>();
        foreach (GameObject pointGameObject in pointGameObjects)
        {
            pts.Add(pointGameObject.transform.position);
        }

        UpdateLineRenderer(lineRenderer, pts, Line_Color);
        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        UpdateLineRenderer(curveRenderer, curve, Curve_Color);
    }

    private void OnGUI()
    {
        if (Event.current.isMouse && Event.current.clickCount == 2 && Event.current.button == 0)
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            InsertNewControlPoint(rayPos);
        }
    }

    private LineRenderer CreateLineRenderer()
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = Line_Width;
        lr.endWidth = Line_Width;

        return lr;
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, List<Vector2> points, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; ++i)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    private void InsertNewControlPoint(Vector2 position)
    {
        if (pointGameObjects.Count >= 16)
        {
            Debug.Log("Cannot create any new control points. Max number is 16");
            return;
        }

        GameObject pointGameObject = Instantiate(pointPrefab, position, Quaternion.identity, transform);
        pointGameObject.name = "ControlPoint_" + pointGameObjects.Count;
        pointGameObjects.Add(pointGameObject);
    }
}