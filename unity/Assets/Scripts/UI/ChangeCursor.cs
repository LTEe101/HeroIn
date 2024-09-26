using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    private Texture2D _defaultCursor;  // �⺻ Ŀ�� �̹���
    private int cursorWidth = 128;
    private int cursorHeight = 128;
    private void Start()
    {
        // �⺻ Ŀ���� �ε�
        _defaultCursor = Managers.Resource.Load<Texture2D>("Art/Image/cursor-off");

        // �⺻ Ŀ���� ����
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
                // ũ�⸦ ������ �����Ͽ� ���ο� �ȼ� ������ ����
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
