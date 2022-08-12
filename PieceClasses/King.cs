using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class King : PieceBaseClass
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

    // Old function with body commented-out. Should never be called.
    public override void OptionsGrid(bool[,] theGrid, int currRank, int currFile)
    {
        Debug.Log("Running old OptionsGrid function in file King.cs!!!");

        /* bool[,] grid = new bool[8,8];

        //set counters to current values
        int rank = currRank;
        int file = currFile;

        //initialize all to /false/
        for(int i = 0; i< 8; i=i+1){
            for(int j = 0; j < 8; j=j+1) {
                theGrid[i,j] = false;
            }
        }

        //King can move one tile in any direction (granted that it's within bounds of the board)
        
        //LEFTWARDS
        file = file-1;
        rank = currRank;
        if(file >=0 && file<=7)
        {
            theGrid[currRank, file] = true;
            
            //up
            rank = rank+1;
            if(rank>=0 && rank<=7)
                theGrid[rank,file] = true;
            //down
            rank = currRank -1;
            if(rank>=0 && rank<=7)
                theGrid[rank,file] = true;
        }

        //RIGHTWARDS
        file = currFile+1;
        rank = currRank;
        if(file>=0 && file<=7)
        {
            theGrid[rank,file] = true;

            //up
            rank = rank+1;
            if(rank>=0 && rank<=7)
                theGrid[rank,file] =true;
            //down
            rank = currRank-1;
            if(rank>=0 && rank<=7)
                theGrid[rank,file] = true;
        }

        //UP
        rank = currRank+1;
        file = currFile;
        if(rank>=0 && rank<=7)
            theGrid[rank,file] =true;
        
        //DOWN
        rank = currRank-1;
        file = currFile;
        if(rank>=0 && rank<=7)
            theGrid[rank,file] = true; */
        
    }

    // NEW OptionsGrid() function:
    // 	pieces : current locations of all pieces on the board
    // 	optionsGrid : validity of destinations; starts all false
    // 	manager : manager of the current game
    // 	currentPlayer : player whose turn it is
    // 	piece : the piece to determine the valid destinations for
    // 	currRank : current rank of piece
    // 	currFile : current file of piece
    public void OptionsGrid(GameObject[,] pieces, bool[,] optionsGrid, Manager manager, Player currentPlayer, GameObject piece, int currRank, int currFile)
    {
        /* MOVEMENT RULES:
        * One square in any direction, so long as that square is not attacked by an enemy piece. 
        * Able to make a special move, known as castling.
        */
        
        // IMPORTANT NOTE:
        //		The logic used for this function is incomplete!
        //		Reason: does not check if dest puts king in check
        //				does not allow for castling
        
        // Reminder:
        // Vertical movement = changing rank
        // Horizontal movement = changing file
        
        // Recall: Manager has already set all values in optionsGrid to false
        
        // check LEFT
        int file = currFile - 1;
        int rank = currRank;
        if(file >= 0) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }
        
        // check RIGHT
        file = currFile + 1;
        rank = currRank;
        if(file <= 7) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }
        
        // check DOWN
        file = currFile;
        rank = currRank - 1;
        if(rank >= 0) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }
        
        // check UP
        file = currFile;
        rank = currRank + 1;
        if(rank <= 7) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }

        // check LEFT & UP
        rank = currRank + 1;
        file = currFile - 1;
        if(rank <= 7 && file >= 0) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }

        // check RIGHT & UP
        rank = currRank + 1;
        file = currFile + 1;
        if(rank <= 7 && file <= 7) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }

        // check LEFT & DOWN
        rank = currRank - 1;
        file = currFile - 1;
        if(rank >= 0 && file >= 0) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }

        // check RIGHT & DOWN
        rank = currRank - 1;
        file = currFile + 1;
        if(rank >= 0 && file <= 7) {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
        }
        
    } // END OF OptionsGrid()

    public override void MovePiece(Vector3 dest, bool attack)
    {
        finalDest = dest;
        stopHere = dest;
        
        if(attack){
            
            if(dest.x > agent.destination.x)
                stopHere = new Vector3(dest.x - 1.3f, dest.y, dest.z-(1.3f*posZ));
            else
                stopHere = new Vector3(dest.x + 1.1f, dest.y, dest.z-(1.2f*posZ));

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
       
        if(other.gameObject.tag.Contains("Weapon") && other.gameObject != weapon && other.gameObject.tag != weapon.tag)
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
