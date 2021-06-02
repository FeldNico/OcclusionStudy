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

    public int ChildCount = 100;
    
    private static int _attributeIndex = 0;
    private List<int> _indicesList;
    
    public void Start()
    {

        RadialMenuItemMetadata.GenerateRandomList(ChildCount);
        
        FindObjectOfType<ResultManager>().OnStart += RandomizeTypes;

        _indicesList = new List<int>(RadialMenuItemMetadata.ChildIndexList);

        if (transform.childCount == 0)
        {
            //var phi = Mathf.PI * (3f - Mathf.Sqrt(5));
            
            for (int i = 0; i < ChildCount; i++)
            {
                var x = 0.5f * Mathf.Cos(2f * i * Mathf.PI / ChildCount);
                var z = 0.5f * Mathf.Sin(2f * i * Mathf.PI / ChildCount);
                var pos = Vector3.forward * x + Vector3.right * z;
                pos.y = (i % 10 * 2f) / 10 - 0.9f;
                /*
                var y = 1f -  i / (ChildCount - 1f) * 2f;
                var radius = Mathf.Sqrt(1 - y * y);

                var theta = phi * i;

                var x = Mathf.Cos(theta) * radius;
                var z = Mathf.Sin(theta) * radius;
                */
                
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = transform;
                child.transform.localPosition = pos;
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * (1f/transform.localScale.x);
            }
        }
    }
    
    public void RandomizeTypes()
    {
        if (FindObjectOfType<ResultManager>().IsIntroduction)
        {
            var tuple = RadialMenuItemMetadata.AttributesList[Random.Range(0,RadialMenuItemMetadata.AttributesList.Count)];
            ApplyTuple(tuple,Random.Range(0,ChildCount));
        }
        else
        {
            var tuple = RadialMenuItemMetadata.AttributesList[_attributeIndex++ % RadialMenuItemMetadata.AttributesList.Count];
            var index = _indicesList[Random.Range(0, _indicesList.Count)];
            _indicesList.Remove(index);
            ApplyTuple(tuple,index);
        }
    }
    
    public void ApplyTuple((Color, string, string, Color, string, string) tuple, int childIndex)
    {
        ColourType = new RadialMenuItemMetadata.ColourType()
        {
            Color = tuple.Item4,
        };
        TextureType = new RadialMenuItemMetadata.TextureType()
        {
            MaterialGameObjectName = tuple.Item5
        };
        ShapeType = new RadialMenuItemMetadata.ShapeType()
        {
            MeshGameObjectName = tuple.Item6
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

        var targetChild = transform.GetChild(childIndex);
        targetChild.GetComponent<Renderer>().material = GameObject.Find(tuple.Item5).GetComponent<Renderer>().material;;
        targetChild.GetComponent<Renderer>().material.color = tuple.Item4;
        targetChild.GetComponent<MeshFilter>().mesh = GameObject.Find(tuple.Item6).GetComponent<MeshFilter>().mesh;
    }

    private void OnDestroy()
    {
        FindObjectOfType<ResultManager>().OnStart -= RandomizeTypes;
    }
}