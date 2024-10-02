using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPositionAdjuster : MonoBehaviour
{
    public GameObject gameScenePhoto; // �̹��� ������Ʈ
    public RectTransform descText; // �ؽ�Ʈ�� RectTransform

    void Update()
    {
        if (!gameScenePhoto.activeSelf)
        {
            // �̹����� ��Ȱ��ȭ�� ���
            descText.anchoredPosition = new Vector2(360, descText.anchoredPosition.y);
        }
    }
}