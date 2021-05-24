using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRemove : MonoBehaviour
{

    public GameObject sparkEffect;
    public Transform anotherFirePos;
    
    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("BULLET"))
        {
            ContactPoint cp = coll.GetContact(0);

            anotherFirePos = GameObject.Find("FirePos").GetComponent<Transform>();

            Vector3 firePos = anotherFirePos.position;

            Vector3 normal = cp.normal;

            Vector3 realativePos = firePos - coll.transform.position;

            Vector3 reflectVector = Vector3.Reflect(-realativePos, normal);

            GameObject spark = Instantiate(sparkEffect, coll.transform.position, Quaternion.LookRotation
            (reflectVector));

            Destroy(spark, 0.25f);

            Destroy(coll.gameObject);
        }
    }
}
