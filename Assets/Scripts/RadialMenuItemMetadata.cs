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

        var list = new List<(Color, string, string, int, Color, string, string)>();

        var t = (allColours[0], allTextures[0], allTextures[0], 5, allColours[1], allTextures[1], allShapes[1]);

        list.Add(t);

        for (int i = 0; i < 4 * 4 * 15; i++)
        {
            var filteredColour = allColours.Where(color => color != t.Item1);
            var cloudColour = filteredColour.ToArray()[Random.Range(0, filteredColour.Count())];
            var filteredTextures = allTextures.Where(s => s != t.Item2);
            var cloudTexture = filteredTextures.ToArray()[Random.Range(0, filteredTextures.Count())];
            var filteredShapes = allShapes.Where(s => s != t.Item3);
            var cloudShape = filteredShapes.ToArray()[Random.Range(0, filteredShapes.Count())];

            filteredColour = allColours.Where(color => color != t.Item5 && color != cloudColour);
            var targetColour = filteredColour.ToArray()[Random.Range(0, filteredColour.Count())];
            filteredTextures = allTextures.Where(s => s != t.Item6 && s != cloudTexture);
            var targetTexture = filteredTextures.ToArray()[Random.Range(0, filteredTextures.Count())];
            filteredShapes = allShapes.Where(s => s != t.Item7 && s != cloudShape);
            var targetShape = filteredShapes.ToArray()[Random.Range(0, filteredShapes.Count())];

            var indices = Enumerable.Range(0, cloudCount).Where(index => index != t.Item4);
            var childIndex = indices.ToArray()[Random.Range(0, indices.Count())];

            t = (cloudColour, cloudTexture, cloudShape, childIndex, targetColour, targetTexture, targetShape);

            list.Add(t);
        }

        var output = String.Join(",",
            list.Select((tuple) => "(new Color(" + tuple.Item1.r + "f," + tuple.Item1.g + "f," + tuple.Item1.b +
                                   "f),\"" + tuple.Item2 + "\",\"" + tuple.Item3 + "\"," + tuple.Item4 + ",new Color(" +
                                   tuple.Item5.r + "f," + tuple.Item5.g + "f," + tuple.Item5.b + "f),\"" + tuple.Item6 +
                                   "\",\"" + tuple.Item7 + "\")"));
        Debug.Log(output);
    }

    public static List<(Color, string, string,int,Color,string,string)> AttributesList = new List<(Color, string, string,int,Color,string,string)>()
    {
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "TextureStar", 5, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", 19, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCapsule", 28, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCube", 18, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCapsule", 11, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 9, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 29, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCylinder", 28, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 19, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", 5, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 13, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", 3, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 7, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 8, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 22, new Color(1f, 1f, 1f), "TextureStar", "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 1, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 12, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", 28, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCube", 14, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", 8, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 3, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 20, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCapsule", 25, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 12, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", 14, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 7, new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 28, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 8, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCylinder", 5, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 15, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", 18, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCapsule", 13, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", 18, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder", 28, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 27, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCylinder", 10, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCapsule", 16, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 4, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCapsule", 24, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCylinder", 26, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 15, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCylinder", 3, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", 15, new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 14, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 6, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCube", 7, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", 9, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCube", 13, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCapsule", 6, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 14, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 7, new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 8, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 10, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 27, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 23, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 6, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", 5, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", 15, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", 26, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 9, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCylinder", 21, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", 29, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 17, new Color(0f, 1f, 0f), "TextureLine", "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 9, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", 14, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 23, new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", 8, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 13, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", 27, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder", 0, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 11, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 24, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCapsule", 9, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 14, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 9, new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 10, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", 7, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", 24, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 29, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 19, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCube", 27, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCapsule", 25, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 6, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCube", 3, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 10, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 23, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", 19, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCapsule", 9, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 18, new Color(1f, 1f, 1f), "TextureCircle", "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 1, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", 16, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 0, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 24, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", 15, new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCapsule", 10, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 14, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCylinder", 11, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 17, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCylinder", 15, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCapsule", 6, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 27, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCapsule", 9, new Color(0f, 1f, 0f), "TextureLine", "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder", 6, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 13, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCylinder", 4, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", 27, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCube", 20, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 1, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", 5, new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder", 9, new Color(1f, 1f, 1f), "TextureLine", "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 0, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 17, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 12, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 26, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", 19, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 20, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCylinder", 21, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 3, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 12, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 28, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 14, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 6, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", 17, new Color(0f, 1f, 0f), "TextureStar", "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder", 4, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", 12, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 20, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 17, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 8, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCube", 1, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCapsule", 26, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 5, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 25, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCylinder", 27, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 8, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 16, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 2, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCapsule", 13, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 29, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCylinder", 5, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 25, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 4, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", 13, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 1, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", 22, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCylinder", 9, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 5, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 19, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 9, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 12, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCube", 25, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCylinder", 18, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCapsule", 19, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCylinder", 23, new Color(0f, 1f, 0f), "TextureStar", "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 13, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 25, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 13, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCube", 0, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 18, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 24, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 26, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 9, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", 12, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 23, new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCylinder", 4, new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", 17, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", 6, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", 22, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCapsule", 7, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 6, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCylinder", 24, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 11, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCylinder", 17, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 3, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCylinder", 1, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", 17, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 12, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 17, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureRaster", "ShapeCube", 2, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 26, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 4, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCube", 28, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCapsule", 20, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 29, new Color(0.5f, 0f, 0.5f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 19, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCylinder", 22, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", 4, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", 20, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCube", 3, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCapsule", 18, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", 20, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", 23, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 25, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", 8, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 14, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCube", 20, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureRaster", "ShapeCylinder", 3, new Color(1f, 1f, 1f), "TextureLine", "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 20, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCylinder", 22, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCube", 2, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCylinder", 24, new Color(1f, 1f, 1f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 20, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 16, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 24, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCapsule", 2, new Color(1f, 1f, 1f), "TextureLine", "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCube", 24, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 16, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 12, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCapsule", 11, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 0, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", 13, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 11, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 12, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 17, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 10, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCapsule", 17, new Color(1f, 1f, 1f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCylinder", 4, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 29, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCube", 11, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCylinder", 22, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 5, new Color(1f, 0.6470588f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureLine", "ShapeCylinder", 19, new Color(0.5f, 0f, 0.5f), "TextureStar",
            "ShapeCube"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCapsule", 0, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCylinder", 20, new Color(1f, 1f, 1f), "TextureLine",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 29, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", 14, new Color(0f, 1f, 0f), "TextureLine", "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCube", 17, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureLine", "ShapeCylinder", 14, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureStar", "ShapeCapsule", 4, new Color(0.5f, 0f, 0.5f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureCircle", "ShapeCylinder", 20, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCapsule"),
        (new Color(0f, 1f, 0f), "TextureLine", "ShapeCube", 1, new Color(0.5f, 0f, 0.5f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCylinder", 4, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCapsule"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCube", 29, new Color(1f, 0.6470588f, 0f), "TextureStar",
            "ShapeCylinder"),
        (new Color(1f, 0.6470588f, 0f), "TextureCircle", "ShapeCapsule", 1, new Color(0f, 1f, 0f), "TextureLine",
            "ShapeCube"),
        (new Color(1f, 1f, 1f), "TextureLine", "ShapeCylinder", 20, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureStar", "ShapeCube", 4, new Color(0f, 1f, 0f), "TextureRaster",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureRaster", "ShapeCapsule", 21, new Color(1f, 0.6470588f, 0f), "TextureCircle",
            "ShapeCube"),
        (new Color(0f, 1f, 0f), "TextureCircle", "ShapeCylinder", 24, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(1f, 0.6470588f, 0f), "TextureRaster", "ShapeCapsule", 1, new Color(0f, 1f, 0f), "TextureCircle",
            "ShapeCylinder"),
        (new Color(1f, 1f, 1f), "TextureStar", "ShapeCube", 28, new Color(1f, 0.6470588f, 0f), "TextureRaster",
            "ShapeCapsule"),
        (new Color(0.5f, 0f, 0.5f), "TextureCircle", "ShapeCapsule", 14, new Color(0f, 1f, 0f), "TextureStar",
            "ShapeCube"),
        (new Color(1f, 0.6470588f, 0f), "TextureStar", "ShapeCylinder", 18, new Color(1f, 1f, 1f), "TextureRaster",
            "ShapeCapsule")
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