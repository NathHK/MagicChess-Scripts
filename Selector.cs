using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Selector : MonoBehaviour
{

    private Material originalMat;

    private bool tileSelected; //has a tile been selected?
    private bool pieceSelected; //has a chess piece been selected?
    private GameObject selectedPiece; //piece that is selected
    private GameObject selectedTile; //tile that is selected
    private List<GameObject> validTiles; //valid destinations for selectedPiece

    public Manager manager;
    public Board board;

    // Start is called before the first frame update
    void Start()
    {
        tileSelected = false;
        pieceSelected = false;
        validTiles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            SelectTile();
                
        
    }

    void FixedUpdate() 
    {
        //if(Input.GetMouseButtonDown(0))
            //SelectTile();
                
    }

    public void SelectTile()
    {
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out hit, 1000)) 
        {
            
            if(hit.collider.gameObject.layer == 8) //"Tiles" layer
            {

                if(tileSelected) // highlighted tile(s) already exist(s)
                {
                    
                    //revert mat of previously selected tile
                    selectedTile.GetComponent<Tile>().RevertMaterial();

                    //                 <<<*IMPORTANT*>>>
                    //  If a piece has already been selected, the player is attempting to move to the tile hit by this ray.
                    //  We need to check if the tile hit exists within the validTiles list. The list would've already been filled during the call to SelectPiece() which occurs at the end of the if(layer == "Tiles") statement.
                    GameObject tileHit = hit.collider.gameObject; 
                    if(pieceSelected && validTiles.Contains(tileHit)) // VALID!
                    {
                        // Move the selected piece from its current destination (selectedTile) to the desired destination (tileHit)   
                        manager.MovePiece(selectedPiece, selectedTile, tileHit);

                        // After the move has been made, recognize that no tile or piece is currently selected -> set bools to false
                        tileSelected = false;
                        pieceSelected = false;
                    }

                    // Else -> tileHit is not a valid destination, make it the new selected tile.
                    else 
                    {
                        //set value of selectedTile to tile hit by the ray
                        selectedTile = tileHit;
                        //highlight the tile
                        selectedTile.GetComponent<Tile>().HighlightSelected();
                        tileSelected = true;
                        pieceSelected = false;
                        selectedPiece = null;

                    }
                    
                    //revert material of previous valid tiles
                    foreach(GameObject t in validTiles)
                    {
                        t.GetComponent<Tile>().RevertMaterial();
                    }
                    
                    // At this point, either selectedPiece has been moved or the value of selectedTile has changed. Both cases dictate that validTiles is no longer accurate. 
                    validTiles.Clear(); //empty the validTiles list
                    
                }
                
                else //no previously highlighted tile(s) existed
                {
                    //set value of selectedTile to tile hit by the ray
                    selectedTile = hit.collider.gameObject;
                    //change tile's material
                    selectedTile.GetComponent<Tile>().HighlightSelected();
                    tileSelected = true;
                }

                //revert material of previous valid tiles
                foreach(GameObject t in validTiles)
                    {
                        t.GetComponent<Tile>().RevertMaterial();
                    }

                //call function to select piece on selectedTile (if it exists)
                if(tileSelected)
                    SelectPiece(selectedTile);
                
            } //end of if condition for objects in the "Tiles" layer

        } //end of if(Physics.Raycast...)

        else //ray didn't hit anything (this indicates that the player did not click on a tile)
        {
            if(tileSelected) //clear all prior selections
            {
                selectedTile.GetComponent<Tile>().RevertMaterial();
                foreach(GameObject t in validTiles)
                {
                    t.GetComponent<Tile>().RevertMaterial();
                }
                validTiles.Clear(); //empty the validTiles list
            }

            tileSelected = false; //no current tile selection
            pieceSelected = false; //no current piece selection
        }

    } //end of SelectTile()

    public void SelectPiece(GameObject tile)
    {
        int rank = board.TileRank(tile);
        int file = board.TileFile(tile);
        
        if(manager.PieceAtTile(rank,file) != null) //tile is occupied
        {
            GameObject piece = manager.PieceAtTile(rank, file);

            //the selection is only valid if the piece is white
            string team = manager.PieceOwner(piece);
            if(team == "white")
            {
                pieceSelected = true;
                //set value of selectedPiece to piece at this location
                selectedPiece = piece;
            }

            else //piece is black
            {
                pieceSelected = false;
            }
        }

        else //tile is unoccupied
        {
            pieceSelected = false;
        }

        if(pieceSelected) //tile is occupied by valid (white) piece
        {
            tile.GetComponent<Tile>().HighlightGreen(); //change mat to green
            
            // Calculate valid destinations and assign result to validTiles
            validTiles = manager.ValidDestinations(selectedPiece, selectedTile);
            // Highlight each of the valid destinations
            foreach(GameObject t in validTiles)
            {
                t.GetComponent<Tile>().HighlightValid();
            }
        }

    } //end of SelectPiece()

} //END OF CLASS
