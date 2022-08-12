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

    // Old function w/ body removed. Should never be called.
    public override void OptionsGrid(bool[,] theGrid, int currRank, int currFile)
    {

        Debug.Log("Running old OptionsGrid function in file Bishop.cs!!!");

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
         * Bishops can move diagonally in all directions, any number of squares, but cannot jump over other pieces.
		 */
		// Reminder:
		// Vertical movement = changing rank
		// Horizontal movement = changing file
        
		// check LEFT & UP:
        int rank = currRank+1;
        int file = currFile-1;
		
		// The checks "file <= 7" and "rank >= 0" are technically not needed.
		// This is b/c rank is being incremented and file decremented.
		// However, I might keep them there as a "better safe than sorry" approach.
		while(rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
		{
			if(pieces[rank, file] != null) { // piece found at pos [rank, file]
				if(manager.PieceOwner(pieces[rank,file]) == manager.PieceOwner(piece)) {
					optionsGrid[rank, file] = false; // technically not needed, but I feel that it adds clarity and is thus worthwhile
				} else { // it's an enemy piece!
					optionsGrid[rank, file] = true;
				}
				// A piece has been found along this diagonal, so all positions past pos [rank, file] are invalid destinations. Thus, their values should remain false, and we are able to stop looping.
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
				// A piece has been found along this diagonal => break while loop
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
				// A piece has been found along this diagonal => break while loop
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
				// A piece has been found along this diagonal => break while loop
				break;
			} else { // pos [rank, file] is empty
				optionsGrid[rank, file] = true;
			}
			// Move to next pos along this diagonal:
			rank -= 1; // down
			file += 1; // right
		}
		
	} // END OF OptionsGrid()

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
