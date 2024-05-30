using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mine
{
    public partial class Form1 : Form
    {
        private  int GridSize;
        private int jumlahBom;
        private int jumlahBendera;
        private Cell[,] _grid = new Cell[5,5];
        private Stopwatch _stopwatch;
        private bool _firstMove = true;
        private bool sudahPilihLevel = false;
        private Button[,] tombol;
        private int buttonBukanBom;
        private int cekMenang = 0;
        Timer timer;
        Label timerLabel = new Label();



        private void btnEasy_Click(object sender, EventArgs e)
        {
            levelPanel.Hide();
            StartPanel.Hide();
            sudahPilihLevel = true;
            jumlahBom = 10;
            jumlahBendera = 7;
            GridSize = 10;
            _grid = new Cell[GridSize, GridSize];
            ResetGame();
            
        }

        private void btnMedium_Click(object sender, EventArgs e)
        {
            levelPanel.Hide();
            StartPanel.Hide();
            jumlahBom = 15;
            jumlahBendera = 10;
            sudahPilihLevel = true;
            GridSize = 12;
            _grid = new Cell[GridSize, GridSize];
            ResetGame();
        }

        private void btnHard_Click(object sender, EventArgs e)
        {
            levelPanel.Hide();
            StartPanel.Hide();
            jumlahBom = 25;
            jumlahBendera = 15;
            sudahPilihLevel = true;
            GridSize = 13;
            _grid = new Cell[GridSize, GridSize];
            ResetGame();
        }

        class Cell
        {
            public bool IsMine { get; set; }
            public bool IsRevealed { get; set; }
            public int AdjacentMines { get; set; }
            public Button Button { get; set; }

            public int Value { get; set; }
        }

       
        private int GetAdjacentMines(int row, int col)
        {
            int count = 0;
            tombol = new Button[row, col];

            // Periksa sel di atas, di bawah, kiri, dan kanan sel saat ini
            if (row > 0 && _grid[row - 1, col].IsMine) count++;
            if (row < GridSize - 1 && _grid[row + 1, col].IsMine) count++;
            if (col > 0 && _grid[row, col - 1].IsMine) count++;
            if (col < GridSize - 1 && _grid[row, col + 1].IsMine) count++;

            // Periksa sel di sudut kiri atas, kanan atas, kiri bawah, dan kanan bawah
            if (row > 0 && col > 0 && _grid[row - 1, col - 1].IsMine) count++;
            if (row > 0 && col < GridSize - 1 && _grid[row - 1, col + 1].IsMine) count++;
            if (row < GridSize - 1 && col > 0 && _grid[row + 1, col - 1].IsMine) count++;
            if (row < GridSize - 1 && col < GridSize - 1 && _grid[row + 1, col + 1].IsMine) count++;

            return count;
        }


      
        private void btnStart_Click(object sender, EventArgs e)
        {
            levelPanel.Show();
            StartPanel.Hide();
           
        }

        public Form1()
        {
            InitializeComponent();
           

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.RowCount = GridSize;
                tableLayoutPanel.ColumnCount = GridSize;


                // Atur ukuran sel di grid
                for (int i = 0; i < GridSize; i++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f / GridSize));
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f / GridSize));
                }

                for (int row = 0; row < GridSize; row++)
                {
                    for (int col = 0; col < GridSize; col++)
                    {
                        _grid[row, col] = new Cell();
                        _grid[row, col].Button = new Button();
                        _grid[row, col].Button.Dock = DockStyle.Fill;
                        _grid[row, col].Button.Margin = new Padding(0);
                        _grid[row, col].Button.Tag = new Tuple<int, int>(row, col);
                        _grid[row, col].Button.MouseDown += new MouseEventHandler(Button_MouseDown);


                        // Tambahkan tombol ke panel tata letak tabel
                        tableLayoutPanel.Controls.Add(_grid[row, col].Button, col, row);
                    }
                }

                // menempatkan ranjau acak di grid
                Random random = new Random();
                for (int i = 0; i < jumlahBom; i++)
                {
                    int row = random.Next(GridSize);
                    int col = random.Next(GridSize);
                    if (row == 0 && col == 0)
                    {
                        //Kecualikan sel pertama dari daftar sel yang mungkin berisi ranjau
                        i--;
                        continue;
                    }
                    _grid[row, col].IsMine = true;
                }

                int kali = (GridSize * GridSize) - jumlahBom;
                buttonBukanBom = kali;

                // Hitung jumlah angka yang berdekatan untuk setiap sel
                for (int row = 0; row < GridSize; row++)
                {
                    for (int col = 0; col < GridSize; col++)
                    {
                        _grid[row, col].AdjacentMines = GetAdjacentMines(row, col);
                    }
                }

                if (sudahPilihLevel == true)
            {
                // Buat panel untuk menahan label penghitung waktu dan angka
                Panel topPanel = new Panel();
                topPanel.Dock = DockStyle.Top;
                topPanel.Height = 30;

                // Buat label pengatur waktu
                
                timerLabel.AutoSize = true;
                timerLabel.Location = new Point(10, 10);
                timerLabel.Text = string.Format("{0:hh\\Mm\\:ss}", _stopwatch.Elapsed);




                // Buat label jumlah ranjau
                Label mineCountLabel = new Label();
                mineCountLabel.AutoSize = true;
                mineCountLabel.Text = "                     Mines: " + jumlahBom;
                mineCountLabel.Location = new Point(10, 10);

                // Buat label jumlah Bendera
                Label Bendera = new Label();
                Bendera.AutoSize = true;
                Bendera.Text = "\U0001F3F4\U000E0067\U000E0062" + jumlahBendera;
                Bendera.Location = new Point(90, 10);

                // Tambahkan label ke panel atas
                topPanel.Controls.Add(timerLabel);
                topPanel.Controls.Add(mineCountLabel);
                topPanel.Controls.Add(Bendera);


                // Tambahkan panel tata letak tabel dan panel atas ke formulir

                this.Controls.Add(topPanel);
            }
            this.Controls.Add(tableLayoutPanel);







        }

        private void CellButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Tuple<int, int> cell = (Tuple<int, int>)button.Tag;
     
            int row = cell.Item1;
            int col = cell.Item2;
            



            RevealEmptyCells(row, col);
            
            if (_firstMove)
            {
                // ini ketika pemain baru langkah pertama

                // Cek jika sel adalah ranjau
                RevealEmptyCells(row, col);
                _firstMove = false;

                if (_grid[row, col].IsMine)
                {
                    // sel adalah ranjau dan dipindahkan ke sel yang lain
                    _grid[row, col].IsMine = false;
                    PlaceMine(row, col);
                }
                // kurangg
                if (_grid[row, col].AdjacentMines >= 0)
                {
                    _grid[row, col].Value = 0;
                }
            }
            

            if (_grid[row, col].IsMine)
            {
                // Kondisi Pemain Kalah
                GameOver();
                StartPanel.Show();
               // button.Enabled = false;
            }
            if(buttonBukanBom == cekMenang)
            {
                Menang();
                StartPanel.Show();
            }
        }

        // method timer buat terus jalan
        private void timer_Tick(object sender, EventArgs e)
        {

            if (sender == timer)
            {
                timerLabel.Text = string.Format("\U000023F2 " + " {0:hh\\mm\\:ss}", _stopwatch.Elapsed);
            }
        }
        // buat label timer


        public string GetTime()
        {
            string TimeInString = "";
            int min = DateTime.Now.Minute;
            int sec = DateTime.Now.Second;

            TimeInString = ":" + ((min < 10) ? "0" + min.ToString() : min.ToString());
            TimeInString += ":" + ((sec < 10) ? "0" + sec.ToString() : sec.ToString());
            return TimeInString;
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            Tuple<int, int> cell = (Tuple<int, int>)button.Tag;
            int row = cell.Item1;
            int col = cell.Item2;
            if (e is MouseEventArgs mouseEvent && mouseEvent.Button == MouseButtons.Left)
            {
                // timer atas panel
                 
                timer.Interval = (1000);
                timer.Enabled = true;
                timer.Start();
                _stopwatch.Start();
                timer.Tick += new EventHandler(timer_Tick);




                // ini ketika pemain baru langkah pertama
                if (_firstMove)
                {
      
                    RevealEmptyCells(row, col);

                    // Cek jika sel adalah ranjau
                    if (_grid[row, col].IsMine)
                    {
                        // sel adalah ranjau dan dipindahkan ke sel yang lain
                        _grid[row, col].IsMine = false;
                        PlaceMine(row, col);
                    }
                    _firstMove = false;
                }

                // Handle left-click events
                if (_grid[row, col].IsMine)
                {
                    // Kondisi Pemain Kalah
                    GameOver();
                }
                else
                {
                    // Buka sel dan perbarui Grid
                    RevealEmptyCells(row, col);
                    if (buttonBukanBom == cekMenang && cekMenang != 0)
                    {
                        Menang();
                        
                    }
                }
            }
            else if (e is MouseEventArgs mouseEventt && mouseEventt.Button == MouseButtons.Right)
            {
                // Handle right-click events
                if (button.Text == "\U0001F3F4\U000E0067\U000E0062")
                {
                    // Remove the flag
                    button.Text = "";
                    button.BackColor = Color.Gainsboro;
                    jumlahBendera = jumlahBendera + 1;
                }
                else
                {
                    // Place a flag
                    button.Text = "\U0001F3F4\U000E0067\U000E0062";
                    jumlahBendera = jumlahBendera - 1;
                    //button.BackColor = Color.Red;
                }
            }
        }

        private void RevealEmptyCells(int row, int col)
        {
            if (row < 0 || row >= GridSize || col < 0 || col >= GridSize || _grid[row, col].IsRevealed || _grid[row, col].IsMine)
            {
                // Sel di luar batas atau sudah terungkap, jadi dikembalikan
                return;
            }
            cekMenang = cekMenang + 1;

            // Buka sel dan perbarui Grid
            _grid[row, col].IsRevealed = true;
            _grid[row, col].Button.Text = _grid[row, col].AdjacentMines.ToString();
            SetCellTextSize(row, col, 14);
            SetCellColor(row, col);
            // _grid[row, col].Button.Enabled = false;
            // SetCellColor(row, col);

            if (_grid[row, col].AdjacentMines == 0)
            {
                _grid[row, col].Button.Text = "";
                _grid[row, col].Button.Enabled = false;
                // Sel tidak memiliki ranjau yang berdekatan, jadi ungkapkan semua sel yang berdekatan yang bukan ranjau
                RevealEmptyCells(row - 1, col); // top
                RevealEmptyCells(row + 1, col); // bottom
                RevealEmptyCells(row, col - 1); // left
                RevealEmptyCells(row, col + 1); // right
            }
            
        }

      
        private void Menang()
        {
            _firstMove = true;
            MessageBox.Show("Selamat Anda Menang!");
            cekMenang = 0;
            ResetGame();

        }
        private void GameOver()
        {
            // Menampilkan semua bom pada board
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (_grid[row, col].IsMine)
                    {
                        _grid[row, col].Button.Text = "\U0001F4A3";
                    }
                }
            }
            _firstMove = true;
           
            MessageBox.Show("Game Over!");

            // Reset the game
            cekMenang = 0;
            StartPanel.Show();
            ResetGame();
        }

        private void PlaceMine(int excludedRow, int excludedCol)
        {
            Random random = new Random();
            int row = random.Next(GridSize);
            int col = random.Next(GridSize);
            if (_grid[row, col].IsMine || (row == excludedRow && col == excludedCol))
            {
                // The cell is already a mine, or it is the cell that the player clicked on, so try again
                PlaceMine(excludedRow, excludedCol);
            }
            else
            {
                // Place a mine on the cell
                _grid[row, col].IsMine = true;
            }
        }



        private void ResetGame()
        {
            this.Controls.Clear();


            timer = new Timer();
            _stopwatch = new Stopwatch();
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.RowCount = GridSize;
            tableLayoutPanel.ColumnCount = GridSize;


            // Atur ukuran sel di grid
            for (int i = 0; i < GridSize; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f / GridSize));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f / GridSize));
            }

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    _grid[row, col] = new Cell();
                    _grid[row, col].Button = new Button();
                    _grid[row, col].Button.Dock = DockStyle.Fill;
                    _grid[row, col].Button.Margin = new Padding(0);
                    _grid[row, col].Button.Tag = new Tuple<int, int>(row, col);
                    
                    _grid[row, col].Button.MouseDown += new MouseEventHandler(Button_MouseDown);


                    // Tambahkan tombol ke panel tata letak tabel
                    tableLayoutPanel.Controls.Add(_grid[row, col].Button, col, row);
                }
            }

            // menempatkan ranjau acak di grid
            Random random = new Random();
            for (int i = 0; i < jumlahBom; i++)
            {
                int row = random.Next(GridSize);
                int col = random.Next(GridSize);
                if (row == 0 && col == 0)
                {
                    //Kecualikan sel pertama dari daftar sel yang mungkin berisi ranjau
                    i--;
                    continue;
                }
                _grid[row, col].IsMine = true;
            }

            int kali = (GridSize * GridSize) - jumlahBom;
            buttonBukanBom = kali;

            // Hitung jumlah angka yang berdekatan untuk setiap sel
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    _grid[row, col].AdjacentMines = GetAdjacentMines(row, col);
                }
            }

            // Buat panel untuk menahan label penghitung waktu dan angka
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 30;
            topPanel.Width = 100;

            // Buat label pengatur waktu
           // Label timerLabel = new Label();
            timerLabel.AutoSize = true;
            timerLabel.Location = new Point(10, 10);
            timerLabel.Text = string.Format("\U000023F2 " + " {0:hh\\mm\\:ss}  ", _stopwatch.Elapsed);


            // Buat label jumlah Bendera
            Label Bendera = new Label();
            Bendera.AutoSize = true;
            Bendera.Text = "\U0001F3F4\U000E0067\U000E0062 " + jumlahBendera;
            Bendera.Location = new Point(90, 10);

            // Buat label jumlah ranjau
            Label mineCountLabel = new Label();
            mineCountLabel.AutoSize = true;
            mineCountLabel.Text = "\U0001F4A3 " + jumlahBom ;
            mineCountLabel.Location = new Point(150, 10);

           

            // Tambahkan label ke panel atas
            topPanel.Controls.Add(timerLabel);
            topPanel.Controls.Add(Bendera);
            topPanel.Controls.Add(mineCountLabel);
           

            // Tambahkan panel tata letak tabel dan panel atas ke formulir
            this.Controls.Add(tableLayoutPanel);
            this.Controls.Add(topPanel);
        }

        private void SetCellColor(int row, int col)
        {
            if (_grid[row, col].AdjacentMines == 0)
            {
                _grid[row, col].Button.ForeColor = Color.Blue;
            }
            if (_grid[row, col].AdjacentMines == 1)
            {
                _grid[row, col].Button.ForeColor = Color.Brown;
            }
            if (_grid[row, col].AdjacentMines == 2)
            {
                _grid[row, col].Button.ForeColor = Color.Green;
            }
        }

        private void SetCellTextSize(int row, int col, float size)
        {
            _grid[row, col].Button.Font = new Font(_grid[row, col].Button.Font.FontFamily, size);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

    }
}
