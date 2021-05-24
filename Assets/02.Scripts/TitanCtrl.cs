using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TitanCtrl : MonoBehaviour
{

    private Transform titanTr;
    private Transform playerTr;

    public GameObject bloodEffect;

    private Transform anotherFirePos;

    private Animator anim;

    private NavMeshAgent agent;

    public enum STATE {IDLE, TRACE, ATTACK, DIE};

    public STATE state = STATE.IDLE;

    public bool isDie = false;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    public float titanHp = 200.0f;

    public readonly int hashTrace   = Animator.StringToHash("IsTrace");
    public readonly int hashAttack  = Animator.StringToHash("IsAttack");
    public readonly int hashHit     = Animator.StringToHash("Hit");
    public readonly int hashDie     = Animator.StringToHash("Die");

    // Start is called before the first frame update
    void Start()
    {
        titanTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        StartCoroutine(CheckState());
        StartCoroutine(TitanAction());
    }

    IEnumerator CheckState()
    {
        while(isDie == false)
        {
            if(state == STATE.DIE)
            {
                yield break;
            }

            float distance = Vector3.Distance(titanTr.position, playerTr.position);
            if(distance <= attackDist)
            {
                state = STATE.ATTACK;
            }
            else if(distance <= traceDist)
            {
                state = STATE.TRACE;
            }
            else
            {
                state = STATE.IDLE;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator TitanAction()
    {
        while(!isDie)
        {
            switch(state)
            {
                case STATE.IDLE :
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                
                case STATE.TRACE :
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;

                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;

                case STATE.ATTACK :
                    anim.SetBool(hashAttack, true);
                    break;
                
                case STATE.DIE :
                    anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    agent.isStopped = true;
                    isDie = true;
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("BULLET"))
        {
            ContactPoint cp = coll.GetContact(0);

            anotherFirePos = GameObject.Find("FirePos")?.GetComponent<Transform>();

            Vector3 firePos = anotherFirePos.position;

            Vector3 normal = cp.normal;

            Vector3 relativePos = firePos - coll.transform.position; //  cp.point

            Vector3 reflectVector = Vector3.Reflect(-relativePos, normal);

            GameObject blood = Instantiate(bloodEffect, cp.point, Quaternion.LookRotation(reflectVector));
            Destroy(blood, 0.25f);
            anim.SetTrigger(hashHit);
            Destroy(coll.gameObject);
            titanHp -= 20.0f;
            if(titanHp <= 0.0f)
            {
                state = STATE.DIE;
            }
        }
    }
}
