using UnityEngine;

namespace Assets.Script
{
    public static class MapMatrix
    {
        public const int mapHeight = 18;
        public const int mapWidth = 32;

        private static int[,] mapMatrix = new int[mapHeight, mapWidth];

        public static void SetCellValue(int i, int j, int value)
        {
            if (IsValidPosition(i, j))
            {
                mapMatrix[i, j] = value;
            }
            else
            {
                Debug.LogError($"Position ({i},{j}) is out of bounds.");
            }
        }

        public static int GetCellValue(int i, int j)
        {
            if (IsValidPosition(i, j))
            {
                return mapMatrix[i, j];
            }
            else
            {
                Debug.LogError($"Position ({i},{j}) is out of bounds.");
                return (int)TileTypes.Empty; // Return Empty as default
            }
        }

        private static bool IsValidPosition(int i, int j)
        {
            return i >= 0 && i < mapHeight && j >= 0 && j < mapWidth;
        }
    }
}

