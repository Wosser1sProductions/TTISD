﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SharpGL;

namespace TTISDassignment2
{
    abstract class Mover
    {
        protected Point3D _pos;
        public Point3D Pos { get => _pos; }

        public Point Size { get; set; }

        protected Point3D _speed;
        public Point3D Speed { get => _speed; set => _speed = value; }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public double Border { get; set; }

        public bool wentOutOfLeftBorder  = false;
        public bool wentOutOfRightBorder = false;
        public bool hadCollision         = false;

        private bool isLeft  = false;
        private bool isRight = false;
        
        public bool IsOnLeftSide  { get => isLeft; }
        public bool IsOnRightSide { get => isRight; }

        public Mover(double x, double y, double z, double w, double h, double b = 1)
        {
            this._pos      = new Point3D(x, y, z);
            this.Size      = new Point(w, h);
            this.Speed     = new Point3D(0, 0, 0);
            this.FillColor = Colors.White;
            this.Border    = b;
        }

        public Mover(Point3D pos, Size size, Color color, double b = 1)
        {
            this._pos      = pos;
            this.Size      = new Point(size.Width, size.Height);
            this.Speed     = new Point3D(0, 0, 0);
            this.FillColor = color;
            this.Border    = b;
        }

        public void set2DPosition(Point pos)
        {
            this._pos.X = pos.X;
            this._pos.Y = pos.Y;
        }

        public virtual void move(Point3D speed)
        {
            this._pos.Offset(Speed.X, Speed.Y, Speed.Z);
        }

        public virtual void update(Point3D collide_rect)
        {
            this.move(Speed);
            this.hadCollision = false;
            this.wentOutOfLeftBorder = false;
            this.wentOutOfRightBorder = false;

            // Collide with game borders
            if (Pos.X <= 0) {
                _speed.X = -_speed.X;
                _pos.X = 0.0;
                this.wentOutOfLeftBorder = true;
                this.hadCollision = true;
            } else if (Pos.X + Size.X >= collide_rect.X) {
                _speed.X = -_speed.X;
                _pos.X = collide_rect.X - Size.X;
                this.wentOutOfRightBorder = true;
                this.hadCollision = true;
            }

            if (Pos.Y <= 0) {
                _speed.Y = -_speed.Y;
                _pos.Y = 0.0;
                this.hadCollision = true;
            } else if (Pos.Y + Size.Y >= collide_rect.Y) {
                _speed.Y = -_speed.Y;
                _pos.Y = collide_rect.Y - Size.Y;
                this.hadCollision = true;
            }

            isLeft  = Pos.X + Size.X < collide_rect.X / 2;
            isRight = Pos.X > collide_rect.X / 2;
        }

        public virtual bool collidesWith(Mover b)
        {
            bool left  = this.Pos.X < b.Pos.X + b.Size.X;
            bool right = this.Pos.X + this.Size.X > b.Pos.X;
            bool bot   = this.Pos.Y < b.Pos.Y + b.Size.Y;
            bool top   = this.Pos.Y + this.Size.Y > b.Pos.Y;

            if (left && right && bot && top)
            {
                double left_overlap  = b.Pos.X + b.Size.X - this.Pos.X;
                double right_overlap = this.Pos.X + this.Size.X - b.Pos.X;
                double bot_overlap   = b.Pos.Y + b.Size.Y - this.Pos.Y;
                double top_overlap   = this.Pos.Y + this.Size.Y - b.Pos.Y;

                bool strictLeft  = left_overlap < top_overlap && left_overlap < bot_overlap && left_overlap < right_overlap;
                bool strictRight = right_overlap < top_overlap && right_overlap < bot_overlap && right_overlap < left_overlap;

                if (strictLeft)
                {   // Left
                    _pos.X = b.Pos.X + b.Size.X;
                    _speed.X = -_speed.X;
                }
                else if (strictRight)
                {   // Right
                    _pos.X = b.Pos.X - this.Size.X;
                    _speed.X = -_speed.X;
                } else if (top_overlap < bot_overlap)
                {   // Top
                    _pos.Y = b.Pos.Y - this.Size.Y;
                    _speed.Y = -_speed.Y;
                } else
                {   // Bot
                    _pos.Y = b.Pos.Y + b.Size.Y;
                    _speed.Y = -_speed.Y;
                }

                this.hadCollision = true;
                return true;
            }

            return false;
        }
        
        public virtual void grow(Point growrate)
        {
            this.Size.Offset(growrate.X, growrate.Y);
        }

        public virtual void growFromMiddle(Point growrate)
        {
            grow(growrate);
            _pos.X -= growrate.X / 2.0;
            _pos.Y -= growrate.Y / 2.0;
        }

        public abstract void drawFilled(OpenGL gl);
        public abstract void drawBorder(OpenGL gl);
    }
}
