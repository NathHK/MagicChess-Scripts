using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<GameObject> pieces;
    public string name;
    public int forward;
    public bool cpu;
    public bool minimax;
    //public GameObject[,] piecePositions;
    public List<Move> allMoves;
    static Manager manager;
    static Board board;

    // This constructor is used for the human player, so cpu and minimax are both set to false
    public Player(string name, bool positiveZMovement)
    {
        this.name = name;
        pieces = new List<GameObject>();

        if (positiveZMovement == true)
        {
            this.forward = 1;
        }
        else
        {
            this.forward = -1;
        }

        this.cpu = false;
        this.minimax = false;

        this.allMoves = new List<Move>();
    }

    // This constructor is used for cpu players; values for cpu and minimax are provided by the call to the constructor
    public Player(string name, bool positiveZMovement, bool cpu, bool minimax)
    {
        this.name = name;
        pieces = new List<GameObject>();

        if (positiveZMovement == true)
        {
            this.forward = 1;
        }
        else
        {
            this.forward = -1;
        }

        this.cpu = cpu;
        this.minimax = minimax;

        this.allMoves = new List<Move>();
    }

    // Function called by Manager when next turn is AI
    public void CompTurn(GameObject[,] piecePositions)
    {

        GameObject tile;
        List<GameObject> destTiles;

        // Clear the list of possible moves
        allMoves.Clear();

        // Start by compiling a list of all possible moves
        foreach(GameObject piece in pieces) // iterate through each of the current player's pieces
        {
            // Manager.ValidDestinations() returns a List of GameObjects, which are the valid destinations for the 'piece' parameter
            
            // Iterate through all tile positions
            for(int i = 0; i < 8; i=i+1) {
                for(int j = 0; j < 8; j=j+1) {

                    // Start by getting the piece's current position
                    if(manager.PieceAtTile(i,j) == piece) {
                        
                        tile = board.TileFromArray(i,j).gameObject;
                        // All valid destinations for the piece:
                        destTiles = manager.ValidDestinations(piece, tile);

                        // --- <<< DEBUGGING >>> ---

                        // Goal: Determine why AI pieces disregard pieces in their path when judging if a move is valid
                        
                        // We want to print the list of all "valid" destinations to the System Log

                        string combinedList = "PIECE: " + piece.name + "; POS: "+ tile.name + "; DEST: ";
                        foreach(GameObject tileFromList in destTiles) {
                            if(destTiles.FindIndex(a => (a == tileFromList)) == 0)
                                combinedList += tileFromList.name;
                            else
                                combinedList += (", " + tileFromList.name);
                        }

                        Debug.Log("AI-" + name + " found these possible moves: \n" + combinedList);

                        // --- <<< END OF DEBUGGING >>> ---

                        // Add all poss moves for that piece to allMoves:
                        foreach(GameObject dest in destTiles) {
                            allMoves.Add(new Move(piece, tile, dest));
                        }
                    }
                } //inner
            } //outer
            
        } //foreach

        // Comp uses minimax algorithm
        if(this.minimax)
        {
            // Use allMoves to obtain an array of rated moves
            RatedMove[] ratedMoves = RateMoves();
            
            // WHITE is MINimizer (they want to obtain negative--black--pieces.)
            // BLACK is MAXimizer
            bool isMaximizer = (this.name == "black");
            int[] scores = GetScores(ratedMoves);
            bool allZero = true;
            for(int i = 0; i<scores.Length; i=i+1)
                if(scores[i] != 0)
                    allZero = false;


            RatedMove alpha = new RatedMove((allMoves.ToArray())[0], -9999);
            RatedMove beta = new RatedMove(new Move(), 9999);
            RatedMove bestMove = Minimax(0, 0, alpha, beta, isMaximizer, ratedMoves);
            if(allZero)
                manager.MovePiece(ratedMoves[0].pieceToMove, ratedMoves[0].currTile, ratedMoves[0].destTile);
            else
                manager.MovePiece(bestMove.pieceToMove, bestMove.currTile, bestMove.destTile);
        }

        // Comp does NOT use minimax algorithm
        // => get random index in allMoves and execute that move
        else
        {
            allMoves.TrimExcess();
            int r = Random.Range(0, allMoves.Count);
            Move[] all = allMoves.ToArray();
            Move move = all[r];

            manager.MovePiece(move.pieceToMove, move.currTile, move.destTile);
        }

    } //end of CompTurn

    // Using the a-b minimax algorithm, determine which move to execute
    public RatedMove Minimax(int depth, int nodeIndex, RatedMove alpha, RatedMove beta, bool isMaximizer, RatedMove[] ratedMoves)
    {

        RatedMove bestMove;
        
        // Terminating condition == leaf-node is reached
        if(depth == 3)
            return ratedMoves[nodeIndex];

        // Player is MAXIMIZER
        if(isMaximizer)
        {
            Debug.Log(this.name + " is maximizer");
            int bestScore = -9999;
            bestMove = new RatedMove(new Move(), bestScore);

            for(int i = 0; i < 2; i=i+1)
            {
                RatedMove curr = Minimax(depth+1, nodeIndex*2 + i, alpha, beta, false, ratedMoves);
                Debug.Log(depth);

                bestMove = MaxMove(bestMove, curr);
                alpha = MaxMove(alpha, bestMove);

                // A-B pruning
                if(beta.score <= alpha.score)
                    break;
            }
            return bestMove;
        }

        // Player is MINIMIZER
        else
        {
            int bestScore = 9999;
            bestMove = new RatedMove(new Move(), bestScore);

            for(int i = 0; i < 2; i=i+1)
            {
                RatedMove curr = Minimax(depth+1, nodeIndex*2 + i, alpha, beta, true, ratedMoves);
                
                bestMove = MaxMove(bestMove, curr);
                beta = MaxMove(beta, bestMove);

                // A-B pruning
                if(beta.score <= alpha.score)
                    break;
            }
            return bestMove;
        }
    } //end of Minimax()

    public int[] GetScores(RatedMove[] ratedMoves)
    {
        int[] scores = new int[ratedMoves.Length];
        for(int i = 0; i < ratedMoves.Length; i=i+1)
            scores[i] = ratedMoves[i].score;
        
        Debug.Log("scores length = " + scores.Length + "; scores[last] = " + scores[scores.Length-1]);
        return scores;
    }

    public RatedMove[] RateMoves()
    {
        // Convert allMoves to an array
        allMoves.TrimExcess();
        Move[] movesArr = allMoves.ToArray();
        // Create an array of length = number of moves held in allMoves
        RatedMove[] ratedMoves = new RatedMove[movesArr.Length];

        // To calculate a move's score, we need to know whether it results in a 'capture' and, if so, the type of its victim

        // Iterate through each index in movesArr and fill ratedMoves
        for(int i = 0; i < movesArr.Length; i=i+1)
        {
            Move currMove = movesArr[i];
            GameObject dest = currMove.destTile;
            Debug.Log(dest.gameObject.name);
            int destRank = board.TileRank(dest);
            int destFile = board.TileFile(dest);

            // Get the value of the piece at destTile
            // (value of occupant piece = score for move to that dest)
            int score = manager.GetPieceValue(destRank, destFile);

            // RatedMove at index i = currMove + score
            ratedMoves[i] = new RatedMove(currMove, score);
        }
        return ratedMoves;
    }

    // Returns the RatedMove with highest score (or m1 if equal)
    public RatedMove MaxMove(RatedMove m1, RatedMove m2)
    {
        int score1 = m1.score;
        int score2 = m2.score;

        if(score1 >= score2)
            return m1;
        else
            return m2;
    }

    public void SetManager(Manager m)
    {
        manager = m;
    }

    public void SetBoard(Board b)
    {
        board = b;
    }

} //END OF CLASS Player
