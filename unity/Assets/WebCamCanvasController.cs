using UnityEngine;

public class WebCamCanvasController : MonoBehaviour, IMotionGameScript
{
    public Canvas webcamCanvas;

    public bool IsEnabled
    {
        get { return webcamCanvas.gameObject.activeSelf; }
        set { webcamCanvas.gameObject.SetActive(value); }
    }

    void Start()
    {
        webcamCanvas.gameObject.SetActive(false); // �ʱ� ���� ��Ȱ��ȭ
    }

    public void ShowCanvas()
    {
        webcamCanvas.gameObject.SetActive(true); // Canvas Ȱ��ȭ
    }

    public void HideCanvas()
    {
        webcamCanvas.gameObject.SetActive(false); // Canvas ��Ȱ��ȭ
    }
}
