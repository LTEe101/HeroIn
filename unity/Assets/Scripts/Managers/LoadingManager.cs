using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    // ���� ���� �񵿱������� �ε�
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return null;
        // _time ���� �ʱ�ȭ
        float time = 0f;

        // �񵿱������� ���� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("�� �ε� ����: " + sceneName);

        // �� �ε� �Ϸ� �� �ڵ����� ���� Ȱ��ȭ���� �ʵ��� ����
        operation.allowSceneActivation = false;
        Debug.Log("allowSceneActivation�� false�� ����");

        // �� �ε� ���� ���� Ȯ�� (0.9f����)
        while (operation.progress < 0.9f)
        {
            Debug.Log("�ε� �����: " + (operation.progress * 100) + "%");
            yield return null;
        }

        Debug.Log("�ε� ����� 90% ����");

        // ���ϴ� ��� �ð� (��: 1��)
        while (time < 5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("��� �ð� �Ϸ�. �� Ȱ��ȭ");

        // �� Ȱ��ȭ
        operation.allowSceneActivation = true;
        Debug.Log("allowSceneActivation�� true�� �����Ͽ� �� Ȱ��ȭ");
    }
 }   
