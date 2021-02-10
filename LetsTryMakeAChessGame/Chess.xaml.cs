using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ChessApp;

namespace LetsTryMakeAChessGame
{
    /// <summary>
    /// Interaction logic for Chess.xaml
    /// </summary>
    public partial class Chess : Page
    {
        Chessboard chessboard = new Chessboard();
        public ObservableCollection<Pieces> Piece { get; set; }
        Cell currentCell;

        public Chess()
        {
            Piece = new ObservableCollection<Pieces>();
            InitializeComponent();
            CreateButtonGrid();

            Test.Text = "White Move History";
            OtherTest.Text = "Black Move History";

            NewGame();
            UpdateBoardState();
        }

        private void CreateButtonGrid()
        {
            for (int row = 0; row < 8; row++)
            {
                bool isBlack = row % 2 == 1;
                for (int col = 0; col < 8; col++)
                {
                    var button = new Button() { Background = isBlack ? Brushes.Black : Brushes.White, Foreground = !isBlack ? Brushes.Black : Brushes.White };
                    button.Name = Rulebook.ArrayToCellRow[col].ToString() + Rulebook.ArrayToCellColumn[row].ToString();
                    button.Click += GridButton_Click;
                    button.FontSize = 50;
                    button.Tag = chessboard.Board[row, col];
                    Chessboard.Children.Add(button);
                    isBlack = !isBlack;
                }
            }
        }

        private void GridButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if users turn

            // Get info about Cell
            Button clickedButton = (sender as Button);
            Cell clickedCell = clickedButton.Tag as Cell;

            // If user intends to move
            if (clickedCell.IsLegal)
            {
                PrintMoveHistory(currentCell, clickedCell, clickedButton);
                MovePiece(currentCell, clickedCell);
            }

            // If user wants to look for legal moves
            else if (clickedCell.IsOccupied)
            {
                chessboard.ClearMarkedLegalMoves();
                chessboard.FindLegalMoves(clickedCell.piece);
            }

            // If user didn't click on legal or occupied
            else
            {
                chessboard.ClearMarkedLegalMoves();
            }

            List<Pieces> whitePieces = SearchForPieces(true);
            List<Pieces> blackPieces = SearchForPieces(false);

            UpdateBoardState();
            currentCell = clickedCell;

            if (IsGameOver(whitePieces, blackPieces))
            {
                NewGame();
            }

            //ComputerMove(blackPieces);

            //UpdateBoardState();

            //if (IsGameOver(whitePieces, blackPieces))
            //{
            //    NewGame();
            //}

        }

        private void ComputerMove(List<Pieces> blackPieces)
        {
            Random rnd = new Random();
            bool hasNotMoved = true;
            while (hasNotMoved)
            {
                Pieces piece = blackPieces[rnd.Next(0, blackPieces.Count - 1)];
                if (chessboard.HasLegalMove(piece))
                {
                    // Get list of legal moves for that piece
                    List<Cell> legalPositions = new List<Cell>();

                    foreach (Cell cell in chessboard.Board)
                    {
                        if (cell.IsLegal)
                        {
                            legalPositions.Add(cell);
                        }
                    }

                    Cell newCell = legalPositions[rnd.Next(0, legalPositions.Count - 1)];
                    MovePiece(piece.Position, newCell);

                    hasNotMoved = false;
                }
                else
                {
                    blackPieces.Remove(piece);
                }
            }
        }

        private bool IsGameOver(List<Pieces> whitePieces, List<Pieces> blackPieces)
        {

            if (!KingExists(whitePieces))
            {
                Test.Text = "White Loses!";
                return true;
            }
            if (!KingExists(blackPieces))
            {
                OtherTest.Text = "Black Loses!";
                return true;
            }
            return false;
        }

        private void PrintMoveHistory(Cell currentCell, Cell clickedCell, Button clickedButton)
        {
            string message = "\r\n ";
            if (currentCell.piece.Name == "Pawn")
            {
                message += $"{clickedButton.Name}";
            }
            else if (!clickedCell.IsOccupied)
            {
                message += $"{Rulebook.ConvertPieceToInitial(currentCell.piece)}{clickedButton.Name}";
            }
            else
            {
                message += $"{Rulebook.ConvertPieceToInitial(currentCell.piece)}x{clickedButton.Name}";
            }

            if (currentCell.piece.IsWhite)
            {
                Test.Text += message;
            }
            else
            {
                OtherTest.Text += message;
            }
        }

        private void MovePiece(Cell beforeCell, Cell afterCell)
        {
            // move piece to new cell and set that cell to be occupied
            afterCell.piece = beforeCell.piece;
            afterCell.IsOccupied = true;
            beforeCell.piece.Position = afterCell;

            // remove piece from old cell and set it to be not occupied
            beforeCell.piece = null;
            beforeCell.IsOccupied = false;

            chessboard.ClearMarkedLegalMoves();
        }

