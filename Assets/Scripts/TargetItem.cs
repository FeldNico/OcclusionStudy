using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TargetItem : MonoBehaviour
{
    private System.Random _random = new System.Random();
    private GameObject _sphereDummy;

    public RadialMenuItemMetadata.ColourType ColourType;
    public RadialMenuItemMetadata.ShapeType ShapeType;
    public RadialMenuItemMetadata.TextureType TextureType;

    private void Start()
    {
        _sphereDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphereDummy.name = "SphereDummy";
        _sphereDummy.GetComponent<Renderer>().enabled = false;
        _sphereDummy.GetComponent<Collider>().enabled = false;

        FindObjectOfType<ResultManager>().OnStart += OnStart;

        RadialMenuItem.OnSelect += HandleType;

        FindObjectOfType<HololensManager>().TriggerMenu += TriggerMenu;
    }

    private void TriggerMenu()
    {
        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
    }

    private void OnStart()
    {
        ColourType = new RadialMenuItemMetadata.ColourType() {Color = Color.cyan};
        ShapeType = new RadialMenuItemMetadata.ShapeType() {MeshGameObjectName = "SphereDummy"};
        TextureType = new RadialMenuItemMetadata.TextureType() {MaterialGameObjectName = "InteractionOrb(Clone)"};

        HandleType(ColourType);
        HandleType(ShapeType);
        HandleType(TextureType);
    }

    private int _index = 0;

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

    private void OnDestroy()
    {
        RadialMenuItem.OnSelect -= HandleType;
        FindObjectOfType<ResultManager>().OnStart -= OnStart;
        FindObjectOfType<HololensManager>().TriggerMenu -= TriggerMenu;

        if (_sphereDummy != null)
        {
            DestroyImmediate(_sphereDummy);
        }
    }
}