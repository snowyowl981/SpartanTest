using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;

    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip fireSfx;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private Animator playerAnim;
    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
            playerAnim.SetTrigger(hashFire);
        }
    }

    void Fire()
    {
        Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        audioSource.PlayOneShot(fireSfx, 0.8f);
    }
}
