//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class ArrowShooter : MonoBehaviour
//{
//    [SerializeField] private Animator bowAnimator;
//    [SerializeField] private GameObject existingArrow;
//    [SerializeField] private Transform firePoint;
//    [SerializeField] private float arrowSpeed = 0.02f; // 속도를 더 낮춤
//    [SerializeField] private float searchRadius = 30f;
//    [SerializeField] private float arrowLifetime = 5f;

//    private Rigidbody arrowRb;
//    private LineRenderer trajectoryLine;
//    private Transform currentTarget;
//    private Vector3 lastTargetPosition;

//    void Start()
//    {
//        SetupArrow();
//        SetupTrajectoryLine();
//    }

//    void Update()
//    {
//        currentTarget = FindClosestTarget();

//        if (currentTarget != null)
//        {
//            lastTargetPosition = currentTarget.position;
//            Debug.DrawLine(firePoint.position, lastTargetPosition, Color.red);
//        }
//    }

//    private void SetupArrow()
//    {
//        if (existingArrow != null)
//        {
//            arrowRb = existingArrow.GetComponent<Rigidbody>();
//            if (arrowRb == null)
//            {
//                Debug.LogError("No Rigidbody found on the arrow!");
//            }
//        }
//        else
//        {
//            Debug.LogError("Existing arrow not assigned in the inspector!");
//        }
//    }

//    private void SetupTrajectoryLine()
//    {
//        trajectoryLine = gameObject.AddComponent<LineRenderer>();
//        trajectoryLine.startWidth = 0.05f;
//        trajectoryLine.endWidth = 0.05f;
//        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
//        trajectoryLine.startColor = Color.blue;
//        trajectoryLine.endColor = Color.blue;
//    }

//    public void Shoot()
//    {
//        if (existingArrow == null || arrowRb == null)
//        {
//            Debug.LogError("Arrow or its Rigidbody is missing!");
//            return;
//        }

//        existingArrow.transform.position = firePoint.position;
//        existingArrow.transform.rotation = firePoint.rotation;

//        arrowRb.isKinematic = true; // 물리 시뮬레이션을 비활성화
//        arrowRb.useGravity = false;

//        if (bowAnimator != null)
//        {
//            bowAnimator.SetTrigger("Shoot");
//        }

//        Vector3 targetPosition = lastTargetPosition;

//        // 화살의 초기 방향을 설정
//        Vector3 direction = (targetPosition - firePoint.position).normalized;
//        existingArrow.transform.forward = direction;

//        // 등속 운동으로 화살 발사
//        StartCoroutine(MoveArrow(targetPosition));

//        Debug.Log($"Arrow fired. Direction: {direction}, Target: {targetPosition}");
//    }

//    private IEnumerator MoveArrow(Vector3 targetPosition)
//    {
//        float distanceTraveled = 0f;
//        float totalDistance = Vector3.Distance(firePoint.position, targetPosition);
//        Vector3 startPosition = firePoint.position;
//        //List<Vector3> trajectoryPoints = new List<Vector3>();

//        while (distanceTraveled < totalDistance)
//        {
//            float t = distanceTraveled / totalDistance;
//            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);

//            existingArrow.transform.position = newPosition;

//            // 화살의 방향을 현재 이동 방향으로 부드럽게 조정
//            Vector3 direction = (targetPosition - existingArrow.transform.position).normalized;
//            existingArrow.transform.rotation = Quaternion.Slerp(existingArrow.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

//            // 궤적 추가
//            //trajectoryPoints.Add(newPosition);
//            //trajectoryLine.positionCount = trajectoryPoints.Count;
//            //trajectoryLine.SetPositions(trajectoryPoints.ToArray());

//            distanceTraveled += arrowSpeed * Time.deltaTime;
//            yield return null;
//        }

//        // 목표 지점에 도달하면 화살 멈춤
//        existingArrow.transform.position = targetPosition;

//        yield return new WaitForSeconds(arrowLifetime);
//        ResetArrow();
//        trajectoryLine.positionCount = 0; // 궤적 초기화
//    }

