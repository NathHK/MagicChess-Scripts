using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PieceBaseClass : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract List<Transform> GetValidDestinations(GameObject piece, Transform currTile);

    public abstract void OptionsGrid(bool[,] theGrid, int currRank, int currFile);

    public abstract void MovePiece(Vector3 dest, bool attack);

}
