using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    // 다음 씬을 비동기적으로 로드
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return null;
        // _time 변수 초기화
        float time = 0f;

        // 비동기적으로 씬을 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("씬 로드 시작: " + sceneName);

        // 씬 로드 완료 후 자동으로 씬을 활성화하지 않도록 설정
        operation.allowSceneActivation = false;
        Debug.Log("allowSceneActivation을 false로 설정");

        // 씬 로드 진행 상태 확인 (0.9f까지)
        while (operation.progress < 0.9f)
        {
            Debug.Log("로딩 진행률: " + (operation.progress * 100) + "%");
            yield return null;
        }

        Debug.Log("로딩 진행률 90% 도달");

        // 원하는 대기 시간 (예: 1초)
        while (time < 5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("대기 시간 완료. 씬 활성화");

        // 씬 활성화
        operation.allowSceneActivation = true;
        Debug.Log("allowSceneActivation을 true로 설정하여 씬 활성화");
    }
 }   
