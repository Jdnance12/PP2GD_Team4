using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlComputer : MonoBehaviour
{
    [Header("---- Bools ----")]
    public bool playerInRange;
    public bool playerInteracted;

    [Header("---- Screen Images ----")]
    [SerializeField] public GameObject aiFace;
    [SerializeField] GameObject interactImage;
    [SerializeField] Collider triggerCollider;
    [SerializeField] List<Image> screens;

    [Header("---- Original Colors ----")]
    List<Color> origScreenColors;
    List<Color> screenColorRed;

    [Header("---- Floats ----")]
    [SerializeField] float pulseDuration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        aiFace.SetActive(false);

        origScreenColors = new List<Color>();
        screenColorRed = new List<Color>();

        foreach(Image screen in screens)
        {
            origScreenColors.Add(screen.color);
            screenColorRed.Add(new Color(1, 0, 0, 0.5f));
        }

        StartCoroutine(flashRed());
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButton("Interact") && !playerInteracted)
            {
                playerInteracted = true;
                interactImage.SetActive(false);
                triggerCollider.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    IEnumerator flashRed()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            float t = Mathf.PingPong(timer, pulseDuration) / pulseDuration;

            for (int i = 0; i < screens.Count; i++)
            {
                screens[i].color = Color.Lerp(origScreenColors[i], screenColorRed[i], t);
            }
            yield return null;
        }
    }
}
