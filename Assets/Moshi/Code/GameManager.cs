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
        Time.timeScale = 1f; // บังคับให้เวลาเดินปกติทุกครั้งที่เริ่มฉาก
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // เชื่อมเหตุการณ์ตอนบอสตายจาก BossHealth ของเพื่อนโดยตรง
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
        StartCoroutine(ShowEndScreenRoutine(losePanel, loseDelay));
    }

    // เรียกใช้เมื่อ Boss ตาย
    private void HandleBossWin()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        StartCoroutine(ShowEndScreenRoutine(winPanel, winDelay));
    }

    private IEnumerator ShowEndScreenRoutine(GameObject panelToShow, float delay)
    {
        // รอให้ท่าตายเล่นจบก่อน (ใช้ Realtime เผื่อมีการปรับเวลา)
        yield return new WaitForSecondsRealtime(delay);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }

        // หยุดเวลาทั้งหมดในเกม (ตัวละคร บอส ลูกไฟ และสกิลหยุดนิ่งทันที)
        Time.timeScale = 0f;
    }

    // ฟังก์ชันเสริม เผื่อเอาไปผูกกับปุ่ม Restart ในอนาคต
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}