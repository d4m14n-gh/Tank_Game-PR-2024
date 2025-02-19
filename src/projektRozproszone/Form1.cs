using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace projektRozproszone
{
    public partial class Form1 : Form
    {
        private Game game = new Game();
        public Form1()
        {
            InitializeComponent();
        }

        // private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) { }
        private long lastTime = DateTime.Now.Ticks;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                double deltaTime = (DateTime.Now.Ticks - lastTime) / (double)TimeSpan.TicksPerMillisecond;
                lastTime = DateTime.Now.Ticks;
                game.Update(deltaTime);
                if (!IsDisposed && !Disposing)
                    Invoke(Refresh);
                Thread.Sleep(10);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BackColor = GameMethods.BgColor;
            backgroundWorker1.RunWorkerAsync();
            game.OnReset = Reset;
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool AllocConsole();
            //AllocConsole();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            game.Draw(g, Size, 6.0);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            game.setKey(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            game.setKey(e.KeyCode, false);
        }
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta / 120;
            for (int i = 0; i < delta; i++)
                if (game.targetZoom < 100.0f)
                    game.targetZoom *= 1.1f;
            for (int i = delta; i < 0; i++)
                if (game.targetZoom > 2.0f)
                    game.targetZoom /= 1.1f;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = false;
            //if (await Client.Connect())
            //{
            panel1.Enabled = false;
            panel1.Hide();
            if (!connecting)
                backgroundWorker2.RunWorkerAsync();
            Random rng = new Random();
            game.GamePlayer.Position = new Vector2(0, 0) + new Vector2((rng.NextDouble() - 0.5) * 30, (rng.NextDouble() - 0.5) * 30);
            game.Camera = new Vector2(0, 0);
            game.targetZoom = 10;
            //}
            button1.Enabled = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            game.GamePlayer.PlayerColor = colorDialog1.Color;
        }

        private void textBox1_TextChanged(object? sender, EventArgs e)
        {
            game.GamePlayer.PlayerName = textBox1.Text.Trim();
        }
        public void Reset(object sender, EventArgs args)
        {
            panel1.Show();
            panel1.Enabled = true;
            Color color = game.GamePlayer.PlayerColor;
            int id = game.GamePlayer.PlayerId;
            game = new Game();
            game.GamePlayer.PlayerId = id;
            Client.Player = game.GamePlayer;
            game.GamePlayer.PlayerColor = color;
            game.GamePlayer.PlayerName = textBox1.Text.Trim();
            game.OnReset = Reset;
        }
        bool connecting = false;
        private async void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            connecting = true;
            Client.Player = game.GamePlayer;
            int port = 50080;
            int.TryParse(textBoxPort.Text, out port);
            await Client.Sync(textBoxAdress.Text, port);
            connecting = false;
            Reset(this, EventArgs.Empty);
            //while (true)
            //{
            //    if (Client.isConnected)
            //    {
            //        await Client.Sync(PlayerSyncData.GetPlayerState(game.GamePlayer));
            //        await Console.Out.WriteAsync("->");
            //        await Task.Delay(1000 / 64);
            //    }
            //    else
            //    {
            //        panel1.Enabled = true;
            //        panel1.Show();
            //        //await Console.Out.WriteLineAsync("connecting");
            //        //await Client.Connect();
            //        //await Task.Delay(1000);
            //    }
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            game.GamePlayer.PlayerColor = GameMethods.RandomColor(new Random());
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = new Point(e.X, e.Y);
            game.MousePoint = (pos, Size);
        }

        private void textBoxAdress_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
