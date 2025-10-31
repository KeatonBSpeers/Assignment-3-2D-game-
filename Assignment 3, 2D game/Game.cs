using Raylib_cs;
using System;
using System.Numerics;
using System.Security.Cryptography;

namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        float playerX = 600;
        float playerY = 100;

        // Opponent Variables
        float oppenentX = 200;
        float oppenentY = 100;
        // movement speed in pixels per second
        float speed = 200f;
        //Ball Variables
        float ballX = 400;
        float ballY = 300;

        float velocityX = 0f;
        float velocityY = 0f;
        float gravity = 1500f;     // pixels per second^2

        // screen-related
        readonly float wallThickness = 20f;
        int screenWidth;
        int screenHeight;

        // Walls
        Rectangle leftWall;
        Rectangle rightWall;
        Rectangle topWall;
        Rectangle bottomWall;

        readonly float wallJumpImpulse = 300f;       // horizontal push when wall-jumping





        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Assignment 3 - Pong Keaton");
            Window.SetSize(800, 600);
            Color Red = new Color(255, 0, 0);
            Window.SetFpsToMonitorRefreshRate();


            // initialize walls
            leftWall = new Rectangle(0, 0, wallThickness, screenHeight);
            rightWall = new Rectangle(screenWidth - wallThickness, 0, wallThickness, screenHeight);
            topWall = new Rectangle(0, 0, screenWidth, wallThickness);
            bottomWall = new Rectangle(0, screenHeight - wallThickness, screenWidth, wallThickness);

        }

        public class Collision //Collision system for paddles and ball
        {
            Vector2 poistion;
            Vector2 velocity;
            Vector2 size;
            Vector2 color;

            public void setup()
            {
                //place ball on screen, random position currently
                poistion = Random.Vector2(Window.Size);
                //Create vector to move ball and react accordingly to direction
                Vector2 direction = Random.Direction();
                float Speed = 300f;
                velocity = direction * Speed;
                size = new Vector2(15, 15);


            }

        }
        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.Black);
            Draw.FillColor = Color.OffWhite;
            Draw.Rectangle(oppenentX, oppenentY, 25, 130);

            // delta time for frame-independent movement
            float dt = Raylib.GetFrameTime();

            // update walls to follow screen size
            leftWall = new Rectangle(0, 0, wallThickness, screenHeight);
            rightWall = new Rectangle(screenWidth - wallThickness, 0, wallThickness, screenHeight);
            topWall = new Rectangle(0, 0, screenWidth, wallThickness);
            bottomWall = new Rectangle(0, screenHeight - wallThickness, screenWidth, wallThickness);


            // WASD movement (S = up, W = down, D = left, A = right)
            if (Raylib.GetRandomValue(0, 9) == 0) // 1-in-10 chance (0..9)
            {
                if (Raylib.IsKeyDown(KeyboardKey.S))
                {

                    playerY -= speed * dt;

                }
            }
            if (Raylib.GetRandomValue(0, 9) == 0)
            {
                if (Raylib.IsKeyDown(KeyboardKey.W))
                {
                    playerY += speed * dt;
                }
            }

            if (Raylib.GetRandomValue(0, 9) == 0) // 1-in-10 chance (0..9)
            {
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    playerX -= speed * dt;
                }
            }
            if (Raylib.GetRandomValue(0, 9) == 0) // 1-in-10 chance (0..9)
            { if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    playerX += speed * dt;
                }
            }

            //oppenent movement arrow keys
            if (Raylib.GetRandomValue(0, 9) == 0) // 1-in-10 chance (0..9)
            {
                if (Raylib.IsKeyDown(KeyboardKey.Down))
                {
                    oppenentY -= speed * dt;
                }
            }
            if (Raylib.GetRandomValue(0, 9) == 0)
            { if (Raylib.IsKeyDown(KeyboardKey.Up))
                {
                    oppenentY += speed * dt;
                }
            }
            if (Raylib.GetRandomValue(0, 9) == 0)
            { if (Raylib.IsKeyDown(KeyboardKey.Right))
                {
                    oppenentX -= speed * dt;
                }
            }
            if (Raylib.GetRandomValue(0, 9) == 0)
            { if (Raylib.IsKeyDown(KeyboardKey.Left))
                {
                    oppenentX += speed * dt;
                }
            }

            // update ball position 
            ballX += velocityY * dt;
            // apply gravity to ball's vertical velocity
             += gravity * dt;
            // draw ball
            Draw.FillColor = Color.OffWhite;
            Draw.Circle(ballX, ballY, 15);

            // optional: clamp the player inside the window so the circle stays visible
            float radius = 50f;
            playerX = Math.Clamp(playerX, radius, Raylib.GetScreenWidth() - radius);
            playerY = Math.Clamp(playerY, radius, Raylib.GetScreenHeight() - radius);

         

                Draw.FillColor = Color.OffWhite;
                Draw.Rectangle(playerX, playerY, 25, 130);
            }
        }
    } 
