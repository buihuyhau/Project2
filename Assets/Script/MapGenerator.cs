using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Script
{
    public class MapGenerator : MonoBehaviour
    {
        private const int EmptyTileValue = 0;

        public Tile block;
        public Tile brick;
        public RuleTile bush;
        public RuleTile water;
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

            // Tạo các cụm bush ngẫu nhiên
            CreateRandomClusters(3, 2, 3, 3); // Tạo 3 cụm bush ngẫu nhiên

            // Tạo các cụm water ngẫu nhiên
            CreateRandomClusters(2, 3, 4, 4); // Tạo 2 cụm water ngẫu nhiên với kích thước lớn hơn

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
                        int value = Random.Range(EmptyTileValue, 3); // Tránh sinh ra bush và water ngẫu nhiên
                        MapMatrix.SetCellValue(i, j, value);
                    }
                }
            }
        }

        private void CreateRandomClusters(int clusterCount, int minSize, int maxSize, int value)
        {
            for (int k = 0; k < clusterCount; k++)
            {
                int shape = Random.Range(0, 3); // 0: Hình chữ nhật, 1: Hình vuông, 2: Hình chữ L

                int clusterWidth = 0;
                int clusterHeight = 0;

                switch (shape)
                {
                    case 0: // Hình chữ nhật
                        clusterWidth = Random.Range(minSize, maxSize);
                        clusterHeight = Random.Range(minSize, maxSize);
                        break;
                    case 1: // Hình vuông
                        clusterWidth = Random.Range(minSize, maxSize);
                        clusterHeight = clusterWidth;
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
                    case 1: // Hình vuông
                        CreateCluster(startX, startY, clusterWidth, clusterHeight, value);
                        break;
                    case 2: // Hình chữ L
                        CreateLShape(startX, startY, value);
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
                    MapMatrix.SetCellValue(i, j, value); // Đặt giá trị cho bush hoặc water
                }
            }
        }

        private void CreateLShape(int startX, int startY, int value)
        {
            // Tạo phần thân chính của hình chữ L (hình vuông 2x2)
            for (int i = startX; i < startX + 2; i++)
            {
                for (int j = startY; j < startY + 2; j++)
                {
                    MapMatrix.SetCellValue(i, j, value); // Đặt giá trị cho bush hoặc water
                }
            }

            // Thêm nhánh dọc của hình chữ L với độ rộng 2 ô
            for (int i = startX + 2; i < startX + 4; i++)
            {
                for (int j = startY; j < startY + 2; j++)
                {
                    MapMatrix.SetCellValue(i, j, value);
                }
            }

            // Thêm nhánh ngang của hình chữ L với độ rộng 2 ô
            for (int i = startX; i < startX + 2; i++)
            {
                for (int j = startY + 2; j < startY + 4; j++)
                {
                    MapMatrix.SetCellValue(i, j, value);
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
                    return bush; // RuleTile là một loại của TileBase
                case 4:
                    return water; // RuleTile là một loại của TileBase
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

