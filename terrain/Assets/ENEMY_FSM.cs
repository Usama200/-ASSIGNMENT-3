using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY_FSM : MonoBehaviour
{
    public enum enemy_state { PATROL, CHASE, ATTACK };
    [SerializeField]
    private enemy_state currentstate;
    private Health playerhealth = null;
    public float maxDamage = 10f;
    public enemy_state CurrentState
    {
        get
        {
            return currentstate;
        }
        set
        {
            currentstate = value;
            StopAllCoroutines();
            switch (currentstate)
            {
                case enemy_state.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case enemy_state.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case enemy_state.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
            }
        }
    }

    private checkmyvision checkmyVision;
    private UnityEngine.AI.NavMeshAgent agent = null;
    private Transform playertransform = null;
    private Transform patrolDestination = null;

    private void Awake()
    {
        checkmyVision = GetComponent<checkmyvision>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerhealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playertransform = playerhealth.GetComponent<Transform>();

    }

    // Start is called before the first frame update

    void Start()
    {
        GameObject[] destination = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destination[Random.Range(0, destination.Length)].GetComponent<Transform>();
        currentstate = enemy_state.PATROL;


    }
    public IEnumerator EnemyPatrol()
    {
        while (currentstate == enemy_state.PATROL)
        {
            checkmyVision.sensitity = checkmyvision.enmsensitity.High;
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);

            while (agent.pathPending)
                yield return null;

            if (checkmyVision.targetInSight)
            {
                agent.isStopped = true;
                currentstate = enemy_state.CHASE;
                yield break;
            }
            yield break;
        }

    }
    public IEnumerator EnemyChase()
    {
        while (currentstate == enemy_state.CHASE)
        {
            checkmyVision.sensitity = checkmyvision.enmsensitity.Low;
            agent.isStopped = false;
            agent.SetDestination(checkmyVision.lastKhownSighting);

            while (agent.pathPending)
            {
                yield return null;
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                if (!checkmyVision.targetInSight)
                    currentstate = enemy_state.PATROL;
                else
                    currentstate = enemy_state.ATTACK;
                yield break;
            }
            yield return null;
        }

    }
    public IEnumerator EnemyAttack()
    {
        while (currentstate == enemy_state.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playertransform.position);
            while (agent.pathPending)
                yield return null;

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                currentstate = enemy_state.CHASE;
            }
            else
            {
                playerhealth.healthpoints -= maxDamage * Time.deltaTime;
            }
            yield return null;
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
