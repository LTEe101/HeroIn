using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public float avoidanceDistance = 1f;
    public LayerMask obstacleLayer;
    public GameObject enemyCanvas; // 캔버스를 여기서 참조

    private Animator animator;
    private bool isHit = false;
    private Rigidbody rb;

    public event System.Action<GameObject> OnEnemyDeath;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyCanvas.SetActive(false); // 기본적으로 캔버스를 비활성화
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            Move();
        }
    }

    // 가장 가까운 적이 활성화될 때 호출할 메서드
    public void ActivateCanvas()
    {
        enemyCanvas.SetActive(true); // 캔버스를 활성화
    }

    // 적이 비활성화될 때 호출할 메서드
    public void DeactivateCanvas()
    {
        enemyCanvas.SetActive(false); // 캔버스를 비활성화
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
            collision.gameObject.SetActive(false);
            HitByArrow();
        }
    }

    private void HitByArrow()
    {
        rb.velocity = Vector3.zero;
        enemyCanvas.SetActive(false); // 화살에 맞으면 캔버스 비활성화
        animator.SetTrigger("Die");
        OnEnemyDeath?.Invoke(gameObject);
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        DeactivateCanvas();
        yield return new WaitForSeconds(3f);
        EnemyManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}
