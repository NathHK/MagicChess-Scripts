using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knight : PieceBaseClass
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
        Debug.Log("Running old OptionsGrid function in file Knight.cs!!!");
        
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

        //Knight moves in an L shape
        //There are 8 different moves that can be performed 
        if(rank+2 <=7 && file+1 <=7 && rank+2 >=0 && file+1 >=0)
            theGrid[rank+2, file+1] = true;
        if(rank+2 <=7 && file-1 >=0 && rank+2 >=0 && file-1 <=7)
            theGrid[rank+2, file-1] = true;
        if(rank-2 >=0 && file+1 <=7 && rank-2 <=7 && file+1 >=0)
            theGrid[rank-2, file+1] = true;
        if(rank-2 >=0 && file-1 >=0 && rank-2 <=7 && file-1 <=7)
            theGrid[rank-2, file-1] = true;
        if(rank+1 <=7 && file+2 <=7 && rank+1 >=0 && file+2 >=0)
            theGrid[rank+1, file+2] = true;
        if(rank+1 <=7 && file-2 >=0 && rank+1 >=0 && file-2 <=7)
            theGrid[rank+1, file-2] = true;
        if(rank-1 >=0 && file+2 <=7 && rank-1 <=7 && file+2 >=0)
            theGrid[rank-1, file+2] = true;
        if(rank-1 >=0 && file-2 >=0 && rank-1 <=7 && file-2 <=7)
            theGrid[rank-1, file-2] = true; */

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
		
		/* MOVEMENT RULE:
		 * ‘L’ shape’: two squares in a horizontal or vertical direction, then move one square horizontally or vertically. 
		 * Only piece able to jump over other pieces.
		 */
		 
		// Reminder:
		// Vertical movement = changing rank
		// Horizontal movement = changing file
		 
		// There are 8 unique destinations to check
		int rank = currRank + 2;
		int file = currFile + 1;
		 
		if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank + 2;
		file = currFile - 1;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank - 2;
		file = currFile + 1;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank - 2;
		file = currFile - 1;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank + 1;
		file = currFile + 2;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank + 1;
		file = currFile - 2;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank - 1;
		file = currFile + 2;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
		
		rank = currRank - 1;
		file = currFile - 2;
		 
        if(rank <=7 && file <=7 && rank >=0 && file >=0) {
			if(pieces[rank, file] != null) { // piece found at dest 
				if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
			} else { // dest is empty
				optionsGrid[rank, file] = true;
			}
		}
	
	} // END OF OptionsGrid()
	

    public override void MovePiece(Vector3 dest, bool attack)
    {

        finalDest = dest;
        stopHere = dest;
        
        if(attack){
            
            //RIGHT
            if(dest.x > agent.destination.x)
                stopHere = new Vector3(dest.x - 1.5f, dest.y, dest.z);
            //LEFT
            else if(dest.x < agent.destination.x)
                stopHere = new Vector3(dest.x + 1.5f, dest.y, dest.z);
            //UP
            else if(dest.z > agent.destination.z) 
                stopHere = new Vector3(dest.x, dest.y, dest.z -(1.5f*posZ));
            //DOWN
            else if(dest.z < agent.destination.z)
                stopHere = new Vector3(dest.x, dest.y, dest.z +(1.5f*posZ));

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
