using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generación de Celdas")]
    public int numberOfCells = 150;
    public float spawnRadius = 50f;
    public Vector2Int roomSizeMin = new Vector2Int(3, 3);
    public Vector2Int roomSizeMax = new Vector2Int(8, 8);

    private List<Cell> cells = new List<Cell>();

    private void Start()
    {
        GenerateInitialCells();
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

            Cell newCell = new Cell(new Vector2(posX, posY), new Vector2(width, height));
            cells.Add(newCell);
        }

        Debug.Log($"{cells.Count} celdas generadas.");
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
