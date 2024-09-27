using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] leftHandPoints;
    public GameObject[] rightHandPoints;

    void Start()
    {
        
    }

    void Update()
    {
        // UDP 프로토콜로 전송된 데이터를 받아온다.
        string leftdata = udpReceive.leftHandData;
        string rightdata = udpReceive.rightHandData;

        // 데이터를 클리닝 (대괄호 제거)
        leftdata = leftdata.Remove(0, 1);
        leftdata = leftdata.Remove(leftdata.Length - 1, 1);

        rightdata = rightdata.Remove(0, 1);
        rightdata = rightdata.Remove(rightdata.Length - 1, 1);

        // 쉼표를 기준으로 데이터를 분할
        string[] leftpoints = leftdata.Split(',');
        string[] rightpoints = rightdata.Split(',');

        print(leftpoints);
        print(rightpoints);

        // 각 손의 좌표 데이터는 21개 점씩, 즉 63개의 값이 필요 (x, y, z).

        // 왼손 데이터 처리
        for (int i = 0; i < 21; i++)
        {
            float x = 5 - float.Parse(leftpoints[i * 3]) / 100;
            float y = float.Parse(leftpoints[i * 3 + 1]) / 100;
            float z = float.Parse(leftpoints[i * 3 + 2]) / 100;

            leftHandPoints[i].transform.localPosition = new Vector3(x, y, z);
        }

        // 오른손 데이터 처리
        for (int i = 0; i < 21; i++)
        {
            float x = 5 - float.Parse(rightpoints[i * 3]) / 100;
            float y = float.Parse(rightpoints[i * 3 + 1]) / 100;
            float z = float.Parse(rightpoints[i * 3 + 2]) / 100;

            rightHandPoints[i].transform.localPosition = new Vector3(x, y, z);
        }
    }
}
