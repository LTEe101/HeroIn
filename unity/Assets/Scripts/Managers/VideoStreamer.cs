using UnityEngine;
using System.Threading.Tasks;
using NativeWebSocket;

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
            ParseData(message);
        };

        // 웹소켓 연결 시도
        await websocket.Connect();

        // 웹캠 시작
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
#endif

        if (webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            SendFrame();
        }
    }

    async void SendFrame()
    {
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
        if (receivedData.StartsWith("Left"))
        {
            string data = receivedData.Split(':')[1];
            leftHandData = data;
        }
        else if (receivedData.StartsWith("Right"))
        {
            string data = receivedData.Split(':')[1];
            rightHandData = data;
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
        if (websocket != null)
        {
            await websocket.Close();
        }
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