        // If occupied add name, else clear content
        // if legal change colour to yellow, else change it back to original colour
        private void UpdateBoardState()
        {
            foreach(Button button in Chessboard.Children)
            {
                Cell cell = button.Tag as Cell;

                if (cell.IsOccupied)
                {
                    button.Content = Rulebook.ConvertPieceToInitial(cell.piece);
                }
                else
                {
                    button.Content = "";
                }

                if (cell.IsLegal)
                {
                    button.Background = Brushes.Yellow;
                }
                else
                {
                    if (Rulebook.BlackCells.Contains(button.Name))
                    {
                        button.Background = Brushes.Black;
                    }
                    else
                    {
                        button.Background = Brushes.White;
                    }
                }
            }
        }

        private void NewGame()
        {
            chessboard.ClearBoard();

            Pawn whitePawn1 = new Pawn(true, chessboard.Board[6, 0]);
            Pawn whitePawn2 = new Pawn(true, chessboard.Board[6, 1]);
            Pawn whitePawn3 = new Pawn(true, chessboard.Board[6, 2]);
            Pawn whitePawn4 = new Pawn(true, chessboard.Board[6, 3]);
            Pawn whitePawn5 = new Pawn(true, chessboard.Board[6, 4]);
            Pawn whitePawn6 = new Pawn(true, chessboard.Board[6, 5]);
            Pawn whitePawn7 = new Pawn(true, chessboard.Board[6, 6]);
            Pawn whitePawn8 = new Pawn(true, chessboard.Board[6, 7]);

            Rook whiteRook1 = new Rook(true, chessboard.Board[7, 0]);
            Knight whiteKnight1 = new Knight(true, chessboard.Board[7, 1]);
            Bishop whiteBishop1 = new Bishop(true, chessboard.Board[7, 2]);
            Queen whiteQueen = new Queen(true, chessboard.Board[7, 3]);
            King whiteKing = new King(true, chessboard.Board[7, 4]);
            Bishop whiteBishop2 = new Bishop(true, chessboard.Board[7, 5]);
            Knight whiteKnight2 = new Knight(true, chessboard.Board[7, 6]);
            Rook whiteRook2 = new Rook(true, chessboard.Board[7, 7]);

            Pawn blackPawn1 = new Pawn(false, chessboard.Board[1, 0]);
            Pawn blackPawn2 = new Pawn(false, chessboard.Board[1, 1]);
            Pawn blackPawn3 = new Pawn(false, chessboard.Board[1, 2]);
            Pawn blackPawn4 = new Pawn(false, chessboard.Board[1, 3]);
            Pawn blackPawn5 = new Pawn(false, chessboard.Board[1, 4]);
            Pawn blackPawn6 = new Pawn(false, chessboard.Board[1, 5]);
            Pawn blackPawn7 = new Pawn(false, chessboard.Board[1, 6]);
            Pawn blackPawn8 = new Pawn(false, chessboard.Board[1, 7]);

            Rook blackRook1 = new Rook(false, chessboard.Board[0, 0]);
            Knight blackKnight1 = new Knight(false, chessboard.Board[0, 1]);
            Bishop blackBishop1 = new Bishop(false, chessboard.Board[0, 2]);
            Queen blackQueen = new Queen(false, chessboard.Board[0, 3]);
            King blackKing = new King(false, chessboard.Board[0, 4]);
            Bishop blackBishop2 = new Bishop(false, chessboard.Board[0, 5]);
            Knight blackKnight2 = new Knight(false, chessboard.Board[0, 6]);
            Rook blackRook2 = new Rook(false, chessboard.Board[0, 7]);
        }

        private List<Pieces> SearchForPieces(bool isWhite)
        {
            List<Pieces> pieces = new List<Pieces>();

            foreach(Button button in Chessboard.Children)
            {
                Cell cell = button.Tag as Cell;
                if (cell.IsOccupied)
                {
                    if (cell.piece.IsWhite == isWhite)
                    {
                        pieces.Add(cell.piece);
                    }
                }
            }

            return pieces;
        }

        private bool KingExists(List<Pieces> pieces)
        {
            foreach (Pieces piece in pieces)
            {
                if (piece.Name == "King")
                {
                    return true;
                }
            }
            return false;
        }


        // Use images instead of letters?
        public string ImageSource(Pieces piece)
        {
            string source = "pack://application:,,,/ChessImages/";
            if (piece.Name == "BlackPawn" || piece.Name =="WhitePawn")
            {
                source += piece.Name + ".png";
            }
            else
            {
                if (piece.IsWhite)
                {
                    source += "White" + piece.Name + ".png";
                }
                else
                {
                    source += "Black" + piece.Name + ".png";
                }
            }
            return source;
        }

    }
}
