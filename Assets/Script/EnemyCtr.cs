using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtr : MonoBehaviour
{
    private List<Vector2> PathToBomberMan = new List<Vector2>();
    private List<Vector2> RandomPath = new List<Vector2>();
    private List<Vector2> CurrentPath = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMoving;

    public GameObject BomberMan;
    public GameObject ItemPrefab;
    public float MoveSpeed;
    public float AttackRange;
    public float PatrolDistance;

    private Vector2 patrolPoint;
    private bool patrolForward = true;

    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererMov;
    public AnimatedSpriteRenderer spriteRendererDeath;

    void Awake()
    {
        PathFinder = GetComponent<PathFinder>();
        SetInitialPatrolPoint();
        isMoving = true;
    }

    void SetInitialPatrolPoint()
    {
        patrolPoint = (Vector2)transform.position + Vector2.right * PatrolDistance;
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
            RandomPath = GeneratePatrolPath();
            CurrentPath = RandomPath;
        }
        else
        {
            CurrentPath = PathToBomberMan;
        }
    }

    List<Vector2> GeneratePatrolPath()
    {
        List<Vector2> patrolPath = new List<Vector2>();
        patrolPath.Add(patrolPoint);
        patrolPath.Add((Vector2)transform.position);
        return patrolPath;
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
        isMoving = false;
        spriteRendererMov.enabled = false;
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

        if (BomberMan == null)
        {
            Patrol();
            return;
        }

        ReCalculatePath();

        if (CurrentPath.Count == 0)
        {
            Patrol();
            return;
        }

        MoveAlongPath();
    }

    void Patrol()
    {
        if (CurrentPath.Count == 0)
        {
            SetInitialPatrolPoint();
            CurrentPath = GeneratePatrolPath();
        }

        if (isMoving && CurrentPath.Count > 0)
        {
            Vector2 target = CurrentPath[CurrentPath.Count - 1];
            if (Vector2.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
            }
            else
            {
                CurrentPath.RemoveAt(CurrentPath.Count - 1);
                patrolForward = !patrolForward;
            }
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
}
