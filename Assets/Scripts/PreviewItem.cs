using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PreviewItem : MonoBehaviour
{
    public TMP_Text CountText;

    private System.Random _random = new System.Random();

    public RadialMenuItemMetadata.ColourType ColourType;
    public RadialMenuItemMetadata.ShapeType ShapeType;
    public RadialMenuItemMetadata.CountType CountType;

    private void Start()
    {
        FindObjectOfType<ResultManager>().OnStart += () =>
        {
            ColourType = new RadialMenuItemMetadata.ColourType() {Color = Color.grey};
            ShapeType = new RadialMenuItemMetadata.ShapeType() {MeshGameObjectName = "ShapeSphere"};
            CountType = new RadialMenuItemMetadata.CountType() {Count = 1};
            
            FindObjectOfType<Target>().GenerateTargets(ColourType, ShapeType, CountType);
            RandomizeTypes();
        };
    }

    public void RandomizeTypes()
    {
        var colourList = new List<Color>() {Color.blue, Color.green, Color.yellow, Color.red};
        colourList = colourList.Where(color => color != ColourType.Color).ToList();
        ColourType = new RadialMenuItemMetadata.ColourType()
        {
            Color = colourList[_random.Next(0, colourList.Count)]
        };

        var shapeList = new List<string>() {"ShapeSphere", "ShapeCube", "ShapeCylinder"};
        shapeList = shapeList.Where(s => s != ShapeType.MeshGameObjectName).ToList();
        ShapeType = new RadialMenuItemMetadata.ShapeType()
        {
            MeshGameObjectName = shapeList[_random.Next(0, shapeList.Count)]
        };

        var countList = new List<int>() {1, 3, 5, 10};
        countList = countList.Where(s => s != CountType.Count).ToList();
        CountType = new RadialMenuItemMetadata.CountType()
        {
            Count = countList[_random.Next(0, countList.Count)]
        };

        HandleType(ColourType);
        HandleType(ShapeType);
        HandleType(CountType);
    }

    private void HandleType(RadialMenuItemMetadata.IItemType type)
    {
        switch (type)
        {
            case RadialMenuItemMetadata.ShapeType t:
            {
                ShapeType = (RadialMenuItemMetadata.ShapeType) type;
                GetComponent<MeshFilter>().mesh =
                    GameObject.Find(ShapeType.MeshGameObjectName).GetComponent<MeshFilter>().mesh;
                break;
            }
            case RadialMenuItemMetadata.CountType t:
            {
                CountType = (RadialMenuItemMetadata.CountType) type;
                CountText.text = "x" + CountType.Count;
                break;
            }
            case RadialMenuItemMetadata.ColourType t:
            {
                ColourType = (RadialMenuItemMetadata.ColourType) type;
                GetComponent<Renderer>().material.color = ColourType.Color;
                break;
            }
        }
    }
}