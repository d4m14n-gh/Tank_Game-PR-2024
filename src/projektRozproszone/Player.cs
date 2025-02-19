using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace projektRozproszone
{
    internal struct PlayerSyncData
    {
        public int PI { get; set; }
        public string PN { get; set; }
        public Vector2 P { get; set; }
        public Vector2 V { get; set; }
        public Color C { get; set; }
        public float R { get; set; }
        public float T { get; set; }

        public int H { get; set; }
        public List<(Vector2, Vector2)> B { get; set; }

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            FloatFormatHandling = FloatFormatHandling.String,
            FloatParseHandling = FloatParseHandling.Decimal,
        };
        public PlayerSyncData(string Json)
        {
            try
            {
                this = JsonConvert.DeserializeObject<PlayerSyncData>(Json);
            }
            catch
            {
                this = new PlayerSyncData();
            }
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this); 
        }
        public static PlayerSyncData GetPlayerState(Player player)
        {
            try
            {
                List<(Vector2, Vector2)> b = new List<(Vector2, Vector2)>();
                foreach (Bullet bullet in player.Bullets)
                    if(bullet!=null&&bullet.Velocity.Magnitude>1)
                        if (b.Count<10)
                            b.Add((bullet.Position, bullet.Velocity));
                return new PlayerSyncData() {T = player.Cooldown, R = player.Rotation, C = player.PlayerColor, PN = player.PlayerName, P = player.Position, V = player.Velocity, H = player.Hp, PI = player.PlayerId, B=b};
            }
            catch
            {
                return new PlayerSyncData() {T = player.Cooldown, R = player.Rotation, C = player.PlayerColor, PN = player.PlayerName, P = player.Position, V = player.Velocity, H = player.Hp, PI = player.PlayerId, B=new List<(Vector2, Vector2)>()};
            }
        }
        public static void Set(Player player, PlayerSyncData ps)
        {
            if(new Random().NextDouble()>0.5)
                player.Position = ps.P;
            player.Velocity = ps.V;
            player.PlayerColor = ps.C;
            player.Hp = ps.H;
            player.Rotation = ps.R;
            player.PlayerId = player.PlayerId;
            player.PlayerName = ps.PN;
            player.Cooldown = ps.T;
        }
    }
    internal class Player : GameObject
    {
        public Color PlayerColor { get; set; } = Color.Goldenrod; //GameMethods.RandomColor(new Random()); //Color.Goldenrod;
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int Hp { get; set; } = 100;

        public Player(Game game, string name) : base(game, name + "->GameObject") 
        {
            Layer = 1;
            PlayerName = name;
        }
        public float Rotation { get; set; } = 0f;
        public override void Draw(Graphics g, Vector2 camera, Size size, double scale = 1.0f)
        {
            float x = (float)GetScreenPosition(camera, size, scale).x;
            float y = (float)GetScreenPosition(camera, size, scale).y;
            const float p = 5;
            float r = (float)(p * scale);

            using(Pen borderPen = new Pen(GameMethods.Stroke2Color, 1))
            using(SolidBrush circleBrush = new SolidBrush(PlayerColor))
            using(SolidBrush FontBrush = new SolidBrush(Color.FromArgb(20, 10, 10)))
            using(SolidBrush FontBrush2 = new SolidBrush(Color.WhiteSmoke))
            using(SolidBrush gun = new SolidBrush(Color.FromArgb(63, 63, 64)))
            {
                borderPen.Width = (float)(scale * p * 0.075);

                float width = (float)scale * 4.0f;
                float leng = (float)scale * (6f + 1.5f*MathF.Abs(1.0f-2*Cooldown/150f));
                Matrix rotationMatrix = new Matrix();
                rotationMatrix.RotateAt(Rotation, new PointF(x, y));

                g.Transform = rotationMatrix;
                g.FillRectangle(gun, x, y - width / 2, leng, width);
                g.DrawRectangle(borderPen, x, y-width/2, leng, width);

                g.ResetTransform();


                g.FillEllipse(circleBrush, x - r, y - r, 2 * r, 2 * r);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(x - r, y - r, 2*r, 2*r);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        Color centerColor = Color.FromArgb(25, GameMethods.StrokeColor);
                        Color edgeColor = Color.FromArgb(75, GameMethods.StrokeColor); 
                        brush.CenterColor = centerColor;
                        brush.SurroundColors = new Color[] { edgeColor };
                        g.FillPath(brush, path);
                    }
                }

                g.DrawEllipse(borderPen, x - r, y - r, 2 * r, 2 * r);
     
                using (StringFormat sf = new StringFormat())
                using(Font font = new Font(FontFamily.GenericSansSerif, (float)Math.Max(scale * p * 0.3, 1), FontStyle.Regular))
                {
                    
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    FontBrush2.Color = GameMethods.StrokeColor;
                    FontBrush.Color = Color.LightGray;
                    float d = (float)scale * 0.1f;
                    string text = PlayerName;
                    g.DrawString(text, font, FontBrush2, new PointF(x + d, y), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x - d, y), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x, y - d), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x, y + d), sf);

                    g.DrawString(text, font, FontBrush2, new PointF(x + d, y + d), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x - d, y - d), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x + d, y - d), sf);
                    g.DrawString(text, font, FontBrush2, new PointF(x - d, y + d), sf);

                    g.DrawString(text, font, FontBrush, new PointF(x, y), sf);
                }

                float barWidth = (float)(10*scale);
                float barHeight = (float)(2*scale);

                // Obliczanie szerokości paska zdrowia na podstawie poziomu zdrowia
                int currentBarWidth = (int)(barWidth * (Hp / 100.0));

                // Rysowanie wypełnionego prostokąta reprezentującego pasek zdrowia

                // Rysowanie ramki wokół paska zdrowia
                using (SolidBrush circleBrush2 = new SolidBrush(Color.ForestGreen))
                using (Pen borderPen2 = new Pen(GameMethods.Stroke2Color, 1))
                {
                    borderPen2.Width = (float)(scale * p * 0.045);
                    circleBrush2.Color = Color.DarkGray;
                    g.FillRectangle(circleBrush2, x-barWidth/2, y-(int)(8*scale), barWidth, barHeight);
                    circleBrush2.Color = Color.ForestGreen;
                    g.FillRectangle(circleBrush2, x - barWidth / 2, y - (int)(8 * scale), currentBarWidth, barHeight);
                    g.DrawRectangle(borderPen2, x - barWidth / 2, y - (int)(8 * scale), barWidth, barHeight);
                }
            }
        }
        public float Cooldown { get; set; } = 0;
        public ConcurrentQueue<Bullet> Bullets = new ConcurrentQueue<Bullet>();
        public void Shoot()
        {
            if (Cooldown>0)
                return;
            Random random = new Random((int)DateTime.Now.Ticks);
            Vector2 dif = (Game.Mouse - Position).ToUnitVector();

            
            Bullet bullet = new Bullet(Game, this);
            bullet.Position = Position + new Vector2(dif * 8);

            dif = dif.Rotated((random.NextDouble() - 0.5) * 0.3);
            bullet.Velocity = dif * 50.0;
            bullet.Spawn();
            Bullets.Enqueue(bullet);
            if(Bullets.Count > 10)
            {
                Bullet? todestro = null;
                Bullets.TryDequeue(out todestro);
                if(todestro!=null)
                    todestro.Destroy();
            }

            Cooldown = 150;
        }
        public override void Start()
        {

        }
        public override void Update(double time)
        {
            if (Position.x == 1000)
                return;
            if(Cooldown > 0)
                Cooldown -= (float)time;
            var direction = (Game.Mouse - Position).ToUnitVector();
            Rotation = 180 / 3.14f * MathF.Atan2(-(float)direction.y, (float)direction.x); 
            double v0 = 12.0f;
            double vx = Velocity.x;
            double vy = Velocity.y;
            if (Game.IsPressed(Keys.W))
            {
                vy = v0;
            }
            if (Game.IsPressed(Keys.S))
            {
                vy = -v0;
            }
            if (Game.IsPressed(Keys.A))
            {
                vx = -v0;
            }
            if (Game.IsPressed(Keys.D))
            {
                vx = v0;
            }
            if (Game.IsPressed(Keys.E))
            {
                Shoot();
            }
            Velocity = new Vector2(vx, vy);
            foreach (GameObject go in Game.GameObjects)
            {
                if (go.GetType() == typeof(Bullet))
                {
                    if ((go.Position - Position).Magnitude < 6)
                    {
                        //go.Drag = 5;
                        //go.Destroy();
                        Hp -= 1;
                        if (Hp == 0)
                            Game.Reset();
                        //Task.Run(() => { NewClient.Eat(go.Position); });
                        //NewClient.Eat(go.Position);
                    }

                }
            }
        }
    }
}
