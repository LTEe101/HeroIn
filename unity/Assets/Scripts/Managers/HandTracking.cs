using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public static HandTracking Instance; // Singleton instance

    public UDPReceive udpReceive;

    public Vector3[] leftHandPositions = new Vector3[21]; // 저장된 좌표 데이터
    public Vector3[] rightHandPositions = new Vector3[21]; // 저장된 좌표 데이터

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

        leftdata = CleanData(leftdata);
        rightdata = CleanData(rightdata);

        string[] leftpoints = leftdata.Split(',');
        string[] rightpoints = rightdata.Split(',');

        if (leftpoints.Length != 63 || rightpoints.Length != 63)
        {
            Debug.LogWarning("Received data is not in expected format.");
            return;
        }

        // 왼손 데이터 처리
        for (int i = 0; i < 21; i++)
        {
            float x = float.Parse(leftpoints[i * 3]) / 100;
            float y = float.Parse(leftpoints[i * 3 + 1]) / 100;
            float z = float.Parse(leftpoints[i * 3 + 2]) / 100;

            leftHandPositions[i] = new Vector3(x, y, z);
        }

        // 오른손 데이터 처리
        for (int i = 0; i < 21; i++)
        {
            float x = float.Parse(rightpoints[i * 3]) / 100;
            float y = float.Parse(rightpoints[i * 3 + 1]) / 100;
            float z = float.Parse(rightpoints[i * 3 + 2]) / 100;

            rightHandPositions[i] = new Vector3(x, y, z);
        }
    }

    private string CleanData(string data)
    {
        return data.TrimStart('[').TrimEnd(']');
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HandTracking : MonoBehaviour
//{
//    public static HandTracking Instance; // Singleton instance

//    public UDPReceive udpReceive;
//    public GameObject[] leftHandPoints;
//    public GameObject[] rightHandPoints;

//    public Vector3[] leftHandPositions = new Vector3[21]; // 저장된 좌표 데이터
//    public Vector3[] rightHandPositions = new Vector3[21]; // 저장된 좌표 데이터

//    void Awake()
//    {
//        // Singleton pattern
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Update()
//    {
//        // UDP 프로토콜로 전송된 데이터를 받아온다.
//        string leftdata = udpReceive.leftHandData;
//        string rightdata = udpReceive.rightHandData;

//        leftdata = CleanData(leftdata);
//        rightdata = CleanData(rightdata);

//        string[] leftpoints = leftdata.Split(',');
//        string[] rightpoints = rightdata.Split(',');

//        if (leftpoints.Length != 63 || rightpoints.Length != 63)
//        {
//            Debug.LogWarning("Received data is not in expected format.");
//            return;
//        }

//        // 왼손 데이터 처리
//        for (int i = 0; i < 21; i++)
//        {
//            float x = float.Parse(leftpoints[i * 3]) / 100;
//            float y = float.Parse(leftpoints[i * 3 + 1]) / 100;
//            float z = float.Parse(leftpoints[i * 3 + 2]) / 100;

//            leftHandPositions[i] = new Vector3(x, y, z);
//            leftHandPoints[i].transform.position = new Vector3(x, y, z); // 위치를 worldPosition으로 설정
//        }

//        // 오른손 데이터 처리
//        for (int i = 0; i < 21; i++)
//        {
//            float x = float.Parse(rightpoints[i * 3]) / 100;
//            float y = float.Parse(rightpoints[i * 3 + 1]) / 100;
//            float z = float.Parse(rightpoints[i * 3 + 2]) / 100;

//            rightHandPositions[i] = new Vector3(x, y, z);
//            rightHandPoints[i].transform.position = new Vector3(x, y, z); // 위치를 worldPosition으로 설정
//        }
//    }

//    private string CleanData(string data)
//    {
//        return data.TrimStart('[').TrimEnd(']');
//    }
//}
