using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("End Game UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Delay Before Pause (วินาที)")]
    [Tooltip("หน่วงเวลาให้เล่นท่าตายจบก่อนหยุดเกม")]
    public float loseDelay = 1.5f;
    public float winDelay = 2.5f;

    [Header("Boss Reference")]
    public BossHealth bossHealth;

    private bool isGameEnded = false;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (bossHealth != null)
        {
            bossHealth.OnBossDeath += HandleBossWin;
        }
    }

    void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnBossDeath -= HandleBossWin;
        }
    }

    // เรียกใช้เมื่อ Player ตาย
    public void TriggerLose()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // หยุดเพลง BG ทันทีที่ Player ตาย
        if (SoundManager.Instance != null) SoundManager.Instance.StopBGM();

        StartCoroutine(ShowEndScreenRoutine(losePanel, loseDelay, false));
    }

    // เรียกใช้เมื่อ Boss ตาย
    private void HandleBossWin()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // หยุดเพลง BG ทันที + เล่นเสียงบอสตาย
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.bossDieSound);
        }

        StartCoroutine(ShowEndScreenRoutine(winPanel, winDelay, true));
    }

    private IEnumerator ShowEndScreenRoutine(GameObject panelToShow, float delay, bool isWin)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }

        // เล่นเสียงชนะตอนป้าย Win เด้งขึ้นมา
        if (isWin && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.winSound);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}