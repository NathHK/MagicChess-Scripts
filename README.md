# MagicChess-Scripts
Scripts from my MagicChess Unity project, sourced directly from the local repository on my computer.

This is a project I am (semi-)actively working on; I'd say it's roughly 80% complete. For information on what's left, see **Issues**—it's almost entirely bug-squashing.

## What is "MagicChess"?

MagicChess is an implementation of *Harry Potter's* Wizard's Chess, made in Unity.

## Game Features

This repository only includes the project's C# scripts, but they are part of a full game, designed and develped by myself. Here is a list of its features:

#### 1. Board, Animation, and Navigation

   - All models (the board and pieces) are 3D.
   - Each piece has a distinct model and animation set, with animations for _idle_, _walk_, _attack_, _get hurt_, and _die_.
   - Every animation, except _idle_, has a corresponding sound. Collision between weapon and piece also plays a custom sound. 
   - Pieces move across the board when selected (clicked using mouse), provided you choose a valid destination tile. 
   - Pieces use physics when they hit other pieces. Upon death, a piece breaks-apart. 
   
#### 2. Chess Rules and AI

   - Gameplay abides by all the rules of standard chess.
   - When it is the player's turn, selecting a piece highlights various tiles according to the rules of chess:
     - **Green** - Applied to the selected piece's current tile if said piece belongs to the player (i.e. is white).
     - **Yellow** - Applied to the selected piece's current tile if said piece does not belong to the player (i.e. is black), OR the player selects an empty tile.
     - **Blue** - Applied to all valid destination tiles of the selected piece (granted the piece belongs to the player (i.e. is white)).
   - AI move-selection depends on the user's chosen gameplay mode (see next bullet point).
   
#### 3. Three Gameplay Modes   

   1. **Standard:** The player controls the white pieces. Their AI-opponent always selects a _**random**_ move, chosen from a pool of all valid moves. 
   2. **Hard:** The player controls the white pieces. Their AI-opponent selects moves using the _**minimax algorithm with alpha-beta pruning**_.
   3. **Simulation:** Both teams are controlled by AI, and move-selection is done using the _**minimax algorithm**_. The two AI teams will take turns moving until end of game, or until the user selects a different gamemode (whichever comes first). 
   
## Asset Credits

["Tiny Chess" by ThreeBox](https://assetstore.unity.com/packages/3d/environments/tiny-chess-110350) - All chess pieces and their animations. Can be purchased for US$5 from the Unity Asset Store. 

["Wood Set Pieces" by Eternal Echoes Entertainment](https://assetstore.unity.com/packages/3d/props/wood-set-pieces-33853) - Wooden texture I used for designing the chess board.

[freesound.org](https://freesound.org/) - Source of all sound effects. 

## License

This work is published under the **GNU General Public License v3.0**.

