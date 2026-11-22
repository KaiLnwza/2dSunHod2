using UnityEngine;
using System.Collections.Generic;

public class BossMovement : MonoBehaviour
{
    public enum MovePattern { Idle, CustomPathLoop, ChasePlayer }

    [Header("Movement")]
    public MovePattern currentPattern = MovePattern.CustomPathLoop;

    [Header("Move Line")]
    public Transform pathContainer;
    public float moveSpeed = 5f;
    public bool smoothCurve = true; 

    [Header("FaceTurn")]
    public Transform player;

    private List<Transform> waypoints = new List<Transform>();
    private int currentIndex = 0;
    private float segmentProgress = 0f;
    private Vector3 initialScale;
    private bool canMove = true;

    void Start()
    {
        initialScale = transform.localScale;
        RefreshWaypoints();
    }

    public void RefreshWaypoints()
    {
        waypoints.Clear();
        if (pathContainer == null) return;

        foreach (Transform child in pathContainer)
        {
            waypoints.Add(child);
        }
    }

    void Update()
    {
        FacePlayer();

        if (!canMove) return;

        switch (currentPattern)
        {
            case MovePattern.Idle:
                break;

            case MovePattern.CustomPathLoop:
                MoveAlongCustomPath();
                break;

            case MovePattern.ChasePlayer:
                if (player != null)
                    transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                break;
        }
    }

    void MoveAlongCustomPath()
    {
        if (waypoints.Count < 2) return;

        if (!smoothCurve)
        {
            Transform target = waypoints[currentIndex];
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) < 0.05f)
            {
                currentIndex = (currentIndex + 1) % waypoints.Count;
            }
        }
        else
        {
            int count = waypoints.Count;
            Vector3 p0 = waypoints[(currentIndex - 1 + count) % count].position;
            Vector3 p1 = waypoints[currentIndex].position;
            Vector3 p2 = waypoints[(currentIndex + 1) % count].position;
            Vector3 p3 = waypoints[(currentIndex + 2) % count].position;

            float segmentLength = Vector3.Distance(p1, p2);
            if (segmentLength < 0.01f) segmentLength = 0.01f;

            segmentProgress += (moveSpeed / segmentLength) * Time.deltaTime;

            if (segmentProgress >= 1f)
            {
                segmentProgress -= 1f;
                currentIndex = (currentIndex + 1) % count;
            }
            else
            {
                transform.position = GetCatmullRomPosition(segmentProgress, p0, p1, p2, p3);
            }
        }
    }

    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    void FacePlayer()
    {
        if (player == null) return;
        float dir = player.position.x - transform.position.x;
        if (dir > 0.05f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (dir < -0.05f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    public void SetMovementActive(bool active) => canMove = active;

    public void ChangePattern(MovePattern newPattern) => currentPattern = newPattern;

    void OnDrawGizmos()
    {
        if (pathContainer == null || pathContainer.childCount < 2) return;

        List<Transform> pts = new List<Transform>();
        foreach (Transform child in pathContainer) pts.Add(child);

        int count = pts.Count;
        Gizmos.color = Color.green;

        for (int i = 0; i < count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pts[i].position, 0.2f);

            Gizmos.color = Color.green;
            if (!smoothCurve)
            {
                Gizmos.DrawLine(pts[i].position, pts[(i + 1) % count].position);
            }
            else
            {
                Vector3 p0 = pts[(i - 1 + count) % count].position;
                Vector3 p1 = pts[i].position;
                Vector3 p2 = pts[(i + 1) % count].position;
                Vector3 p3 = pts[(i + 2) % count].position;

                Vector3 prevPos = p1;
                for (int step = 1; step <= 20; step++)
                {
                    float t = step / 20f;
                    Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                    Gizmos.DrawLine(prevPos, newPos);
                    prevPos = newPos;
                }
            }
        }
    }
}