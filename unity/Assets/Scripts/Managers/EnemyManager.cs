using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IMotionGameScript
{
    public static EnemyManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3; // 동시에 존재할 수 있는 적의 최대 수
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 0f, 8f); // 스폰 영역 크기
    [SerializeField] private Vector3 spawnAreaRotation = Vector3.zero; // 스폰 영역 회전값

    private List<GameObject> activeEnemies = new List<GameObject>(); // 활성화된 적 리스트
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

    // 적 생성 메서드
    private void SpawnEnemy()
    {
        if (activeEnemies.Count < maxEnemies) // 최대 적 수를 넘지 않을 때만 생성
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
            activeEnemies.Add(enemy); // 활성화된 적 리스트에 추가
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

    // 스폰 위치 계산
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        // 회전 적용
        randomPoint = Quaternion.Euler(spawnAreaRotation) * randomPoint;

        return transform.position + randomPoint;
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
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(spawnAreaRotation), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, spawnAreaSize); // 스폰 영역을 기즈모로 표시
    }
}
