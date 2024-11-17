using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script
{
    public class MapGenerator : MonoBehaviour
    {
        private const int EmptyTileValue = 0;
        private const int RandomTileRange = 3;

        public Tile block;
        public Tile brick;
        public RuleTile dust; // Changed to RuleTile
        public Tilemap tilemap;

        private void Start()
        {
            // Sinh bản đồ ngẫu nhiên
            GenerateRandomMap();

            // Vẽ bản đồ lên Tilemap
            RenderMap();

            // Căn chỉnh vị trí bản đồ với trung tâm camera
            SetMapToCenter();
        }

        private void GenerateRandomMap()
        {
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 0; j < MapMatrix.mapWidth; j++)
                {
                    if (IsBorderCell(i, j))
                    {
                        MapMatrix.SetCellValue(i, j, 1); // Viền là ô phá được
                    }
                    else
                    {
                        // Ensure clusters of tiles are formed
                        int value = Random.Range(EmptyTileValue, RandomTileRange);
                        if (value == 3 && !IsClusterFormed(i, j))
                        {
                            value = Random.Range(EmptyTileValue, 3); // Re-roll if cluster is not formed
                        }
                        MapMatrix.SetCellValue(i, j, value);
                    }
                }
            }

            // Đặt các ô trống tại các vị trí xác định
            MapMatrix.SetCellValue(1, 1, EmptyTileValue);
            MapMatrix.SetCellValue(1, 2, EmptyTileValue);
            MapMatrix.SetCellValue(2, 1, EmptyTileValue);
        }
        
        private bool IsBorderCell(int i, int j)
        {
            return i == 0 || i == MapMatrix.mapHeight - 1 || j == 0 || j == MapMatrix.mapWidth - 1;
        }

        private bool IsClusterFormed(int i, int j)
        {
            int clusterSize = 0;
            if (i > 0 && MapMatrix.GetCellValue(i - 1, j) == 3) clusterSize++;
            if (i < MapMatrix.mapHeight - 1 && MapMatrix.GetCellValue(i + 1, j) == 3) clusterSize++;
            if (j > 0 && MapMatrix.GetCellValue(i, j - 1) == 3) clusterSize++;
            if (j < MapMatrix.mapWidth - 1 && MapMatrix.GetCellValue(i, j + 1) == 3) clusterSize++;
            return clusterSize >= 3; // Ensure at least 3 neighboring tiles are of the same type
        }

        private void RenderMap()
        {
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 0; j < MapMatrix.mapWidth; j++)
                {
                    Vector3Int tilePosition = new Vector3Int(j, MapMatrix.mapHeight - 1 - i, 0);
                    TileBase tile = GetTileFromMatrix(i, j);

                    tilemap.SetTile(tilePosition, tile);
                }
            }

            // Refresh the tilemap to ensure RuleTile rules are applied
            tilemap.RefreshAllTiles();
        }

        private TileBase GetTileFromMatrix(int i, int j)
        {
            switch (MapMatrix.GetCellValue(i, j))
            {
                case 1:
                    return block;
                case 2:
                    return brick;
                case 3:
                    return dust; // RuleTile is a type of TileBase
                default:
                    return null;
            }
        }

        // Phương thức để căn chỉnh bản đồ về trung tâm camera
        private void SetMapToCenter()
        {
            // Lấy thông tin từ Camera chính (Main Camera)
            Camera mainCamera = Camera.main;

            // Tính toán vị trí trung tâm của bản đồ
            Vector3 mapCenter = new Vector3(MapMatrix.mapWidth / 2f, MapMatrix.mapHeight / 2f, 0f);

            // Tính toán vị trí của Camera
            Vector3 cameraPos = mainCamera.transform.position;

            // Di chuyển bản đồ sao cho trung tâm của nó khớp với trung tâm của Camera
            Vector3 mapPosition = new Vector3(cameraPos.x, cameraPos.y, 0);
            tilemap.transform.position = mapPosition - mapCenter * tilemap.cellSize.x;
        }
    }
}

