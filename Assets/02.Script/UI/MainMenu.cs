using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 화면의 UI 기능(게임 시작, 종료)을 담당하는 스크립트입니다.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // 게임 시작 (Play 버튼과 연결)
    public void PlayGame()
    {
        // Build Settings에 등록된 다음 씬을 로드합니다.
        // 현재는 MainMenu -> GameScene 순서라고 가정
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // 게임 종료 (Quit 버튼과 연결)
    public void QuitGame()
    {
        Debug.Log("Game Quit!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 모드 종료
#else
        Application.Quit(); // 빌드된 게임 종료
#endif
    }
}
