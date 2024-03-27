﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace agario
{
    internal class Player : GameObject
    {
        public Color Color { get { return circleBrush.Color; } set { lock(this) circleBrush.Color = value; borderPen.Color = Color.FromArgb(value.R / 2, value.G / 2, value.B / 2); } }
        private static Player player;
        public static Player MPlayer { get { return player; } set { player = value; } }
        public double Size { get; set; }
        public double R { get { return (double)(Math.Sqrt(Size/Math.PI)); } }
        private bool phantom = false;
        public bool Phantom { get { return phantom; } set { phantom = value; if (value) Color = Color.FromArgb(50, Color); } }
        public Player(string name) {
            this.Name = name;
            if(player == null)
                player = this;
            Size = 3.14f;
            Drag = 1.0f;
            Initialize();
        }
        public Player(string name, PlayerState ps)
        {
            this.Name = name;
            if (player == null)
                player = this;
            Position = new Vector2(ps.posX, ps.posY);
            Velocity = new Vector2(ps.vX, ps.vY);
            Size = ps.size;
            Color = ps.color;
            Drag = 1.0f;
            Initialize();
        }
        private Pen borderPen = new Pen(Color.Gray, 5);
        private SolidBrush circleBrush = new SolidBrush(Color.White);
        private SolidBrush FontBrush = new SolidBrush(Color.FromArgb(20, 10, 10));
        private SolidBrush FontBrush2 = new SolidBrush(Color.WhiteSmoke);
        public override void Draw(Graphics g, double camerax, double cameray, Size size, double scale = 1.0f)
        {
            int x = GetScreenPosition(camerax, cameray, size, scale).X;
            int y = GetScreenPosition(camerax, cameray, size, scale).Y;
            int r = (int)(Size * scale);

            borderPen.Width = (float)(scale/4.0);
            // Rysowanie koła
            lock (this)
            {
                g.FillEllipse(circleBrush, x - r, y - r, 2 * r, 2 * r);
                // Rysowanie obramowania koła
                g.DrawEllipse(borderPen, x - r, y - r, 2 * r, 2 * r);
            }


            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            Font font = new Font("Arial", (float)Math.Max(scale*R, 1));
            Font font2 = new Font("Arial", (float)Math.Max(scale*R, 1));
            if(Color.GetBrightness()<0.5f)
                g.DrawString(Name, font2, FontBrush2, new PointF(x, y), sf);
            else
                g.DrawString(Name, font, FontBrush, new PointF(x, y), sf);

            font.Dispose();
            font2.Dispose();
            sf.Dispose();

        }
        private bool ready = true;
        public void Shoot()
        {
            if (!ready)
                return;
            Random random = new Random((int)DateTime.Now.Ticks);
            Vector2 dif = (Game.Mouse - Position).ToUnitVector();
            dif = dif.Rotated((random.NextDouble()-0.5)*0.3);
            Bullet bullet = new Bullet(this.Position + new Vector2(dif*(Size+1.5)), this);
            bullet.Color = Color;
            bullet.Velocity = dif*70.0;
            
            bullet = new Bullet(this.Position + new Vector2(-dif*(Size+1.5)), this);
            bullet.Color = Color;
            bullet.Velocity = -dif*70.0;
            //bullet.Size = 0.5;
            ready = false;
            Task.Run(() => { Thread.Sleep(50); ready = true; });
        }
        public override void Start()
        {

        }
        public override void Update(double time)
        {
            if (Player.MPlayer == this)
            {
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
                foreach (GameObject go in GameObject.GameObjects)
                {
                    if (go.GetType() == typeof(ExpPoint))
                    {
                        if ((go.Position - Position).Magnitude < Size) {
                            Size = (double)Math.Sqrt(Size * Size + 0.1);
                            go.Destroy();
                            Task.Run(() => { NewClient.Eat(go.Position); });
                        }
                        
                    }
                    else if (go.GetType() == typeof(Player) && this != go)
                    {
                        Player pl = (Player)go;
                        if (!pl.Phantom && (go.Position - Position).Magnitude < Size && Size > pl.Size)//*1.2f)
                        {
                            go.Destroy();
                            Task.Run(() => { NewClient.Kill(pl.Name); });
                            Size = (double)Math.Sqrt(Size * Size + (pl.Size * pl.Size)/2.0);
                        }
                    }
                }
               
            }
        }
        public void Set(PlayerState ps)
        {
            Position = new Vector2(ps.posX, ps.posY);
            Velocity = new Vector2(ps.vX, ps.vY);
            Size = ps.size;
            Color = ps.color;
        }
        
    }
}
