using System.Collections.Generic;
using UnityEngine;
using Assets.Script;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefab;
    private int numberOfEnemies = 8;
    

    private List<Vector3> emptyCells = new List<Vector3>();

    void Start()
    {
        FindEmptyCells();
        SpawnEnemies();
    }

    void FindEmptyCells()
    {
        int gridWidth = MapMatrix.mapWidth;
        int gridHeight = MapMatrix.mapHeight;

        for (int i = 5; i < gridHeight; i++)
        {
            for (int j = 5; j < gridWidth; j++)
            {
                if (MapMatrix.GetCellValue(i, j) == 0) // Assuming 0 indicates an empty cell
                {
                    Vector3 cellPosition = new Vector3Int(i, j, 0); // Adjust y-coordinate to match map
                    emptyCells.Add(cellPosition);
                }
            }
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (emptyCells.Count == 0) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            //Vector3 spawnPosition = emptyCells[randomIndex];
            Vector3 spawnPosition = new Vector3(emptyCells[randomIndex].y - (float)15.5, MapMatrix.mapHeight - (float)9.5 - emptyCells[randomIndex].x, 0);
            int randomEnemy = Random.Range(0, enemyPrefab.Count);
            Instantiate(enemyPrefab[randomEnemy], spawnPosition, Quaternion.identity);
            emptyCells.RemoveAt(randomIndex);
        }
    }
}
