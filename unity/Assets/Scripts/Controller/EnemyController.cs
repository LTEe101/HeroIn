using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public float avoidanceDistance = 1f;
    public LayerMask obstacle;

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
        Vector3 rayStart = transform.position + Vector3.up * 0.3f;  // 적의 중심에서 약간 위쪽에서 Raycast 시작
        Vector3 movement = transform.forward * moveSpeed * Time.fixedDeltaTime;

        // 장애물 감지
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDistance, obstacle))
        {
            // 회피 방향 결정 (랜덤 요소 추가)
            Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;

            // 랜덤한 회전 각도 추가
            //float randomAngle = Random.Range(20f, 50f); 
            avoidanceDirection = Quaternion.AngleAxis(300f, Vector3.up) * avoidanceDirection;

            // 강제로 회전 각도를 변경하여 더 급격한 회전 적용
            transform.rotation = Quaternion.LookRotation(avoidanceDirection);

            // 회피 방향으로 이동 적용
            movement = avoidanceDirection * moveSpeed * Time.fixedDeltaTime;
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