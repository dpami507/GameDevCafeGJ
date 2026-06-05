using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int maxRoomCount;

    Section sectionHead;

    // Sections split from the Binary Space Partitioning
    List<Section> sections = new List<Section>();
    // Rooms generated inside the sections
    List<Room> rooms = new List<Room>();
    // Triangles generated from the Delaunay Triangulation
    List<Triangle> triangles = new List<Triangle>();
    // Room edges MST
    List<Edge> roomEdges = new List<Edge>();

    [Header("Room Settings")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] int maxRoomSize;
    [SerializeField] int minRoomSize;
    [SerializeField] int roomGap;

    [Header("Corridor Settings")]
    [SerializeField] GameObject corridorPrefab;
    [SerializeField] float corridorWidth;

    [Header("Tilemaps")]
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] RuleTile wallRuleTile;

    // Special Room
    Room startRoom;
    Room endRoom;
    public void Generate()
    {
        GenerateSections();
        GenerateRooms();
        DelaunayTriangulation();
        GeneratePaths();
        GenerateTilemap();
        FindSpecialRooms();
    }
    public Vector2 GetStartPos() => startRoom.position;
    void GenerateSections()
    {
        Section initialSection = new Section(transform.position, mapWidth, mapHeight);
        sectionHead = initialSection;

        // Create Sections by cutting in half to a certain amount
        for (int i = 0; i < maxRoomCount; i++)
        {
            SplitSection();
        }
    }
    void SplitSection()
    {
        // Go through randomly until we come to an end
        Section cursor = sectionHead;

        bool searching = true;
        while(searching)
        {
            // Whoops
            if (cursor == null)
            {
                Debug.LogError("Cursor is null");
                break;
            }
            // Get one of the two
            Section possibleSection = cursor.childSections[Random.Range(0, 2)];

            // If its the end break else keep going
            if(possibleSection == null)
            {
                break;
            }
            else
            {
                cursor = possibleSection;
            }
        }

        bool splitHorizontally = cursor.height > cursor.width;
        float newSectionWidth = 0, newSectionHeight = 0;
        if (splitHorizontally)
        {
            // Calc their width and height for horizontal
            newSectionWidth = cursor.width;
            newSectionHeight = cursor.height / 2;

            // Create the sections

            Vector2 section1Pos = cursor.position;
            Section section1 = new Section(section1Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(0, section1);

            Vector2 section2Pos = new Vector2(cursor.position.x, cursor.position.y + newSectionHeight);
            Section section2 = new Section(section2Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(1, section2);

            sections.Add(section1);
            sections.Add(section2);
        }
        else
        {
            // Calc their width and height for vertical
            newSectionWidth = cursor.width / 2;
            newSectionHeight = cursor.height;

            // Create the sections

            Vector2 section1Pos = cursor.position;
            Section section1 = new Section(section1Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(0, section1);

            Vector2 section2Pos = new Vector2(cursor.position.x + newSectionWidth, cursor.position.y);
            Section section2 = new Section(section2Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(1, section2);

            sections.Add(section1);
            sections.Add(section2);
        }

        sections.Remove(cursor);
    }
    void GenerateRooms()
    {
        // Go through each section
        foreach (var section in sections)
        {
            // Check if the room is big enough for a room
            if (section.width < (minRoomSize + roomGap) || section.height < (minRoomSize + roomGap))
            {
                Debug.Log($"{section} is to small for a room");
                continue; // Its to small for a room
            }

            // Calculate the bounds for positioning the rooms
            float x1, y1, x2, y2;

            // Top right corner
            x1 = section.position.x + section.width - minRoomSize;
            y1 = section.position.y + section.height - minRoomSize;
            Vector2 topLeft = new Vector2 (x1, y1);

            // Bottom left corner
            x2 = section.position.x + minRoomSize;
            y2 = section.position.y + minRoomSize;
            Vector2 bottomRight = new Vector2 (x2, y2);

            // Genreate Random Pos
            Vector2 roomPos = new Vector2(
                Random.Range(topLeft.x, bottomRight.x),
                Random.Range(topLeft.y, bottomRight.y)
            );

            // Set size so it doesn't overlap
            float roomWidth = Mathf.Min(
                Mathf.Abs(roomPos.x - section.position.x - roomGap),                     // Distance from left
                Mathf.Abs((section.position.x + section.width - roomGap) - roomPos.x),   // Distance from right
                Mathf.Abs(maxRoomSize)                                         // Max room size
            );
            float roomHeight = Mathf.Min(
                Mathf.Abs(roomPos.y - section.position.y - roomGap),                     // Distance from top
                Mathf.Abs((section.position.y + section.height - roomGap) - roomPos.y),  // Distance from bottom
                Mathf.Abs(maxRoomSize)                                         // Max room size
            );
            // Adjust so it can be used for scale
            roomWidth *= 2;
            roomHeight *= 2;

            if (roomWidth < minRoomSize || roomHeight < minRoomSize)
            {
                Debug.Log($"{section} is to small for a room");
                continue;
            }

            // Create Room Class
            Room newRoom = new Room(roomPos, roomWidth, roomHeight);
            rooms.Add(newRoom);
        }
    }
    void DelaunayTriangulation()
    {
        // Create the SUPER TRIANGLE
        Vector2 center = new Vector2(mapWidth / 2f, mapHeight / 2f);
        float size = Mathf.Max(mapWidth, mapHeight) * 10f;

        Vector2 p1 = new Vector2(center.x - size, center.y - size);
        Vector2 p2 = new Vector2(center.x, center.y + size);
        Vector2 p3 = new Vector2(center.x + size, center.y - size);
        Triangle superTriangle = new Triangle(p1, p2, p3);

        triangles.Add(superTriangle);

        // Go through each room center and split all triangles that land its its circumcircle
        foreach (var room in rooms)
        {
            if (!superTriangle.pointInCircumcircle(room.position))
                Debug.LogWarning($"Room at {room.position} is OUTSIDE the super triangle!");

            List<Triangle> badTriangles = new List<Triangle>();
            foreach (var triangle in triangles)
            {
                if (triangle.pointInCircumcircle(room.position))
                {
                    badTriangles.Add(triangle);
                }
            }

            List<Edge> polygon = new List<Edge>();
            foreach (var triangle in badTriangles)
            {
                // Create the triangle from edges
                Edge[] edges = new Edge[]
                {
                    new Edge(triangle.p1, triangle.p2),
                    new Edge(triangle.p2, triangle.p3),
                    new Edge(triangle.p3, triangle.p1)
                };
                // Go through each edge and see if it is shared by the bad triangles
                foreach (var edge in edges)
                {
                    bool shared = badTriangles.Any(other =>
                        other != triangle && other.hasEdge(edge.p1, edge.p2));

                    if(!shared)
                    {
                        polygon.Add(edge);
                    }
                }
            }

            // remove the bad triangles
            foreach (var badTriangle in badTriangles)
            {
                triangles.Remove(badTriangle);
            }

            // Re triangulate the new triangles using the polygon
            foreach (var edge in polygon)
            {
                triangles.Add(new Triangle(room.position, edge.p1, edge.p2));
            }
        }

        triangles.RemoveAll(t =>
            t.hasVertex(p1) || t.hasVertex(p2) || t.hasVertex(p3));
    }
    void GeneratePaths()
    {
        HashSet<Edge> edges = new HashSet<Edge>();
        HashSet<Vector2> connectedNodes = new HashSet<Vector2>();

        // Populate edges list
        foreach(var triangle in triangles)
        {
            // Add the edges
            edges.Add(new Edge(triangle.p1, triangle.p2));
            edges.Add(new Edge(triangle.p2, triangle.p3));
            edges.Add(new Edge(triangle.p3, triangle.p1));
        }

        // Sort by length
        List<Edge> sortedEdges = edges.OrderBy(e => e.GetLength()).ToList();

        // Add first edge
        connectedNodes.Add(sortedEdges[0].p1);
        connectedNodes.Add(sortedEdges[0].p2);
        roomEdges.Add(sortedEdges[0]);
        sortedEdges.RemoveAt(0);

        // Get a path
        while (sortedEdges.Count > 0)
        {
            Edge best = null;
            int bestIndex = -1;

            for (int i = 0; i < sortedEdges.Count; i++)
            {
                Edge edge = sortedEdges[i];

                bool hasP1 = connectedNodes.Contains(edge.p1);
                bool hasP2 = connectedNodes.Contains(edge.p2);

                if (hasP1 ^ hasP2)
                {
                    best = edge;
                    bestIndex = i;
                    break;
                }
            }

            if (best == null) break;

            connectedNodes.Add(best.p1);
            connectedNodes.Add(best.p2);
            roomEdges.Add(best);
            sortedEdges.RemoveAt(bestIndex);
        }

        // Add a couple more paths
        int extraPathsToAdd = sortedEdges.Count / 2;
        int counter = 0;
        for (int i = sortedEdges.Count - 1; i >= 0; i--)
        {
            // Skips edges we have already
            if (roomEdges.Contains(sortedEdges[i]))
            {
                continue;
            }

            // Add the new edge if we can
            if(counter < extraPathsToAdd)
            {
                counter++;
                roomEdges.Add(sortedEdges[i]);
            }
        }
    }
    void GenerateTilemap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // Paint rooms
        foreach (var room in rooms)
            PaintRoom(room);

        // Paint corridors
        foreach (var edge in roomEdges)
            PaintCorridor(edge);

        // Paint walls around all floor tiles
        PaintWalls();
    }
    void PaintRoom(Room room)
    {
        int xMin = Mathf.RoundToInt(room.position.x - room.width / 2f);
        int xMax = Mathf.RoundToInt(room.position.x + room.width / 2f);
        int yMin = Mathf.RoundToInt(room.position.y - room.height / 2f);
        int yMax = Mathf.RoundToInt(room.position.y + room.height / 2f);

        for (int x = xMin; x < xMax; x++)
            for (int y = yMin; y < yMax; y++)
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }
    void PaintCorridor(Edge edge)
    {
        // Convert room centers to tile coords
        Vector2 start = edge.p1;
        Vector2 end = edge.p2;

        // L-shaped: horizontal then vertical
        Vector2 corner = new Vector2(end.x, start.y);

        PaintCorridorSegment(start, corner);
        PaintCorridorSegment(corner, end);
    }
    void PaintCorridorSegment(Vector2 from, Vector2 to)
    {
        int corridorHalfWidth = Mathf.RoundToInt(corridorWidth / 2f);

        int xMin = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

        // Fatten the corridor by half width in the perpendicular direction
        bool isHorizontal = Mathf.Abs(to.x - from.x) > Mathf.Abs(to.y - from.y);
        if (isHorizontal)
        {
            yMin -= corridorHalfWidth;
            yMax += corridorHalfWidth;
        }
        else
        {
            xMin -= corridorHalfWidth;
            xMax += corridorHalfWidth;
        }

        for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }
    void PaintWalls()
    {
        // If a neighbor has no floor tile, place a wall tile there
        var floorPositions = new HashSet<Vector3Int>();
        foreach (var pos in floorTilemap.cellBounds.allPositionsWithin)
            if (floorTilemap.HasTile(pos))
                floorPositions.Add(pos);

        Vector3Int[] neighbors = {
        Vector3Int.up, Vector3Int.down,
        Vector3Int.left, Vector3Int.right,
        new Vector3Int(1, 1, 0),  new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)
    };

        foreach (var pos in floorPositions)
        {
            foreach (var neighbor in neighbors)
            {
                Vector3Int wallPos = pos + neighbor;
                if (!floorPositions.Contains(wallPos))
                    wallTilemap.SetTile(wallPos, wallRuleTile);
            }
        }
    }
    void FindSpecialRooms()
    {
        // Break case
        if(rooms.Count < 2)
        {
            Debug.LogError("Not enough rooms");
            return;
        }

        // Initial Rooms
        startRoom = rooms[0];
        endRoom = rooms[1];

        // Find largest distant rooms
        for (var i = 0; i < rooms.Count; i++)
        {
            for (var j = i + 1; j < rooms.Count; j++)
            {
                if (Vector2.Distance(rooms[i].position, rooms[j].position) > 
                    Vector2.Distance(startRoom.position, endRoom.position))
                {
                    startRoom = rooms[i];
                    endRoom = rooms[j];
                }

            }
        }

        GameObject startRoomObj = new GameObject("Start Room");
        startRoomObj.transform.position = startRoom.position;
        
        GameObject endRoomObj = new GameObject("End Room");
        endRoomObj.transform.position = endRoom.position;
    }
    public void SetTilesToColor(Vector2 point, float radius, Color color)
    {
        Vector3Int cellPos = wallTilemap.WorldToCell(new Vector3(point.x, point.y, 0));

        Vector2Int topLeft = new Vector2Int(cellPos.x - (int)radius, cellPos.y + (int)radius);
        Vector2Int bottomRight = new Vector2Int(cellPos.x + (int)radius, cellPos.y - (int)radius);
        for (int i = topLeft.x; i < bottomRight.x; i++)
        {
            for (int j = topLeft.y; j > bottomRight.y; j--)
            {
                float dist = Vector2.Distance(new Vector2(i, j), (Vector2Int)cellPos);
                if (dist <= radius)
                {
                    // Get the current bullet color as HSV
                    Color.RGBToHSV(color, out float h, out float s, out float v);
                    s = (radius - dist) / radius;

                    // Get the current wall color
                    Color currentWallColor;
                    if(wallTilemap.HasTile(cellPos))
                        currentWallColor = wallTilemap.GetColor(new Vector3Int(i, j, 0));
                    else if (floorTilemap.HasTile(cellPos))
                        currentWallColor = floorTilemap.GetColor(new Vector3Int(i, j, 0));
                    else 
                        currentWallColor = Color.white;

                    Color newColor = Color.Lerp(currentWallColor, Color.HSVToRGB(h, s, v), 0.5f);

                    // Set it
                    wallTilemap.SetColor(new Vector3Int(i, j, 0), newColor);
                    floorTilemap.SetColor(new Vector3Int(i, j, 0), newColor);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        foreach (var edge in roomEdges)
        {
            Gizmos.DrawLine(edge.p1, edge.p2);
        }
    }
}
class Edge
{
    public Vector2 p1, p2;

    public Edge(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public float GetLength() => Vector2.Distance(p1, p2);
    public override bool Equals(object obj) => obj is Edge e && ((e.p1 == p1 && e.p2 == p2) || (e.p1 == p2 && e.p2 == p1));
    public override int GetHashCode() => p1.GetHashCode() ^ p2.GetHashCode();
}
class Triangle
{
    public Vector2 p1, p2, p3;
    public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
    public bool pointInCircumcircle(Vector2 point)
    {
        double ax_ = p1.x - point.x;
        double ay_ = p1.y - point.y;
        double bx_ = p2.x - point.x;
        double by_ = p2.y - point.y;
        double cx_ = p3.x - point.x;
        double cy_ = p3.y - point.y;
        return (
            (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
            (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
            (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
        ) < 0;
    }
    public bool hasEdge(Vector2 a, Vector2 b)
    {
        return (this.p1 == a && this.p2 == b) ||
               (this.p2 == a && this.p3 == b) ||
               (this.p3 == a && this.p1 == b) ||
               (this.p1 == b && this.p2 == a) ||
               (this.p2 == b && this.p3 == a) ||
               (this.p3 == b && this.p1 == a);
    }
    public bool hasVertex(Vector2 vert)
    {
        float eps = 0.001f;
        return Vector2.Distance(p1, vert) < eps ||
               Vector2.Distance(p2, vert) < eps ||
               Vector2.Distance(p3, vert) < eps;
    }
}