using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    //for fov;
    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    //for checking player in proximity;
    public float Hearradius;
    [Range(0, 360)]
    public float Hearangle;

    public LayerMask PlayerMask;
    public LayerMask WallMask;


    public bool canSeePlayer;

    public Transform target;
    public Transform Spawn;
    public NavMeshAgent Enemy;

    public bool canHearPlayer;

    //for making the ai go to where approximately where the player is;
    public bool canFindPlayer;

    public float Fradius;
    [Range(0, 360)]
    public float Fangle;

    public LayerMask FindtargetMask;
    public LayerMask FindobstructionMask;

    //adjusting speed where enemy chase player;
    public float ChaseSpeed = 4;

    public float FindSpeed = 2;

    public float RoamSpeed = 1;


    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
        this.GetComponent<Ai>().enabled = false;
        this.GetComponent<Find>().enabled = false;
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
            PlayerInAreaCheck();
            CheckHearing();
        }
    }

    public void Update()
    {
        if(canFindPlayer == false && canHearPlayer == false && canSeePlayer == false)
        {
            GetComponent<NavMeshAgent>().speed = RoamSpeed;
            ReturnPos();
        }
       
        if (canFindPlayer == true && canHearPlayer == true && canSeePlayer == false)
        {
            FindPlayer();
            GetComponent<NavMeshAgent>().speed = FindSpeed;
        }
        else
        {
            NotFindPlayer();
        }

        if (canHearPlayer == true && canSeePlayer == false && canFindPlayer == false)
        {
            Roam();
            GetComponent<NavMeshAgent>().speed = RoamSpeed;
        }
        else
        {
            NotRoam();
        }

        if (canSeePlayer == true && canHearPlayer == true && canFindPlayer == true)
        {
            ChasePlayer();
            GetComponent<NavMeshAgent>().speed = ChaseSpeed;
        }

        


    }


    //Check if enemy can see player;
    public void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    public void ChasePlayer()
    {
        //transform.LookAt(target);
        Enemy.SetDestination(target.position);
    }

    public void ReturnPos()
    {
        Enemy.SetDestination(Spawn.position);
    }


    //Checking if player is in proximity;
    public void PlayerInAreaCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, Hearradius, PlayerMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < Hearangle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, WallMask))
                    canHearPlayer = true;
                else
                    canHearPlayer = false;
            }
            else
                canHearPlayer = false;
        }
        else if (canHearPlayer)
            canHearPlayer = false;
    }
    
    //Checking if player is in range for enemy to hear;
    public void CheckHearing()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, Fradius, FindtargetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < Fangle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, FindobstructionMask))
                    canFindPlayer = true;
                else
                    canFindPlayer = false;
            }
            else
                canFindPlayer = false;
        }
        else if (canFindPlayer)
            canFindPlayer = false;
    }

    //activating other scripts;

    public void Roam()
    {
        if(canHearPlayer == true)
        {
            this.GetComponent<Ai>().enabled = true;
        }
    }

    public void NotRoam()
    {
        this.GetComponent<Ai>().enabled = false;
    }

    public void FindPlayer()
    {
        this.GetComponent<Find>().enabled = true;
    }

    public void NotFindPlayer()
    {
        this.GetComponent<Find>().enabled = false;
    }
}