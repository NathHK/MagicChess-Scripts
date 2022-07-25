using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pawn : PieceBaseClass
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
        throw new System.NotImplementedException();
    }

    //Manager must call this function because the pawn's movement depends on whether or not it has already moved
    //  NOTE: This is never called???
    public List<Transform> GetValidDestinations(GameObject piece, Transform currTile, bool hasMoved)
    {
        List<Transform> validDestinations = new List<Transform>();
        return validDestinations;
    }

    public override void OptionsGrid(bool[,] theGrid, int currRank, int currFile)
    {
        throw new System.NotImplementedException();
    }

    //Manager must call this function because the pawn's movement depends on whether or not it has already moved
    public void OptionsGrid(bool[,] theGrid, int currRank, int currFile, bool hasMoved)
    {
        
        Debug.Log("Running function OptionsGrid in file Pawn.cs!!!");

        // Manager has already set all indeces to false

        int rankPlus = currRank+1;
        int filePlus = currFile+1;
        int rankMin = currRank-1;
        int fileMin = currFile-1;
        
        //One tile ahead is always allowed (unless occupied by teammate)
        int val = currRank + posZ;
        theGrid[val, currFile] = true;
        val = val + posZ;

        
        if(!hasMoved)
            theGrid[val, currFile] = true;
        
        
    }

    // NOTE: 
    // I've moved all code from Manager.ValidDestionations() that's relevant to pawns to this new function (and slightly edited it to work when called from this new position) and changed the code in Manager.ValidDestionations() so that it calls this function rather than the one above.
    public void OptionsGrid(GameObject[,] pieces, bool[,] optionsGrid, Manager manager, Player currentPlayer, int currRank, int currFile, bool hasMoved) {

        int rankPlus = currRank+1;
        int filePlus = currFile+1;
        int rankMin = currRank-1;
        int fileMin = currFile-1;

        if(currentPlayer.name == "white"){
            if(currFile != 7) // check for possible captures
                if(pieces[rankPlus, filePlus] != null && manager.PieceOwner(pieces[rankPlus, filePlus]) == "black")
                    optionsGrid[rankPlus,filePlus] = true;
            if(currFile != 0) // check for possible captures   
                if(pieces[rankPlus, fileMin] != null && manager.PieceOwner(pieces[rankPlus, fileMin]) == "black")
                    optionsGrid[rankPlus, fileMin] = true;
            if(pieces[rankPlus, currFile] != null) // 1 space fwd = occ
                optionsGrid[rankPlus, currFile] = false;
            else // 1 space fwd = open
                optionsGrid[rankPlus, currFile] = true;
            if(!hasMoved) // unmoved pawn => check 2 spaces forward
            {
                if(pieces[rankPlus+1, currFile] != null) // 2 sp fwd = occ
                    optionsGrid[rankPlus+1, currFile] = false;
                else if(pieces[rankPlus+1, currFile] == null) // 2 sp fwd = open
                {
                    if(pieces[rankPlus, currFile] != null) // 1 sp fwd = occ
                        optionsGrid[rankPlus+1, currFile] = false; // 2 sp fwd = inv
                    else // 1 sp fwd = open
                        optionsGrid[rankPlus+1, currFile] = true; // 2 sp fwd = val
                }
            }
        }
        
        else //currentPlayer is black
        {
            if(currFile != 7) // check for possible captures
                if(pieces[rankMin, filePlus] != null && manager.PieceOwner(pieces[rankMin, filePlus]) == "white")
                    optionsGrid[rankMin, filePlus] = true;
            if(currFile != 0) // check for possible captures
                if(pieces[rankMin, fileMin] != null && manager.PieceOwner(pieces[rankMin, fileMin]) == "white")
                    optionsGrid[rankMin, fileMin] = true;
            if(pieces[rankMin, currFile] != null) // 1 space fwd = occ
                optionsGrid[rankMin, currFile] = false;
            else // 1 space fwd = open
                optionsGrid[rankMin, currFile] = true;
            if(!hasMoved) // unmoved pawn => check 2 spaces forward
            {
                if(pieces[rankMin-1, currFile] != null) // 2 sp fwd = occ
                    optionsGrid[rankMin-1, currFile] = false;
                else if(pieces[rankMin-1, currFile] == null) // 2 sp fwd = open
                {
                    if(pieces[rankMin, currFile] != null) // 1 sp fwd = occ
                        optionsGrid[rankMin-1, currFile] = false; // 2 sp fwd = inv
                    else // 1 sp fwd = open
                        optionsGrid[rankMin-1, currFile] = true; // 2 sp fwd = val
                }
            }
        }

    }

    public override void MovePiece(Vector3 dest, bool attack)
    {
        finalDest = dest;
        stopHere = dest;
        
        if(attack){
            
            //if opponent is straight ahead, only subtract from z <-- Opponent is never straight ahead dumbdumb -.-
            if(finalDest.x == agent.destination.x)
                stopHere = new Vector3(dest.x, dest.y, dest.z-(1.75f*posZ));
            //
            
            //otherwise, movement is along diagonal
            else
            {
                if(dest.x > agent.destination.x)
                    stopHere = new Vector3(dest.x - 1.1f, dest.y, dest.z-(1.3f*posZ));
                else
                    stopHere = new Vector3(dest.x + 1.1f, dest.y, dest.z-(1.2f*posZ));

            }
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

    private void OnCollisionEnter(Collision other) {
       
        if(other.gameObject.tag.Contains("Weapon") && other.gameObject != weapon && other.gameObject.tag != weapon.tag)
        {
            Debug.Log(other.gameObject.tag);
            if(anim.GetBool("injured")){
                anim.SetBool("death", true);
                timeOfDeath = Time.time;
            }
            else{
                anim.SetBool("injured", true);
                firstHit = Time.time;
                injured.Play();
            }
        }
    }
    
    public Vector3 GetFinalDest()
    {
        return finalDest;
    }


}
