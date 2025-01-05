using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Off
{
    public List<Vertex> Vertices { get; set; }
    public List<Face> Faces { get; set; }
    public int VertexCount { get; set; }
    public int FaceCount { get; set; }
    public int EdgesCount { get; set; }
    public string Comments { get; set; }

    public Off()
    {
        Vertices = new List<Vertex>();
        Faces = new List<Face>();
    }

    public override string ToString()
    {
        var content = "OFF";
        content += Environment.NewLine;
        content += $"{VertexCount} {FaceCount} {EdgesCount}{Environment.NewLine}";
        foreach (var vertex in Vertices)
        {
            content += $"{vertex}{Environment.NewLine}";
        }
        foreach (var face in Faces)
        {
            content += $"{face}{Environment.NewLine}";
        }
        return content.TrimEnd();
        //return $"Vertices: {Vertices.Count}, Faces: {Faces.Count}, Edges: {EdgesCount}";
    }

    private static Off Parse(string line)
    {
        Off off = new Off();

        // Read the number of vertices, faces and edges
        var counts = line.SplitSpace();
        off.VertexCount = int.Parse(counts[0]);
        off.FaceCount = int.Parse(counts[1]);
        off.EdgesCount = int.Parse(counts[2]);

        return off;
    }

    /// <summary>
    /// Parse
    /// </summary>
    /// <param name="lines"></param>
    /// <remarks>
    /// <code>
    /// Line 1
    ///     OFF
    /// Line 2
    ///     vertex_count face_count edge_count
    /// One line for each vertex:
    ///     x y z
    ///     for vertex 0, 1, ..., vertex_count-1
    /// One line for each polygonal face:
    ///     n v1 v2 ... vn,
    ///     the number of vertices, and the vertex indices for each face. 
    /// Source: https://people.sc.fsu.edu/~jburkardt/data/off/off.html
    /// </code>
    /// </remarks>
    public static Off Parse(string[] lines)
    {
        int lineIndex = 0;

        bool IsValidLine(string line)
        {
            if (line.Trim().StartsWith("#"))
                return false;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            return true;
        }

        lines = lines.Where(IsValidLine).ToArray();

        string header = lines[lineIndex++].Trim();

        // Ensure it's an OFF file
        if (header != "OFF")
        {
            throw new InvalidOperationException("This is not a valid OFF file.");
        }

        // Read the number of vertices, faces and edges
        var offLine = lines[lineIndex++];
        var off = Off.Parse(offLine);

        off.Comments = string.Join(Environment.NewLine, lines.Where(e => !IsValidLine(e)));

        // Read vertices
        for (int i = 0; i < off.VertexCount; i++)
        {
            var vertexData = lines[lineIndex++];
            off.Vertices.Add(Vertex.Parse(vertexData));
        }

        // Read faces
        for (int i = 0; i < off.FaceCount; i++)
        {
            var faceData = lines[lineIndex++];
            off.Faces.Add(Face.Parse(faceData));
        }

        return off;
    }

}

public class Vertex
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vertex(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vertex Parse(string line)
    {
        var parts = line.SplitSpace();
        if (parts.Length != 3)
        {
            foreach (var item in parts)
            {
                Console.WriteLine(item);
            }
            throw new FormatException($"Invalid vertex format. {line}");
        }

        var partX = parts[0];
        var partY = parts[1];
        var partZ = parts[2];

        if (partX.TryDouble(out var x) && partY.TryDouble(out var y) && partZ.TryDouble(out var z))
        {
            return new Vertex(x, y, z);
        }

        throw new FormatException($"Invalid parse vertex format. {line}");
    }

    public override string ToString()
    {
        return $"{X.AsString()} {Y.AsString()} {Z.AsString()}";
    }
}

public class Face
{
    public List<int> VertexIndices { get; set; }
    public Color FaceColor { get; set; }

    public Face(List<int> vertexIndices, Color faceColor = null)
    {
        VertexIndices = vertexIndices;
        FaceColor = faceColor;
    }

    public static Face Parse(string line)
    {
        var parts = line.SplitSpace();
        if (parts.Length < 4)
        {
            throw new FormatException("Invalid face format.");
        }

        var vertexCount = int.Parse(parts[0]);
        var vertexIndices = new List<int>();
        for (int i = 1; i <= vertexCount; i++)
        {
            vertexIndices.Add(int.Parse(parts[i]));
        }

        Color color = null;
        if (parts.Length > vertexCount + 1)
        {
            var colorParts = parts.Skip(vertexCount + 1).ToArray();
            color = Color.Parse(colorParts);
        }

        return new Face(vertexIndices, color);
    }

    public override string ToString()
    {
        var face = $"{VertexIndices.Count}  {string.Join(" ", VertexIndices)}";
        if (FaceColor is Color)
        {
            face += $"  {FaceColor}";
        }
        return $"{face}";
    }
}

public class Color
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int Alpha { get; set; }

    public static Color Default => new Color();

    public Color(int red = 128, int green = 128, int blue = 128, int alpha = 255)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public Color(double red, double green, double blue, double alpha = 1.0)
    {
        Red = (int)(255 * red);
        Green = (int)(255 * green);
        Blue = (int)(255 * blue);
        Alpha = (int)(255 * alpha);
        DoubleRepresentation = true;
    }

    private bool DoubleRepresentation = false;

    public static Color Parse(string colorParts)
    {
        var parts = colorParts.SplitSpace();
        return Parse(parts);
    }

    public static Color Parse(string[] parts)
    {
        if (parts.Length < 3)
        {
            return null;
        }

        var partR = parts[0];
        var partG = parts[1];
        var partB = parts[2];

        if (partR.TryInteger(out var rInt) && partG.TryInteger(out var gInt) && partB.TryInteger(out var bInt))
        {
            if (parts.Length > 3)
            {
                var partA = parts[3];
                if (partA.TryInteger(out var aInt))
                    return new Color(rInt, gInt, bInt, aInt);
            }
            return new Color(rInt, gInt, bInt);
        }

        if (partR.TryDouble(out var r) && partG.TryDouble(out var g) && partB.TryDouble(out var b))
        {
            if (parts.Length > 3)
            {
                var partA = parts[3];
                if (partA.TryDouble(out var a))
                    return new Color(r, g, b, a);
            }
            return new Color(r, g, b);
        }

        return null;
    }

    public override string ToString()
    {
        if (DoubleRepresentation)
        {
            var r = Red / 255.0;
            var g = Green / 255.0;
            var b = Blue / 255.0;
            var a = Alpha / 255.0;
            if (Alpha != byte.MaxValue)
                return $"{r.AsString()} {g.AsString()} {b.AsString()} {a.AsString()}";
            return $"{r.AsString()} {g.AsString()} {b.AsString()}";
        }

        if (Alpha != byte.MaxValue)
            return $"{Red} {Green} {Blue} {Alpha}";
        return $"{Red} {Green} {Blue}";
    }
}


public static class ParseUtils
{
    public static bool TryDouble(this string s, out double result)
    {
        return double.TryParse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }

    public static bool TryInteger(this string s, out int result)
    {
        return int.TryParse(s, out result);
    }

    public static string[] SplitSpace(this string s)
    {
        return s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string AsString(this double value)
    {
        return (Math.Sign(value) == -1 ? "" : " ") + value.ToString("0.000000", CultureInfo.InvariantCulture);
    }
}