using System.Collections;
using UnityEngine;

public class AutoMoveForward : MonoBehaviour
{
    public float moveSpeed = 2f;  // ĳ������ �̵� �ӵ�
    private Animator animator;
    private bool isHit = false;

    void Start()
    {
        animator = GetComponent<Animator>();  // Animator ������Ʈ ��������
        //animator.SetBool("isRunning", true);  // ������ �� �ִϸ��̼��� ���
    }

    void Update()
    {
        if (!isHit)
        {
            // ĳ������ '��' �������� �̵� (Local ������ Z �� ����)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // �ִϸ��̼� ���� ���� (�ʿ��� ���)
            //animator.SetBool("isRunning", true);
        }
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
        // ������ ���߱�
        moveSpeed = 0f;

        // ������ �Ѿ����� �ִϸ��̼� ��� (�ִϸ����Ϳ� "Fall" Ʈ���Ű� �ִٰ� ����)
        animator.SetTrigger("Die");

        // 4�� �� ĳ���� ����
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}