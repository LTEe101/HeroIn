using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IMotionGameScript
{
    public static EnemyManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 0f, 8f);
    [SerializeField] private Vector3 spawnAreaOffset = new Vector3(-20f, 0f, -20f);
    [SerializeField] private Vector3 spawnAreaRotation = Vector3.zero; // 스폰 영역의 회전값

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> runningEnemies = new List<GameObject>();

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


    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (runningEnemies.Count < maxEnemies)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
            runningEnemies.Add(enemy);
            activeEnemies.Add(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        SpawnEnemy(); // 새로운 적 생성
    }

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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(spawnAreaRotation), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, spawnAreaSize);
    }
}