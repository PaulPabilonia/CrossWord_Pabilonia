using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace CrossWord_Pabilonia
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        List<cells> cls = new List<cells>();
        public String puzzleFile = "Puzzle\\puzzle_1.txt";
        public Form1()
        {
            puzzleList();
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            iBoard();
            
        }
        private void puzzleList()
        {//to read the file and create a new list
            String line = "";
            using (StreamReader sr = new StreamReader(puzzleFile))
            {
                line = sr.ReadLine(); //ignore the first line in the file.
                while((line = sr.ReadLine()) != null)
                {
                    string[] arrLine = line.Split('|'); //to turn puzzle to an array
                    cls.Add(new cells(Int32.Parse(arrLine[0]), Int32.Parse(arrLine[1]),arrLine[2],arrLine[3],arrLine[4],arrLine[5]));
                }
            }

        }
        private void iBoard()
        {
            //changing background colors to black
            board.BackgroundColor = Color.Black;
            board.DefaultCellStyle.BackColor = Color.Black;

            for(int i = 0; i < 21; i++)
            {
                board.Rows.Add(); // to add 20 rows in the board using data grid
            }
            //to set height of rows 
            foreach (DataGridViewRow r in board.Rows)
            {
                r.Height = board.Height / board.Rows.Count;
            }
            //to set width of column 
            foreach (DataGridViewColumn c in board.Columns)
            {
                c.Width = board.Width / board.Columns.Count;
            }
            //to set all box to read only
            for ( int row = 0; row < board.Rows.Count; row++)
            {
                for (int column = 0; column < board.Columns.Count; column++)
                {
                    board[column, row].ReadOnly = true;
                }
            }
            foreach(cells i in cls) // to print the puzzle cells.
            {
                int startCol = i.X;
                int starRow = i.Y;
                char[] word = i.word.ToCharArray();
                
                for ( int j = 0; j < word.Length; j++)
                {
                    if(i.direction.ToUpper() == "ACROSS")
                    {
                        insertCell(starRow, startCol + j, word[j].ToString());
                    }
                    if (i.direction.ToUpper() == "DOWN")
                    {
                        insertCell(starRow + j, startCol, word[j].ToString());
                    }
                }
            }
        }
        //to insert letters in a cell
        private void insertCell(int row, int column,String letter)
        {
            DataGridViewCell cell = board[column, row];
            cell.Style.BackColor = Color.White;
            cell.ReadOnly = false;
            cell.Style.SelectionBackColor = Color.SaddleBrown;
            cell.Tag = letter;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit(); // exit the program
        }

        private void board_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {// this is to have the numbers in the cells
            String number = "";
            if (cls.Any(c => (number = c.number) != "" && c.X == e.ColumnIndex && c.Y == e.RowIndex))
            {
                Rectangle rec = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.FillRectangle(Brushes.White, rec);
                e.Graphics.DrawString(number, f, Brushes.Black, rec);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
                
            }
        }

        private void board_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //to make all letters to Uppercase
            try
            {
                board[e.ColumnIndex, e.RowIndex].Value = board[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();
            }
            catch { }
            //If the user input more than 1 letter in a cell, it will take only the 0 index letter
            try
            {
                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1)
                {
                    board[e.ColumnIndex, e.RowIndex].Value = board[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0,1).ToUpper();
                }
            }
            catch { }
            //Correct letter = Dark green &   Wrong letter = red
            try
            {
                if (board[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper().Equals(board[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                {
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkGreen;
                }
                else
                {
                    board[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
                }
            }
            catch { }
        }
        private void gameWinner()
        { // to check if the palyer win or not
            bool win = true;
            for (int row = 0; row <board.Rows.Count; row++)
            {
                for (int col = 0; col < board.Columns.Count; col++)
                {
                    if ((board[col,row].Style.ForeColor != Color.DarkGreen) && (board[col, row].Style.BackColor == Color.White))
                    {
                        win = false;
                    }
                }
            }
            if (win)
            {
                MessageBox.Show("YOU COMPLETED THE PUZZLE!");
            }
            else
            {
                MessageBox.Show("SORRY TRY AGAIN! WRONG WORDS!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gameWinner();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
    public class cells
    {
        //to create the cells of the puzzles
        public int X;
        public int Y;
        public String direction;
        public String number;
        public String word;
        public String clue;

        //constructor
        public cells (int x, int y, String d,String n, String w, String c)
        {
            this.X = x;
            this.Y = y;
            this.direction = d;
            this.number = n;
            this.word = w;
            this.clue = c;
        }
    }
}
