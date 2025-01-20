using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class EMPWeapon : MonoBehaviour
{
    private GameManager gm;

    public bool empActive;
    public bool radialActive;

    [SerializeField] public GameObject upgradeObject;
    [SerializeField] public UpgradeManager upgradeManager;

    [SerializeField] public GameObject radialPrefab;
    [SerializeField] public GameObject wavePrefab;

    [SerializeField] public GameObject wavePointObj;
    [SerializeField] public GameObject radialPointObj;
    [SerializeField] public Transform radialPointTrans;
    [SerializeField] public Transform wavePointTrans;

    [SerializeField] public GameObject textObject;
    [SerializeField] private TMP_Text objectText;

    [SerializeField] public float destroyTimer;
    [SerializeField] public int waveSpeed;
    [SerializeField] public float maxDistance;
    [SerializeField] public float displayDuration;

    //[SerializeField] public AnimatorController playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;

        upgradeObject = GameObject.Find("Game Manager");
        upgradeManager = upgradeObject.GetComponent<UpgradeManager>();

        textObject = GameObject.Find("Error Text");
        objectText = textObject.GetComponent<TMP_Text>();
        textObject.SetActive(false);

        wavePointObj = GameObject.Find("EMP Wave Position");
        radialPointObj = GameObject.Find("EMP Radial Position");
        wavePointTrans = wavePointObj.transform;
        radialPointTrans = radialPointObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RadialDistrupt()
    {
        if (empActive == true && radialActive == true)
        {
            GameObject pulse = Instantiate(radialPrefab, radialPointTrans.position, Quaternion.identity);
            pulse.transform.localScale = Vector3.zero;

            StartCoroutine(ScaleUp(pulse, destroyTimer));

            Destroy(pulse, destroyTimer);

            gm.player.GetComponent<PlayerController>().ToggleSkillMenu();
        }
        else
        {
            StartCoroutine(flashText());
        }
    }
    public void WaveDisrupt()
    {
        if(empActive == true)
        {
            GameObject wave = Instantiate(wavePrefab, wavePointTrans.position, wavePointTrans.rotation);

            StartCoroutine(MoveWave(wave, maxDistance, destroyTimer));

            gm.player.GetComponent<PlayerController>().ToggleSkillMenu();
        }
        else
        {
            StartCoroutine(flashText());
        }
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
    IEnumerator flashText()
    {
        textObject.SetActive(true);
        objectText.text = "Unlock EMP with an Upgrade Station";

        yield return new WaitForSeconds(displayDuration);

        textObject.SetActive(false);
    }
}
