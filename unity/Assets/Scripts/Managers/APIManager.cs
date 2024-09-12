using UnityEngine;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "http://j11e101.p.ssafy.io:5000";

    // 로그인 요청을 보내는 함수
    public IEnumerator Login(string userId, string password, System.Action<User> onSuccess, System.Action<string> onError)
    {
        // 주석 처리된 서버 요청 부분 유지
        // 서버에 보낼 데이터 설정
        // WWWForm form = new WWWForm();
        // form.AddField("userId", userId);
        // form.AddField("password", password);

        // POST 요청을 보낼 UnityWebRequest 객체 생성
        // using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        // {
        //     // 요청 전송 및 응답 대기
        //     yield return request.SendWebRequest();

        //     // 오류 처리
        //     if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        //     {
        //         Debug.LogError("Error: " + request.error);
        //         onError?.Invoke(request.error);
        //         yield break; // 오류 발생 시 메소드 종료
        //     }

        //     // 서버 응답 처리
        //     string responseData = request.downloadHandler.text;
        //     Debug.Log("Response Data: " + responseData);

        //     // 응답 데이터를 User 객체로 변환
        //     User user = ParseUserFromResponse(responseData);
        //     onSuccess?.Invoke(user);
        // }

        // 가짜 데이터로 테스트
        yield return new WaitForSeconds(0.2f); // 서버 응답 대기 시뮬레이션

        // 가짜 User 객체 생성
        User user = ParseUserFromResponse();
        onSuccess?.Invoke(user);

        // 오류 처리 테스트용
         //onError?.Invoke("가짜 오류 메시지"); // 주석 해제 시 오류 처리 테스트 가능

        yield return null;
    }

    // 응답 데이터에서 User 객체를 생성하는 함수
    private User ParseUserFromResponse()
    {
        // 실제 응답 데이터 파싱 로직 구현 (예: JSON 파싱)
        // 여기서는 간단히 가짜 데이터를 반환하도록 설정
        User user = new User();
        user.title = "대단한 기록";  // 실제 파싱된 데이터로 설정
        user.name = "김태훈";
        user.userId = "zxader";
        return user;
    }
}
