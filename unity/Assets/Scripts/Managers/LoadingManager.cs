using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public List<string> soundPaths = new List<string>
    {
        "2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Machine Movement 03",
        "2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Machine Movement 03",
        "2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Machine Movement 03",
        "2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Servo Motor Power Up Long 03",
        "2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Servo Motor Power Down Long 04"
    };
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

        yield return StartCoroutine(PlaySoundEffects());

        // ���ϴ� ��� �ð� (��: 1��)
        while (time < 3f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("��� �ð� �Ϸ�. �� Ȱ��ȭ");

        // �� Ȱ��ȭ
        operation.allowSceneActivation = true;
        Debug.Log("allowSceneActivation�� true�� �����Ͽ� �� Ȱ��ȭ");
    }
    IEnumerator PlaySoundEffects()
    {
        foreach (string path in soundPaths)
        {
            // SoundManager�� Play�� ����Ͽ� ���� ���
            Managers.Sound.Play(path, Define.Sound.Effect, 0.4f);

            // ���� ���� ���̸�ŭ ��� (���⼭�� ���Ƿ� 1�ʷ� ����)
            yield return new WaitForSeconds(1.0f);  // �Ǵ� �� ���� ���� ���̸�ŭ ���
        }
    }
}   
