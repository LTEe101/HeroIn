using System.Collections;
using UnityEngine;

public class AutoMoveForward : MonoBehaviour
{
    public float moveSpeed = 2f;  // 캐릭터의 이동 속도
    private Animator animator;
    private bool isHit = false;

    void Start()
    {
        animator = GetComponent<Animator>();  // Animator 컴포넌트 가져오기
        //animator.SetBool("isRunning", true);  // 시작할 때 애니메이션을 재생
    }

    void Update()
    {
        if (!isHit)
        {
            // 캐릭터의 '앞' 방향으로 이동 (Local 기준의 Z 축 방향)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // 애니메이션 상태 유지 (필요한 경우)
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
        // 움직임 멈추기
        moveSpeed = 0f;

        // 앞으로 넘어지는 애니메이션 재생 (애니메이터에 "Fall" 트리거가 있다고 가정)
        animator.SetTrigger("Die");

        // 4초 후 캐릭터 제거
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}