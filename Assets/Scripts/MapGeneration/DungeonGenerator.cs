using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using System.Threading.Tasks;
using System;
using System.Linq;

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
    public Vector2 roomSizeThreshold = new Vector2(5, 5); // Tamaño mínimo para ser habitación

    [Header("Tiles")]
    public GameObject tilePrefab;
    public GameObject wallPrefab;
    public List<Material> floorMaterials = new List<Material>();
    public List<Material> wallMaterials = new List<Material>();
    public Transform dungeonParent; // Padre para los tiles generados
    private List<Cell> cells = new List<Cell>();
    private List<Cell> rooms = new List<Cell>();
    private List<(Vector2, Vector2)> delaunayEdges = new List<(Vector2, Vector2)>();
    private List<Edge> mstEdges = new List<Edge>();
    private List<Edge> finalEdges = new List<Edge>();
    private Dictionary<Vector2Int, TileType> tileMap = new Dictionary<Vector2Int, TileType>();
    private HashSet<(Vector2Int, Vector2Int)> connectedTiles = new HashSet<(Vector2Int, Vector2Int)>();

    //Synchronization events
    private event Action DungeonHeavyWorkDone;
    public event Action DungeonGenerated;

    private System.Random rnd;

    /*This queue is used to enqueue actions in non main threads, and execute
    those actions in the main one.*/
    Queue<Action> mainThreadActionQueue;
    public void Awake()
    {
        mainThreadActionQueue = new Queue<Action>();
        rnd = new System.Random();
        DungeonHeavyWorkDone += FinishDungeonGeneration;
    }
    public void GenerateDungeon()
    {
        Task.Run(() =>
        {
            GenerateInitialCells();
            SeparateAndSelectRooms();
            Debug.Log("Dungeon heavy work already done");
            lock (mainThreadActionQueue)
            {
                mainThreadActionQueue.Enqueue(() =>
                {
                    DungeonHeavyWorkDone.Invoke();
                });
            }
        });
    }

    private void FinishDungeonGeneration()
    {
        GenerateWalls();
        Debug.Log("Muros generados.");

        MergeCorridorCells();
        GenerateFloors();
        Debug.Log("Suelos generados.");
        DungeonGenerated.Invoke();
    }

    private void Update()
    {
        lock (mainThreadActionQueue)
        {
            while (mainThreadActionQueue.Count > 0)
            {
                Action action = mainThreadActionQueue.Dequeue();
                action();
            }
        }
    }

    public List<Cell> GetDungeonRooms()
    {
        return rooms;
    }

    private void GenerateInitialCells()
    {
        cells.Clear();

        for (int i = 0; i < numberOfCells; i++)
        {
            int width, height;

            // Repetir hasta que la celda no sea demasiado alargada
            do
            {
                //double t = System.Math.Pow(rnd.NextDouble(), 2);  // Random.value²
                double t = rnd.NextDouble();  //Not using pow, because it makes rooms smaller
                width = (int)System.Math.Round(Lerp(roomSizeMin.x, roomSizeMax.x, t));
                t = System.Math.Pow(rnd.NextDouble(), 2);
                height = (int)System.Math.Round(Lerp(roomSizeMin.y, roomSizeMax.y, t));
            }
            while (System.Math.Max(width/height, height/width) > 1.8); // if proportion between width and height
            //is bigger than 1.8, repeat

            // Generar posición aleatoria en circunferencia (esquina inferior izquierda)
            float angle = (float)(rnd.NextDouble() * System.Math.PI * 2.0f);
            float distance = (float)(rnd.NextDouble() * spawnRadius);

            int posX = (int)System.Math.Round(System.Math.Cos(angle) * distance);
            int posY = (int)System.Math.Round(System.Math.Sin(angle) * distance);


            Cell newCell = new Cell(new Vector2(posX, posY), new Vector2(width, height));
            cells.Add(newCell);
        }

        Debug.Log($"{cells.Count} celdas generadas.");
    }

    private void SeparateAndSelectRooms()
    {
        SeparateCells();

        rooms = SelectRooms();
        FillEmptySpacesWithSmallCells();

        var centers = GetRoomCenters();

        delaunayEdges = GenerateDelaunayEdges(centers);

        Debug.Log($"Se han generado {delaunayEdges.Count} bordes de Delaunay.");

        mstEdges = GenerateMST(delaunayEdges);

        Debug.Log($"MST generado con {mstEdges.Count} conexiones.");

        finalEdges = AddLoops(mstEdges, delaunayEdges, 0.15f); // Añadimos ~15% de bucles

        Debug.Log($"Conexiones finales: {finalEdges.Count}.");

        CreateCorridors(finalEdges);

        Debug.Log("Pasillos creados.");

        /*
        This part of the process is commented because Unity instantiation in
        Task.Run doesn't work and must be done in the main Unity thread. See
        current code in Update()

        GenerateWalls();
        Debug.Log("Muros generados.");

        MergeCorridorCells();
        GenerateFloors();
        Debug.Log("Suelos generados.");
        */

    }


    private void SeparateCells()
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
                    a.position += new Vector2(
                        System.Math.Sign(move.x),
                        System.Math.Sign(move.y)
                    );
                }

                // Esperar un frame después de mover cada celda
                //yield return null;
            }
            // Esperar un frame después de procesar todas las celdas
            //yield return null;
        }

        Debug.Log($"Separación terminada en {iterations} iteraciones.");

        return;

    }


    private bool AreOverlapping(Cell a, Cell b, out Vector2Int pushDirection)
    {
        Vector2 aMin = a.position - Vector2.one; // Expandimos A una unidad hacia fuera
        Vector2 aMax = a.position + a.size + Vector2.one; // Expandimos el tamaño de A

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
        List<Cell> selectedRooms = new List<Cell>();

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

        Debug.Log($"Se han seleccionado {selectedRooms.Count} habitaciones.");
        return selectedRooms;
    }

    private void FillEmptySpacesWithSmallCells()
    {
        // Paso 1: Calcular el bounding box
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (var cell in cells)
        {
            Vector2 cellMin = cell.position;
            Vector2 cellMax = cell.position + cell.size;

            min = Vector2.Min(min, cellMin);
            max = Vector2.Max(max, cellMax);
        }

        // Definir el tamaño del grid
        int width = (int)System.Math.Ceiling(max.x - min.x);
        int height = (int)System.Math.Ceiling(max.y - min.y);


        bool[,] grid = new bool[width, height];

        // Paso 2: Marcar los tiles ocupados
        foreach (var cell in cells)
        {
            for (int x = 0; x < (int)System.Math.Round(cell.size.x); x++)
            {
                for (int y = 0; y < (int)System.Math.Round(cell.size.y); y++)
                {
                    int gridX = (int)System.Math.Round(cell.position.x - min.x) + x;
                    int gridY = (int)System.Math.Round(cell.position.y - min.y) + y;

                    if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height)
                    {
                        grid[gridX, gridY] = true;
                    }
                }
            }
        }

        // Paso 3: Crear celdas 1x1 donde falte
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!grid[x, y])
                {
                    // Ahora creamos 1x1 en posición real
                    Vector2 pos = new Vector2(min.x + x, min.y + y);
                    Cell newCell = new Cell(pos, Vector2.one);
                    newCell.isRoom = false; // Las pequeñas no son habitaciones

                    cells.Add(newCell);
                }
            }
        }

        Debug.Log("Huecos rellenados con celdas 1x1.");
    }

    private List<Vector2> GetRoomCenters()
    {
        List<Vector2> centers = new List<Vector2>();

        foreach (var room in rooms)
        {
            centers.Add(room.position + room.size / 2f);
        }

        return centers;
    }


    private List<(Vector2, Vector2)> GenerateDelaunayEdges(List<Vector2> points)
    {
        Polygon polygon = new Polygon();

        foreach (var p in points)
        {
            polygon.Add(new Vertex(p.x, p.y));
        }

        var mesh = polygon.Triangulate();

        List<(Vector2, Vector2)> edges = new List<(Vector2, Vector2)>();

        var verticesList = new List<Vertex>(mesh.Vertices);

        foreach (var e in mesh.Edges)
        {
            var p0 = verticesList[e.P0];
            var p1 = verticesList[e.P1];

            edges.Add((new Vector2((float)p0.X, (float)p0.Y), new Vector2((float)p1.X, (float)p1.Y)));
        }

        return edges;
    }

    private List<Edge> GenerateMST(List<(Vector2, Vector2)> delaunayEdges)
    {
        List<Edge> edges = new List<Edge>();

        foreach (var e in delaunayEdges)
        {
            edges.Add(new Edge(e.Item1, e.Item2));
        }

        List<Edge> result = new List<Edge>();
        Dictionary<Vector2, Vector2> parent = new Dictionary<Vector2, Vector2>();

        foreach (var edge in edges)
        {
            if (!parent.ContainsKey(edge.a)) parent[edge.a] = edge.a;
            if (!parent.ContainsKey(edge.b)) parent[edge.b] = edge.b;
        }

        edges.Sort((x, y) => x.weight.CompareTo(y.weight)); // Ordenar por peso

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
        List<Edge> allEdges = new List<Edge>();
        HashSet<(Vector2, Vector2)> existing = new HashSet<(Vector2, Vector2)>();

        // Añadir todos los edges del MST
        foreach (var e in mst)
        {
            allEdges.Add(e);
            existing.Add(NormalizeEdge((e.a, e.b)));
        }

        // Buscar qué edges de Delaunay no están en el MST
        foreach (var e in delaunay)
        {
            var norm = NormalizeEdge(e);
            if (!existing.Contains(norm) && (float)rnd.NextDouble() < loopChance)
            {
                allEdges.Add(new Edge(e.Item1, e.Item2));
                existing.Add(norm);
            }
        }

        return allEdges;
    }

    // Normalizar un edge (asegura que A->B == B->A)
    private (Vector2, Vector2) NormalizeEdge((Vector2, Vector2) edge)
    {
        if (edge.Item1.x < edge.Item2.x || (edge.Item1.x == edge.Item2.x && edge.Item1.y < edge.Item2.y))
            return edge;
        else
            return (edge.Item2, edge.Item1);
    }

    private void CreateCorridors(List<Edge> connections)
    {
        foreach (var edge in connections)
        {
            Vector2 start = edge.a;
            Vector2 end = edge.b;

            Vector2Int startInt = new Vector2Int((int)System.Math.Round(start.x), (int)System.Math.Round(start.y));
            Vector2Int endInt = new Vector2Int((int)System.Math.Round(end.x), (int)System.Math.Round(end.y));

            Vector2Int current = startInt;

            while (current.x != endInt.x)
            {
                Vector2Int previous = current;
                current.x += (endInt.x > current.x) ? 1 : -1;
                ActivateCorridorAt(current);

                // Registrar conexión directa de borde a borde en X
                connectedTiles.Add((previous, current));
                connectedTiles.Add((current, previous));
            }

            while (current.y != endInt.y)
            {
                Vector2Int previous = current;
                current.y += (endInt.y > current.y) ? 1 : -1;
                ActivateCorridorAt(current);

                // Registrar conexión directa de borde a borde en Y
                connectedTiles.Add((previous, current));
                connectedTiles.Add((current, previous));
            }
        }
    }


    private void ActivateCorridorAt(Vector2Int center)
    {
        // 5x5 tiles alrededor (pasillo ancho)
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);

                foreach (var cell in cells)
                {
                    if (cell.isRoom) continue;

                    Vector2 cellMin = cell.position;
                    Vector2 cellMax = cell.position + cell.size;

                    if (pos.x >= cellMin.x && pos.x < cellMax.x &&
                        pos.y >= cellMin.y && pos.y < cellMax.y)
                    {
                        cell.isCorridor = true;

                        // ✅ Guardar conexiones de tiles adyacentes
                        connectedTiles.Add((center, pos));
                        connectedTiles.Add((pos, center));
                    }
                }
            }
        }
    }

    private bool AreConnected(Vector2Int a, Vector2Int b)
    {
        return connectedTiles.Contains((a, b)) || connectedTiles.Contains((b, a));
    }



    private void OnDrawGizmos()
    {
        if (cells == null) return;

        foreach (var cell in cells)
        {
            Vector3 center = new Vector3(cell.position.x + cell.size.x / 2f, 0, cell.position.y + cell.size.y / 2f);

            if (cell.isRoom)
            {
                // Dibujar primero un cubo sólido semitransparente para habitaciones
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Verde semitransparente
                Gizmos.DrawCube(center, new Vector3(cell.size.x, 1f, cell.size.y));

                // Luego bordes más vivos
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
            else if (cell.isCorridor)
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.8f); // Azul más intenso para pasillos
                Gizmos.DrawCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
            else
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
            }
        }

        if (delaunayEdges != null && delaunayEdges.Count > 0)
        {
            Gizmos.color = Color.red;

            foreach (var edge in delaunayEdges)
            {
                Gizmos.DrawLine(new Vector3(edge.Item1.x, 0, edge.Item1.y), new Vector3(edge.Item2.x, 0, edge.Item2.y));
            }
        }

        if (mstEdges != null && mstEdges.Count > 0)
        {
            Gizmos.color = Color.yellow;

            foreach (var edge in mstEdges)
            {
                Gizmos.DrawLine(new Vector3(edge.a.x, 0.5f, edge.a.y), new Vector3(edge.b.x, 0.5f, edge.b.y));
            }
        }

        if (finalEdges != null && finalEdges.Count > 0)
        {
            Gizmos.color = Color.blue;

            foreach (var edge in finalEdges)
            {
                Gizmos.DrawLine(new Vector3(edge.a.x, 1f, edge.a.y), new Vector3(edge.b.x, 1f, edge.b.y));
            }
        }
    }

    private void MergeCorridorCells()
    {
        // Paso 1: Crear mapa binario de ocupación
        List<Cell> corridorCells = cells.FindAll(c => c.isCorridor);

        if (corridorCells.Count == 0) return;

        // Obtener bounding box
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (var c in corridorCells)
        {
            minX = System.Math.Min(minX, c.position.x);
            minY = System.Math.Min(minY, c.position.y);
            maxX = System.Math.Max(maxX, c.position.x + c.size.x);
            maxY = System.Math.Max(maxY, c.position.y + c.size.y);
        }

        int width = (int)System.Math.Ceiling(maxX - minX);
        int height = (int)System.Math.Ceiling(maxY - minY);

        bool[,] map = new bool[width, height];

        // Paso 2: Marcar las celdas ocupadas
        foreach (var c in corridorCells)
        {
            int startX = (int)System.Math.Round(c.position.x - minX);
            int startY = (int)System.Math.Round(c.position.y - minY);

            for (int x = 0; x < (int)c.size.x; x++)
            {
                for (int y = 0; y < (int)c.size.y; y++)
                {
                    map[startX + x, startY + y] = true;
                }
            }
        }

        // Paso 3: Buscar bloques rectangulares máximos
        List<Cell> merged = new List<Cell>();
        bool[,] visited = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!map[x, y] || visited[x, y]) continue;

                // Expandimos el bloque lo máximo posible en X y luego en Y
                int maxWidth = 1;
                while (x + maxWidth < width && map[x + maxWidth, y] && !visited[x + maxWidth, y])
                    maxWidth++;

                int maxHeight = 1;
                bool valid = true;

                while (y + maxHeight < height && valid)
                {
                    for (int i = 0; i < maxWidth; i++)
                    {
                        if (!map[x + i, y + maxHeight] || visited[x + i, y + maxHeight])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid) maxHeight++;
                }

                // Marcar como visitado
                for (int dx = 0; dx < maxWidth; dx++)
                {
                    for (int dy = 0; dy < maxHeight; dy++)
                    {
                        visited[x + dx, y + dy] = true;
                    }
                }

                Vector2 position = new Vector2(minX + x, minY + y);
                Vector2 size = new Vector2(maxWidth, maxHeight);
                Cell newCell = new Cell(position, size) { isCorridor = true };
                merged.Add(newCell);
            }
        }

        // Paso 4: Limpiar y reemplazar
        cells.RemoveAll(c => c.isCorridor);
        cells.AddRange(merged);

        Debug.Log($"Corridors fusionados en {merged.Count} bloques rectangulares sin solapamiento.");
    }

    private void GenerateFloors()
    {
        foreach (var cell in cells)
        {
            if (cell.isRoom || cell.isCorridor)
            {
                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                float scaleX = cell.size.x / 10f;
                float scaleZ = cell.size.y / 10f;
                floor.transform.localScale = new Vector3(scaleX, 1f, scaleZ);

                Vector3 center = new Vector3(cell.position.x + cell.size.x / 2f, 0, cell.position.y + cell.size.y / 2f);
                floor.transform.position = center;

                floor.transform.SetParent(dungeonParent);
                floor.name = $"Floor_{(cell.isRoom ? "Room" : "Corridor")}_{center.x}_{center.z}";

                // 🎯 Aquí asignamos un material desde la lista
                if (floorMaterials.Count > 0)
                {
                    Renderer r = floor.GetComponent<Renderer>();
                    Material mat = new Material(floorMaterials[0]); // Crea una instancia para modificar
                    r.material = mat;
                }

            }
        }

        Debug.Log("Suelos generados como planos por celda.");
    }


    private void GenerateWalls()
    {
        // 1. Marcar todos los tiles de suelo
        foreach (var cell in cells)
        {
            if (cell.isRoom || cell.isCorridor)
            {
                for (int x = 0; x < (int)cell.size.x; x++)
                {
                    for (int y = 0; y < (int)cell.size.y; y++)
                    {
                        Vector2Int pos = new Vector2Int((int)cell.position.x + x, (int)cell.position.y + y);
                        tileMap[pos] = TileType.Floor;
                    }
                }
            }
        }

        // 2. Identificar celdas vacías vecinas que necesitan muro
        List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        List<Vector2Int> floorTiles = new List<Vector2Int>(tileMap.Keys);
        foreach (var floorTile in floorTiles)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = floorTile + dir;
                if (!tileMap.ContainsKey(neighbor) && !AreConnected(floorTile, neighbor))
                {
                    wallPositions.Add(neighbor);
                    tileMap[neighbor] = TileType.Wall;
                }
            }
        }

        // 3. Agrupar posiciones contiguas en bloques rectangulares (como en MergeCorridorCells)
        List<BoundsInt> wallBlocks = MergeWallPositions(wallPositions);

        // 4. Generar mesh por bloque
        foreach (var bounds in wallBlocks)
        {
            CreateWallBlock(bounds.position, bounds.size);
        }

        Debug.Log($"Muros generados como {wallBlocks.Count} bloques compactos.");
    }

    private List<BoundsInt> MergeWallPositions(HashSet<Vector2Int> wallPositions)
    {
        List<BoundsInt> result = new List<BoundsInt>();

        if (wallPositions.Count == 0)
            return result;

        // Bounding box
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (var pos in wallPositions)
        {
            minX = System.Math.Min(minX, pos.x);
            minY = System.Math.Min(minY, pos.y);
            maxX = System.Math.Max(maxX, pos.x);
            maxY = System.Math.Max(maxY, pos.y);
        }

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        bool[,] grid = new bool[width, height];
        bool[,] visited = new bool[width, height];

        foreach (var pos in wallPositions)
        {
            int x = pos.x - minX;
            int y = pos.y - minY;
            grid[x, y] = true;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!grid[x, y] || visited[x, y]) continue;

                int w = 1;
                while (x + w < width && grid[x + w, y] && !visited[x + w, y]) w++;

                int h = 1;
                bool valid = true;
                while (y + h < height && valid)
                {
                    for (int i = 0; i < w; i++)
                    {
                        if (!grid[x + i, y + h] || visited[x + i, y + h])
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid) h++;
                }

                for (int dx = 0; dx < w; dx++)
                    for (int dy = 0; dy < h; dy++)
                        visited[x + dx, y + dy] = true;

                Vector3Int pos = new Vector3Int(minX + x, 0, minY + y);
                Vector3Int size = new Vector3Int(w, 5, h); // Altura 5 fija
                result.Add(new BoundsInt(pos, size));
            }
        }

        return result;
    }

    private void CreateWallBlock(Vector3Int position, Vector3Int size)
    {
        GameObject wall = new GameObject($"Wall_{position.x}_{position.z}");
        wall.transform.SetParent(dungeonParent);

        MeshFilter mf = wall.AddComponent<MeshFilter>();
        MeshRenderer mr = wall.AddComponent<MeshRenderer>();
        MeshCollider collider = wall.AddComponent<MeshCollider>();

        Mesh mesh = CreateCubeMesh(size.x, size.y, size.z);
        mf.mesh = mesh;
        collider.sharedMesh = mesh;

        wall.transform.position = new Vector3(position.x, 0, position.z);

        if (wallMaterials.Count > 0)
        {
            mr.material = wallMaterials[0];
        }
    }

    private Mesh CreateCubeMesh(float width, float height, float depth)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8]
        {
            new Vector3(0, 0, 0),                    // 0
            new Vector3(width, 0, 0),                // 1
            new Vector3(width, 0, depth),            // 2
            new Vector3(0, 0, depth),                // 3
            new Vector3(0, height, 0),               // 4
            new Vector3(width, height, 0),           // 5
            new Vector3(width, height, depth),       // 6
            new Vector3(0, height, depth)            // 7
        };

        int[] triangles = new int[]
        {
            // Bottom
            0, 2, 1, 0, 3, 2,
            // Top
            4, 6, 5, 4, 7, 6,
            // Front
            4, 5, 1, 4, 1, 0,
            // Back
            6, 7, 3, 6, 3, 2,
            // Left
            7, 4, 0, 7, 0, 3,
            // Right
            5, 6, 2, 5, 2, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void PlaceWall(Vector2Int position, Vector2Int direction)
    {
        if (wallPrefab == null)
        {
            Debug.LogWarning("No hay prefab de muro asignado!");
            return;
        }

        Quaternion rotation = Quaternion.identity;
        if (direction == Vector2Int.left || direction == Vector2Int.right)
        {
            rotation = Quaternion.Euler(0f, 90f, 0f); // Rotar para muros horizontales
        }

        // Ahora generamos 5 cubos apilados (altura 5 unidades)
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = new Vector3(position.x + 0.5f, i + 0.5f, position.y + 0.5f); // Altura 0.5, 1.5, 2.5

            GameObject wallPiece = Instantiate(wallPrefab, spawnPos, rotation, dungeonParent);

            if (wallMaterials.Count > 0)
            {
                Material randomMat = wallMaterials[rnd.Next(0, wallMaterials.Count)];
                MeshRenderer renderer = wallPiece.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    renderer.material = randomMat;
                }
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
            this.weight = Vector2.Distance(a, b);
        }
    }

    public class Cell
    {
        public Vector2 position; // ESQUINA INFERIOR IZQUIERDA
        public Vector2 size;
        public bool isRoom = false;
        public bool isCorridor = false;

        public Cell(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// This function gives the position in the center of the room.
        /// </summary>
        /// <returns>
        /// The position in the center of the room.
        /// </returns>
        public Vector2 GetCenter()
        {
            return new Vector2(position.x + size.x / 2, position.y + size.y / 2);
        }
        /// <summary>
        /// This function gives a position contained in a room.
        /// </summary>
        /// <param name="insidePositionOffset">
        /// The distance offset is used to add distance to the walls of the
        /// room, very useful to generate points for spawning gameobjects
        /// and avoid clipping them through walls
        /// </param>
        /// <returns> A random position in a room </returns>
        public Vector2 GetRandomPositionInside(float insidePositionOffset)
        {
            return new Vector2(UnityEngine.Random.Range(position.x + insidePositionOffset, position.x + size.x - insidePositionOffset),
                                UnityEngine.Random.Range(position.y + insidePositionOffset, position.y + size.y - insidePositionOffset));
        }
    }

    public enum TileType
    {
        Empty,
        Floor,
        Wall
    }

    private double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

}
