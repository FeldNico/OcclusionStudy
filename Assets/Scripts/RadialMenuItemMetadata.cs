using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuItemMetadata
{

    public abstract class IItemType
    {
    }

    public class ColourType : IItemType
    {
        public Color Color;
    }

    public class TextureType : IItemType
    {
        public string MaterialGameObjectName;
    }

    public class ShapeType : IItemType
    {
        public string MeshGameObjectName;
    }

    public static List<RadialMenuItemMetadata> ExampleMenu = new List<RadialMenuItemMetadata>()
    {
        new RadialMenuItemMetadata()
        {
            Name = "Colour",
            Prefab = "ColourPrefab",
            Radius = 0.07f,
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "ColourOrange",
                    Prefab = "ColourOrangePrefab",
                    Type = new ColourType()
                    {
                        Color = new Color(1,165f/255,0)
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ColourGreen",
                    Prefab = "ColourGreenPrefab",
                    Type = new ColourType()
                    {
                        Color = Color.green
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ColourPurple",
                    Prefab = "ColourPurplePrefab",
                    Type = new ColourType()
                    {
                        Color = new Color(128f/256,0,128f/256)
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ColourWhite",
                    Prefab = "ColourWhitePrefab",
                    Type = new ColourType()
                    {
                        Color = Color.white
                    }
                }
            }
        },
        new RadialMenuItemMetadata()
        {
            Name = "Shape",
            Prefab = "ShapePrefab",
            Radius = 0.07f,
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "ShapeCapsule",
                    Prefab = "ShapeCapsulePrefab",
                    Type = new ShapeType()
                    {
                        MeshGameObjectName = "ShapeCapsule"
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ShapeCube",
                    Prefab = "ShapeCubePrefab",
                    Type = new ShapeType()
                    {
                        MeshGameObjectName = "ShapeCube"
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ShapeCylinder",
                    Prefab = "ShapeCylinderPrefab",
                    Type = new ShapeType()
                    {
                        MeshGameObjectName = "ShapeCylinder"
                    }
                }
            }
        },
        new RadialMenuItemMetadata()
        {
            Name = "Confirm",
            Prefab = "CheckmarkPrefab",
        },
        new RadialMenuItemMetadata()
        {
            Name = "Texture",
            Prefab = "TexturePrefab",
            Radius = 0.07f,
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "TextureStar",
                    Prefab = "StarPrefab",
                    Type = new TextureType()
                    {
                        MaterialGameObjectName = "TextureStar"
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "TextureCircle",
                    Prefab = "CirclePrefab",
                    Type = new TextureType()
                    {
                        MaterialGameObjectName = "TextureCircle"
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "TextureLine",
                    Prefab = "LinesPrefab",
                    Type = new TextureType()
                    {
                        MaterialGameObjectName = "TextureLine"
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "TextureRaster",
                    Prefab = "RasterPrefab",
                    Type = new TextureType()
                    {
                        MaterialGameObjectName = "TextureRaster"
                    }
                }
            }
        }
    };

    public string Name;
    public string Prefab;
    public float Radius = -1f;
    public IItemType Type;
    public List<RadialMenuItemMetadata> Children;
}