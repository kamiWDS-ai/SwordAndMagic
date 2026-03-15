using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("退出游戏");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // 在编辑器里停止运行
        #else
        Application.Quit();  // 打包后真正退出
        #endif
    }
}