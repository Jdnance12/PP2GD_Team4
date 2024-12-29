using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class EMPWeapon : MonoBehaviour
{
    private gameManager gm;

    [SerializeField] public Upgrade_Menu upgradeMenu;

    [SerializeField] public GameObject radialPrefab;
    [SerializeField] public GameObject wavePrefab;
    [SerializeField] public Transform radialPoint;
    [SerializeField] public Transform wavePoint;
    [SerializeField] public float destroyTimer;
    [SerializeField] public int waveSpeed;
    [SerializeField] public float maxDistance;

    [SerializeField] public AnimatorController playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        upgradeMenu = GetComponent<Upgrade_Menu>();
        gm = gameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RadialDistrupt()
    { 
        GameObject pulse = Instantiate(radialPrefab, radialPoint.position, Quaternion.identity);
        pulse.transform.localScale = Vector3.zero;

        StartCoroutine(ScaleUp(pulse, destroyTimer));

        Destroy(pulse, destroyTimer);

        gm.player.GetComponent<Player_Controller>().ToggleSkillMenu();
    }
    public void WaveDisrupt()
    {
        GameObject wave = Instantiate(wavePrefab, wavePoint.position, wavePoint.rotation);

        StartCoroutine(MoveWave(wave, maxDistance, destroyTimer));

        gm.player.GetComponent<Player_Controller>().ToggleSkillMenu();
    }
    IEnumerator MoveWave(GameObject wave, float maxDistance, float destroyTimer)
    {
        float startTime = Time.time;
        Vector3 startPos = wave.transform.position;

        while (Time.time < startTime + destroyTimer && wave != null)
        {
            wave.transform.Translate(Vector3.forward * waveSpeed * Time.deltaTime);
            
            if(Vector3.Distance(startPos, wave.transform.position) >= maxDistance)
            {
                break;
            }
            yield return null;
        }
        if (wave != null)
        {
            Destroy(wave);
        }
    }
    IEnumerator ScaleUp(GameObject pulse, float duration)
    {
        float elapsedTime = 0f;
        Vector3 targetScale = new Vector3(100, 100, 100);

        while(elapsedTime < destroyTimer && pulse != null)
        {
            pulse.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (pulse != null)
        {
            pulse.transform.localScale = targetScale;
        }
    }
}
