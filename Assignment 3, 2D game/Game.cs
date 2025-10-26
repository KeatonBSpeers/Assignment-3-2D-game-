// Include the namespaces (code libraries) you need below.
using System;
using System.Data;
using System.Numerics;

// The namespace your code is in.
namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        // Window size
        int windowWidth = 800;
        int windowHeight = 600;

        Vector2 PlayerPos;
        float playerVelocityY = 0f;
        float gravity = 6.5f;
        float jumpstrength = 10f;
        float groundY = 500f;
        float groundheight = 600f;
        float radius = 25f;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetSize(windowWidth, windowHeight);
            Window.ClearBackground(color: Color.White);
            Window.SetTitle("Dino Runner Clone");

            // Start player near top middle
              PlayerPos = new Vector2(windowWidth / 2, 100);
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.White);

            Draw.Rectangle(0, (int)groundY, windowWidth, (int)groundheight);

            playerVelocityY += gravity; //Apply Gravity
            PlayerPos.Y = playerVelocityY;

            if (PlayerPos.Y > groundY - radius)
            {
                PlayerPos.Y =groundY - radius;
                playerVelocityY = 5f; //Stop Falling
            }
            //Ground
            Draw.FillColor = Color.Black;
            Draw.Rectangle(0, (int)groundY, Window.Width, (int)groundheight);

            // --- Draw player ---
            Draw.FillColor = Color.Green;
            Draw.LineColor = Color.Black;
            Draw.Circle(PlayerPos.X, PlayerPos.Y, radius);
        }
    }
}

