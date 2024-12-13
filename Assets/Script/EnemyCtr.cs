using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtr : MonoBehaviour
{
    private List<Vector2> PathToBomberMan = new List<Vector2>();
    private List<Vector2> CurrentPath = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMoving;

    public GameObject BomberMan;
    public GameObject ItemPrefab;
    public float MoveSpeed = 0.5f;
    public float AttackRange;

    private Vector2 currentDirection;
    //private bool movingForward = true;

    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererMov;
    public AnimatedSpriteRenderer spriteRendererDeath;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    void Awake()
    {
        PathFinder = GetComponent<PathFinder>();
        activeSpriteRenderer = spriteRendererMov;
        isMoving = true;
        MoveSpeed = 0.5f;
        currentDirection = Vector2.right; // Bắt đầu di chuyển theo hướng phải
    }

    public void ReCalculatePath()
    {
        if (BomberMan != null && Vector2.Distance(transform.position, BomberMan.transform.position) <= AttackRange)
        {
            PathResult pathResult = PathFinder.GetPath(BomberMan.transform.position);
            PathToBomberMan = pathResult.Path; // Assuming PathResult has a property Path of type List<Vector2>
        }

        if (PathToBomberMan.Count == 0)
        {
            CurrentPath.Clear();
        }
        else
        {
            CurrentPath = PathToBomberMan;
        }
    }

    public void Damage(int source)
    {
        if (source == 1)
        {
            DeathSequence();
        }
    }

    void DeathSequence()
    {
        enabled = false;
        isMoving = false;
        spriteRendererMov.enabled = enabled;
        spriteRendererDeath.enabled = true;

        DropItem();
        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    void OnDeathSequenceEnded()
    {
        Destroy(gameObject);
    }

    void DropItem()
    {
        Instantiate(ItemPrefab, transform.position, Quaternion.identity);
    }

    void Update()
    {
        if (!isMoving) return;

        ReCalculatePath();

        if (CurrentPath.Count == 0)
        {
            Patrol();
            return;
        }
        spriteRendererMov.enabled = true;
        activeSpriteRenderer = spriteRendererMov;
        activeSpriteRenderer.idle = false; // Luôn giữ idle là false để hiệu ứng chuyển động luôn hoạt động
        MoveAlongPath();
    }

    void Patrol()
    {
        Vector2 currentPosition = transform.position;
        Vector2Int matrixPosition = WorldToMatrixPosition(currentPosition);
        Vector2Int nextMatrixPosition = matrixPosition + Vector2Int.RoundToInt(currentDirection);

        if (IsPositionWalkable(nextMatrixPosition))
        {
            Vector2 nextPosition = MatrixToWorldPosition(nextMatrixPosition);
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, MoveSpeed * Time.deltaTime);
        }
        else
        {
            // Đổi hướng khi gặp chướng ngại vật hoặc cuối đường
            currentDirection = GetNextDirection(matrixPosition);
        }
    }

    void MoveAlongPath()
    {
        if (CurrentPath.Count == 0) return;

        Vector2 target = CurrentPath[CurrentPath.Count - 1];
        if (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
        }
        else
        {
            CurrentPath.RemoveAt(CurrentPath.Count - 1);
        }
    }

    bool IsPositionWalkable(Vector2Int matrixPosition)
    {
        // Kiểm tra xem vị trí có thể đi được không (không có chướng ngại vật)
        return MapMatrix.GetCellValue(matrixPosition.x, matrixPosition.y) == (int)TileTypes.Empty;
    }

    Vector2Int GetNextDirection(Vector2Int matrixPosition)
    {
        // Kiểm tra các hướng theo thứ tự: phải, trái, lên, xuống
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (var direction in directions)
        {
            Vector2Int nextMatrixPosition = matrixPosition + direction;
            if (IsPositionWalkable(nextMatrixPosition))
            {
                return new Vector2Int( -direction.y, direction.x);
            }
        }

        // Nếu không có hướng nào đi được, quay lại hướng ngược lại
        return -Vector2Int.RoundToInt(currentDirection);
    }

    Vector2Int WorldToMatrixPosition(Vector2 worldPosition)
    {
        // Chuyển đổi tọa độ thế giới sang tọa độ ma trận
        return new Vector2Int(MapMatrix.mapHeight - Mathf.RoundToInt(worldPosition.y + 9.5f),  Mathf.RoundToInt(worldPosition.x + 15.5f));
    }

    Vector2 MatrixToWorldPosition(Vector2Int matrixPosition)
    {
        // Chuyển đổi tọa độ ma trận sang tọa độ thế giới
        return new Vector2(matrixPosition.y - 15.5f,MapMatrix.mapHeight - matrixPosition.x - 9.5f);
    }
}
