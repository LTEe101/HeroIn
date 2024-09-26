using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public float avoidanceDistance = 1f;
    public LayerMask obstacleLayer;

    private Animator animator;
    private bool isHit = false;
    private Rigidbody rb;

    public event System.Action<GameObject> OnEnemyDeath;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 movement = transform.forward * moveSpeed * Time.fixedDeltaTime;

        // 장애물 감지
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDistance, obstacleLayer))
        {
            // 장애물을 감지하면 회전
            Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                         Quaternion.LookRotation(avoidanceDirection),
                                                         rotationSpeed * Time.fixedDeltaTime);
        }

        // 이동 적용
        rb.MovePosition(rb.position + movement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow") && !isHit)
        {
            isHit = true;
            HitByArrow();
        }
    }

    private void HitByArrow()
    {
        rb.velocity = Vector3.zero;
        animator.SetTrigger("Die");
        OnEnemyDeath?.Invoke(gameObject);
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        EnemyManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}