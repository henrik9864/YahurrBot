using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot.Modules
{
    class Tetris : Module
    {
        User currentUser;
        const int DEF_WIDTH = 10;
        const int DEF_HEIGHT = 13;

        public override void ParseCommands(string[] commands, MessageEventArgs e)
        {
            if (currentUser == null)
            {
                if (commands[0] == "!tetris")
                {
                    PlayGame(e.User, e);
                    currentUser = e.User;
                    e.Channel.SendMessage(currentUser.Name);
                }
            }
            else if(commands[0] == "!tetris" && commands[1] == "end")
            {
                currentUser = null;
            }
        }

        public static List<char> symbols = new List<char>() { 'L', 'J', 'T', 'I', '5', '2', 'O' };

        void PlayGame(User user, MessageEventArgs e)
        {
            Square[,] board = MakeBoard(DEF_WIDTH, DEF_HEIGHT);
            ShowBoard(board, e);
        }

        Square[,] MakeBoard(int width, int heigth)
        {
            Square[,] square = new Square[width, heigth];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
                {
                    square[i, j] = new Square('-');
                }
            }

            for(int i = 0; i < heigth; i++)
            {
                square[0, i].symbol = '|';
                square[width - 1, i].symbol = '|';
            }

            return square;
        }

        void ShowBoard(Square[,] board, MessageEventArgs e)
        {
            String outMessage = "";
            
            for(int i = 0; i < DEF_HEIGHT; i++)
            {
                for(int j = 0; j < DEF_WIDTH; j++)
                {
                    outMessage = outMessage + board[j, i].symbol;
                }
                Console.WriteLine(outMessage);
                outMessage = outMessage + Environment.NewLine;
            }

            e.Channel.SendMessage(outMessage);
        }
    }

    class Square
    {
        private char SquareSymbol;
        public char symbol
        {
            get
            {
                return SquareSymbol;
            }

            set
            {
                SquareSymbol = value;
            }
        }

        public Square(char sym)
        {
            symbol = sym;
        }
    }

    internal class Brick
    {
        //static String[,,] _LBrick = { { { "010" }, { "010" }, { "011" } }, { { "000" }, { "111" }, { "100" } }, { { "110" }, { "010" }, { "010" } }, { { "001" }, { "111" }, { "000" } } };
        //static String[,,] _JBrick = { { { "010" }, { "010" }, { "110" } }, { { "100" }, { "111" }, { "000" } }, { { "011" }, { "010" }, { "010" } }, { { "000" }, { "111" }, { "001" } } };
        //static String[,,] _TBrick = { { { "010" }, { "111" }, { "000" } }, { { "010" }, { "011" }, { "010" } }, { { "000" }, { "111" }, { "010" } }, { { "010" }, { "110" }, { "010" } } };
        //static String[,,] _5Brick = { { { "011" }, { "110" }, { "000" } }, { { "010" }, { "011" }, { "001" } }, { { "000" }, { "011" }, { "110" } }, { { "100" }, { "110" }, { "010" } } };
        //static String[,,] _2Brick = { { { "110" }, { "011" }, { "000" } }, { { "001" }, { "011" }, { "010" } }, { { "000" }, { "110" }, { "011" } }, { { "010" }, { "110" }, { "100" } } };
        //static String[,,] _IBrick = { { { "0010" }, { "0010" }, { "0010" }, { "0010" } }, { { "0000" }, { "0000" }, { "1111" }, { "0000" } }, { { "0100" }, { "0100" }, { "0100" }, { "0100" } }, { { "0000" }, { "1111" }, { "0000" }, { "1111" } } };
        //static String[,,] _OBrick = { { { "11" }, { "11" } }, { { "11" }, { "11" } }, { { "11" }, { "11" } }, { { "11" }, { "11" } } };

        protected int rotation;
        public int rotate
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation += value;
            }
        }

        private char BrickSymbol;
        public char symbol
        {
            get
            {
                return BrickSymbol;
            }

            set
            {
                BrickSymbol = value;
            }
        }

        private bool movement;
        public bool moving
        {
            get
            {
                return movement;
            }

            set
            {
                movement = value;
            }
        }

        private int[] position;
        public int[] pos
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }
    }

    class _LBrick : Brick
    {
        static String[,,] shape = { { { "010" }, { "010" }, { "011" } }, { { "000" }, { "111" }, { "100" } }, { { "110" }, { "010" }, { "010" } }, { { "001" }, { "111" }, { "000" } } };



        public _LBrick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _JBrick : Brick
    {
        static String[,,] shape = { { { "010" }, { "010" }, { "110" } }, { { "100" }, { "111" }, { "000" } }, { { "011" }, { "010" }, { "010" } }, { { "000" }, { "111" }, { "001" } } };

        public _JBrick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _TBrick : Brick
    {
        static String[,,] shape = { { { "010" }, { "111" }, { "000" } }, { { "010" }, { "011" }, { "010" } }, { { "000" }, { "111" }, { "010" } }, { { "010" }, { "110" }, { "010" } } };

        public _TBrick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _5Brick : Brick
    {
        static String[,,] shape = { { { "011" }, { "110" }, { "000" } }, { { "010" }, { "011" }, { "001" } }, { { "000" }, { "011" }, { "110" } }, { { "100" }, { "110" }, { "010" } } };

        public _5Brick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _2Brick : Brick
    {
        static String[,,] shape = { { { "110" }, { "011" }, { "000" } }, { { "001" }, { "011" }, { "010" } }, { { "000" }, { "110" }, { "011" } }, { { "010" }, { "110" }, { "100" } } };

        public _2Brick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _IBrick : Brick
    {
        static String[,,] shape = { { { "0010" }, { "0010" }, { "0010" }, { "0010" } }, { { "0000" }, { "0000" }, { "1111" }, { "0000" } }, { { "0100" }, { "0100" }, { "0100" }, { "0100" } }, { { "0000" }, { "1111" }, { "0000" }, { "1111" } } };

        public _IBrick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }

    class _OBrick : Brick
    {
        static String[,,] shape = { { { "11" }, { "11" } }, { { "11" }, { "11" } }, { { "11" }, { "11" } }, { { "11" }, { "11" } } };

        public _OBrick(int posX, int posY)
        {
            int[] temp = { posX, posY };
            pos = temp;
        }
    }
}
