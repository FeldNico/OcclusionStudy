using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Target : MonoBehaviour
{
    private System.Random _random = new System.Random();

    public RadialMenuItemMetadata.ColourType ColourType;
    public RadialMenuItemMetadata.ShapeType ShapeType;
    public RadialMenuItemMetadata.CountType CountType;

    public void Start()
    {
        RadialMenuItem.OnSelect += (type) =>
        {
            SetType(type);
        };
    }

    public void GenerateTargets(RadialMenuItemMetadata.ColourType colorType, RadialMenuItemMetadata.ShapeType shapeType, RadialMenuItemMetadata.CountType countType)
    {
        ColourType =  colorType;
        ShapeType = shapeType;
        CountType = countType;
        
        var count = countType.Count;
        Mesh mesh = GameObject.Find(shapeType.MeshGameObjectName).GetComponent<MeshFilter>().mesh;;
        Color color = colorType.Color;

        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (child != transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject();
            go.transform.localScale = Vector3.one * 0.03f;
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3((float) (_random.NextDouble()*0.5f- 0.25f),
                (float) (_random.NextDouble()*0.5f- 0.25f),
                (float) (_random.NextDouble()*0.5f- 0.25f));
            var filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.material.color = color;
        }
    }

    public void SetType(RadialMenuItemMetadata.IItemType type)
    {
        Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
        Color color = GetComponentInChildren<MeshRenderer>().material.color;

        var children = GetComponentsInChildren<Transform>().Where(transform1 => transform1 != transform).ToList();
        
        switch (type)
        {
            case RadialMenuItemMetadata.ColourType t:
            {
                ColourType = (RadialMenuItemMetadata.ColourType) type;
                foreach (var child in children)
                {
                    child.GetComponent<Renderer>().material.color = ColourType.Color;
                }
                break;
            }
            case RadialMenuItemMetadata.ShapeType t:
            {
                ShapeType = (RadialMenuItemMetadata.ShapeType) type;
                foreach (var child in children)
                {
                    child.GetComponent<MeshFilter>().mesh = GameObject.Find(ShapeType.MeshGameObjectName).GetComponent<MeshFilter>().mesh;
                }
                break;
            }
            case RadialMenuItemMetadata.CountType t:
            {
                CountType = (RadialMenuItemMetadata.CountType) type;

                GenerateTargets(ColourType, ShapeType,
                    new RadialMenuItemMetadata.CountType() {Count = CountType.Count});
                break;
            }
        }
    }
}