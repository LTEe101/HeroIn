using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IMotionGameScript
{
    public static EnemyManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3; // 총 생성할 적의 수
    [SerializeField] private Vector3[] spawnAreaPositions = new Vector3[2]; // 두 개의 고정된 스폰 위치

    private List<GameObject> activeEnemies = new List<GameObject>(); // 활성화된 적 리스트
    private int enemiesSpawned = 0; // 생성된 적의 수
    public System.Action onEnemyDestroyed; // 적이 제거될 때 호출될 콜백

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SpawnEnemy(); // 게임 시작 시 한 명의 적만 생성
    }

    private void SpawnEnemy()
    {
        if (enemiesSpawned < maxEnemies) // 총 생성된 적의 수가 최대치에 도달하지 않았을 때만 생성
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
            activeEnemies.Add(enemy); // 활성화된 적 리스트에 추가
            enemiesSpawned++; // 생성된 적의 수 증가

            // 적이 생성된 직후 캔버스 활성화 시도
            StartCoroutine(ActivateCanvasImmediately(enemy));
        }
    }

    // 캔버스 활성화 코루틴
    private IEnumerator ActivateCanvasImmediately(GameObject enemy)
    {
        yield return new WaitForEndOfFrame(); // 1프레임 대기 후 실행

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.ActivateCanvas(); // 적이 생성된 후 즉시 캔버스 활성화
        }
    }

    // 적 제거 메서드
    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy); // 리스트에서 적 제거
            Destroy(enemy); // 적 오브젝트 제거

            // 적이 제거되면 새로운 적 생성
            SpawnEnemy();

            // 적이 제거되면 점수 업데이트 콜백 호출
            if (onEnemyDestroyed != null)
            {
                onEnemyDestroyed.Invoke(); // 콜백 호출
            }
        }
    }

    // 스폰 위치 계산 (두 개의 위치 중 랜덤 선택)
    private Vector3 GetRandomSpawnPosition()
    {
        // 배열에서 무작위로 하나의 스폰 위치 선택
        int randomIndex = Random.Range(0, spawnAreaPositions.Length);
        return spawnAreaPositions[randomIndex];
    }

    // 가장 가까운 적 반환
    public Transform GetNearestEnemy(Vector3 position)
    {
        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in activeEnemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    // 스폰 영역 시각화를 위한 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 두 개의 고정된 스폰 위치를 기즈모로 표시
        foreach (Vector3 spawnPosition in spawnAreaPositions)
        {
            Gizmos.DrawSphere(spawnPosition, 0.5f); // 각 스폰 위치에 구 형태의 기즈모 그리기
        }
    }
}
