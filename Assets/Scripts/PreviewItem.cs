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
    public RadialMenuItemMetadata.TextureType TextureType;

    private void Start()
    {
        var sphereDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereDummy.name = "SphereDummy";
        sphereDummy.GetComponent<Renderer>().enabled = false;
        sphereDummy.GetComponent<Collider>().enabled = false;
        
        FindObjectOfType<ResultManager>().OnStart += () =>
        {
            ColourType = new RadialMenuItemMetadata.ColourType() {Color = Color.cyan};
            ShapeType = new RadialMenuItemMetadata.ShapeType() {MeshGameObjectName = "SphereDummy"};
            TextureType = new RadialMenuItemMetadata.TextureType() {MaterialGameObjectName = "InteractionOrb"};
            
            FindObjectOfType<Target>().GenerateTargets(ColourType, ShapeType, TextureType);
            RandomizeTypes();
        };
    }

    public void RandomizeTypes()
    {
        var colourList = new List<Color>() {new Color(1,165f/255,0), Color.green,  new Color(128f/256,0,128f/256), Color.white};
        colourList = colourList.Where(color => color != ColourType.Color).ToList();
        ColourType = new RadialMenuItemMetadata.ColourType()
        {
            Color = colourList[_random.Next(0, colourList.Count)]
        };

        var shapeList = new List<string>() {"ShapeCapsule", "ShapeCube", "ShapeCylinder"};
        shapeList = shapeList.Where(s => s != ShapeType.MeshGameObjectName).ToList();
        ShapeType = new RadialMenuItemMetadata.ShapeType()
        {
            MeshGameObjectName = shapeList[_random.Next(0, shapeList.Count)]
        };

        var countList = new List<string>() {"TextureStar","TextureCircle","TextureLine","TextureRaster"};
        countList = countList.Where(s => s != TextureType.MaterialGameObjectName).ToList();
        TextureType = new RadialMenuItemMetadata.TextureType()
        {
            MaterialGameObjectName = countList[_random.Next(0, countList.Count)]
        };

        HandleType(ColourType);
        HandleType(ShapeType);
        HandleType(TextureType);
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
            case RadialMenuItemMetadata.TextureType t:
            {
                TextureType = (RadialMenuItemMetadata.TextureType) type;
                var color = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material = GameObject.Find(TextureType.MaterialGameObjectName)
                    .GetComponent<Renderer>().material;
                GetComponent<Renderer>().material.color = color;
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