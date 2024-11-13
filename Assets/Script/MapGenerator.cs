using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Các thông số của bản đồ
    public int mapHeight = 18; // Số hàng của bản đồ
    public int mapWidth = 32;  // Số cột của bản đồ
    public float tileHeight = 1f; // Chiều cao mỗi ô trong bản đồ
    public float tileWidth = 1f;  // Chiều rộng mỗi ô trong bản đồ

    public GameObject destructibleTilePrefab;  // Prefab cho ô phá được
    public GameObject indestructibleImpassableTilePrefab; // Prefab cho ô không phá được và không đi qua được
    public GameObject indestructiblePassableTilePrefab; // Prefab cho ô không phá được nhưng có thể đi qua được
    public GameObject borderTilePrefab;  // Prefab cho viền của bản đồ

    private int[,] mapMatrix; // Ma trận lưu trữ trạng thái các ô

    private void Start()
    {
        GenerateMap();  // Tạo bản đồ khi trò chơi bắt đầu
        CenterCamera();  // Căn giữa camera khi khởi tạo
    }

    private void GenerateMap()
    {
        // Khởi tạo ma trận (18x32) với giá trị mặc định là ô không phá được và không thể đi qua
        mapMatrix = new int[mapHeight, mapWidth];

        // Sinh ma trận với các loại ô:
        // 0 - Ô phá được
        // 1 - Ô không phá được và không đi qua được
        // 2 - Ô không phá được nhưng có thể đi qua
        // 3 - Viền của bản đồ (không phá được và không đi qua được)

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (i == 0 || i == mapHeight - 1 || j == 0 || j == mapWidth - 1)
                {
                    mapMatrix[i, j] = 3; // Viền
                }
                else
                {
                    mapMatrix[i, j] = Random.Range(0, 3); // Sinh ô ngẫu nhiên: 0, 1, hoặc 2
                }
            }
        }

        // Tạo các ô trên bản đồ dựa vào ma trận
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Vector3 position = new Vector3(j * tileWidth, i * tileHeight, 0); // Tính vị trí của mỗi ô

                GameObject tilePrefab = null;

                // Chọn prefab tùy theo giá trị trong ma trận
                switch (mapMatrix[i, j])
                {
                    case 0:
                        tilePrefab = destructibleTilePrefab;
                        break;
                    case 1:
                        tilePrefab = indestructibleImpassableTilePrefab;
                        break;
                    case 2:
                        tilePrefab = indestructiblePassableTilePrefab;
                        break;
                    case 3:
                        tilePrefab = borderTilePrefab;
                        break;
                }

                // Instantiate (tạo) ô tại vị trí đã tính toán
                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, position, Quaternion.identity);
                }
            }
        }
    }

    private void CenterCamera()
    {
        // Tính toán tỷ lệ khung hình của màn hình
        float aspectRatio = (float)Screen.width / Screen.height;

        // Điều chỉnh camera để hiển thị toàn bộ bản đồ
        Camera.main.orthographicSize = (mapHeight * tileHeight) / 2f;

        // Căn giữa camera trên bản đồ
        Camera.main.transform.position = new Vector3(
            (mapWidth * tileWidth) / 2f,  // Vị trí trung tâm trên trục X
            (mapHeight * tileHeight) / 2f, // Vị trí trung tâm trên trục Y
            -10f                          // Vị trí của camera trên trục Z (giữ camera ở phía trước bản đồ)
        );
    }
}
