using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class WindowTransparent : MonoBehaviour
{
    // 导入 user32.dll 库以使用 Windows API 函数
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    // 定义一个结构来存储窗口边框的边距大小
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    // 导入 user32.dll 以获取活动窗口句柄(HWND)
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // 导入 Dwmapi.dll 以将窗口边框扩展到客户区域
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // 导入 user32.dll 以修改窗口属性
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // 导入 user32.dll 以设置窗口位置
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    // 导入 user32.dll 以设置分层窗口属性(透明度)
    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    // 代码中使用的常量和变量
    const int GWL_EXSTYLE = -20;        // 修改窗口样式的索引
    const uint WS_EX_LAYERED = 0x00080000;    // 分层窗口的扩展样式
    const uint WS_EX_TRANSPARENT = 0x00000020; // 透明窗口的扩展样式
    const uint LWA_ALPHA = 0x00000002;        // 使用Alpha通道
    const uint LWA_COLORKEY = 0x00000001;     // 使用颜色键

    // 窗口句柄
    private IntPtr windowHandle;

    public enum TransparencyMode
    {
        FullTransparent,    // 整个窗口透明
        ColorKey           // 指定颜色透明（推荐用于桌面宠物）
    }
    
    [Header("透明窗口设置")]
    public TransparencyMode transparencyMode = TransparencyMode.ColorKey;
    [Range(0, 255)]
    public byte transparency = 255;     // 透明度 (0=完全透明, 255=完全不透明)
    public bool clickThrough = true;    // 是否允许点击穿透
    public bool alwaysOnTop = true;     // 是否始终置顶
    public Color transparentColor = Color.black; // 透明颜色（纯黑色）

    void Start()
    {
        // 获取当前窗口句柄
        windowHandle = GetActiveWindow();
        
        if (Application.isEditor)
        {
            Debug.Log("在Unity编辑器中运行 - 窗口透明效果仅在独立构建中生效");
            Debug.Log($"当前窗口句柄: {windowHandle}");
            return;
        }
        
        // 设置窗口透明
        SetWindowTransparent();
        
        Debug.Log("透明窗口设置完成");
    }

    void SetWindowTransparent()
    {
        if (Application.isEditor)
        {
            Debug.Log("在编辑器中无法应用窗口透明设置");
            return;
        }
        
        if (windowHandle == IntPtr.Zero)
        {
            Debug.LogError("无法获取窗口句柄");
            return;
        }

        try
        {
            // 设置窗口为分层窗口
            SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED);
            
            if (transparencyMode == TransparencyMode.FullTransparent)
            {
                // 整个窗口透明
                SetLayeredWindowAttributes(windowHandle, 0, transparency, LWA_ALPHA);
                Debug.Log($"窗口透明度设置为: {transparency}");
            }
            else if (transparencyMode == TransparencyMode.ColorKey)
            {
                // 指定颜色透明 - 将纯黑色背景变为透明
                uint colorKey = RGB(transparentColor.r, transparentColor.g, transparentColor.b);
                SetLayeredWindowAttributes(windowHandle, colorKey, 0, LWA_COLORKEY);
                Debug.Log($"颜色键透明设置完成，透明颜色: RGB({(int)(transparentColor.r * 255)}, {(int)(transparentColor.g * 255)}, {(int)(transparentColor.b * 255)})");
            }
            
            // 如果需要点击穿透
            if (clickThrough)
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"设置窗口透明时出错: {e.Message}");
        }
    }

    // RGB颜色转换函数
    private uint RGB(float r, float g, float b)
    {
        return (uint)((byte)(r * 255) | ((byte)(g * 255) << 8) | ((byte)(b * 255) << 16));
    }

    // 动态调整透明度
    public void SetTransparency(byte alpha)
    {
        transparency = alpha;
        if (Application.isEditor)
        {
            Debug.Log("在编辑器中无法应用透明度设置");
            return;
        }
        
        if (windowHandle != IntPtr.Zero)
        {
            SetLayeredWindowAttributes(windowHandle, 0, transparency, LWA_ALPHA);
        }
    }

    // 切换点击穿透
    public void ToggleClickThrough()
    {
        clickThrough = !clickThrough;
        if (Application.isEditor)
        {
            Debug.Log("在编辑器中无法应用点击穿透设置");
            return;
        }
        
        if (windowHandle != IntPtr.Zero)
        {
            if (clickThrough)
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(windowHandle, GWL_EXSTYLE, WS_EX_LAYERED);
            }
        }
    }

    // 设置窗口始终置顶
    public void SetAlwaysOnTop(bool onTop)
    {
        if (Application.isEditor)
        {
            Debug.Log("在编辑器中无法应用置顶设置");
            return;
        }
        
        if (windowHandle != IntPtr.Zero)
        {
            IntPtr insertAfter = onTop ? new IntPtr(-1) : new IntPtr(-2); // -1 = HWND_TOPMOST, -2 = HWND_NOTOPMOST
            SetWindowPos(windowHandle, insertAfter, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0004); // SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE
        }
    }

    void OnApplicationQuit()
    {
        // 退出时恢复窗口正常状态
        if (windowHandle != IntPtr.Zero)
        {
            SetWindowLong(windowHandle, GWL_EXSTYLE, 0);
            SetLayeredWindowAttributes(windowHandle, 0, 255, LWA_ALPHA);
        }
    }

    // 用于测试的GUI按钮（仅在编辑器中可见）
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUILayout.BeginArea(new Rect(10, 10, 250, 350));
            
            GUILayout.Label("透明窗口控制 (仅预览模式)");
            GUILayout.Label("⚠️ 在编辑器中仅可调整参数，实际效果需构建后测试");
            
            // 透明度模式选择
            GUILayout.Label("透明度模式:");
            transparencyMode = (TransparencyMode)GUILayout.SelectionGrid((int)transparencyMode, 
                new string[] { "全透明", "颜色键透明" }, 1);
            
            // 透明度滑块（仅在全透明模式下有效）
            if (transparencyMode == TransparencyMode.FullTransparent)
            {
                GUILayout.Label($"透明度: {transparency}");
                transparency = (byte)GUILayout.HorizontalSlider(transparency, 0, 255);
                
                if (GUILayout.Button("预览透明度设置"))
                {
                    SetTransparency(transparency);
                }
            }
            
            // 透明颜色选择（仅在颜色键模式下有效）
            if (transparencyMode == TransparencyMode.ColorKey)
            {
                GUILayout.Label("透明颜色 (RGB):");
                transparentColor = new Color(
                    GUILayout.HorizontalSlider(transparentColor.r, 0, 1),
                    GUILayout.HorizontalSlider(transparentColor.g, 0, 1),
                    GUILayout.HorizontalSlider(transparentColor.b, 0, 1)
                );
                
                GUILayout.Label($"当前透明颜色: RGB({(int)(transparentColor.r * 255)}, {(int)(transparentColor.g * 255)}, {(int)(transparentColor.b * 255)})");
                
                if (GUILayout.Button("预览颜色键设置"))
                {
                    SetWindowTransparent();
                }
            }
            
            if (GUILayout.Button($"点击穿透: {(clickThrough ? "开启" : "关闭")} (仅预览)"))
            {
                ToggleClickThrough();
            }
            
            if (GUILayout.Button($"置顶: {(alwaysOnTop ? "开启" : "关闭")} (仅预览)"))
            {
                alwaysOnTop = !alwaysOnTop;
                SetAlwaysOnTop(alwaysOnTop);
            }
            
            if (GUILayout.Button("预览所有设置"))
            {
                SetWindowTransparent();
            }
            
            GUILayout.Space(10);
            GUILayout.Label("💡 提示: 构建为独立应用程序以测试真实效果");
            
            GUILayout.EndArea();
        }
    }
}
