using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Manager : MonoBehaviour
{
    public GameObject selectorObj;
    static Manager instance;
    public Board board;
    public GameObject gameOver;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    private GameObject[,] pieces; //tracks location of chess pieces

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    private List<GameObject> movedPawns;

    private bool[,] optionsGrid;

    private bool attack;
    private NavMeshAgent movingPiece;
    private bool moveSent;
    private string gameMode;
    private bool endGame;
    private float simStart;
    private bool firstMove;

    void Awake() 
    {
        instance = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8,8];
        movedPawns = new List<GameObject>();

        //On start, game-mode is NORMAL, so white is human, black is basic cpu
        gameMode = "normal";
        white = new Player("white", true);
        black = new Player("black", false, true, false);

        currentPlayer = white;
        otherPlayer = black;
        currentPlayer.SetManager(instance);
        currentPlayer.SetBoard(board);

        InitialSetup();

        optionsGrid = new bool[8,8];

        attack = false;
        moveSent = false;
        endGame = false;

    }

    // Update is called once per frame
    void Update()
    {

        if(moveSent && movingPiece.remainingDistance == 0)
        {
            moveSent = false;
            CheckForGameOver();
            
            if(!endGame)
                NextPlayer();
        }

        if(gameMode == "sim" && firstMove && (Time.time - simStart >= 2f)){
            currentPlayer.CompTurn(pieces);
            firstMove = false;
        }

    }


    private void InitialSetup()
    {
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 0, 1);
        AddPiece(whiteBishop, white, 0, 2);
        AddPiece(whiteQueen, white, 0, 3);
        AddPiece(whiteKing, white, 0, 4);
        AddPiece(whiteBishop, white, 0, 5);
        AddPiece(whiteKnight, white, 0, 6);
        AddPiece(whiteRook, white, 0, 7);

        for (int i = 0; i < 8; i=i+1)
        {
            AddPiece(whitePawn, white, 1, i);
        }

        AddPiece(blackRook, black, 7, 0);
        AddPiece(blackKnight, black, 7, 1);
        AddPiece(blackBishop, black, 7, 2);
        AddPiece(blackQueen, black, 7, 3);
        AddPiece(blackKing, black, 7, 4);
        AddPiece(blackBishop, black, 7, 5);
        AddPiece(blackKnight, black, 7, 6);
        AddPiece(blackRook, black, 7, 7);

        for (int i = 0; i < 8; i=i+1)
        {
            AddPiece(blackPawn, black, 6, i);
        }


    }

    public void AddPiece(GameObject prefab, Player player, int row, int col)
    {
        GameObject pieceObj = board.AddPiece(prefab, row, col);
        pieces[row, col] = pieceObj;
        player.pieces.Add(pieceObj);
    }

    public GameObject PieceAtTile(int rank, int file)
    {
        return pieces[rank, file];
    }

    //returns name of team the given piece belongs to
    public string PieceOwner(GameObject piece)
    {
        string owner = "";

        if(white.pieces.Contains(piece))
            owner = "white";
        
        else if(black.pieces.Contains(piece))
            owner = "black";

        return owner;
    }

    // Returns a list of tiles that the given piece is able to move to
    //      called by: Selector -> SelectPiece() 
    //            and:   Player -> GetAllMoves()
    public List<GameObject> ValidDestinations(GameObject piece, GameObject currTile)
    {
        //start by clearing optionsGrid (sets all to 'false')
        ClearValid();

        string team = PieceOwner(piece); //which team the piece belongs to

        string type = piece.name; // name of the GameObject
        Transform startTile = currTile.transform;

        List<GameObject> validTiles = new List<GameObject>(); // valid destinations will be added to this list

        int currRank = board.TileRank(currTile);
        int currFile = board.TileFile(currTile);

        //fill the optionsGrid 2d-array using correct call to OptionsGrid()
        if(type.Contains("Bishop")){
            piece.GetComponent<Bishop>().OptionsGrid(optionsGrid, currRank,currFile);

            // Edit optionsGrid, so that bishop can't jump over other pieces

            // check LEFT & UP
            int rank = currRank+1;
            int file = currFile-1;
            bool pieceFound = false;
            int pFoundAt = file;
            bool validFound = false;
            int vFoundAt = file;

            while(file>=0 && file<=7 && rank<=7 && rank>=0){
                if(!pieceFound && pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file<pFoundAt) || (validFound && file < vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file-1;
                rank = rank+1;
            }
            // check RIGHT & UP
            file = currFile+1;
            rank = currRank+1;
            pieceFound = false;
            validFound = false;
            pFoundAt = file;
            vFoundAt = file;
            while(file<=7 && file>=0 && rank<=7 && rank>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file > pFoundAt) || (validFound && file>vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file+1;
                rank = rank+1;    
            }
            // check LEFT & DOWN
            file = currFile-1;
            rank = currRank-1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank<=7 && rank>=0 && file>=0 && file<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank>pFoundAt) || (validFound && rank>vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
                file = file-1;    
            }
            // check RIGHT & DOWN
            rank = currRank-1;
            file = currFile+1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank>=0 && rank<=7 && file<=7 && file>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank<pFoundAt) || (validFound && rank<vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
                file = file+1;
            }
        } //end of BISHOP

        else if(type.Contains("King")){
            piece.GetComponent<King>().OptionsGrid(optionsGrid, currRank,currFile);
        }

        else if(type.Contains("Horse")){
            piece.GetComponent<Knight>().OptionsGrid(optionsGrid, currRank,currFile);
        }

        else if(type.Contains("Pawn")){ // <--- SPECIAL CASE
            bool hasMoved = movedPawns.Contains(piece);
            piece.GetComponent<Pawn>().OptionsGrid(optionsGrid, currRank, currFile, hasMoved);

            int rankPlus = currRank+1;
            int filePlus = currFile+1;
            int rankMin = currRank-1;
            int fileMin = currFile-1;

            // NOTE:
            // Why am I setting optionsGrid indexes to 'false'--again? 
            // I start this function with ClearValid(), which sets all values to false. As such, I can't think of any reason for checking conditions under the premise of doing so (i.e. it's redundant).
            // It makes more sense to check conditions that reveal valid destinations and change the relevant optionsGrid index to 'true'.

            if(currentPlayer.name == "white"){
                if(currFile != 7) // check for possible captures
                    if(pieces[rankPlus, filePlus] != null && PieceOwner(pieces[rankPlus, filePlus]) == "black")
                        optionsGrid[rankPlus,filePlus] = true;
                if(currFile != 0) // check for possible captures   
                    if(pieces[rankPlus, fileMin] != null && PieceOwner(pieces[rankPlus, fileMin]) == "black")
                        optionsGrid[rankPlus, fileMin] = true;
                if(pieces[rankPlus, currFile] != null) // 1 space fwd = occ
                    optionsGrid[rankPlus, currFile] = false;
                if(pieces[rankPlus+1, currFile] != null) // 2 spaces fwd = occ
                    optionsGrid[rankPlus+1, currFile] = false;
                else if(pieces[rankPlus+1, currFile] == null) // 2 sp fwd = open
                    if(pieces[rankPlus, currFile] != null) // 1 space fwd = occ
                        optionsGrid[rankPlus, currFile] = false;
                    else // 1 space fwd = open
                        optionsGrid[rankPlus, currFile] = true;
            }
            
            // TODO: Apply above changes to the code below!
            else //currentPlayer is black
            {
                if(currFile != 7)
                    if(pieces[rankMin, filePlus] != null && PieceOwner(pieces[rankMin, filePlus]) == "white")
                        optionsGrid[rankMin, filePlus] = true;
                if(currFile != 0)
                    if(pieces[rankMin, fileMin] != null && PieceOwner(pieces[rankMin, fileMin]) == "white")
                        optionsGrid[rankMin, fileMin] = true;
                if(pieces[rankMin, currFile] != null)
                    optionsGrid[rankMin, currFile] = false;
                if(pieces[rankMin-1, currFile] != null)
                    optionsGrid[rankMin-1, currFile] = false;
            }

        }

        else if(type.Contains("Queen")){
            piece.GetComponent<Queen>().OptionsGrid(optionsGrid, currRank,currFile);

            // This one's a mess, huh? :')
            //check LEFT & UP
            int rank = currRank+1;
            int file = currFile-1;
            bool pieceFound = false;
            int pFoundAt = file;
            bool validFound = false;
            int vFoundAt = file;

            while(file>=0 && file<=7 && rank<=7 && rank>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file<pFoundAt) || (validFound && file < vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file-1;
                rank = rank+1;
            }
            //check RIGHT & UP
            file = currFile+1;
            rank = currRank+1;
            pieceFound = false;
            validFound = false;
            pFoundAt = file;
            vFoundAt = file;
            while(file<=7 && file>=0 && rank<=7 && rank>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file > pFoundAt) || (validFound && file>vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file+1;
                rank = rank+1;    
            }
            //check LEFT & DOWN
            file = currFile-1;
            rank = currRank-1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank<=7 && rank>=0 && file>=0 && file<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank>pFoundAt) || (validFound && rank>vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
                file = file-1;    
            }
            //check RIGHT & DOWN
            rank = currRank-1;
            file = currFile+1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank>=0 && rank<=7 && file<=7 && file>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank<pFoundAt) || (validFound && rank<vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
                file = file+1;
            }
            //check LEFT
            rank = currRank;
            file = currFile-1;
            pieceFound = false;
            pFoundAt = file;
            validFound = false;
            vFoundAt = file;

            while(file>=0 && file<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file<pFoundAt) || (validFound && file < vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file-1;
            }
            //check RIGHT
            file = currFile+1;
            pieceFound = false;
            validFound = false;
            pFoundAt = file;
            vFoundAt = file;
            while(file<=7 && file>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file > pFoundAt) || (validFound && file>vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file+1;    
            }
            //check UP
            file = currFile;
            rank = currRank+1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank<=7 && rank>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank>pFoundAt) || (validFound && rank>vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank+1;    
            }
            //check DOWN
            rank = currRank-1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank>=0 && rank<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank<pFoundAt) || (validFound && rank<vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
            }
        } //end of Queen

        else if(type.Contains("Rook")){
            piece.GetComponent<Rook>().OptionsGrid(optionsGrid, currRank,currFile);

            //Can't access tiles beyond other pieces
            //check LEFT
            int rank = currRank;
            int file = currFile-1;
            bool pieceFound = false;
            int pFoundAt = file;
            bool validFound = false;
            int vFoundAt = file;

            while(file>=0 && file<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file<pFoundAt) || (validFound && file < vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file-1;
            }
            //check RIGHT
            file = currFile+1;
            pieceFound = false;
            validFound = false;
            pFoundAt = file;
            vFoundAt = file;
            while(file<=7 && file>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = file;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = file;
                }
                if((pieceFound && file > pFoundAt) || (validFound && file>vFoundAt))
                    optionsGrid[rank,file] = false;

                file = file+1;    
            }
            //check UP
            file = currFile;
            rank = currRank+1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank<=7 && rank>=0){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank>pFoundAt) || (validFound && rank>vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank+1;    
            }
            //check DOWN
            rank = currRank-1;
            pieceFound = false;
            pFoundAt = rank;
            validFound = false;
            vFoundAt = rank;
            while(rank>=0 && rank<=7){
                if(pieces[rank,file] != null && PieceOwner(pieces[rank,file])== PieceOwner(piece))
                {
                    pieceFound = true;
                    pFoundAt = rank;
                }
                if(!validFound && pieces[rank,file] != null && optionsGrid[rank,file] && PieceOwner(pieces[rank,file]) != "" && PieceOwner(pieces[rank,file]) != PieceOwner(piece)){
                    validFound = true;
                    vFoundAt = rank;
                }
                if((pieceFound && rank<pFoundAt) || (validFound && rank<vFoundAt))
                    optionsGrid[rank,file] = false;
                rank = rank-1;
            }
        } //end of ROOK

        // Iterate through optionsGrid. For each /true/, check for a piece at the corresponding position in the pieces array
        if(team == "white") //the piece attempting to move is white
        {
            for(int i = 0; i<8; i=i+1){
                for(int j = 0; j<8; j=j+1){

                    // Destination is valid if the tile is empty OR it's occupied by opposing team (black)
                    // => if the tile's occupant isn't white, add tile to validTiles
                    if(optionsGrid[i,j] && !white.pieces.Contains(pieces[i, j])){
                        validTiles.Add(board.TileFromArray(i,j).gameObject);
                    }
                }
            }
        }

        else if(team == "black")
        {
            for(int i = 0; i<8; i=i+1){
                for(int j = 0; j<8; j=j+1){

                    // Destination is valid if the tile is empty OR it's occupied by opposing team (black)
                    // => if the tile's occupant isn't white, add tile to validTiles
                    if(optionsGrid[i,j] && !black.pieces.Contains(pieces[i, j])){
                        validTiles.Add(board.TileFromArray(i,j).gameObject);
                    }
                }
            }
        }


        return validTiles;

    } // end of ValidDestinations()

    

    // Move the given piece and update the pieces[,] array 
    public void MovePiece(GameObject piece, GameObject currTile, GameObject destTile)
    {

        // Determine which movement function to call based on the piece object's name in the Unity editor.
        if(destTile == null)
            Debug.Log("dest is null");
        // Convert destTile to a Vector3
        Vector3 dest = board.TileToPos(destTile.transform);
        
        //If the piece's destination has an enemy on it, set attack to /true/
        GameObject occupant = pieces[board.TileRank(destTile), board.TileFile(destTile)];
        
        if(occupant != null && PieceOwner(occupant) != PieceOwner(piece)){
            
            attack = true;
            occupant.GetComponent<NavMeshAgent>().transform.LookAt(piece.transform);

            //Remove soon-to-be destroyed piece from its team's list
            if(PieceOwner(occupant) == "black")
                black.pieces.Remove(occupant);
            else if(PieceOwner(occupant) == "white")
                white.pieces.Remove(occupant);
        }

        else
            attack = false;

        // <<< PAWN >>>
        if(piece.name.Contains("Pawn")){
            
            piece.GetComponent<Pawn>().MovePiece(dest, attack);

            //If this pawn is not already in movedPawns, add it
            if(!movedPawns.Contains(piece))
                movedPawns.Add(piece);
        }

        // <<< ROOK >>>
        else if(piece.name.Contains("Rook")){
            piece.GetComponent<Rook>().MovePiece(dest, attack);
        }
        //

        // <<< KING >>>
        else if(piece.name.Contains("King")){
            piece.GetComponent<King>().MovePiece(dest, attack);
        }
        //

        // <<< KNIGHT >>>
            // *** Knights can move over other pieces of EITHER team! ***
        else if(piece.name.Contains("Horse")){
            piece.GetComponent<Knight>().MovePiece(dest, attack);
        }
        //

        // <<< QUEEN >>>
        else if(piece.name.Contains("Queen")){
            piece.GetComponent<Queen>().MovePiece(dest, attack);
        }
        //

        // << BISHOP >>>
        else if(piece.name.Contains("Bishop")){
            piece.GetComponent<Bishop>().MovePiece(dest, attack);
        }
        //


        pieces[board.TileRank(destTile), board.TileFile(destTile)] = piece;
        pieces[board.TileRank(currTile), board.TileFile(currTile)] = null;
        

        // <<< ******************************************************* >>>
        // Need to go to next turn, but prevent it from starting until this turn's move is complete.
        
        //maybe track the moving piece's NavMeshAgent's behaviour?
        movingPiece = piece.GetComponent<NavMeshAgent>();
        moveSent = true;
        

    } //end of MovePiece()

    public int GetPieceValue(int rank, int file)
    {
        GameObject piece = pieces[rank,file];
        int val;

        if(piece == null)
            val =  0;
        
        else
        {
            if(piece.name.Contains("Pawn"))
                val =  10;
            else if(piece.name.Contains("Rook"))
                val =  50;
            else if(piece.name.Contains("Horse"))
                val =  30;
            else if(piece.name.Contains("Bishop"))
                val =  30;
            else if(piece.name.Contains("Queen"))
                val =  90;
            else if(piece.name.Contains("King"))
                val =  900;
            else
                throw new System.Exception("Unknown piece type");
        }

        //black pieces are negative
        if(val!=0 && PieceOwner(piece) == "black")
            val = 0 - val;

        return val;
    }

    public int EvaluateBoard()
    {
        int total = 0;
        for(int i = 0; i<8; i=i+1){
            for(int j = 0; j<8; j=j+1){
                total = total + GetPieceValue(i,j);
            }
        }
        return total;
    }

    // Reset optionsGrid to all values = false
    public void ClearValid()
    {
        for(int i = 0; i<8; i=i+1)
            for(int j = 0; j<8; j=j+1)
                optionsGrid[i,j] = false;
    }

    //Go to other player's turn
    public void NextPlayer()
    {

        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;

        //if white is next AND gM is not simulation => activate selector
        bool sel = (currentPlayer == white) && (gameMode != "sim");
        selectorObj.SetActive(sel);

        //if (new) current player is cpu, hand off to Player script, providing the 2d array of all pieces' current positioning
        if(currentPlayer.cpu)
            currentPlayer.CompTurn(pieces);
    }

    //After each move, check for existence of GAME-OVER conditions:
    public void CheckForGameOver()
    {
        int numWhite = 0;
        int numBlack = 0;
        bool wKFound = false;
        bool bKFound = false;
        bool wKnFound = false;
        bool bKnFound = false;
        bool wBFound = false;
        bool bBFound = false;

        foreach(GameObject obj in white.pieces)
        {
            numWhite = numWhite + 1;
            if(obj.name.Contains("King"))
                wKFound = true;
            else if(obj.name.Contains("Horse"))
                wKnFound = true;
            else if(obj.name.Contains("Bishop"))
                wBFound = true;
        }
        foreach(GameObject obj in black.pieces)
        {
            numBlack = numBlack + 1;
            if(obj.name.Contains("King"))
                bKFound = true;
            else if(obj.name.Contains("Horse"))
                bKnFound = true;
            else if(obj.name.Contains("Bishop"))
                bBFound = true;
        }

        if(!wKFound || !bKFound)
            endGame = true;
        else if(numWhite == 1 && numBlack == 1)
            endGame = true;
        else if((numWhite == 2 && (wBFound || wKnFound)) && (numBlack == 2 && (bBFound || bKnFound)))
            endGame = true;
        else if(numWhite == 1 && (numBlack == 2 && (bBFound || bKnFound)))
            endGame = true;
        else if(numBlack == 1 && (numWhite == 2 && (wBFound || wKnFound)))
            endGame = true;

        gameOver.SetActive(endGame);
    }

    //Called when a game-mode is selected from the main menu in-game. 
    //  -> clears all variables and initializes new ones where needed; 
    //  -> ends with a call to InitialSetup()
    public void NewGame(string type)
    {
        Debug.Log(type);

        for(int i = 0; i<8; i=i+1){
            for(int j =0; j<8; j=j+1)
            {
                //Clear highlights
                board.tiles[i,j].gameObject.GetComponent<Tile>().RevertMaterial();
                //Destroy pieces
                if(pieces[i,j] != null){
                    Destroy(pieces[i,j]);
                }
            }
        }
        
        //Players are initialized based on the chosen game-mode:

        if(type == "sim")
        {
            selectorObj.SetActive(false);
            white = new Player("white", true, true, false);
            black = new Player("black", false, true, false);
        }
        else if(type == "normal")
        {
            selectorObj.SetActive(true);
            white = new Player("white", true);
            black = new Player("black", false, true, false);
        }
        else if(type == "hard")
        {
            selectorObj.SetActive(true);
            white = new Player("white", true);
            black = new Player("black", false, true, true);
        }
        else
            throw new System.Exception("Invalid game-mode.");
        

        gameMode = type;

        pieces = new GameObject[8,8];
        movedPawns = new List<GameObject>();
        currentPlayer = white;
        otherPlayer = black;
        optionsGrid = new bool[8,8];
        attack = false;
        moveSent = false;
        movingPiece = null;
        endGame = false;
        InitialSetup();

        if(gameMode == "sim"){
            simStart = Time.time;
            firstMove = true;
        }
        
    }

} //end of class Manager 
