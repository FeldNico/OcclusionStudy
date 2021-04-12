using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuItemMetadata
{

    public interface IItemType
    {
    }

    public class ColourType : IItemType
    {
        public Color Color;
    }

    public class CountType : IItemType
    {
        public int Count;
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
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "ColourRed",
                    Prefab = "ColourRedPrefab",
                    Type = new ColourType()
                    {
                        Color = Color.red
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
                    Name = "ColourBlue",
                    Prefab = "ColourBluePrefab",
                    Type = new ColourType()
                    {
                        Color = Color.blue
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "ColourYellow",
                    Prefab = "ColourYellowPrefab",
                    Type = new ColourType()
                    {
                        Color = Color.yellow
                    }
                }
            }
        },
        new RadialMenuItemMetadata()
        {
            Name = "Shape",
            Prefab = "ShapePrefab",
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "ShapeSphere",
                    Prefab = "ShapeSpherePrefab",
                    Type = new ShapeType()
                    {
                        MeshGameObjectName = "ShapeSphere"
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
            Name = "Count",
            Prefab = "CountPrefab",
            Children = new List<RadialMenuItemMetadata>()
            {
                new RadialMenuItemMetadata()
                {
                    Name = "CountOne",
                    Prefab = "CountOnePrefab",
                    Type = new CountType()
                    {
                        Count = 1
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "CountThree",
                    Prefab = "CountThreePrefab",
                    Type = new CountType()
                    {
                        Count = 3
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "CountFive",
                    Prefab = "CountFivePrefab",
                    Type = new CountType()
                    {
                        Count = 5
                    }
                },
                new RadialMenuItemMetadata()
                {
                    Name = "CountTen",
                    Prefab = "CountTenPrefab",
                    Type = new CountType()
                    {
                        Count = 10
                    }
                }
            }
        }
    };

    public string Name;
    public string Prefab;
    public IItemType Type;
    public List<RadialMenuItemMetadata> Children;
}