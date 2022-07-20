using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public GameObject pieceToMove;
    public GameObject currTile;
    public GameObject destTile;

    public Move(GameObject piece, GameObject currTile, GameObject destTile)
    {
        this.pieceToMove = piece;
        this.currTile = currTile;
        this.destTile = destTile;
    }

    public Move()
    {
        this.pieceToMove = null;
        this.currTile = null;
        this.destTile = null;
    }

}
