using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPositionAdjuster : MonoBehaviour
{
    public GameObject gameScenePhoto; // 이미지 오브젝트
    public RectTransform descText; // 텍스트의 RectTransform

    void Update()
    {
        if (!gameScenePhoto.activeSelf)
        {
            // 이미지가 비활성화된 경우
            descText.anchoredPosition = new Vector2(360, descText.anchoredPosition.y);
        }
    }
}