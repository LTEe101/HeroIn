using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IMotionGameScript
{
    public static EnemyManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3; // �� ������ ���� ��
    [SerializeField] private Vector3[] spawnAreaPositions = new Vector3[2]; // �� ���� ������ ���� ��ġ

    private List<GameObject> activeEnemies = new List<GameObject>(); // Ȱ��ȭ�� �� ����Ʈ
    private int enemiesSpawned = 0; // ������ ���� ��
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

    private void SpawnEnemy()
    {
        if (enemiesSpawned < maxEnemies) // �� ������ ���� ���� �ִ�ġ�� �������� �ʾ��� ���� ����
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
            activeEnemies.Add(enemy); // Ȱ��ȭ�� �� ����Ʈ�� �߰�
            enemiesSpawned++; // ������ ���� �� ����

            // ���� ������ ���� ĵ���� Ȱ��ȭ �õ�
            StartCoroutine(ActivateCanvasImmediately(enemy));
        }
    }

    // ĵ���� Ȱ��ȭ �ڷ�ƾ
    private IEnumerator ActivateCanvasImmediately(GameObject enemy)
    {
        yield return new WaitForEndOfFrame(); // 1������ ��� �� ����

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.ActivateCanvas(); // ���� ������ �� ��� ĵ���� Ȱ��ȭ
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

    // ���� ��ġ ��� (�� ���� ��ġ �� ���� ����)
    private Vector3 GetRandomSpawnPosition()
    {
        // �迭���� �������� �ϳ��� ���� ��ġ ����
        int randomIndex = Random.Range(0, spawnAreaPositions.Length);
        return spawnAreaPositions[randomIndex];
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

        // �� ���� ������ ���� ��ġ�� ������ ǥ��
        foreach (Vector3 spawnPosition in spawnAreaPositions)
        {
            Gizmos.DrawSphere(spawnPosition, 0.5f); // �� ���� ��ġ�� �� ������ ����� �׸���
        }
    }
}
