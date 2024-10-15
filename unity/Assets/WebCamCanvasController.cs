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
        webcamCanvas.gameObject.SetActive(false); // 초기 상태 비활성화
    }

    public void ShowCanvas()
    {
        webcamCanvas.gameObject.SetActive(true); // Canvas 활성화
    }

    public void HideCanvas()
    {
        webcamCanvas.gameObject.SetActive(false); // Canvas 비활성화
    }
}
