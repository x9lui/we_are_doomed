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
    public Vector2 roomSizeThreshold = new Vector2(5, 5); // Tamaño mínimo para ser habitación

    [Header("Tiles")]
    public GameObject tilePrefab;
    public GameObject wallPrefab;

    [Header("Materiales por Bioma")]
    public List<BiomeMaterials> biomeMaterialSets = new List<BiomeMaterials>();
    private Dictionary<Vector2Int, int> tileToCluster = new Dictionary<Vector2Int, int>();
    public Transform dungeonParent; // Padre para los tiles generados
    private List<Cell> cells = new List<Cell>();
    private List<Cell> rooms = new List<Cell>();
    private List<(Vector2, Vector2)> delaunayEdges = new List<(Vector2, Vector2)>();
    private List<Edge> mstEdges = new List<Edge>();
    private List<Edge> finalEdges = new List<Edge>();
    private Dictionary<Vector2Int, TileType> tileMap = new Dictionary<Vector2Int, TileType>();
    private HashSet<(Vector2Int, Vector2Int)> connectedTiles = new HashSet<(Vector2Int, Vector2Int)>();


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

            // Repetir hasta que la celda no sea demasiado alargada
            do
            {
                width = Mathf.RoundToInt(Mathf.Lerp(roomSizeMin.x, roomSizeMax.x, Mathf.Pow(Random.value, 2)));
                height = Mathf.RoundToInt(Mathf.Lerp(roomSizeMin.y, roomSizeMax.y, Mathf.Pow(Random.value, 2)));
            }
            while (Mathf.Abs(width - height) > 3); // Si la diferencia es mayor que 3, repetir

            // Generar posición aleatoria en circunferencia (esquina inferior izquierda)
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(0f, spawnRadius);

            int posX = Mathf.RoundToInt(Mathf.Cos(angle) * distance);
            int posY = Mathf.RoundToInt(Mathf.Sin(angle) * distance);

            Cell newCell = new Cell(new Vector2(posX, posY), new Vector2(width, height));
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

        Debug.Log($"Se han generado {delaunayEdges.Count} bordes de Delaunay.");

        mstEdges = GenerateMST(delaunayEdges);

        Debug.Log($"MST generado con {mstEdges.Count} conexiones.");

        finalEdges = AddLoops(mstEdges, delaunayEdges, 0.15f); // Añadimos ~15% de bucles

        Debug.Log($"Conexiones finales: {finalEdges.Count}.");

        CreateCorridors(finalEdges);

        Debug.Log("Pasillos creados.");

        GenerateWalls();
        Debug.Log("Muros generados.");

        GenerateFloors();
        Debug.Log("Suelos generados.");

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
                    a.position += new Vector2(
                        Mathf.Sign(move.x),
                        Mathf.Sign(move.y)
                    );
                }

                // Esperar un frame después de mover cada celda
                //yield return null;
            }
            // Esperar un frame después de procesar todas las celdas
            //yield return null;
        }

        Debug.Log($"Separación terminada en {iterations} iteraciones.");

        yield return null;

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
        int width = Mathf.CeilToInt(max.x - min.x);
        int height = Mathf.CeilToInt(max.y - min.y);

        bool[,] grid = new bool[width, height];

        // Paso 2: Marcar los tiles ocupados
        foreach (var cell in cells)
        {
            for (int x = 0; x < Mathf.RoundToInt(cell.size.x); x++)
            {
                for (int y = 0; y < Mathf.RoundToInt(cell.size.y); y++)
                {
                    int gridX = Mathf.RoundToInt(cell.position.x - min.x) + x;
                    int gridY = Mathf.RoundToInt(cell.position.y - min.y) + y;

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
            if (!existing.Contains(norm) && Random.value < loopChance)
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

            Vector2Int startInt = new Vector2Int(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
            Vector2Int endInt = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));

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

    private void GenerateFloors()
    {
        if (tilePrefab == null || biomeMaterialSets.Count == 0)
        {
            Debug.LogWarning("Faltan prefabs o materiales de biomas.");
            return;
        }

        tileToCluster = AssignBiomesByKMeans();

        foreach (var cell in cells)
        {
            if (cell.isRoom || cell.isCorridor)
            {
                for (int x = 0; x < (int)cell.size.x; x++)
                {
                    for (int y = 0; y < (int)cell.size.y; y++)
                    {
                        Vector2Int pos = new Vector2Int((int)cell.position.x + x, (int)cell.position.y + y);
                        Vector3 tilePos = new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f);

                        GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, dungeonParent);

                        int cluster = tileToCluster.ContainsKey(pos) ? tileToCluster[pos] : 0;
                        BiomeMaterials biome = biomeMaterialSets[Mathf.Clamp(cluster, 0, biomeMaterialSets.Count - 1)];

                        if (biome.floorMaterials.Count > 0)
                        {
                            Material mat = biome.floorMaterials[Random.Range(0, biome.floorMaterials.Count)];
                            MeshRenderer rend = tile.GetComponent<MeshRenderer>();
                            if (rend != null) rend.material = mat;
                        }
                    }
                }
            }
        }

        Debug.Log("Suelos generados por bioma.");
    }


    private void GenerateWalls()
    {
        // Primero marcar los tiles de suelo
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

        // CREA UNA COPIA DEL LISTADO DE TILES PARA RECORRER
        List<Vector2Int> floorTiles = new List<Vector2Int>(tileMap.Keys);

        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var floorTile in floorTiles) // ⬅️ RECORREMOS LA COPIA
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = floorTile + dir;
                if (!tileMap.ContainsKey(neighbor))
                {
                    if (!AreConnected(floorTile, neighbor))
                    {
                        PlaceWall(neighbor, dir);
                        tileMap[neighbor] = TileType.Wall; // Marcar como muro
                    }
                }
            }
        }
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
            rotation = Quaternion.Euler(0f, 90f, 0f);

        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = new Vector3(position.x + 0.5f, i + 0.5f, position.y + 0.5f);
            GameObject wallPiece = Instantiate(wallPrefab, spawnPos, rotation, dungeonParent);

            int cluster = tileToCluster.ContainsKey(position) ? tileToCluster[position] : 0;
            BiomeMaterials biome = biomeMaterialSets[Mathf.Clamp(cluster, 0, biomeMaterialSets.Count - 1)];

            if (biome.wallMaterials.Count > 0)
            {
                Material mat = biome.wallMaterials[Random.Range(0, biome.wallMaterials.Count)];
                MeshRenderer rend = wallPiece.GetComponent<MeshRenderer>();
                if (rend != null) rend.material = mat;
            }
        }
    }

    private Dictionary<Vector2Int, int> AssignBiomesByKMeans()
    {
        int k = biomeMaterialSets.Count;
        List<Vector2Int> tileCoords = new List<Vector2Int>();
        List<Vector2> positions = new List<Vector2>();

        foreach (var kvp in tileMap)
        {
            if (kvp.Value == TileType.Floor)
            {
                tileCoords.Add(kvp.Key);
                positions.Add(kvp.Key);
            }
        }

        List<Vector2> centroids = new List<Vector2>();
        HashSet<int> used = new HashSet<int>();
        while (centroids.Count < k)
        {
            int idx = Random.Range(0, positions.Count);
            if (used.Add(idx)) centroids.Add(positions[idx]);
        }

        Dictionary<Vector2Int, int> tileToCluster = new Dictionary<Vector2Int, int>();

        bool changed = true;
        int iterations = 0;
        while (changed && iterations < 20)
        {
            changed = false;
            iterations++;

            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int tile = tileCoords[i];
                Vector2 pos = positions[i];

                float minDist = float.MaxValue;
                int bestCluster = -1;

                for (int j = 0; j < centroids.Count; j++)
                {
                    float dist = (pos - centroids[j]).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestCluster = j;
                    }
                }

                if (!tileToCluster.ContainsKey(tile) || tileToCluster[tile] != bestCluster)
                {
                    tileToCluster[tile] = bestCluster;
                    changed = true;
                }
            }

            Vector2[] newCentroids = new Vector2[k];
            int[] counts = new int[k];

            foreach (var kvp in tileToCluster)
            {
                newCentroids[kvp.Value] += kvp.Key;
                counts[kvp.Value]++;
            }

            for (int i = 0; i < k; i++)
            {
                if (counts[i] > 0)
                    centroids[i] = newCentroids[i] / counts[i];
            }
        }

        Debug.Log($"[Biomas] K-Means hecho en {iterations} iteraciones.");
        return tileToCluster;
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
    }

    public enum TileType
    {
        Empty,
        Floor,
        Wall
    }

    [System.Serializable]
    public class BiomeMaterials
    {
        public string biomeName;
        public List<Material> floorMaterials;
        public List<Material> wallMaterials;
    }


}
