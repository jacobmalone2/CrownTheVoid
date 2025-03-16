using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Tool for generating icons of items - created by Game Dev Guide,
// "Creating An Inventory System in Unity" on YouTube
public class IconGenerator : MonoBehaviour
{
    public string prefix;
    public string pathFolder;

    private Camera camera;

    [ContextMenu("Screenshot")]
    private void Screenshot()
    {
        TakeScreenshot($"{Application.dataPath}/{pathFolder}/{prefix}.png");
    }

    private void TakeScreenshot(string fullPath)
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
