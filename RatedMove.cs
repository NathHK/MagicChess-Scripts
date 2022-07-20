using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatedMove
{
    public GameObject pieceToMove;
    public GameObject currTile;
    public GameObject destTile;
    public int score;

    //construct a RatedMove from an evaluated Move
    public RatedMove(Move move, int score)
    {
        this.pieceToMove = move.pieceToMove;
        this.currTile = move.currTile;
        this.destTile = move.destTile;
        this.score = score;
    }

}
