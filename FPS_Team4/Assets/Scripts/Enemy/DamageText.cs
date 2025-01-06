using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    public float lifeTime = 1.0f;
    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        Destroy(gameObject, lifeTime );
        transform.localPosition += new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string damage)
    {
        text.text = damage;
    }
}
