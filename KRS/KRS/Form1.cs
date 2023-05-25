using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace WindowsFormsApplication309
{
    public partial class Form1 : Form
    {
        private Game game;
        private PictureBox[,] pbs = new PictureBox[3, 3];
        private Image Cross;
        private Image Nought;

        public Form1()
        {
            InitializeComponent();
            Init();
            game = new Game();
            Build(game);
        }

        void Init()
        {

            Cross = Image.FromFile(Application.StartupPath + "/cross.png");
            Nought = Image.FromFile(Application.StartupPath + "/circle.png");

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    pbs[i, j] = new PictureBox { Parent = this, Size = new Size(100, 100), Top = i * 100, Left = j * 100, BorderStyle = BorderStyle.FixedSingle, Tag = new Point(i, j), Cursor = Cursors.Hand, SizeMode = PictureBoxSizeMode.StretchImage };
                    pbs[i, j].Click += pb_Click;
                }

            new Button { Parent = this, Top = 320, Text = "Reset" }.Click += delegate { game = new Game(); Build(game); };
        }

        private void Build(Game game)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    pbs[i, j].Image = game.Items[i, j] == FieldState.Cross ? Cross : (game.Items[i, j] == FieldState.Nought ? Nought : Image.FromFile(Application.StartupPath + "/" + game.Scores[i, j] + ".png"));
        }

        void pb_Click(object sender, EventArgs e)
        {
            game.MakeMove((Point)(sender as Control).Tag);
            Build(game);

            if (game.Winned)
                MessageBox.Show(string.Format("{0} is winner!", game.CurrentPlayer == 1 ? "Cross" : "Nought"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    class Game
    {
        public FieldState[,] Items = new FieldState[3, 3];
        public int[,] Scores = new int[3, 3];
        public int CurrentPlayer = 1;
        public bool Winned;

        public void MakeMove(Point p)
        {
            if (Items[p.X, p.Y] != FieldState.Empty)
                return;

            if (Winned)
                return;

            Items[p.X, p.Y] = CurrentPlayer == 1 ? FieldState.Cross : FieldState.Nought;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (Items[i, j] == FieldState.Empty)
                    {
                        FieldState[,] NewItems = (FieldState[,])Items.Clone();
                        NewItems[i, j] = CurrentPlayer != 1 ? FieldState.Cross : FieldState.Nought;
                        Scores[i, j] = Minimax(NewItems, 0, -CurrentPlayer);

                    }

                }

            if (CheckWinner(Items, FieldState.Cross) || CheckWinner(Items, FieldState.Nought))
            {
                Winned = true;
                return;
            }
            CurrentPlayer *= -1;
        }

        private bool CheckWinner(FieldState[,] Items, FieldState state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Items[i, 0] == state && Items[i, 1] == state && Items[i, 2] == state)
                    return true;
                if (Items[0, i] == state && Items[1, i] == state && Items[2, i] == state)
                    return true;
            }

            if (Items[0, 0] == state && Items[1, 1] == state && Items[2, 2] == state)
                return true;

            if (Items[0, 2] == state && Items[1, 1] == state && Items[2, 0] == state)
                return true;

            return false;
        }
        int Score(FieldState[,] Items, int depth)
        {
            if (CheckWinner(Items, FieldState.Cross))
            {
                return 10 - depth;
            }
            else if ( CheckWinner(Items, FieldState.Nought))
            {
                return depth - 10;
            }
            return 0;
        }
        int Minimax(FieldState[,] ItemsNow, int depth, int Player)
        {
            if (CheckWinner(ItemsNow, FieldState.Cross) || CheckWinner(ItemsNow, FieldState.Nought))
            {
                return Score(ItemsNow, depth);
            }
            depth++;
            int[,] scores = new int[3,3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if(ItemsNow[i,j] == FieldState.Empty)
                    {
                        FieldState[,] ItemsPossible = (FieldState[,])ItemsNow.Clone();
                        ItemsPossible[i, j] = Player == 1 ? FieldState.Cross : FieldState.Nought; 
                        scores[i,j] = Minimax(ItemsPossible, depth, -Player);
                    }
                    
                }
            if(Player == 1)
            {
                int max = -100;
                foreach (int m in scores)
                {
                    if (m > max && m != 0)
                    {
                        max = m;
                    }
                }
                return max;
            }
            else
            {
                int min = 100;
                foreach (int m in scores)
                {
                    if (m < min && m != 0)
                    {
                        min = m;
                    }
                }
                return min;
            }
        }
    }
        enum FieldState
        {
            Empty,
            Cross,
            Nought
        }
    
}