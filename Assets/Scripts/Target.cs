using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Target : MonoBehaviour
{
    private System.Random _random = new System.Random();

    public int TargetCount = 30;
    
    public RadialMenuItemMetadata.ColourType ColourType;
    public RadialMenuItemMetadata.ShapeType ShapeType;
    public RadialMenuItemMetadata.TextureType TextureType;

    public void Start()
    {
        RadialMenuItem.OnSelect += SetType;
    }

    public void GenerateTargets(RadialMenuItemMetadata.ColourType colorType, RadialMenuItemMetadata.ShapeType shapeType, RadialMenuItemMetadata.TextureType textureType)
    {
        ColourType =  colorType;
        ShapeType = shapeType;
        TextureType = textureType;

        var material = GameObject.Find(textureType.MaterialGameObjectName).GetComponent<Renderer>().material;
        Mesh mesh = GameObject.Find(shapeType.MeshGameObjectName).GetComponent<MeshFilter>().mesh;;
        Color color = colorType.Color;

        if (transform.childCount == 0)
        {
            for (int i = 0; i < TargetCount; i++)
            {
                var go = new GameObject();
                go.transform.localScale = Vector3.one * 0.03f;
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3((float) (_random.NextDouble()*0.5f- 0.25f),
                    (float) (_random.NextDouble()*0.5f- 0.25f),
                    (float) (_random.NextDouble()*0.5f- 0.25f));
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
            }
        }
        foreach (var child in GetComponentsInChildren<Renderer>())
        {
            child.GetComponent<MeshFilter>().mesh = mesh;
            child.material = material;
            child.material.color = color;
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
            case RadialMenuItemMetadata.TextureType t:
            {
                TextureType = (RadialMenuItemMetadata.TextureType) type;
                foreach (var child in children)
                {
                    child.GetComponent<Renderer>().material = GameObject.Find(TextureType.MaterialGameObjectName).GetComponent<Renderer>().material;
                    child.GetComponent<Renderer>().material.color = ColourType.Color;
                }
                break;
            }
        }
    }

    private void OnDestroy()
    {
        RadialMenuItem.OnSelect -= SetType;
    }
}