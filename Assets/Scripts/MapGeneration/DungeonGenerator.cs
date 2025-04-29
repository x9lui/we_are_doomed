using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generación de Celdas")]
    public int numberOfCells = 150;
    public float spawnRadius = 50f;
    public Vector2Int roomSizeMin = new Vector2Int(3, 3);
    public Vector2Int roomSizeMax = new Vector2Int(8, 8);

    [Header("Separación de Celdas")]
    public int maxSeparationIterations = 1000;

    private List<Cell> cells = new List<Cell>();

    private void Start()
    {
        GenerateInitialCells();
        StartCoroutine(SeparateCells());
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

            // Generar posición aleatoria en circunferencia
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(0f, spawnRadius);

            int posX = Mathf.RoundToInt(Mathf.Cos(angle) * distance);
            int posY = Mathf.RoundToInt(Mathf.Sin(angle) * distance);

            Cell newCell = new Cell(new Vector2(posX, posY), new Vector2(width, height));
            cells.Add(newCell);
        }

        Debug.Log($"{cells.Count} celdas generadas.");
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
            }

            yield return null; // esperar un frame para evitar freeze
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

    private void OnDrawGizmos()
    {
        if (cells == null) return;

        foreach (var cell in cells)
        {
            Vector3 center = new Vector3(cell.position.x + cell.size.x / 2f, 0, cell.position.y + cell.size.y / 2f);
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(center, new Vector3(cell.size.x, 1f, cell.size.y));
        }
    }

    public class Cell
    {
        public Vector2 position;
        public Vector2 size;

        public Cell(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }
    }
}
