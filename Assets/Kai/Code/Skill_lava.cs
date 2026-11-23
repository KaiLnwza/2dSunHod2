using System.Collections;
using UnityEngine;

public class Skill_Lava : MonoBehaviour
{
    [Header("ลาก Object จาก Hierarchy มาใส่")]
    public GameObject lavaWarningObj; 
    public GameObject lavaFloorObj;   

    [Header("ตั้งค่าเวลา (วินาที)")]
    public float warningDuration = 1.5f; 
    public float lavaDuration = 15f;     

    void Start()
    {
        if (lavaWarningObj != null) lavaWarningObj.SetActive(false);
        if (lavaFloorObj != null) lavaFloorObj.SetActive(false);
    }

    public IEnumerator ExecuteSkill()
    {
        if (lavaWarningObj != null) lavaWarningObj.SetActive(true);
        yield return new WaitForSeconds(warningDuration);

        if (lavaWarningObj != null) lavaWarningObj.SetActive(false);
        if (lavaFloorObj != null) lavaFloorObj.SetActive(true);

        yield return new WaitForSeconds(lavaDuration);

        if (lavaFloorObj != null) lavaFloorObj.SetActive(false);
    }
}