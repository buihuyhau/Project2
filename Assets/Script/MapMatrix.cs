using UnityEngine;

namespace Assets.Script
{
    public static class MapMatrix
    {
        public const int mapHeight = 18; // Kích thước cố định của ma trận
        public const int mapWidth = 32;  // Kích thước cố định của ma trận

        private static int[,] mapMatrix = new int[mapHeight, mapWidth];

        // Phương thức để thay đổi giá trị của ma trận tại vị trí (i, j)
        public static void SetCellValue(int i, int j, int value)
        {
            if (IsValidPosition(i, j))
            {
                mapMatrix[i, j] = value;
            }
            else
            {
                Debug.LogError($"Vị trí ({i},{j}) ngoài phạm vi ma trận.");
            }
        }

        // Phương thức để lấy giá trị của ô tại vị trí (i, j)
        public static int GetCellValue(int i, int j)
        {
            if (IsValidPosition(i, j))
            {
                return mapMatrix[i, j];
            }
            else
            {
                Debug.LogError($"Vị trí ({i},{j}) ngoài phạm vi ma trận.");
                return -1;
            }
        }

        //// Lưu ma trận vào file
        //public static void SaveToFile(string filePath)
        //{
        //    using (var writer = new StreamWriter(filePath))
        //    {
        //        for (int i = 0; i < mapHeight; i++)
        //        {
        //            var line = new StringBuilder();
        //            for (int j = 0; j < mapWidth; j++)
        //            {
        //                line.Append(mapMatrix[i, j]);
        //                if (j < mapWidth - 1)
        //                {
        //                    line.Append(",");
        //                }
        //            }
        //            writer.WriteLine(line.ToString());
        //        }
        //    }
        //}

        //// Tải ma trận từ file
        //public static void LoadFromFile(string filePath)
        //{
        //    if (File.Exists(filePath))
        //    {
        //        var lines = File.ReadAllLines(filePath);
        //        for (int i = 0; i < mapHeight; i++)
        //        {
        //            var values = lines[i].Split(',');
        //            for (int j = 0; j < mapWidth; j++)
        //            {
        //                mapMatrix[i, j] = int.Parse(values[j]);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("File không tồn tại.");
        //    }
        //}

        // Kiểm tra xem vị trí (i, j) có nằm trong phạm vi của ma trận không
        private static bool IsValidPosition(int i, int j)
        {
            return i >= 0 && i < mapHeight && j >= 0 && j < mapWidth;
        }
    }
}

