using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    private Texture2D _defaultCursor;  // 기본 커서 이미지
    private int cursorWidth = 128;
    private int cursorHeight = 128;
    private void Start()
    {
        // 기본 커서를 로드
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");

        // 기본 커서를 적용
        if (_defaultCursor != null)
        {
            Texture2D resizedCursor = ResizeTexture(_defaultCursor, cursorWidth, cursorHeight);
            Cursor.SetCursor(resizedCursor, new Vector2(0f, 0f), CursorMode.ForceSoftware);
        }
        else
        {
            Debug.LogWarning("Default cursor not found at: Art/Image/cursor-off");
        }
    }
    private Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
    {
        Texture2D resizedTexture = new Texture2D(width, height);
        Color[] pixels = originalTexture.GetPixels();
        Color[] resizedPixels = resizedTexture.GetPixels();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 크기를 비율로 조정하여 새로운 픽셀 색상을 설정
                int originalX = Mathf.FloorToInt(x * ((float)originalTexture.width / width));
                int originalY = Mathf.FloorToInt(y * ((float)originalTexture.height / height));
                resizedPixels[y * width + x] = originalTexture.GetPixel(originalX, originalY);
            }
        }

        resizedTexture.SetPixels(resizedPixels);
        resizedTexture.Apply();
        return resizedTexture;
    }
}
