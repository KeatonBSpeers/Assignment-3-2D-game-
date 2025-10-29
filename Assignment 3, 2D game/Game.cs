using Raylib_cs;
using System;
using System.Numerics;

namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        float playerX = 100;
        float playerY = 100;
        // movement speed in pixels per second
        float speed = 200f;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Assignment 3 - Keaton");
            Window.SetSize(800, 600);
            Color Red = new Color(255, 0, 0);
            Window.SetFpsToMonitorRefreshRate();
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.Black);

            // delta time for frame-independent movement
            float dt = Raylib.GetFrameTime();

            // WASD movement (W = up, S = down, A = left, D = right)
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                playerY -= speed * dt;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                playerY += speed * dt;
            }
            if (Raylib.IsKeyDown(KeyboardKey.PageUp))
            {
                playerX -= speed * dt;
            }
            if (Raylib.IsKeyDown(KeyboardKey.PageDown))
            {
                playerX += speed * dt;
            }

            // optional: clamp the player inside the window so the circle stays visible
            float radius = 50f;
            playerX = Math.Clamp(playerX, radius, Raylib.GetScreenWidth() - radius);
            playerY = Math.Clamp(playerY, radius, Raylib.GetScreenHeight() - radius);

            Draw.FillColor = Color.OffWhite;
            Draw.Rectangle(playerX, playerY,25, 130);
        }
    }
}