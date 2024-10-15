using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public float avoidanceDistance = 1f;
    public LayerMask obstacleLayer;
    public GameObject enemyCanvas; // ĵ������ ���⼭ ����

    private Animator animator;
    private bool isHit = false;
    private Rigidbody rb;

    public event System.Action<GameObject> OnEnemyDeath;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyCanvas.SetActive(false); // �⺻������ ĵ������ ��Ȱ��ȭ
    }

    void FixedUpdate()
    {
        if (!isHit)
        {
            Move();
        }
    }

    // ���� ����� ���� Ȱ��ȭ�� �� ȣ���� �޼���
    public void ActivateCanvas()
    {
        enemyCanvas.SetActive(true); // ĵ������ Ȱ��ȭ
    }

    // ���� ��Ȱ��ȭ�� �� ȣ���� �޼���
    public void DeactivateCanvas()
    {
        enemyCanvas.SetActive(false); // ĵ������ ��Ȱ��ȭ
    }

    void Move()
    {
        Vector3 movement = transform.forward * moveSpeed * Time.fixedDeltaTime;

        // ��ֹ� ����
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, avoidanceDistance, obstacleLayer))
        {
            // ��ֹ��� �����ϸ� ȸ��
            Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                         Quaternion.LookRotation(avoidanceDirection),
                                                         rotationSpeed * Time.fixedDeltaTime);
        }

        // �̵� ����
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
        enemyCanvas.SetActive(false); // ȭ�쿡 ������ ĵ���� ��Ȱ��ȭ
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
