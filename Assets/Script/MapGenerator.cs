using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script
{
    public class MapGenerator : MonoBehaviour
    {
        public Tile blockTile;
        public Tile brickTile;
        public RuleTile bushTile;
        public RuleTile waterTile;
        public Tilemap tilemap;

        private void Start()
        {
            GenerateRandomMap();
            RenderMap();
            SetMapToCenter();
        }

        private void GenerateRandomMap()
        {
            CreateBush(3, 2, 4); // Create 3 random bush clusters

            CreatePuddle(2, 2, 4); // Create 2 random water clusters

            CreateBorders();// Generate border tiles

            // Randomly generate the remaining tiles
            for (int i = 1; i < MapMatrix.mapHeight -1; i++)
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
            // Set empty tiles at specific positions
            MapMatrix.SetCellValue(1, 1, (int)TileTypes.Empty);
            MapMatrix.SetCellValue(1, 2, (int)TileTypes.Empty);
            MapMatrix.SetCellValue(2, 1, (int)TileTypes.Empty);

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


        private void CreateBush(int clusterCount, int minSize, int maxSize)
        {
            CreateRandomClusters(clusterCount, minSize, maxSize, (int)TileTypes.Bush);
        }

        private void CreatePuddle(int clusterCount, int minSize, int maxSize)
        {
            CreateRandomClusters(clusterCount, minSize, maxSize, (int)TileTypes.Water);
        }

        private void CreateRandomClusters(int clusterCount, int minSize, int maxSize, int TileTypes)
        {
            List<Vector2Int> clusterPositions = new List<Vector2Int>();

            for (int k = 0; k < clusterCount; k++)
            {
                int shape = Random.Range(0, 2); // 0: Rectangle, 1: L-Shape
                int clusterWidth = Random.Range(minSize, maxSize);
                int clusterHeight = Random.Range(minSize, maxSize);

                int startX, startY;
                bool validPosition;

                do
                {
                    validPosition = true;
                    startX = Random.Range(4, MapMatrix.mapHeight - clusterHeight - 4);
                    startY = Random.Range(4, MapMatrix.mapWidth - clusterWidth - 4);

                    foreach (var pos in clusterPositions)
                    {
                        if (Vector2Int.Distance(new Vector2Int(startX, startY), pos) < Mathf.Max(clusterWidth, clusterHeight))
                        {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                clusterPositions.Add(new Vector2Int(startX, startY));

                switch (shape)
                {
                    case 0: // Rectangle
                        CreateCluster(startX, startY, clusterWidth, clusterHeight, TileTypes);
                        break;
                    case 1: // L-Shape
                        CreateLShape(startX, startY, TileTypes);
                        break;
                }
            }
        }

        private void CreateCluster(int startX, int startY, int width, int height, int value)
        {
            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    MapMatrix.SetCellValue(i, j, value); // Set TileTypes for bush or water
                }
            }
        }

        private void CreateLShape(int startX, int startY, int value)
        {
            // Create the vertical part of the L-shape
            CreateCluster(startX, startY, Random.Range(2, 6), 2, value);
            // Create the horizontal part of the L-shape
            CreateCluster(startX + 2, startY-2,  2, Random.Range(2, 6), value);
        }


        private void RenderMap()
        {
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 0; j < MapMatrix.mapWidth; j++)
                {
                    Vector3Int tilePosition = new Vector3Int(j, MapMatrix.mapHeight - 1 - i, 0);
                    tilemap.SetTile(tilePosition, GetTileFromMatrix(i, j));
                }
            }

            tilemap.RefreshAllTiles(); // Refresh tilemap to apply RuleTile rules
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
        }
    }
}

