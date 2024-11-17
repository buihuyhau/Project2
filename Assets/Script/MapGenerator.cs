using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script
{
    public class MapGenerator : MonoBehaviour
    {
        private const int EmptyTileValue = 0;

        public Tile block;
        public Tile brick;
        public RuleTile dust;
        public Tilemap tilemap;

        private void Start()
        {
            GenerateRandomMap();
            RenderMap();
            SetMapToCenter();
        }

        private void GenerateRandomMap()
        {
            // Đặt các ô trống tại các vị trí xác định
            MapMatrix.SetCellValue(1, 1, EmptyTileValue);
            MapMatrix.SetCellValue(1, 2, EmptyTileValue);
            MapMatrix.SetCellValue(2, 1, EmptyTileValue);

            // Tạo các cụm dust ngẫu nhiên
            CreateRandomDustClusters(5); // Tạo 5 cụm dust ngẫu nhiên

            // Sinh ngẫu nhiên các ô còn lại
            for (int i = 0; i < MapMatrix.mapHeight; i++)
            {
                for (int j = 0; j < MapMatrix.mapWidth; j++)
                {
                    if (IsBorderCell(i, j))
                    {
                        MapMatrix.SetCellValue(i, j, 1); // Viền là ô phá được
                    }
                    else if (MapMatrix.GetCellValue(i, j) == EmptyTileValue)
                    {
                        int value = Random.Range(EmptyTileValue, 3); // Tránh sinh ra dust ngẫu nhiên
                        MapMatrix.SetCellValue(i, j, value);
                    }
                }
            }
        }

        private void CreateRandomDustClusters(int clusterCount)
        {
            for (int k = 0; k < clusterCount; k++)
            {
                int shape = Random.Range(0, 3); // 0: Hình chữ nhật, 1: Hình vuông, 2: Hình chữ L

                int clusterWidth = 0;
                int clusterHeight = 0;

                switch (shape)
                {
                    case 0: // Hình chữ nhật
                        clusterWidth = 2;
                        clusterHeight = 3;
                        break;
                    case 1: // Hình vuông
                        clusterWidth = 2;
                        clusterHeight = 2;
                        break;
                    case 2: // Hình chữ L
                        clusterWidth = 4; // Hình chữ L có chiều rộng tối đa là 4
                        clusterHeight = 4; // Hình chữ L có chiều cao tối đa là 4
                        break;
                }

                // Điều chỉnh phạm vi để tránh vượt quá ranh giới bản đồ
                int startX = Random.Range(1, MapMatrix.mapHeight - clusterHeight - 1);
                int startY = Random.Range(1, MapMatrix.mapWidth - clusterWidth - 1);

                switch (shape)
                {
                    case 0: // Hình chữ nhật
                        CreateDustCluster(startX, startY, 2, 3);
                        break;
                    case 1: // Hình vuông
                        CreateDustCluster(startX, startY, 2, 2);
                        break;
                    case 2: // Hình chữ L
                        CreateDustLShape(startX, startY);
                        break;
                }
            }
        }

        private void CreateDustCluster(int startX, int startY, int width, int height)
        {
            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    MapMatrix.SetCellValue(i, j, 3); // Đặt giá trị 3 cho dust
                }
            }
        }

        private void CreateDustLShape(int startX, int startY)
        {
            // Tạo phần thân chính của hình chữ L (hình vuông 2x2)
            for (int i = startX; i < startX + 2; i++)
            {
                for (int j = startY; j < startY + 2; j++)
                {
                    MapMatrix.SetCellValue(i, j, 3); // Đặt giá trị 3 cho dust
                }
            }

            // Thêm nhánh dọc của hình chữ L với độ rộng 2 ô
            for (int i = startX + 2; i < startX + 4; i++)
            {
                for (int j = startY; j < startY + 2; j++)
                {
                    MapMatrix.SetCellValue(i, j, 3);
                }
            }

            // Thêm nhánh ngang của hình chữ L với độ rộng 2 ô
            for (int i = startX; i < startX + 2; i++)
            {
                for (int j = startY + 2; j < startY + 4; j++)
                {
                    MapMatrix.SetCellValue(i, j, 3);
                }
            }
        }

        private bool IsBorderCell(int i, int j)
        {
            return i == 0 || i == MapMatrix.mapHeight - 1 || j == 0 || j == MapMatrix.mapWidth - 1;
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

            // Làm mới tilemap để đảm bảo các quy tắc của RuleTile được áp dụng
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
                    return dust; // RuleTile là một loại của TileBase
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

