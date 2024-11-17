using Assets.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BombController : MonoBehaviour
{
    [Header("Bomb")]
    public GameObject bombPrefab;
    public float bombFuseTime = 3f;
    public int bombAmount = 1;
    private int bombsRemaining;

    [Header("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;

    [Header("Destructible")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;

    [Header("UI Button")]
    public Button bombButton;  // Reference to the button UI element

    public void OnEnable()
    {
        bombsRemaining = bombAmount;

        // Set up button click listener
        if (bombButton != null)
            bombButton.onClick.AddListener(OnBombButtonClick);
    }

    public void OnDisable()
    {
        // Remove button click listener to avoid memory leaks
        if (bombButton != null)
            bombButton.onClick.RemoveListener(OnBombButtonClick);
    }

    // This method is called when the button is clicked
    public void OnBombButtonClick()
    {
        if (bombsRemaining > 0)
            StartCoroutine(PlaceBomb());
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;

        GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        bombsRemaining--;

        yield return new WaitForSeconds(bombFuseTime);

        position = bomb.transform.position;
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.5f;

        // Instantiate the explosion effect at the bomb's position
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        // Check and create explosions in four directions
        ExplodeInDirection(position, Vector2.up, explosionRadius);   // Explode upward
        ExplodeInDirection(position, Vector2.down, explosionRadius); // Explode downward
        ExplodeInDirection(position, Vector2.left, explosionRadius); // Explode left
        ExplodeInDirection(position, Vector2.right, explosionRadius); // Explode right

        Destroy(bomb);  // Destroy the bomb after the explosion
        bombsRemaining++;
    }

    private void ExplodeInDirection(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
            return;

        position += direction;

        // Chuyển đổi tọa độ từ Tilemap sang ma trận (góc trên bên trái)
        Vector3Int cell = destructibleTiles.WorldToCell(position);

        // Lấy tọa độ y của Tilemap, nhưng điều chỉnh sao cho phù hợp với ma trận (góc trên bên trái)
        int matrixY = MapMatrix.mapHeight - cell.y - 1; // Chuyển đổi tọa độ Y

        // Kiểm tra giá trị của ô trong ma trận
        int cellValue = MapMatrix.GetCellValue(matrixY, cell.x); // Lấy giá trị ô từ ma trận

        // Kiểm tra nếu ô không phải null và giá trị không phải là 1 (có thể phá hủy)
        if (cellValue != 1) // Ví dụ: 1 là ô không thể phá hủy
        {
            // Tạo hiệu ứng vụ nổ tại ô có thể phá hủy
            Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
            explosion.SetDirection(direction);
            explosion.DestroyAfter(explosionDuration);

            // Cập nhật ma trận (thay thế giá trị trong ma trận)
            MapMatrix.SetCellValue(matrixY, cell.x, 0); // Cập nhật giá trị ô trong ma trận (ví dụ: đổi thành 0)

            // Xóa ô trong Tilemap
            ClearDestructible(position);
        }

        // Tiếp tục nổ ở các ô phía sau trong cùng hướng
        ExplodeInDirection(position, direction, length - 1);
    }

    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        // Kiểm tra nếu tile không phải null (và có thể phá hủy)
        if (tile != null)
        {
            destructibleTiles.SetTile(cell, null);  // Xóa tile trong Tilemap
            Instantiate(destructiblePrefab, position, Quaternion.identity);  // Tạo hiệu ứng phá hủy (nếu có)
        }
    }

    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
            other.isTrigger = false;
    }
}
