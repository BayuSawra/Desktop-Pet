using UnityEngine;

public class TransparentBackground : MonoBehaviour
{
    [Header("透明背景设置")]
    public bool enableTransparentBackground = true;
    public Color backgroundColor = Color.black; // 纯黑色背景，用于颜色键透明
    
    void Start()
    {
        if (enableTransparentBackground)
        {
            SetupTransparentBackground();
        }
    }
    
    void SetupTransparentBackground()
    {
        // 设置主摄像机的背景色为纯黑色
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = backgroundColor;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            
            Debug.Log($"透明背景设置完成，背景色: RGB({(int)(backgroundColor.r * 255)}, {(int)(backgroundColor.g * 255)}, {(int)(backgroundColor.b * 255)})");
        }
        else
        {
            Debug.LogWarning("未找到主摄像机");
        }
    }
    
    // 动态设置背景色
    public void SetBackgroundColor(Color color)
    {
        backgroundColor = color;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = backgroundColor;
        }
    }
    
    // 设置纯黑色背景（用于颜色键透明）
    public void SetBlackBackground()
    {
        SetBackgroundColor(Color.black);
    }
    
    // 设置自定义颜色背景
    public void SetCustomBackground(float r, float g, float b)
    {
        Color customColor = new Color(r, g, b, 1.0f);
        SetBackgroundColor(customColor);
    }
    
    // 用于测试的GUI按钮（仅在编辑器中可见）
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUILayout.BeginArea(new Rect(10, 320, 250, 200));
            
            GUILayout.Label("透明背景控制");
            
            GUILayout.Label("背景颜色 (RGB):");
            backgroundColor = new Color(
                GUILayout.HorizontalSlider(backgroundColor.r, 0, 1),
                GUILayout.HorizontalSlider(backgroundColor.g, 0, 1),
                GUILayout.HorizontalSlider(backgroundColor.b, 0, 1)
            );
            
            GUILayout.Label($"当前背景色: RGB({(int)(backgroundColor.r * 255)}, {(int)(backgroundColor.g * 255)}, {(int)(backgroundColor.b * 255)})");
            
            if (GUILayout.Button("应用背景色"))
            {
                SetBackgroundColor(backgroundColor);
            }
            
            if (GUILayout.Button("设置为纯黑色"))
            {
                SetBlackBackground();
            }
            
            GUILayout.EndArea();
        }
    }
}
