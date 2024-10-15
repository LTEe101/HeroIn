using UnityEngine;
using System.Threading.Tasks;
using NativeWebSocket;
using System;
using UnityEngine.UI;

public class VideoStreamer : MonoBehaviour, IMotionGameScript
{
    public string websocketUrl = "ws://j11e101.p.ssafy.io:8000/ws";
    private WebSocket websocket;
    private WebCamTexture webCamTexture;
    public RawImage webcamDisplay;
    public WebCamCanvasController webcamCanvasController;  // WebCamCanvasController 참조 추가
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
        // 처음에 Canvas를 비활성화
        if (webcamCanvasController != null)
            webcamCanvasController.HideCanvas();

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

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif

        // 웹캠 상태 체크 후 강제로 종료
        if (webCamTexture != null && !webCamTexture.isPlaying)
        {
            // 웹캠이 멈추지 않았는데 상태가 false일 때 강제 종료
            if (webCamTexture.isPlaying == false)
            {
                ForceStopWebCam();
            }
        }

        if (webCamTexture != null && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            SendFrame();
        }
    }

    // 웹캠 강제 종료 메서드
    private void ForceStopWebCam()
    {
        if (webCamTexture != null)
        {
            Debug.LogWarning("웹캠이 정상적으로 멈추지 않았으므로 강제로 해제합니다.");
            webCamTexture.Stop(); // Stop 호출 시도
            Destroy(webCamTexture); // 강제로 리소스 해제
            webCamTexture = null;
            Debug.Log("웹캠 리소스가 강제로 해제되었습니다.");
        }
    }

    private void StartWebcam()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            webCamTexture = new WebCamTexture();
            webCamTexture.requestedWidth = 1280;
            webCamTexture.requestedHeight = 720;
            webCamTexture.requestedFPS = 30;

            // 첫 번째 웹캠을 사용하여 WebCamTexture를 생성합니다.
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                Debug.Log("No webcam detected.");
                return;
            }

            // 이미 선언된 webCamTexture를 사용하여 영상을 RawImage에 할당합니다.
            webcamDisplay.texture = webCamTexture;

            webCamTexture.Play();

            // 웹캠이 시작되면 Canvas 활성화
            if (webcamCanvasController != null)
                webcamCanvasController.ShowCanvas();

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
            }
            else if (handType == "Right")
            {
                rightHandData = data;
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

        Texture2D sourceTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        sourceTexture.SetPixels32(source.GetPixels32());
        sourceTexture.Apply();

        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;

        Graphics.Blit(sourceTexture, rt);
        Texture2D resizedTexture = new Texture2D(width, height);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = null;
        rt.Release();
        Destroy(sourceTexture);

        return resizedTexture;
    }

    public async Task CloseWebSocketAndStopCam()
    {
        // 웹소켓 종료
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.Close();
            Debug.Log("WebSocket 연결이 종료되었습니다.");
        }

        // 웹캠 종료
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            Debug.Log("웹캠 정지됨");

            // 강제로 웹캠 리소스 해제
            Destroy(webCamTexture);
            webCamTexture = null;
            Debug.Log("웹캠 리소스가 파괴되었습니다.");
        }
    }

    async void OnApplicationQuit()
    {
        await CloseWebSocketAndStopCam();
    }

    private async void OnDisable()
    {
        await CloseWebSocketAndStopCam();
    }

    private void LogReceivedData(string message)
    {
        // Debug.Log($"수신된 데이터: {message}");
    }
}
