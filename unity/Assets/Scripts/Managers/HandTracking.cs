using UnityEngine;

public class HandTracking : MonoBehaviour, IMotionGameScript
{
    public static HandTracking Instance; // Singleton instance

    public UDPReceive udpReceive;

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
        // Singleton pattern
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
        // UDP 프로토콜로 전송된 데이터를 받아온다.
        string leftdata = udpReceive.leftHandData;
        string rightdata = udpReceive.rightHandData;

        // 왼손 데이터 처리
        if (leftdata == "NoData")
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
                Debug.LogWarning("Left hand data is invalid.");
                leftHandDataValid = false;

                // 왼손 데이터 초기화
                for (int i = 0; i < leftHandPositions.Length; i++)
                {
                    leftHandPositions[i] = Vector3.zero;
                }
            }
        }

        // 오른손 데이터 처리
        if (rightdata == "NoData")
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
                Debug.LogWarning("Right hand data is invalid.");
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
