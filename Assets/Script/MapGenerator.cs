using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script
{
    public class MapGenerator : MonoBehaviour
    {
        public Tile blockTile;
        public Tile brickTile;
        public Tile doorTile;
        public RuleTile bushTile;
        public RuleTile waterTile;
        public Tilemap tilemap;
        public Tilemap bushAndWaterTilemap;
        List<RectInt> regions = new List<RectInt>
            {
                new RectInt(3, 3, 3, 3),
                new RectInt(1, 12, 5, 5),
                new RectInt(1, 22, 5, 5),
                new RectInt(9, 3, 5, 5),
                new RectInt(9, 12, 5, 5),
                new RectInt(9, 22, 5, 5)
            };

        private void Start()
        {
            GenerateRandomMap();
            CheckAndModifyClosedLoop();
            RenderMap();
            SetMapToCenter();
        }

        private void GenerateRandomMap()
        {
            CreateRandomClusters(4, 2, 4, (int)TileTypes.Bush); // Create 4 random bush clusters
            CreateRandomClusters(2, 3, 4, (int)TileTypes.Water); // Create 2 random water clusters
            CreateBorders(); // Generate border tiles
            FillEmptyTiles(); // Randomly generate the remaining tiles
            SetSpecificEmptyTiles(); // Set empty tiles at specific positions
        }

        private void CreateBorders()
        {
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                MapMatrix.SetCellValue(i, 0, (int)TileTypes.Block);
                MapMatrix.SetCellValue(i, MapMatrix.mapWidth - 1, (int)TileTypes.Block);
            }

            for (int j = 1; j < MapMatrix.mapWidth - 1; j++)
            {
                MapMatrix.SetCellValue(0, j, (int)TileTypes.Block);
                MapMatrix.SetCellValue(MapMatrix.mapHeight - 1, j, (int)TileTypes.Block);
            }
        }

        private void CreateRandomClusters(int clusterCount, int minSize, int maxSize, int tileType)
        {
            for (int k = 0; k < clusterCount; k++)
            {
                int startX, startY;
                RectInt region = regions[Random.Range(0, regions.Count)];
                startX = Random.Range(region.xMin, region.xMax);
                startY = Random.Range(region.yMin, region.yMax);

                regions.Remove(region);

                switch (Random.Range(0, 2))
                {
                    case 0: // Rectangle
                        CreateCluster(startX, startY, Random.Range(minSize, maxSize), Random.Range(minSize, maxSize), tileType);
                        break;
                    case 1: // L-Shape
                        CreateLShape(startX, startY, tileType);
                        break;
                }
            }
        }

        private void CreateCluster(int startX, int startY, int height, int width, int value)
        {
            for (int i = startX; i < startX + height; i++)
            {
                for (int j = startY; j < startY + width; j++)
                {
                    MapMatrix.SetCellValue(i, j, value); // Set TileTypes for bush or water
                }
            }
        }

        private void CreateLShape(int startX, int startY, int value)
        {
            int ranH = Random.Range(3, 6);
            // Create the vertical part of the L-shape
            CreateCluster(startX, startY, ranH, 2, value);
            // Create the horizontal part of the L-shape
            CreateCluster(startX + Random.Range(0, ranH - 2), startY - Random.Range(0, 3), 2, Random.Range(3, 6), value);
        }

        private void FillEmptyTiles()
        {
            for (int i = 1; i < MapMatrix.mapHeight - 1; i++)
            {
                for (int j = 1; j < MapMatrix.mapWidth - 1; j++)
                {
                    if (MapMatrix.GetCellValue(i, j) == (int)TileTypes.Empty)
                    {
                        int value = Random.Range((int)TileTypes.Empty, (int)TileTypes.Brick + 1); // Avoid spawning bush and water randomly
                        MapMatrix.SetCellValue(i, j, value);
                    }
                }
            }
        }

        private void SetSpecificEmptyTiles()
        {
            MapMatrix.SetCellValue(1, 1, (int)TileTypes.Empty);
            MapMatrix.SetCellValue(1, 2, (int)TileTypes.Empty);
            MapMatrix.SetCellValue(2, 1, (int)TileTypes.Empty);
            MapMatrix.SetCellValue(1, 0, (int)TileTypes.Door);
            MapMatrix.SetCellValue(16, 31, (int)TileTypes.Door);

        }

        private void RenderMap()
        {
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 0; j < MapMatrix.mapWidth; j++)
                {
                    Vector3Int tilePosition = new Vector3Int(j, MapMatrix.mapHeight - 1 - i, 0);
                    TileBase tile = GetTileFromMatrix(i, j);

                    if (tile == bushTile || tile == waterTile)
                    {
                        bushAndWaterTilemap.SetTile(tilePosition, tile);
                    }
                    else
                    {
                        tilemap.SetTile(tilePosition, tile);
                    }
                }
            }

            tilemap.RefreshAllTiles(); // Refresh main tilemap
            bushAndWaterTilemap.RefreshAllTiles(); // Refresh BushAndWater tilemap
        }

        private TileBase GetTileFromMatrix(int i, int j)
        {
            switch ((TileTypes)MapMatrix.GetCellValue(i, j))
            {
                case TileTypes.Block:
                    return blockTile;
                case TileTypes.Brick:
                    return brickTile;
                case TileTypes.Bush:
                    return bushTile;
                case TileTypes.Water:
                    return waterTile;
                case TileTypes.Door:
                    return doorTile;
                default:
                    return null;
            }
        }

        private void SetMapToCenter()
        {
            Camera mainCamera = Camera.main;
            Vector3 mapCenter = new Vector3(MapMatrix.mapWidth / 2f, MapMatrix.mapHeight / 2f, 0f);
            Vector3 cameraPos = mainCamera.transform.position;
            Vector3 mapPosition = new Vector3(cameraPos.x, cameraPos.y, 0);
            tilemap.transform.position = mapPosition - mapCenter * tilemap.cellSize.x;
            bushAndWaterTilemap.transform.position = mapPosition - mapCenter * tilemap.cellSize.x;
        }

        private void CheckAndModifyClosedLoop()
        {
            bool[,] visited = new bool[MapMatrix.mapHeight, MapMatrix.mapWidth];
            for (int i = 1; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 1; j < MapMatrix.mapWidth; j++)
                {
                    if (MapMatrix.GetCellValue(i, j) == (int)TileTypes.Block && !visited[i, j])
                    {
                        List<Vector2Int> loopTiles = new List<Vector2Int>();
                        if (IsClosedLoop(i, j, visited, loopTiles))
                        {
                            ModifyLoopTiles(loopTiles);
                        }
                    }
                }
            }
        }

        private bool IsClosedLoop(int i, int j, bool[,] visited, List<Vector2Int> loopTiles)
        {
            bool touchesBorder = false;
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            stack.Push(new Vector2Int(i, j));
            loopTiles.Add(new Vector2Int(i, j));
            visited[i, j] = true;

            while (stack.Count > 0)
            {
                Vector2Int current = stack.Pop();

                // Check if current tile is on the border
                if (IsBorderTile(current.x, current.y))
                {
                    touchesBorder = true;
                }

                foreach (Vector2Int neighbor in GetNeighbors(current.x, current.y))
                {
                    if (MapMatrix.GetCellValue(neighbor.x, neighbor.y) == (int)TileTypes.Block)
                    {
                        if (!visited[neighbor.x, neighbor.y])
                        {
                            visited[neighbor.x, neighbor.y] = true;
                            stack.Push(neighbor);
                            loopTiles.Add(neighbor);
                        }
                    }
                }
            }

            // Only consider it a closed loop if it doesn't touch the border
            return !touchesBorder;
        }

        private List<Vector2Int> GetNeighbors(int i, int j)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // Right neighbor
            if (j < MapMatrix.mapWidth - 1)
                neighbors.Add(new Vector2Int(i, j + 1));

            // Below neighbor
            if (i < MapMatrix.mapHeight - 1)
                neighbors.Add(new Vector2Int(i + 1, j));

            // Bottom-right diagonal neighbor
            if (i < MapMatrix.mapHeight - 1 && j < MapMatrix.mapWidth - 1)
                neighbors.Add(new Vector2Int(i + 1, j + 1));

            // Bottom-left diagonal neighbor
            if (i < MapMatrix.mapHeight - 1 && j > 0)
                neighbors.Add(new Vector2Int(i + 1, j - 1));

            // Top-right diagonal neighbor
            if (i > 0 && j < MapMatrix.mapWidth - 1)
                neighbors.Add(new Vector2Int(i - 1, j + 1));

            return neighbors;
        }

        private void ModifyLoopTiles(List<Vector2Int> loopTiles)
        {
            HashSet<Vector2Int> borderTiles = new HashSet<Vector2Int>();
            foreach (Vector2Int tile in loopTiles)
            {
                foreach (Vector2Int neighbor in GetNeighbors(tile.x, tile.y))
                {
                    if (MapMatrix.GetCellValue(neighbor.x, neighbor.y) != (int)TileTypes.Block)
                    {
                        borderTiles.Add(tile);
                        break;
                    }
                }
            }

            foreach (Vector2Int tile in loopTiles)
            {
                if (!borderTiles.Contains(tile))
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        MapMatrix.SetCellValue(tile.x, tile.y, (int)TileTypes.Empty);
                    }
                    else
                    {
                        MapMatrix.SetCellValue(tile.x, tile.y, (int)TileTypes.Brick);
                    }
                }
            }
        }
        private bool IsBorderTile(int i, int j)
        {
            return i == 0 || i == MapMatrix.mapHeight - 1 || j == 0 || j == MapMatrix.mapWidth - 1;
        }
    }
}

