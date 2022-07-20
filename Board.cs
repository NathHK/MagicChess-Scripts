using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // 8x8 array holding the 64 tiles on the chess board
    public Transform[,] tiles = new Transform[8, 8];

    // create an array of length 8 for each of the 8 files
    public Transform[] fileA, fileB, fileC, fileD, fileE, fileF, fileG, fileH = new Transform[8];

    public bool[,] occupiedTiles = new bool[8, 8];

    private void Awake() 
    {
        SetTiles();
        InitializeOccupations();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Returns the Transform at the desired index in the 2d tiles array
    public Transform TileFromArray(int rank, int file)
    {
        return tiles[rank,file];
    }

    public int TileFile(GameObject tileObj)
    {
        int file;
        string lett = tileObj.name.Substring(0,1);
        
        // switch ensures that we search within the correct file array
        switch(lett) 
        {
            case "a":
                file = 0;
                return(file);
            
            case "b":
                file = 1;
                return(file);

            case "c":
                file = 2;
                return(file);
            
            case "d":
                file = 3;
                return(file);
            
            case "e":
                file = 4;
                return(file);

            case "f":
                file = 5;
                return(file);

            case "g":
                file = 6;
                return(file);
            
            case "h":
                file = 7;
                return(file);

            //the default case should never be reached, so print a Debug
            default:
                Debug.Log("TileToPos(tile) switch default!");
                return(0);
        }
    }

    public int TileRank(GameObject tileObj)
    {
        int rank;
        string lett = tileObj.name.Substring(0,1);

        switch(lett) 
        {
            case "a":
                rank = System.Array.IndexOf(fileA, tileObj.transform);
                return(rank);
            
            case "b":
                rank = System.Array.IndexOf(fileB, tileObj.transform);
                return(rank);

            case "c":
                rank = System.Array.IndexOf(fileC, tileObj.transform);
                return(rank);
            
            case "d":
                rank = rank = System.Array.IndexOf(fileD, tileObj.transform);
                return(rank);
            
            case "e":
                rank = System.Array.IndexOf(fileE, tileObj.transform);
                return(rank);

            case "f":
                rank = System.Array.IndexOf(fileF, tileObj.transform);
                return(rank);

            case "g":
                rank = System.Array.IndexOf(fileG, tileObj.transform);
                return(rank);
            
            case "h":
                rank = System.Array.IndexOf(fileH, tileObj.transform);
                return(rank);

            //the default case should never be reached, so print a Debug
            default:
                Debug.Log("TileToPos(tile) switch default!");
                return(0);
        }
    }

    public Vector3 TileToPos(int rank, int file)
    {
        float xPos = tiles[rank, file].position.x;
        float yPos = 0;
        float zPos = tiles[rank, file].position.z;
        return new Vector3(xPos, yPos, zPos);
    }

    public Vector3 TileToPos(Transform tile) 
    {
        int rank;
        int file;
        string lett = tile.name.Substring(0,1);
        
        // switch ensures that we search within the correct file array
        switch(lett) 
        {
            case "a":
                file = 0;
                rank = System.Array.IndexOf(fileA, tile);
                return(TileToPos(rank, file));
            
            case "b":
                file = 1;
                rank = System.Array.IndexOf(fileB, tile);
                return(TileToPos(rank, file));

            case "c":
                file = 2;
                rank = System.Array.IndexOf(fileC, tile);
                return(TileToPos(rank, file));
            
            case "d":
                file = 3;
                rank = rank = System.Array.IndexOf(fileD, tile);
                return(TileToPos(rank, file));
            
            case "e":
                file = 4;
                rank = System.Array.IndexOf(fileE, tile);
                return(TileToPos(rank, file));

            case "f":
                file = 5;
                rank = System.Array.IndexOf(fileF, tile);
                return(TileToPos(rank, file));

            case "g":
                file = 6;
                rank = System.Array.IndexOf(fileG, tile);
                return(TileToPos(rank, file));
            
            case "h":
                file = 7;
                rank = System.Array.IndexOf(fileH, tile);
                return(TileToPos(rank, file));

            //the default case should never be reached, so print a Debug
            default:
                Debug.Log("TileToPos(tile) switch default!");
                return(TileToPos(0, 0));
        }
    }

    // fill the tiles array 
    public void SetTiles()
    {
        Transform[] file = new Transform[8]; 
        for(int i = 0; i<8; i=i+1)
        {
            for(int j = 0; j<8; j=j+1)
            {
                switch(j)
                {
                    case 0:
                        file = fileA;
                        break;
                    case 1:
                        file = fileB;
                        break;
                    case 2:
                        file = fileC;
                        break;
                    case 3:
                        file = fileD;
                        break;
                    case 4:
                        file = fileE;
                        break;
                    case 5:
                        file = fileF;
                        break;
                    case 6:
                        file = fileG;
                        break;
                    case 7:
                        file = fileH;
                        break;
                    default:
                        Debug.Log("Default case.");
                        break;
                }
                tiles[i,j] = file[i];
            }
        } //end outer for()

    } //end fillBoard()

    // print the contents of the tiles array
    public void PrintTiles()
    {
        int rank = 0;
        for(int i = 0; i<8; i=i+1){
            rank = i+1;
            for(int j = 0; j<8; j=j+1){
                Debug.Log("Rank: " + rank + "; Tile: " + tiles[i,j].name);
            }
        }
    }

    // print the contents of the occupations 2d array
    public void PrintOccupations()
    {
        int rank = 0;
        for(int i = 0; i<8; i=i+1)
        {
            rank = i+1;
            for(int j = 0; j<8; j=j+1){
                Debug.Log("Rank: " + rank + "; Tile: " + tiles[i,j].name + "; Occupied?: " + occupiedTiles[i,j] + ";");
            }
        }
    }

    // print the contents of a file array
    public void PrintFile(Transform[] array, GameObject letter)
    {
        int count = 0;

        foreach(Transform child in array)
        {
            Debug.Log("File: " + letter + "; Position: " + count + "; Tile: " + child.name);
            
            count = count+1;
        }

    }

    // at game start, tiles of rank 1, 2, 7, and 8 are occupied
    public void InitializeOccupations()
    {
        bool val = false;
        for(int i = 0; i<8; i=i+1)
        {
            //Note: i = rank - 1;
            if(i == 0 || i == 1 || i == 6 || i == 7)
                val = true;
            else
                val = false;

            for(int j = 0; j<8; j=j+1)
            {
                occupiedTiles[i,j] = val;
            }
        }
    }

    public GameObject AddPiece(GameObject piece, int row, int col)
    {
        
        GameObject newPiece = Instantiate(piece, TileToPos(row, col), Quaternion.identity);
        if(newPiece.tag == "Black")
            newPiece.transform.Rotate(Vector3.up, 180);
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void SelectPiece(GameObject piece)
    {
        
    }
} // END OF CLASS
