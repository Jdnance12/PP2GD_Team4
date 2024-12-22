using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEMP : MonoBehaviour
{

    [SerializeField] float disruptionRadius;

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, disruptionRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDisrupt disruptable = hitCollider.gameObject.GetComponent<IDisrupt>();
            if (disruptable != null)
            {
                disruptable.causeDisrupt();
            }
        }

        Destroy(gameObject);
    }
}
