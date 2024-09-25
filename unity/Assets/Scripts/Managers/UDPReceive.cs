using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client; 
    public int port = 5052;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string leftHandData;
    public string rightHandData;

    public void Start()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                string receivedData = Encoding.UTF8.GetString(dataByte);

                // 데이터 분석
                ParseData(receivedData);

                if (printToConsole)
                {
                    print("Left Hand Data: " + leftHandData);
                    print("Right Hand Data: " + rightHandData);
                }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // 데이터를 분석하여 왼손과 오른손 데이터를 분리
    private void ParseData(string receivedData)
    {
        if (receivedData.StartsWith("Left"))
        {
            leftHandData = receivedData.Split(':')[1];
        }
        else if (receivedData.StartsWith("Right"))
        {
            rightHandData = receivedData.Split(':')[1];
        }
    }

    private void OnApplicationQuit()
    {
        startRecieving = false;
        client.Close(); // UdpClient 종료
        receiveThread.Join(); // 스레드가 종료될 때까지 대기
    }
}
