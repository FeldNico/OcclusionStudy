using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public static void GenerateRandomList(int cloudCount)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        var allColours = new[]
            {new Color(1f, 0.6470588f, 0f), new Color(0f, 1f, 0f), new Color(0.5f, 0f, 0.5f), new Color(1f, 1f, 1f)};
        var allTextures = new[] {"TextureStar", "TextureRaster", "TextureLine", "TextureCircle"};
        var allShapes = new[] {"ShapeCapsule", "ShapeCube", "ShapeCylinder"};

        var list = new List<(Color, string, string, Color, string, string)>();

        var t = (allColours[0], allTextures[0], allTextures[0], allColours[1], allTextures[1], allShapes[1]);

        list.Add(t);

        for (int i = 0; i < 4 * 15; i++)
        {
            var filteredColour = allColours.Where(color => color != t.Item1);
            var cloudColour = filteredColour.ToArray()[Random.Range(0, filteredColour.Count())];
            var filteredTextures = allTextures.Where(s => s != t.Item2);
            var cloudTexture = filteredTextures.ToArray()[Random.Range(0, filteredTextures.Count())];
            var filteredShapes = allShapes.Where(s => s != t.Item3);
            var cloudShape = filteredShapes.ToArray()[Random.Range(0, filteredShapes.Count())];

            filteredColour = allColours.Where(color => color != t.Item4 && color != cloudColour);
            var targetColour = filteredColour.ToArray()[Random.Range(0, filteredColour.Count())];
            filteredTextures = allTextures.Where(s => s != t.Item5 && s != cloudTexture);
            var targetTexture = filteredTextures.ToArray()[Random.Range(0, filteredTextures.Count())];
            filteredShapes = allShapes.Where(s => s != t.Item6 && s != cloudShape);
            var targetShape = filteredShapes.ToArray()[Random.Range(0, filteredShapes.Count())];

            t = (cloudColour, cloudTexture, cloudShape, targetColour, targetTexture, targetShape);

            list.Add(t);
        }

        var indices = Enumerable.Range(0, cloudCount).ToList();
        ChildIndexList.Add(indices[Random.Range(0, cloudCount)]);
        for (int i = 1; i < cloudCount; i++)
        {
            var filteredIndices = indices.Where(index => index != ChildIndexList[i - 1]).ToArray();
            ChildIndexList.Add(filteredIndices[Random.Range(0, filteredIndices.Count())]);
        }

        Debug.Log(String.Join(",", ChildIndexList));

        var output = String.Join(",",
            list.Select((tuple) => "(new Color(" + tuple.Item1.r + "f," + tuple.Item1.g + "f," + tuple.Item1.b +
                                   "f),\"" + tuple.Item2 + "\",\"" + tuple.Item3 + "\", new Color(" +
                                   tuple.Item4.r + "f," + tuple.Item4.g + "f," + tuple.Item4.b + "f),\"" + tuple.Item5 +
                                   "\",\"" + tuple.Item6 + "\")"));

        Debug.Log(output);
    }

    public static List<(Color, string, string, Color, string, string)> AttributesList =
        new List<(Color, string, string, Color, string, string)>()
        {
            (new Color(1f, 0.6470588f, 0f), "TextureStar", "TextureStar", new Color(0f, 1f, 0f), "TextureRaster",
                "ShapeCube"),
            (new Color(1f, 1f, 1f), "TextureLine", "ShapeCylinder", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCube", new Color(0f, 1f, 0f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(1f, 1f, 1f), "TextureStar", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", new Color(0f, 1f, 0f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", new Color(1f, 1f, 1f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCube", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCapsule"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", new Color(1f, 0.6470588f, 0f),
                "TextureCircle", "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", new Color(1f, 1f, 1f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", new Color(0.5f, 0f, 0.5f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCylinder"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureLine",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", new Color(1f, 1f, 1f), "TextureStar",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", new Color(0f, 1f, 0f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", new Color(1f, 1f, 1f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureRaster",
                "ShapeCapsule"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCapsule"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", new Color(0.5f, 0f, 0.5f), "TextureLine",
                "ShapeCube"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", new Color(1f, 1f, 1f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCylinder", new Color(0.5f, 0f, 0.5f),
                "TextureStar", "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", new Color(0f, 1f, 0f), "TextureRaster",
                "ShapeCube"),
            (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureLine",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", new Color(1f, 1f, 1f), "TextureRaster",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCube", new Color(0f, 1f, 0f), "TextureCircle",
                "ShapeCylinder"),
            (new Color(0f, 1f, 0f), "TextureLine", "ShapeCylinder", new Color(1f, 1f, 1f), "TextureStar",
                "ShapeCapsule"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureCircle",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", new Color(0f, 1f, 0f), "TextureRaster",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCube", new Color(1f, 1f, 1f), "TextureStar",
                "ShapeCylinder"),
            (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCube", new Color(1f, 1f, 1f), "TextureLine",
                "ShapeCapsule"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureStar",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", new Color(1f, 1f, 1f), "TextureLine",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureRaster",
                "ShapeCube"),
            (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCylinder", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCapsule"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureLine",
                "ShapeCylinder"),
            (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCapsule", new Color(0f, 1f, 0f), "TextureStar",
                "ShapeCube"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureCircle",
                "ShapeCylinder"),
            (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureLine",
                "ShapeCube"),
            (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureStar",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCube"),
            (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureLine",
                "ShapeCapsule"),
            (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", new Color(1f, 1f, 1f), "TextureRaster",
                "ShapeCylinder"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", new Color(0.5f, 0f, 0.5f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", new Color(1f, 1f, 1f), "TextureLine",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureStar",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureRaster",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureRaster",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", new Color(0f, 1f, 0f), "TextureCircle",
                "ShapeCapsule"),
            (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", new Color(1f, 0.6470588f, 0f), "TextureRaster",
                "ShapeCube"),
            (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", new Color(0f, 1f, 0f), "TextureLine",
                "ShapeCapsule"),
            (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", new Color(1f, 0.6470588f, 0f), "TextureCircle",
                "ShapeCylinder"),
            (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", new Color(0.5f, 0f, 0.5f), "TextureStar",
                "ShapeCube")
        };

    public static List<int> ChildIndexList = new List<int>()
    {
        89, 82, 78, 70, 66, 26, 93, 9, 43, 56, 5, 60, 82, 6, 68, 7, 78, 52, 8, 25, 10, 32, 58, 64, 54, 82, 94, 97, 46,
        27, 93, 6, 11, 87, 38, 7, 25, 61, 22, 41, 15, 0, 22, 75, 23, 8, 32, 92, 85, 18, 16, 12, 4, 78, 96, 87, 54, 24,
        56, 48, 26, 8, 67, 71, 67, 43, 82, 9, 81, 86, 58, 76, 61, 34, 84, 51, 6, 36, 42, 30, 77, 16, 81, 85, 35, 74, 93,
        38, 61, 27, 96, 36, 61, 46, 47, 68, 45, 19, 47, 83
    };

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
                        Color = new Color(1, 165f / 255, 0)
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
                        Color = new Color(128f / 256, 0, 128f / 256)
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