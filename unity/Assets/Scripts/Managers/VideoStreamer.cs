using UnityEngine;
using System.Net.Sockets;
using System.Net;

public class VideoStreamer : MonoBehaviour, IMotionGameScript
{
    public string ipAddress = "127.0.0.1";
    public int port = 5052;
    private UdpClient client;
    private WebCamTexture webCamTexture;
    private const int MAX_PACKET_SIZE = 60000; // UDP 패킷 최대 크기보다 약간 작게 설정
    private const int MAX_IMAGE_SIZE = 640; // 더 작은 크기로 조정

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    void Start()
    {
        client = new UdpClient();
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
    }

    void Update()
    {
        if (webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            SendFrame();
        }
    }

    void SendFrame()
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
        
        client.Send(packet, packet.Length, ipAddress, port);
    }
    
    Destroy(resizedTexture);
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

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
        if (webCamTexture != null)
            webCamTexture.Stop();
    }
}