//    private void ResetArrow()
//    {
//        existingArrow.transform.position = firePoint.position;
//        existingArrow.transform.rotation = firePoint.rotation;
//        arrowRb.velocity = Vector3.zero;
//        arrowRb.isKinematic = true;
//    }

//    private Transform FindClosestTarget()
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
//        Transform closestTarget = null;
//        float closestDistance = Mathf.Infinity;

//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                float distance = Vector3.Distance(firePoint.position, hitCollider.transform.position);
//                if (distance < closestDistance)
//                {
//                    closestDistance = distance;
//                    closestTarget = hitCollider.transform;
//                }
//            }
//        }

//        return closestTarget;
//    }

//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, searchRadius);

//        if (currentTarget != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(firePoint.position, currentTarget.position);
//            Gizmos.DrawSphere(currentTarget.position, 0.5f);
//        }
//    }
//}



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject existingArrow;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 1f; // 속도를 더 낮춤
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private float arrowLifetime = 1.3f;

    private Rigidbody arrowRb;
    private LineRenderer trajectoryLine;
    private Transform currentTarget;
    private Vector3 lastTargetPosition;

    void Start()
    {
        SetupArrow();
        SetupTrajectoryLine();
    }

    void Update()
    {
        currentTarget = FindClosestTarget();

        if (currentTarget != null)
        {
            lastTargetPosition = currentTarget.position;
            Debug.DrawLine(firePoint.position, lastTargetPosition, Color.red);
        }
    }

    private void SetupArrow()
    {
        if (existingArrow != null)
        {
            arrowRb = existingArrow.GetComponent<Rigidbody>();
            if (arrowRb == null)
            {
                Debug.LogError("No Rigidbody found on the arrow!");
            }
        }
        else
        {
            Debug.LogError("Existing arrow not assigned in the inspector!");
        }
    }

    private void SetupTrajectoryLine()
    {
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.blue;
        trajectoryLine.endColor = Color.blue;
    }

    public void Shoot()
    {
        if (existingArrow == null || arrowRb == null)
        {
            Debug.LogError("Arrow or its Rigidbody is missing!");
            return;
        }

        existingArrow.transform.position = firePoint.position;
        existingArrow.transform.rotation = firePoint.rotation;

        arrowRb.isKinematic = true; // 물리 시뮬레이션을 비활성화
        arrowRb.useGravity = false;

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("Shoot");
        }

        Vector3 targetPosition = lastTargetPosition;

        // 화살의 초기 방향을 설정
        Vector3 direction = (targetPosition - firePoint.position).normalized;
        existingArrow.transform.forward = direction;

        // 등속 운동으로 화살 발사
        StartCoroutine(MoveArrow(targetPosition));

        Debug.Log($"Arrow fired. Direction: {direction}, Target: {targetPosition}");
    }

    private IEnumerator MoveArrow(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float totalTime = 0.8f; // 화살이 목표에 도달하는 데 걸리는 총 시간
        Vector3 startPosition = firePoint.position;
        //List<Vector3> trajectoryPoints = new List<Vector3>();

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            //Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
            existingArrow.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 화살의 방향 조정
            Vector3 direction = (targetPosition - existingArrow.transform.position).normalized;
            existingArrow.transform.rotation = Quaternion.Slerp(existingArrow.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2f);


            // 궤적 추가
            //trajectoryPoints.Add(newPosition);
            //trajectoryLine.positionCount = trajectoryPoints.Count;
            //trajectoryLine.SetPositions(trajectoryPoints.ToArray());

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 목표 지점에 도달하면 화살 멈춤
        existingArrow.transform.position = targetPosition;

        yield return new WaitForSeconds(arrowLifetime);
        ResetArrow();
        trajectoryLine.positionCount = 0; // 궤적 초기화
    }

    private void ResetArrow()
    {
        
        existingArrow.transform.position = firePoint.position;
        existingArrow.transform.rotation = firePoint.rotation;
        arrowRb.velocity = Vector3.zero;
        arrowRb.isKinematic = true;
    }

    private Transform FindClosestTarget()
    {
        return EnemyManager.Instance.GetNearestEnemy(transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
            Gizmos.DrawSphere(currentTarget.position, 0.5f);
        }
    }
}