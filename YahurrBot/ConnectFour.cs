using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace YahurrBot
{
    class ConnectFour : Module
    {
        int[,] board = new int[7, 6];

        public override void Load(DiscordClient client)
        {
            Help.addHelp("!connectFour", "A heavily WIP for playing connect four");
        }

        public override void ParseCommands(string[] commands, MessageEventArgs e)
        {
            if (commands[0] == "!connectfour")
            {
                int parseInt = 0;
                if (commands[1] == "restart")
                {
                    drawBoard();
                }
                else if (int.TryParse(commands[1], out parseInt))
                {
                    placeChip(parseInt, 1);
                    if(checkIfWon(parseInt))
                    {
                        parseInt -= 1;
                        int player = board[parseInt, findFirstFree(parseInt) - 1];
                        e.Channel.SendMessage(printBoard() + Environment.NewLine + "Player" + player + " has won the game!");
                        drawBoard();
                        return;
                    }
                }
                e.Channel.SendMessage(printBoard());
            }
        }

        public void drawBoard()
        {
            for (int i = 0; i <= board.GetUpperBound(1); i++)
            {
                for (int o = 0; o <= board.GetUpperBound(0); o++)
                {
                    board[o,i] = 0;
                }
            }
        }

        public string printBoard()
        {
            string brett = "";
            for (int i = 0; i <= board.GetUpperBound(1); i++)
            {
                brett += "`|";
                for (int o = 0; o <= board.GetUpperBound(0); o++)
                {
                    switch (board[o, 5-i])
                    {
                        case 0:
                            brett += "ｏ";
                            break;
                        case 1:
                            brett += "﻿Ｘ";
                            break;
                        case 2:
                            brett += "０﻿";
                            break;
                        default:
                            break;
                    }
                }
                brett += "|`";
                if (i != board.GetUpperBound(1))
                    {
                        brett += Environment.NewLine;
                    }
            }
            brett += Environment.NewLine + "`|﻿１２３４５６７|﻿`";
            return(brett);
        }

        public int findFirstFree(int row)
        {
            for (int i = 0; i < board.GetUpperBound(1); i++)
            {
                if (board[row,i] == 0)
                {
                    return i;
                }
            }
            return board.GetUpperBound(1);
        }

        public bool placeChip(int row, int player)
        {
            row -= 1;
            if (row < 0 || row > 6)
            {
                return false;
            }
            int column = findFirstFree(row); //I dette tilfellet er column brukt for row og det motsatte med row. Jeg var for lat til å fikse det.
            if (column > board.GetUpperBound(1))
            {
                return false;
            }
            else
            {
                board[row, column] = player;
                return true;
            }
        }

        public bool checkIfWon(int row)
        {
            if (checkHorizontal(row) || checkVertical(row) || checkDiagonal1(row) || checkDiagonal2(row))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkHorizontal(int row)
        {
            row -= 1;
            int column = findFirstFree(row) - 1;
            int player = board[row, column];
            int inARow = 1;
            
            for (int i = 1; i <= 3; i++)
            {
                if (row - i < 0)
                {
                    break;
                }
                else if (board[row-i,column] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                if (row + i > 6)
                {
                    break;
                }
                else if (board[row + i, column] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            if (inARow > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkVertical(int row)
        {
            row -= 1;
            int column = findFirstFree(row) - 1;
            int player = board[row, column];
            int inARow = 1;

            for (int i = 1; i <= 3; i++)
            {
                if (column - i < 0)
                {
                    break;
                }
                else if (board[row, column - i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                if (column + i > 6)
                {
                    break;
                }
                else if (board[row, column + i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            if (inARow > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkDiagonal1(int row)
        {
            row -= 1;
            int column = findFirstFree(row) - 1;
            int player = board[row, column];
            int inARow = 1;

            for (int i = 1; i <= 3; i++)
            {
                if (row - i < 0 || column - i < 0)
                {
                    break;
                }
                else if (board[row - i, column - i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                if (row + i > 6 || column + i > 6)
                {
                    break;
                }
                else if (board[row + i, column + i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            if (inARow > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkDiagonal2(int row)
        {
            row -= 1;
            int column = findFirstFree(row) - 1;
            int player = board[row, column];
            int inARow = 1;

            for (int i = 1; i <= 3; i++)
            {
                if (row - i < 0 || column + i > 6)
                {
                    break;
                }
                else if (board[row - i, column + i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                if (row + i > 6 || column - i < 0)
                {
                    break;
                }
                else if (board[row + i, column - i] == player)
                {
                    inARow += 1;
                }
                else
                {
                    break;
                }
            }
            if (inARow > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

