using UnityEngine;

public class HandTracking : MonoBehaviour, IMotionGameScript
{
    public static HandTracking Instance; // 싱글톤 인스턴스

    public VideoStreamer videoStreamer;

    public Vector3[] leftHandPositions = new Vector3[21]; // 저장된 좌표 데이터
    public Vector3[] rightHandPositions = new Vector3[21]; // 저장된 좌표 데이터

    public bool leftHandDataValid = false;
    public bool rightHandDataValid = false;

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // WebSocket을 통해 전송된 데이터를 받아온다.
        string leftdata = videoStreamer.leftHandData;
        string rightdata = videoStreamer.rightHandData;

        // 왼손 데이터 처리
        if (leftdata == "NoData" || string.IsNullOrEmpty(leftdata))
        {
            leftHandDataValid = false;

            // 왼손 데이터 초기화
            for (int i = 0; i < leftHandPositions.Length; i++)
            {
                leftHandPositions[i] = Vector3.zero;
            }
        }
        else
        {
            leftdata = CleanData(leftdata);
            string[] leftpoints = leftdata.Split(',');

            if (leftpoints.Length == 63)
            {
                for (int i = 0; i < 21; i++)
                {
                    float x = float.Parse(leftpoints[i * 3]) / 100;
                    float y = float.Parse(leftpoints[i * 3 + 1]) / 100;
                    float z = float.Parse(leftpoints[i * 3 + 2]) / 100;

                    leftHandPositions[i] = new Vector3(x, y, z);
                }
                leftHandDataValid = true; // 왼손 데이터 유효함
            }
            else
            {
                Debug.LogWarning("왼손 데이터가 유효하지 않습니다.");
                leftHandDataValid = false;

                // 왼손 데이터 초기화
                for (int i = 0; i < leftHandPositions.Length; i++)
                {
                    leftHandPositions[i] = Vector3.zero;
                }
            }
        }

        // 오른손 데이터 처리
        if (rightdata == "NoData" || string.IsNullOrEmpty(rightdata))
        {
            rightHandDataValid = false;

            // 오른손 데이터 초기화
            for (int i = 0; i < rightHandPositions.Length; i++)
            {
                rightHandPositions[i] = Vector3.zero;
            }
        }
        else
        {
            rightdata = CleanData(rightdata);
            string[] rightpoints = rightdata.Split(',');

            if (rightpoints.Length == 63)
            {
                for (int i = 0; i < 21; i++)
                {
                    float x = float.Parse(rightpoints[i * 3]) / 100;
                    float y = float.Parse(rightpoints[i * 3 + 1]) / 100;
                    float z = float.Parse(rightpoints[i * 3 + 2]) / 100;

                    rightHandPositions[i] = new Vector3(x, y, z);
                }
                rightHandDataValid = true; // 오른손 데이터 유효함
            }
            else
            {
                Debug.LogWarning("오른손 데이터가 유효하지 않습니다.");
                rightHandDataValid = false;

                // 오른손 데이터 초기화
                for (int i = 0; i < rightHandPositions.Length; i++)
                {
                    rightHandPositions[i] = Vector3.zero;
                }
            }
        }
    }

    private string CleanData(string data)
    {
        return data.TrimStart('[').TrimEnd(']');
    }
}
