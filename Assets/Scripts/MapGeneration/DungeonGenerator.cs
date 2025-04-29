using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generación de Celdas")]
    public int numberOfCells = 150;
    public float spawnRadius = 50f;
    public Vector2Int roomSizeMin = new Vector2Int(3, 3);
    public Vector2Int roomSizeMax = new Vector2Int(8, 8);

    [Header("Separación de Celdas")]
    public int maxSeparationIterations = 1000;

    [Header("Selección de Habitaciones")]
    public Vector2 roomSizeThreshold = new Vector2(5, 5);

    private List<Cell> cells = new();
    private List<Cell> rooms = new();
    private List<(Vector2, Vector2)> delaunayEdges = new();
    private List<Edge> mstEdges = new();
    private List<Edge> finalEdges = new();

    private void Start()
    {
        GenerateInitialCells();
        StartCoroutine(SeparateAndSelectRooms());
    }

    private void GenerateInitialCells()
    {
        cells.Clear();

        for (int i = 0; i < numberOfCells; i++)
        {
            int width, height;
            do
            {
                width = Mathf.RoundToInt(Mathf.Lerp(roomSizeMin.x, roomSizeMax.x, Mathf.Pow(Random.value, 2)));
                height = Mathf.RoundToInt(Mathf.Lerp(roomSizeMin.y, roomSizeMax.y, Mathf.Pow(Random.value, 2)));
            }
            while (Mathf.Abs(width - height) > 3);

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(0f, spawnRadius);

            int posX = Mathf.RoundToInt(Mathf.Cos(angle) * distance);
            int posY = Mathf.RoundToInt(Mathf.Sin(angle) * distance);

            Cell newCell = new(new Vector2(posX, posY), new Vector2(width, height));
            cells.Add(newCell);
        }

        Debug.Log($"{cells.Count} celdas generadas.");
    }

    private IEnumerator SeparateAndSelectRooms()
    {
        yield return StartCoroutine(SeparateCells());

        rooms = SelectRooms();
        FillEmptySpacesWithSmallCells();

        var centers = GetRoomCenters();
        delaunayEdges = GenerateDelaunayEdges(centers);
        mstEdges = GenerateMST(delaunayEdges);
        finalEdges = AddLoops(mstEdges, delaunayEdges, 0.15f);

        CreateCorridors(finalEdges);

        Debug.Log("Pasillos generados.");
    }

    private IEnumerator SeparateCells()
    {
        bool overlapsExist = true;
        int iterations = 0;

        while (overlapsExist && iterations < maxSeparationIterations)
        {
            overlapsExist = false;
            iterations++;

            for (int i = 0; i < cells.Count; i++)
            {
                var a = cells[i];
                Vector2Int move = Vector2Int.zero;

                for (int j = 0; j < cells.Count; j++)
                {
                    if (i == j) continue;
                    var b = cells[j];

                    if (AreOverlapping(a, b, out Vector2Int direction))
                    {
                        overlapsExist = true;
                        move += direction;
                    }
                }

                if (move != Vector2Int.zero)
                {
                    a.position += new Vector2(Mathf.Sign(move.x), Mathf.Sign(move.y));
                }
            }

            yield return null;
        }

        Debug.Log($"Separación terminada en {iterations} iteraciones.");
    }

    private bool AreOverlapping(Cell a, Cell b, out Vector2Int pushDirection)
    {
        Vector2 aMin = a.position - Vector2.one;
        Vector2 aMax = a.position + a.size + Vector2.one;

        Vector2 bMin = b.position;
        Vector2 bMax = b.position + b.size;

        bool overlapX = aMin.x < bMax.x && aMax.x > bMin.x;
        bool overlapY = aMin.y < bMax.y && aMax.y > bMin.y;

        if (overlapX && overlapY)
        {
            Vector2 aCenter = a.position + a.size / 2f;
            Vector2 bCenter = b.position + b.size / 2f;

            Vector2 rawPush = aCenter - bCenter;

            pushDirection = new Vector2Int(
                rawPush.x > 0 ? 1 : (rawPush.x < 0 ? -1 : 0),
                rawPush.y > 0 ? 1 : (rawPush.y < 0 ? -1 : 0)
            );

            return true;
        }

        pushDirection = Vector2Int.zero;
        return false;
    }

    private List<Cell> SelectRooms()
    {
        List<Cell> selectedRooms = new();

        foreach (var cell in cells)
        {
            if (cell.size.x >= roomSizeThreshold.x || cell.size.y >= roomSizeThreshold.y)
            {
                cell.isRoom = true;
                selectedRooms.Add(cell);
            }
            else
            {
                cell.isRoom = false;
            }
        }

        return selectedRooms;
    }

    private void FillEmptySpacesWithSmallCells()
    {
        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);

        foreach (var cell in cells)
        {
            Vector2 cellMin = cell.position;
            Vector2 cellMax = cell.position + cell.size;
            min = Vector2.Min(min, cellMin);
            max = Vector2.Max(max, cellMax);
        }

        int width = Mathf.CeilToInt(max.x - min.x);
        int height = Mathf.CeilToInt(max.y - min.y);
        bool[,] grid = new bool[width, height];

        foreach (var cell in cells)
        {
            for (int x = 0; x < Mathf.RoundToInt(cell.size.x); x++)
            {
                for (int y = 0; y < Mathf.RoundToInt(cell.size.y); y++)
                {
                    int gridX = Mathf.RoundToInt(cell.position.x - min.x) + x;
                    int gridY = Mathf.RoundToInt(cell.position.y - min.y) + y;
                    if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height)
                        grid[gridX, gridY] = true;
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y])
                {
                    Vector2 pos = new(min.x + x, min.y + y);
                    Cell newCell = new(pos, Vector2.one);
                    cells.Add(newCell);
                }
            }
        }

        Debug.Log("Huecos rellenados con celdas 1x1.");
    }

    private List<Vector2> GetRoomCenters()
    {
        List<Vector2> centers = new();
        foreach (var room in rooms)
            centers.Add(room.position + room.size / 2f);
        return centers;
    }

    private List<(Vector2, Vector2)> GenerateDelaunayEdges(List<Vector2> points)
    {
        Polygon polygon = new();
        foreach (var p in points) polygon.Add(new Vertex(p.x, p.y));
        var mesh = polygon.Triangulate();
        List<(Vector2, Vector2)> edges = new();
        var vertices = new List<Vertex>(mesh.Vertices);
        foreach (var e in mesh.Edges)
        {
            var p0 = vertices[e.P0];
            var p1 = vertices[e.P1];
            edges.Add((new Vector2((float)p0.X, (float)p0.Y), new Vector2((float)p1.X, (float)p1.Y)));
        }
        return edges;
    }

    private List<Edge> GenerateMST(List<(Vector2, Vector2)> delaunayEdges)
    {
        List<Edge> edges = new();
        foreach (var e in delaunayEdges) edges.Add(new Edge(e.Item1, e.Item2));
        edges.Sort((x, y) => x.weight.CompareTo(y.weight));
        Dictionary<Vector2, Vector2> parent = new();
        List<Edge> result = new();

        foreach (var edge in edges)
        {
            if (!parent.ContainsKey(edge.a)) parent[edge.a] = edge.a;
            if (!parent.ContainsKey(edge.b)) parent[edge.b] = edge.b;
        }

        foreach (var edge in edges)
        {
            if (Find(parent, edge.a) != Find(parent, edge.b))
            {
                result.Add(edge);
                Union(parent, edge.a, edge.b);
            }
        }

        return result;
    }

    private Vector2 Find(Dictionary<Vector2, Vector2> parent, Vector2 p)
    {
        if (parent[p] != p)
            parent[p] = Find(parent, parent[p]);
        return parent[p];
    }

    private void Union(Dictionary<Vector2, Vector2> parent, Vector2 a, Vector2 b)
    {
        parent[Find(parent, a)] = Find(parent, b);
    }

    private List<Edge> AddLoops(List<Edge> mst, List<(Vector2, Vector2)> delaunay, float loopChance = 0.15f)
    {
        List<Edge> allEdges = new(mst);
        HashSet<(Vector2, Vector2)> existing = new();
        foreach (var e in mst)
            existing.Add(NormalizeEdge((e.a, e.b)));

        foreach (var e in delaunay)
        {
            var norm = NormalizeEdge(e);
            if (!existing.Contains(norm) && Random.value < loopChance)
                allEdges.Add(new Edge(e.Item1, e.Item2));
        }

        return allEdges;
    }

    private (Vector2, Vector2) NormalizeEdge((Vector2, Vector2) edge)
    {
        return (edge.Item1.x < edge.Item2.x || (edge.Item1.x == edge.Item2.x && edge.Item1.y < edge.Item2.y))
            ? edge : (edge.Item2, edge.Item1);
    }

    private void CreateCorridors(List<Edge> connections)
    {
        foreach (var edge in connections)
        {
            Vector2Int start = Vector2Int.RoundToInt(edge.a);
            Vector2Int end = Vector2Int.RoundToInt(edge.b);
            Vector2Int current = start;

            while (current.x != end.x)
            {
                current.x += (end.x > current.x) ? 1 : -1;
                ActivateCorridorAt(current);
            }

            while (current.y != end.y)
            {
                current.y += (end.y > current.y) ? 1 : -1;
                ActivateCorridorAt(current);
            }
        }
    }

    private void ActivateCorridorAt(Vector2Int pos)
    {
        foreach (var cell in cells)
        {
            if (cell.isRoom) continue;
            Vector2 cellMin = cell.position;
            Vector2 cellMax = cell.position + cell.size;

            if (pos.x >= cellMin.x && pos.x < cellMax.x &&
                pos.y >= cellMin.y && pos.y < cellMax.y)
            {
                cell.isCorridor = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (cells == null) return;

        foreach (var cell in cells)
        {
            Vector3 center = new(cell.position.x + cell.size.x / 2f, 0, cell.position.y + cell.size.y / 2f);

            if (cell.isRoom)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                Gizmos.DrawCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
            else if (cell.isCorridor)
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.6f);
                Gizmos.DrawCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
            else
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
        }
    }

    public class Edge
    {
        public Vector2 a, b;
        public float weight;
        public Edge(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
            weight = Vector2.Distance(a, b);
        }
    }

    public class Cell
    {
        public Vector2 position;
        public Vector2 size;
        public bool isRoom = false;
        public bool isCorridor = false;

        public Cell(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }
    }
}
