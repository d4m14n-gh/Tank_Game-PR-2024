using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRozproszone
{
    internal class Game
    {
        public double targetZoom = 15;
        public double Zoom { get; private set; } = 10;
        public Vector2 Camera { get; set; } = new Vector2(0, 0);
        public (Point, Size) MousePoint { get; set; } = (new Point(0, 0), new Size(1, 1));
        public Vector2 Mouse { get { return MousePoint.Item1.ToGamePoint(MousePoint.Item2, Camera, Zoom); } }
        public Player GamePlayer { get; private set; }
        private List<GameObject> gameObjects = new List<GameObject>();
        public List<GameObject> GameObjects { get { lock(gameObjects) return new List<GameObject>(gameObjects.Where(g => g != null).OrderBy((g) => g.Layer)); } }
        private Dictionary<Keys, bool> keysMap { get; set; } = new Dictionary<Keys, bool>();

        public Game()
        {
            GamePlayer = new Player(this, "unnamed");
            Start();
        }
        public void Draw(Graphics g, Size size, double length)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.TextContrast = 0;
            //draw grid
            using (SolidBrush brush = new SolidBrush(GameMethods.GridColor))
            {
                //g.FillRectangle(brush, 0, 0, size.Width, size.Height);
                double x = GameMethods.GetScreenPosition(Camera, size, 0, 0, Zoom).x % (length * Zoom);
                while (x < size.Width)
                {
                    double y = (GameMethods.GetScreenPosition(Camera, size, 0, 0, Zoom).y) % (length * Zoom);
                    while (y < size.Height)
                    {
                        g.FillEllipse(brush, (int)x, (int)y, (int)(Zoom / 2), (int)(Zoom / 2));
                        y += (length * Zoom);
                    }
                    x += (length * Zoom);
                }
            }
            //draw gameObjects
            foreach (GameObject gameObject in GameObjects)
                gameObject.Draw(g, Camera, size, Zoom);
            GamePlayer.Draw(g, Camera, size, Zoom);

            g.SmoothingMode = SmoothingMode.HighSpeed;
            bool vignete = false;
            if (!vignete)
                return;
            Color centerColor = Color.FromArgb(0, GameMethods.StrokeColor); // Kolor w centrum (najjaśniejszy)
            Color edgeColor = Color.FromArgb(255, GameMethods.StrokeColor); // Kolor na brzegu (najciemniejszy)
            float centerX = (float)(size.Width / 2.0 + 0*(GamePlayer.Position - Camera).x * Zoom);
            float centerY = (float)(size.Height / 2.0 - 0*(GamePlayer.Position - Camera).y * Zoom);
            float xm = Math.Max(size.Width, size.Height) * 2, ym = Math.Max(size.Width, size.Height) * 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(centerX - xm * 0.5f, centerY - ym * 0.5f, xm, ym);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    // Definiowanie centrów gradientu
                    brush.CenterColor = centerColor;
                    brush.SurroundColors = new Color[] { edgeColor };

                    // Rysowanie winiety
                    g.FillPath(brush, path);
                }
            }
        }
        public void Start()
        {
            Client.OnSynced += Sync;
            Client.OnGivedId += (sender, id) => { GamePlayer.PlayerId = id; };
        }
        public void Update(double deltaTime)
        {
            GamePlayer.Go(deltaTime);
            foreach (GameObject gameObject in GameObjects)
                gameObject.Go(deltaTime);
            GamePlayer.Update(deltaTime);



            Zoom = GameMethods.Lerp(Zoom, targetZoom, 0.005f * deltaTime);
            Vector2 directon = (GamePlayer.Position - Camera);
            if (directon.Magnitude > deltaTime * 0.005 * directon.Magnitude)
            {
                double mg = directon.Magnitude;
                directon = directon.ToUnitVector();
                Camera += directon * deltaTime * 0.005 * mg;
            }
        }
        public void FixedUpdate(double deltaTime)
        {
            //Camera = Player.Position;
        }
        public void setKey(Keys key, bool value)
        {
            keysMap[key] = value;
        }
        public bool IsPressed(Keys key)
        {
            return keysMap.ContainsKey(key) && keysMap[key];
        }
        public void AddGameObject(GameObject gameObject)
        {
            lock(gameObjects)
                gameObjects.Add(gameObject);
        }
        public void RemoveGameObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                if(gameObjects.Contains(gameObject))
                gameObjects.Remove(gameObject);
            }
        }
        Dictionary<int, Player> otherPlayers = new Dictionary<int, Player>();
        public void Sync(object? sender, EventArgs args)
        {
            //lock(gameObjects)
                //gameObjects.Clear();
            foreach(var x in otherPlayers.Where(kv => !Client.PlayersInfo.Select(p => p.PI).Contains(kv.Key)).ToDictionary())
                x.Value.Destroy();
            otherPlayers = otherPlayers.Where(kv => Client.PlayersInfo.Select(p => p.PI).Contains(kv.Key)).ToDictionary();
            foreach (PlayerSyncData ps in Client.PlayersInfo)
            {
                if (ps.PI == GamePlayer.PlayerId)
                    continue;
                Player player;
                if (otherPlayers.ContainsKey(ps.PI))
                {
                    player = otherPlayers[ps.PI];
                    PlayerSyncData.Set(player, ps);
                }
                else
                {
                    player = new Player(this, ps.PN);
                    PlayerSyncData.Set(player, ps);
                    otherPlayers.Add(ps.PI, player);
                    player.Spawn();
                }

                foreach (Bullet bullet in player.Bullets)
                    bullet.Destroy();
                player.Bullets.Clear();

                foreach (var bullet in ps.B)
                {
                    Bullet newBullet = new Bullet(this, player);
                    newBullet.Position = bullet.Item1;
                    newBullet.Velocity = bullet.Item2;
                    newBullet.Layer = -player.PlayerId;
                    player.Bullets.Enqueue(newBullet);
                    newBullet.Spawn();
                }
            }
        }
        public EventHandler OnReset { get; set; }
        public void Reset()
        {
            OnReset.Invoke(this, EventArgs.Empty);
        }
    }

    internal static class GameMethods
    {
        public static Color StrokeColor { get; } = Color.FromArgb(43, 43, 44);
        public static Color GridColor { get; } = Color.FromArgb(43, 43, 44);
        public static Color Stroke2Color { get; } = Color.FromArgb(43, 43, 44);

        public static Color BgColor { get; } = Color.FromArgb(113,113, 115);
        public static Vector2 GetScreenPosition(Vector2 camera, Size size, double posx, double posy, double scale)
        {
            double x = (size.Width / 2.0 - scale * (camera.x - posx));
            double y = (size.Height / 2.0 + scale * (camera.y - posy));
            return new Vector2(x, y);
        }
        public static Color RandomColor(Random random)
        {
            return Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
        }
        public static Vector2 ToGamePoint(this Point point, Size screenSize, Vector2 cameraPosition, double scale)
        {
            return new Vector2(cameraPosition.x + (point.X - screenSize.Width / 2.0) / scale, cameraPosition.y + (-point.Y + screenSize.Height / 2.0) / scale);
        }
        public static double Lerp(double a, double b, double t, double ts = 0, double te = 1)
        {
            t = (t - ts) / (te - ts);
            t = Clamp01(t);
            return a + (b - a) * t;
        }
        private static double Clamp01(double value)
        {
            return (value < 0f) ? 0 : (value > 1) ? 1 : value;
        }
    }
}
