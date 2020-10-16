﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
public class NPC_Wander : NetworkBehaviour
{
    //CURRENT BUGS:
    //gets stuck on walls, I think because it's picking a point inside and obstacle that it then never reaches(therefore never activating a new spot)
    public enum STATES
    {
        STOPPED,
        WALKING
    }

    public NavMeshAgent agent;
    public STATES current_state = STATES.STOPPED;
    public float minStopTime = 0f;
    public float maxStopTime = 5f;
    public float range = 5;

    public bool debugFoundPosition = false;
    public Vector3 debugTargetPos;

    [SerializeField] Animator myAnim;




    Vector3 targetPos;
    [SyncVar]
    Vector3 networkPosition;

    [SyncVar]
    Quaternion networkRotation;

    private void Start()
    {
        if (isServer)
        {
            Walk();



        }


    }

    private void Update()
    {
        if (isServer)
        {
            CmdSync(this.gameObject.transform.position, transform.rotation);
            Vector3 moveDirection = targetPos - transform.position;
            moveDirection = new Vector3(-90f, 0f, moveDirection.z);
            if (moveDirection != Vector3.zero) //prevent 'snap' into vector3.zero position once stopped moving
                //transform.rotation = Quaternion.LookRotation(moveDirection);
            if (current_state == STATES.WALKING)
            {
                myAnim.SetBool("isWalking", true);

                float dist = agent.remainingDistance;
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                {
                    //Arrived.
                    Pause();
                }
            }
        }



    }


    void Walk()
    {
        current_state = STATES.WALKING;

        //----find acceptable position on map...
        bool foundDestination = false;
        debugFoundPosition = false;
        targetPos = this.transform.position + new Vector3(Random.insideUnitSphere.x * range, 0f, Random.insideUnitSphere.z * range);
        NavMeshHit hit;

        foundDestination = NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas);
        // while (!foundDestination)
        // {
        //     targetPos = this.transform.position + new Vector3(Random.insideUnitSphere.x * range, 0f, Random.insideUnitSphere.z * range);
        //     foundDestination = NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas);
        // }

 
        debugTargetPos = targetPos;
        debugFoundPosition = true;


        agent.SetDestination(targetPos);

    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
    }

    void Pause()
    {
        myAnim.SetBool("isWalking", false);

        current_state = STATES.STOPPED;
        float randomTime = Random.Range(minStopTime, maxStopTime);
        Invoke("Walk", randomTime);
    }
}
