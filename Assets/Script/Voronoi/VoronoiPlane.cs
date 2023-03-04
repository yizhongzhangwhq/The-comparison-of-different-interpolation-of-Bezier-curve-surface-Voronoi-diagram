using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiPlane : MonoBehaviour
{
    [Header("VoronoiPlane Inspector")]
    [SerializeField] public Vector2Int SpriteSize;
    [SerializeField] public int regionAmount;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0, 0, SpriteSize.x, SpriteSize.y), Vector2.one * 0.5f);
    }
    Texture2D GetDiagram()
    {
        Vector2Int[] centroids = new Vector2Int[regionAmount];
        Color[] Triangles = new Color[regionAmount];
        for (int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, SpriteSize.x), Random.Range(0, SpriteSize.y));
            Triangles[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        Color[] pixelColors = new Color[SpriteSize.x * SpriteSize.y];
        for (int x = 0; x < SpriteSize.x; x++)
        {
            for (int y = 0; y < SpriteSize.y; y++)
            {
                int index = x * SpriteSize.x + y;
                pixelColors[index] = Triangles[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
            }
        }
        return GetImageFromColorArray(pixelColors);
    }

    int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float smallestDst = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
            {
                smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }
    Texture2D GetImageFromColorArray(Color[] pixelColors)
    {
        Texture2D tex = new Texture2D(SpriteSize.x, SpriteSize.y);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixelColors);
        tex.Apply();
        return tex;
    }
}