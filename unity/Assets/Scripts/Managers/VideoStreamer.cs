using UnityEngine;
using System.Threading.Tasks;
using NativeWebSocket;
using System;

public class VideoStreamer : MonoBehaviour, IMotionGameScript
{
    public string websocketUrl = "ws://j11e101.p.ssafy.io:8000/ws";
    private WebSocket websocket;
    private WebCamTexture webCamTexture;
    private const int MAX_PACKET_SIZE = 60000; // 패킷 최대 크기
    private const int MAX_IMAGE_SIZE = 640; // 이미지 크기 조정

    public string leftHandData;
    public string rightHandData;

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    async void Start()
    {
        // WebSocket 초기화 및 이벤트 핸들러 설정
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket 연결 성공");
            StartWebcam();  // 웹소켓 연결 성공 시 웹캠 시작
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket 오류: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket 연결 종료");
        };

        websocket.OnMessage += (bytes) =>
        {
            // 서버로부터 수신한 메시지 처리
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            LogReceivedData(message);  // 수신된 데이터 로깅
            ParseData(message);
        };

        // 웹소켓 연결 시도
        await ConnectWebSocket();
    }

    private void StartWebcam()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            webCamTexture = new WebCamTexture();
            webCamTexture.requestedWidth = 1280;  // 원하는 해상도로 설정
            webCamTexture.requestedHeight = 720;  // 원하는 해상도로 설정
            webCamTexture.requestedFPS = 30;  // 원하는 FPS로 설정

            webCamTexture.Play();

            Debug.Log("웹캠 시작됨");
        }
        else
        {
            Debug.LogWarning("사용 가능한 웹캠이 없습니다.");
        }
    }

    async Task ConnectWebSocket()
    {
        while (true)
        {
            try
            {
                await websocket.Connect();
                break;  // 연결 성공 시 루프 종료
            }
            catch (Exception e)
            {
                Debug.LogError($"WebSocket 연결 실패: {e.Message}");
                await Task.Delay(5000); // 5초 후 재시도
            }
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif

        if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            SendFrame();
        }
    }

    async void SendFrame()
    {
        if (webCamTexture == null)
        {
            Debug.LogWarning("웹캠 텍스처가 없습니다.");
            return;
        }

        Texture2D resizedTexture = ResizeTexture(webCamTexture, MAX_IMAGE_SIZE);

        byte[] jpgData = resizedTexture.EncodeToJPG(75);

        int totalPackets = Mathf.CeilToInt((float)jpgData.Length / MAX_PACKET_SIZE);

        for (int i = 0; i < totalPackets; i++)
        {
            int offset = i * MAX_PACKET_SIZE;
            int size = Mathf.Min(MAX_PACKET_SIZE, jpgData.Length - offset);
            byte[] packet = new byte[size + 4]; // 4바이트는 패킷 번호용

            System.BitConverter.GetBytes(i).CopyTo(packet, 0);
            System.Array.Copy(jpgData, offset, packet, 4, size);

            await websocket.Send(packet);
        }

        Destroy(resizedTexture);
    }

    // 데이터를 분석하여 왼손과 오른손 데이터를 분리
    private void ParseData(string receivedData)
    {
        try
        {
            string[] parts = receivedData.Split(':');
            if (parts.Length != 2)
            {
                Debug.LogWarning($"잘못된 데이터 형식: {receivedData}");
                return;
            }

            string handType = parts[0];
            string data = parts[1];

            if (handType == "Left")
            {
                leftHandData = data;
                //Debug.Log($"왼손 데이터 업데이트: {data}");  // 왼손 데이터 로깅
            }
            else if (handType == "Right")
            {
                rightHandData = data;
                //Debug.Log($"오른손 데이터 업데이트: {data}");  // 오른손 데이터 로깅
            }
            else
            {
                //Debug.LogWarning($"알 수 없는 손 타입: {handType}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 파싱 오류: {e.Message}");
        }
    }

    Texture2D ResizeTexture(WebCamTexture source, int maxSize)
    {
        int width = source.width;
        int height = source.height;
        float aspect = (float)width / height;

        if (width > height)
        {
            width = maxSize;
            height = Mathf.RoundToInt(width / aspect);
        }
        else
        {
            height = maxSize;
            width = Mathf.RoundToInt(height * aspect);
        }

        // WebCamTexture의 데이터를 Texture2D로 복사
        Texture2D sourceTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        sourceTexture.SetPixels32(source.GetPixels32());
        sourceTexture.Apply();

        // 새로운 크기의 Texture2D 생성
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;

        // 리사이징 수행
        Graphics.Blit(sourceTexture, rt);
        Texture2D resizedTexture = new Texture2D(width, height);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();

        // 임시 텍스처 정리
        RenderTexture.active = null;
        rt.Release();
        Destroy(sourceTexture);

        return resizedTexture;
    }

    async void OnApplicationQuit()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.Close();
        }
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }

    private void LogReceivedData(string message)
    {
        //Debug.Log($"수신된 데이터: {message}");
    }

    private void OnDisable()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            Debug.Log("웹캠 정지됨");
        }
    }
}