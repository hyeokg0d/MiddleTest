using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
// 적과 충돌 시 게임오버 패널 표시 및 재시작 담당.
//
public class GameOverUIController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Button restartButton;

    bool _isShown;

    void Awake()
    {
        Time.timeScale = 1f;
        GameScore.ResetScore();
        if (panel != null)
            panel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartCurrentScene);
    }

    void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartCurrentScene);
    }

    public void Bind(GameObject panelObject, Button restart)
    {
        panel = panelObject;
        restartButton = restart;
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartCurrentScene);
            restartButton.onClick.AddListener(RestartCurrentScene);
        }
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (_isShown)
            return;
        _isShown = true;

        Time.timeScale = 0f;
        if (panel != null)
            panel.SetActive(true);
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        GameScore.ResetScore();
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
