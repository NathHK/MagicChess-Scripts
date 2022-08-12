using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Queen : PieceBaseClass
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
        Debug.Log("Running old OptionsGrid function in file Queen.cs!!!");

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
        
        //Queen's movement = combination of rook and bishop movement sets

        //checked for all bounds because my brain can't comprehend white vs black movement direction -.-

        //Bishop can move as far as it wants along any diagonal

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

        //Rook can move as far as it wants up/down/left/right.

        //UP
        rank = currRank+1;
        file = currFile;
        while(rank>=0 && rank<=7){
            theGrid[rank,file] = true;
            rank = rank+1;
        }
        //DOWN
        rank = currRank-1;
        file = currFile;
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
        } */

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
		/* MOVEMENT RULES:
		 * Combination of bishop and rook
		 */
		 
		// Reminder:
		// Vertical movement = changing rank
		// Horizontal movement = changing file
		
		// NOTE:
		// Is there a way to safely access Bishop.cs and Rook.cs?
		// It would be cleaner than copying code from those scripts and pasting it here.
		// I can't rely on the existence of the pieces themselves, since they're destroyed when killed.
		// A possible solution is to change the PieceClasses Update() functions so that, rather than calling Destroy() when the parent piece is killed, the piece is moved off of the board to somewhere out of view.
		
		// --- <<< BISHOP's MOVEMENT >>> ---
		
		// check LEFT & UP:
        int rank = currRank+1;
        int file = currFile-1;
		
		while(rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
		{
			if(pieces[rank, file] != null) { // piece found at pos [rank, file]
				if(manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
				break;
			} else { // pos [rank, file] is empty
				optionsGrid[rank, file] = true;
			}
			// Move to next pos along this diagonal:
			rank += 1; // up
			file -= 1; // left
		}
		
		// check RIGHT & UP:
		rank = currRank + 1;
		file = currFile + 1;
		
		while(rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
		{
			if(pieces[rank, file] != null) { // piece found at pos [rank, file]
				if(manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
				break;
			} else { // pos [rank, file] is empty
				optionsGrid[rank, file] = true;
			}
			// Move to next pos along this diagonal:
			rank += 1; // up
			file += 1; // right
		}
		
		// check LEFT & DOWN:
		rank = currRank - 1;
		file = currFile - 1;
		
		while(rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
		{
			if(pieces[rank, file] != null) { // piece found at pos [rank, file]
				if(manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
				break;
			} else { // pos [rank, file] is empty
				optionsGrid[rank, file] = true;
			}
			// Move to next pos along this diagonal:
			rank -= 1; // down
			file -= 1; // left
		}
		
		// check RIGHT & DOWN:
		rank = currRank - 1;
		file = currFile + 1;
		
		while(rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
		{
			if(pieces[rank, file] != null) { // piece found at pos [rank, file]
				if(manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; 
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
				break;
			} else { // pos [rank, file] is empty
				optionsGrid[rank, file] = true;
			}
			// Move to next pos along this diagonal:
			rank -= 1; // down
			file += 1; // right
		}
		
		// --- <<< ROOK's MOVEMENT >>> ---
		
		// check LEFT
        file = currFile - 1;
        rank = currRank;
        while(file >= 0)
        {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
                // A piece has been found along this path, so all postions past pos [rank, file] are invalid destinations. Thus, their values should remain false, and we are able to stop looping.
                break;
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
            // Move to next pos along this path (leftward):
            file -= 1;
        }
        
        // check RIGHT
        file = currFile + 1;
        rank = currRank;
        while(file <= 7)
        {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
                // A piece has been found along this path => break while loop
                break;
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
            // Move to next pos along this path (rightward):
            file += 1;
        }
        
        // check DOWN
        rank = currRank - 1;
        file = currFile;
        while(rank >= 0)
        {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
                // A piece has been found along this path => break while loop
                break;
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
            // Move to next pos along this path (downward):
            rank -= 1;
        }
        
        // check UP
        rank = currRank + 1;
        file = currFile;
        while(rank <= 7)
        {
            if(pieces[rank, file] != null) { // piece found at pos [rank, file]
                if(manager.PieceOwner(pieces[rank, file]) == manager.PieceOwner(piece)) {
                    optionsGrid[rank, file] = false;
                } else { // it's an enemy piece!
                    optionsGrid[rank, file] = true;
                }
                // A piece has been found along this path => break while loop
                break;
            } else { // pos [rank, file] is empty
                optionsGrid[rank, file] = true;
            }
            // Move to next pos along this path (upward):
            rank += 1;
        }
		
	} // END OF OptionsGrid()
	
    public override void MovePiece(Vector3 dest, bool attack)
    {

        finalDest = dest;
        stopHere = dest;
        
        if(attack){
            
            //RIGHT
            if(dest.x > agent.destination.x && dest.z == agent.destination.z)
                stopHere = new Vector3(dest.x - 1.8f, dest.y, dest.z);
            //LEFT
            else if(dest.x < agent.destination.x && dest.z == agent.destination.z)
                stopHere = new Vector3(dest.x + 1.8f, dest.y, dest.z);
            //UP
            else if(dest.z > agent.destination.z && dest.x == agent.destination.x) 
                stopHere = new Vector3(dest.x, dest.y, dest.z -(1.8f*posZ));
            //DOWN
            else if(dest.z < agent.destination.z && dest.x == agent.destination.x)
                stopHere = new Vector3(dest.x, dest.y, dest.z +(1.8f*posZ));
            else if(dest.x > agent.destination.x){
                //RIGHT & UP
                if(dest.z > agent.destination.z)
                    stopHere = new Vector3(dest.x - 1.4f, dest.y, dest.z-(1.4f*posZ));
                //RIGHT & DOWN
                else if(dest.z < agent.destination.z)
                    stopHere = new Vector3(dest.x - 1.4f, dest.y, dest.z+(1.4f*posZ));
            }
            else if(dest.x < agent.destination.x){
                //LEFT & UP
                if(dest.z > agent.destination.z)
                    stopHere = new Vector3(dest.x + 1.4f, dest.y, dest.z-(1.4f*posZ));
                //LEFT & DOWN
                else if(dest.z < agent.destination.z)
                    stopHere = new Vector3(dest.x + 1.4f, dest.y, dest.z+(1.4f*posZ));
            }
        } //end of if(attack)

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
