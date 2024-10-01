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
        Vector3 rayStart = transform.position + Vector3.up * 0.3f;  // ���� �߽ɿ��� �ణ ���ʿ��� Raycast ����
        Vector3 movement = transform.forward * moveSpeed * Time.fixedDeltaTime;

        // ��ֹ� ����
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDistance, obstacle))
        {
            // ȸ�� ���� ���� (���� ��� �߰�)
            Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;

            // ������ ȸ�� ���� �߰�
            //float randomAngle = Random.Range(20f, 50f); 
            avoidanceDirection = Quaternion.AngleAxis(300f, Vector3.up) * avoidanceDirection;

            // ������ ȸ�� ������ �����Ͽ� �� �ް��� ȸ�� ����
            transform.rotation = Quaternion.LookRotation(avoidanceDirection);

            // ȸ�� �������� �̵� ����
            movement = avoidanceDirection * moveSpeed * Time.fixedDeltaTime;
        }

        // �̵� ����
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