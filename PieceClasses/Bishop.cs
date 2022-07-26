using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bishop : PieceBaseClass
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

        Debug.Log("Running old OptionsGrid function in file Bishop.cs!!!");

        // NOTE:
        // Why did I make a new array if one was received through the function call?
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
        
        //Bishop can move as far as it wants along any diagonal

        //checked for all bounds because my brain can't comprehend white vs black movement direction -.-

        //left and up
        rank = rank+1;
        file = file-1;
        while(rank <= 7 && file >= 0 && 0<=rank && 7>=file){
            theGrid[rank,file] = true;
            rank = rank+1;
            file = file-1;
        }

        //left and down
        rank = currRank-1;
        file = currFile-1;
        while(rank >= 0 && file >= 0 && 7>=rank && 7>=file){
            theGrid[rank,file] = true;
            rank = rank-1;
            file = file-1;
        }

        //right and up
        rank = currRank+1;
        file = currFile+1;
        while(rank <= 7 && file <=7 && 0<=rank && 0<=file){
            theGrid[rank,file] = true;
            rank = rank+1;
            file = file+1;
        }

        //right and down
        rank = currRank-1;
        file = currFile+1;
        while(rank >= 0 && file > 0 && 7>=rank && 7>=file){
            theGrid[rank,file] = true;
            rank = rank-1;
            file = file+1;
        }

    }

    // pieces : current locations of all pieces on the board
    // optionsGrid : validity of destinations; starts all false
    // manager : manager of the current game
    // currentPlayer : player whose turn it is
    // piece : the piece to determine the valid destinations for
    // currRank : current rank of piece
    // currFile : current file of piece
    public void OptionsGrid(GameObject[,] pieces, bool[,] optionsGrid, Manager manager, Player currentPlayer, GameObject piece, int currRank, int currFile) 
    {

        // MOVEMENT RULE:
        // Bishops can move diagonally in all directions, any number of squares, but cannot jump over other pieces.
        
        // TODO:
        // - Merge functionality of code below (taken from Manager.cs) and code from the old OptionsGrid funtion above this one.
        // - Reformat the while statements so that the loop stops once a piece is found along the path. The location of the found piece, and all tiles beyond that, are already marked as false and should remain that way.

        // check LEFT & UP
        int rank = currRank+1;
        int file = currFile-1;
        bool pieceFound = false;
        int pFoundAt = file;
        bool validFound = false;
        int vFoundAt = file;

        while(file >= 0 && file <= 7 && rank <= 7 && rank >= 0) 
        {

            if(!pieceFound && pieces[rank,file] != null && manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece))
            {
                pieceFound = true;
                pFoundAt = file;
            }
            if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && manager.PieceOwner(pieces[rank,file]) != "" && manager.PieceOwner(pieces[rank,file]) != manager.PieceOwner(piece)){
                validFound = true;
                vFoundAt = file;
            }
            if((pieceFound && file < pFoundAt) || (validFound && file < vFoundAt))
                optionsGrid[rank,file] = false;

            file = file-1;
            rank = rank+1;

        }

        // check RIGHT & UP
        file = currFile+1;
        rank = currRank+1;
        pieceFound = false;
        validFound = false;
        pFoundAt = file;
        vFoundAt = file;

        while(file <= 7 && file >= 0 && rank <= 7 && rank >= 0) 
        {

            if(pieces[rank,file] != null && manager.PieceOwner(pieces[rank,file])== manager.PieceOwner(piece))
            {
                pieceFound = true;
                pFoundAt = file;
            }
            if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && manager.PieceOwner(pieces[rank,file]) != "" && manager.PieceOwner(pieces[rank,file]) != manager.PieceOwner(piece)){
                validFound = true;
                vFoundAt = file;
            }
            if((pieceFound && file > pFoundAt) || (validFound && file>vFoundAt))
                optionsGrid[rank,file] = false;

            file = file+1;
            rank = rank+1;    

        }

        // check LEFT & DOWN
        file = currFile-1;
        rank = currRank-1;
        pieceFound = false;
        pFoundAt = rank;
        validFound = false;
        vFoundAt = rank;

        while(rank <= 7 && rank >= 0 && file >= 0 && file <= 7) 
        {

            if(pieces[rank,file] != null && manager.PieceOwner(pieces[rank,file])== manager.PieceOwner(piece))
            {
                pieceFound = true;
                pFoundAt = rank;
            }
            if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && manager.PieceOwner(pieces[rank,file]) != "" && manager.PieceOwner(pieces[rank,file]) != manager.PieceOwner(piece)){
                validFound = true;
                vFoundAt = rank;
            }
            if((pieceFound && rank>pFoundAt) || (validFound && rank>vFoundAt))
                optionsGrid[rank,file] = false;
            rank = rank-1;
            file = file-1;   

        }

        // check RIGHT & DOWN
        rank = currRank-1;
        file = currFile+1;
        pieceFound = false;
        pFoundAt = rank;
        validFound = false;
        vFoundAt = rank;

        while(rank >= 0 && rank <= 7 && file <= 7 && file >= 0) 
        {

            if(pieces[rank,file] != null && manager.PieceOwner(pieces[rank,file])== manager.PieceOwner(piece))
            {
                pieceFound = true;
                pFoundAt = rank;
            }
            if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && manager.PieceOwner(pieces[rank,file]) != "" && manager.PieceOwner(pieces[rank,file]) != manager.PieceOwner(piece)){
                validFound = true;
                vFoundAt = rank;
            }
            if((pieceFound && rank<pFoundAt) || (validFound && rank<vFoundAt))
                optionsGrid[rank,file] = false;
            rank = rank-1;
            file = file+1;

        }

    }

    public override void MovePiece(Vector3 dest, bool attack)
    {

        finalDest = dest;
        stopHere = dest;
        
        if(attack){

            if(dest.x > agent.destination.x){
                // RIGHT & UP
                if(dest.z > agent.destination.z)
                    stopHere = new Vector3(dest.x - 1.5f, dest.y, dest.z-(1.5f*posZ));
                // RIGHT & DOWN
                else if(dest.z < agent.destination.z)
                    stopHere = new Vector3(dest.x - 1.5f, dest.y, dest.z+(1.5f*posZ));
            }
            else if(dest.x < agent.destination.x){
                // LEFT & UP
                if(dest.z > agent.destination.z)
                    stopHere = new Vector3(dest.x + 1.5f, dest.y, dest.z-(1.5f*posZ));
                // LEFT & DOWN
                else if(dest.z < agent.destination.z)
                    stopHere = new Vector3(dest.x + 1.5f, dest.y, dest.z+(1.5f*posZ));
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

    public Vector3 GetFinalDest()
    {
        return finalDest;
    }

    private void OnCollisionEnter(Collision other) {
       
       // Only take damage from weapons that are not your own
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
