using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    public RadialMenuItemMetadata.ColourType ColourType;
    public RadialMenuItemMetadata.ShapeType ShapeType;
    public RadialMenuItemMetadata.TextureType TextureType;

    public int ChildCount = 30;
    
    public void Start()
    {

        RadialMenuItemMetadata.GenerateRandomList(ChildCount);
        
        FindObjectOfType<ResultManager>().OnStart += RandomizeTypes;
        
        if (transform.childCount == 0)
        {
            var phi = Mathf.PI * (3f - Mathf.Sqrt(5));
            
            for (int i = 0; i < ChildCount; i++)
            {
                var y = 1f -  i / (ChildCount - 1f) * 2f;
                var radius = Mathf.Sqrt(1 - y * y);

                var theta = phi * i;

                var x = Mathf.Cos(theta) * radius;
                var z = Mathf.Sin(theta) * radius;
                
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = transform;
                child.transform.localPosition =
                    new Vector3(x,y,z) * 0.51f;
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * (1f/transform.localScale.x);
            }
        }
    }

    private static int _index = 0;
    public void RandomizeTypes()
    {
        if (FindObjectOfType<ResultManager>().IsIntroduction)
        {
            var tuple = RadialMenuItemMetadata.AttributesList[Random.Range(0,RadialMenuItemMetadata.AttributesList.Count)];
            ApplyTuple(tuple);
        }
        else
        {
            var tuple = RadialMenuItemMetadata.AttributesList[_index++ % RadialMenuItemMetadata.AttributesList.Count];
            ApplyTuple(tuple);
        }
    }
    
    public void ApplyTuple((Color, string, string, int, Color, string, string) tuple)
    {
        ColourType = new RadialMenuItemMetadata.ColourType()
        {
            Color = tuple.Item5,
        };
        TextureType = new RadialMenuItemMetadata.TextureType()
        {
            MaterialGameObjectName = tuple.Item6
        };
        ShapeType = new RadialMenuItemMetadata.ShapeType()
        {
            MeshGameObjectName = tuple.Item7
        };

        Color color = tuple.Item1;
        var material = GameObject.Find(tuple.Item2).GetComponent<Renderer>().material;
        Mesh mesh = GameObject.Find(tuple.Item3).GetComponent<MeshFilter>().mesh;
        
        foreach (var child in GetComponentsInChildren<Renderer>())
        {
            if (child.transform != transform)
            {
                child.material = material;
                child.material.color = color;
                child.GetComponent<MeshFilter>().mesh = mesh;
            }
        }

        var targetChild = transform.GetChild(tuple.Item4);
        targetChild.GetComponent<Renderer>().material = GameObject.Find(tuple.Item6).GetComponent<Renderer>().material;;
        targetChild.GetComponent<Renderer>().material.color = tuple.Item5;
        targetChild.GetComponent<MeshFilter>().mesh = GameObject.Find(tuple.Item7).GetComponent<MeshFilter>().mesh;
    }

    private void OnDestroy()
    {
        FindObjectOfType<ResultManager>().OnStart -= RandomizeTypes;
    }
}