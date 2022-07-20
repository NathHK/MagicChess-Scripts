﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rook : PieceBaseClass
{

    private NavMeshAgent agent;
    private Animator anim;

    private bool attackAfterMove;

    private Vector3 finalDest;
    private Vector3 stopHere;

    private float deathTimer = 2f;
    private float firstHit;
    private float timeOfDeath;

    public GameObject weapon;

    private int posZ;

    public AudioSource death;
    public AudioSource walk;
    public AudioSource injured;
    
    private bool walkAudioOn;
    private bool deathPlayed;
    private bool injuredPlayed;
    

    // Start is called before the first frame update
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.destination = gameObject.transform.position;
        agent.updatePosition = false;
        
        stopHere = new Vector3(0,0,0);
        finalDest = stopHere;

        timeOfDeath = Mathf.Infinity;

        if(gameObject.name.Contains("White"))
            posZ = 1;
        else    
            posZ = -1;

        firstHit = 0;

        weapon.GetComponent<Collider>().enabled = false;
        
        walkAudioOn = false;
        injuredPlayed =false; 
        deathPlayed = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(anim.GetBool("attacking"))
            weapon.GetComponent<Collider>().enabled = true;
        else
            weapon.GetComponent<Collider>().enabled = false;

        bool shouldWalk = agent.remainingDistance > agent.radius/2;
        
        //update animation parameters
        anim.SetBool("walking", shouldWalk);

        if(anim.GetBool("walking") && !walkAudioOn){
            walk.PlayDelayed(.15f);
            walkAudioOn = true;
        }

        if(agent.remainingDistance == 0)
            walkAudioOn = false;

        if(anim.GetBool("death") && !deathPlayed){
            death.Play();
            deathPlayed = true;
        }
        

        if(anim.GetBool("injured") && !anim.GetBool("death") && (Time.time - firstHit) > 1.5f){
            anim.SetBool("death", true);
            timeOfDeath = Time.time;
        }
        
        if(Time.time - timeOfDeath >= deathTimer)
            Destroy(gameObject);

        
    }

    public override List<Transform> GetValidDestinations(GameObject piece, Transform currTile)
    {
        List<Transform> validDestinations = new List<Transform>();
        return validDestinations;
    }

    public override void OptionsGrid(bool[,] theGrid, int currRank, int currFile)
    {
        bool[,] grid = new bool[8,8];

        //set counters to current values
        int rank = currRank;
        int file = currFile;

        //initialize all to /false/
        for(int i = 0; i< 8; i=i+1){
            for(int j = 0; j < 8; j=j+1) {
                theGrid[i,j] = false;
            }
        }

        //Rook can move as far as it wants up/down/left/right.

        //UP
        rank = currRank+1;
        while(rank>=0 && rank<=7){
            theGrid[rank,file] = true;
            rank = rank+1;
        }
        //DOWN
        rank = currRank-1;
        while(rank>=0 && rank<=7){
            theGrid[rank,file] = true;
            rank = rank-1;
        }
        //LEFT
        rank = currRank;
        file = currFile-1;
        while(file>=0 && file<=7){
            theGrid[rank,file] = true;
            file = file-1;
        }
        //RIGHT
        rank = currRank;
        file = currFile+1;
        while(file>=0 && file<=7){
            theGrid[rank,file] = true;
            file = file+1;
        }

    }

    public override void MovePiece(Vector3 dest, bool attack)
    {

        finalDest = dest;
        stopHere = dest;
        
        if(attack){
            
            //RIGHT
            if(dest.x > agent.destination.x)
                stopHere = new Vector3(dest.x - 1.9f, dest.y, dest.z);
            //LEFT
            else if(dest.x < agent.destination.x)
                stopHere = new Vector3(dest.x + 1.9f, dest.y, dest.z);
            //UP
            else if(dest.z > agent.destination.z) 
                stopHere = new Vector3(dest.x, dest.y, dest.z -(1.9f*posZ));
            //DOWN
            else if(dest.z < agent.destination.z)
                stopHere = new Vector3(dest.x, dest.y, dest.z +(1.9f*posZ));

        }

        agent.destination = stopHere;

        attackAfterMove = attack;
        anim.SetBool("attacking", attackAfterMove);
        

    }

    private void OnAnimatorMove() {
            
        if (!agent.isOnOffMeshLink)
        {
            transform.position = agent.nextPosition;
        }
    
    }

    public Vector3 GetFinalDest()
    {
        return finalDest;
    }

    private void OnCollisionEnter(Collision other) {
       
        if(other.gameObject.tag.Contains("Weapon") && other.gameObject != weapon)
        {
            Debug.Log(other.gameObject.tag);
            if(anim.GetBool("injured")){
                anim.SetBool("death", true);
                timeOfDeath = Time.time;
                //death.Play();
            }
            else{
                anim.SetBool("injured", true);
                firstHit = Time.time;
                injured.Play();
            }
        }
    }

}
