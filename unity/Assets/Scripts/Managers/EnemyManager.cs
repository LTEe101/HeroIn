using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IMotionGameScript
{
    public static EnemyManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3; // ���ÿ� ������ �� �ִ� ���� �ִ� ��
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 0f, 8f); // ���� ���� ũ��
    [SerializeField] private Vector3 spawnAreaRotation = Vector3.zero; // ���� ���� ȸ����

    private List<GameObject> activeEnemies = new List<GameObject>(); // Ȱ��ȭ�� �� ����Ʈ
    public System.Action onEnemyDestroyed; // ���� ���ŵ� �� ȣ��� �ݹ�

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
        SpawnEnemy(); // ���� ���� �� �� ���� ���� ����
    }

    // �� ���� �޼���
    private void SpawnEnemy()
    {
        if (activeEnemies.Count < maxEnemies) // �ִ� �� ���� ���� ���� ���� ����
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
            activeEnemies.Add(enemy); // Ȱ��ȭ�� �� ����Ʈ�� �߰�
        }
    }

    // �� ���� �޼���
    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy); // ����Ʈ���� �� ����
            Destroy(enemy); // �� ������Ʈ ����

            // ���� ���ŵǸ� ���ο� �� ����
            SpawnEnemy();

            // ���� ���ŵǸ� ���� ������Ʈ �ݹ� ȣ��
            if (onEnemyDestroyed != null)
            {
                onEnemyDestroyed.Invoke(); // �ݹ� ȣ��
            }
        }
    }

    // ���� ��ġ ���
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        // ȸ�� ����
        randomPoint = Quaternion.Euler(spawnAreaRotation) * randomPoint;

        return transform.position + randomPoint;
    }

    // ���� ����� �� ��ȯ
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

    // ���� ���� �ð�ȭ�� ���� ����� �׸���
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(spawnAreaRotation), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, spawnAreaSize); // ���� ������ ������ ǥ��
    }
}
