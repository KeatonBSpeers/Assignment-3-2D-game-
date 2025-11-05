using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MohawkGame2D
{
    public class Game
    {
        // Paddle setup
        float playerX = 50;
        float playerY = 250;
        float opponentX = 725;
        float opponentY = 250;
        float paddleWidth = 25;
        float paddleHeight = 130;
        float paddleSpeed = 300f;

        // Ball list
        List<Ball> balls = new List<Ball>();

        public void Setup()
        {
            Window.SetTitle("Assignment 3 - Multi-Ball Pong");
            Window.SetSize(800, 600);
            Window.SetFpsToMonitorRefreshRate();

            // Start with one ball
            balls.Add(new Ball(400, 300, 200, 150));
        }

        public void Update()
        {
            Window.ClearBackground(Color.Black);
            float dt = Raylib.GetFrameTime();

            // --- Player movement ---
            if (Raylib.IsKeyDown(KeyboardKey.W))
                playerY -= paddleSpeed * dt;
            if (Raylib.IsKeyDown(KeyboardKey.S))
                playerY += paddleSpeed * dt;
            playerY = Math.Clamp(playerY, 0, Window.Height - paddleHeight);

            // --- Opponent AI ---
            if (opponentY + paddleHeight / 2 < balls[0].Y)
                opponentY += paddleSpeed * 0.75f * dt;
            else if (opponentY + paddleHeight / 2 > balls[0].Y)
                opponentY -= paddleSpeed * 0.75f * dt;
            opponentY = Math.Clamp(opponentY, 0, Window.Height - paddleHeight);

            // --- Update all balls ---
            for (int i = 0; i < balls.Count; i++)
            {
                Ball b = balls[i];
                b.X += b.SpeedX * dt;
                b.Y += b.SpeedY * dt;

                // Bounce off top/bottom
                if (b.Y - b.Radius <= 0 || b.Y + b.Radius >= Window.Height)
                {
                    b.SpeedY *= -1;
                    b.Y = Math.Clamp(b.Y, b.Radius, Window.Height - b.Radius);
                }

                // Player paddle collision
                if (b.X - b.Radius <= playerX + paddleWidth &&
                    b.Y >= playerY && b.Y <= playerY + paddleHeight)
                {
                    b.SpeedX = Math.Abs(b.SpeedX); // bounce right
                    b.X = playerX + paddleWidth + b.Radius;
                    if (Raylib.GetRandomValue(0, 4) == 0)
                    {
                        balls.Add(new Ball(b.X, b.Y, 250, -200)); // duplicate!
                    }
                }
                // Opponent paddle collision
                if (b.X + b.Radius >= opponentX &&
                    b.Y >= opponentY && b.Y <= opponentY + paddleHeight)
                {
                    b.SpeedX = -Math.Abs(b.SpeedX); // bounce left
                    b.X = opponentX - b.Radius;
                    if (Raylib.GetRandomValue(0, 4) == 0)
                    {
                        balls.Add(new Ball(b.X, b.Y, -250, 200)); // duplicate!

                    }
                }

                // Bounce off left/right boundaries
                if (b.X - b.Radius <= 0 || b.X + b.Radius >= Window.Width)
                {
                    b.SpeedX *= -1;
                    b.X = Math.Clamp(b.X, b.Radius, Window.Width - b.Radius);
                }

                // Draw the ball
                Draw.FillColor = Color.OffWhite;
                Draw.Circle(b.X, b.Y, b.Radius);
            }

            // --- Draw paddles ---
            Draw.FillColor = Color.OffWhite;
            Draw.Rectangle(playerX, playerY, paddleWidth, paddleHeight);
            Draw.Rectangle(opponentX, opponentY, paddleWidth, paddleHeight);
        }

        // --- Ball structure ---
        public class Ball
        {
            public float X;
            public float Y;
            public float SpeedX;
            public float SpeedY;
            public float Radius = 15f;

            public Ball(float x, float y, float sx, float sy)
            {
                X = x;
                Y = y;
                SpeedX = sx;
                SpeedY = sy;
            }
        }
    }
}
