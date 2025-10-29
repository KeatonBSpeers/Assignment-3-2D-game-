// Include the namespaces (code libraries) you need below.
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;

// The namespace your code is in.
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

        // physics fields
        float velocityX = 0f;
        float velocityY = 0f;
        float gravity = 1500f;     // pixels per second^2
        float jumpSpeed = 600f;    // initial jump velocity (pixels per second)
        float wallJumpImpulse = 300f; // horizontal push when wall-jumping
        float horizontalFriction = 8f; // how quickly horizontal impulse decays

        // double-jump state
        int jumpCount = 0;
        int maxJumps = 2;

        // simple walls (rectangles)
        Rectangle leftWall;
        Rectangle rightWall;
        Rectangle midWall;

        // moving platforms
        List<MovingPlatform> platforms = new List<MovingPlatform>();

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Assignment 3 - Dino Runner Clone");
            Window.SetSize(1200, 600);
            Color Red = new Color(255, 0, 0);

            // initialize walls (20px thick walls at left/right and one vertical platform in the middle)
            float wallThickness = 20f;
            leftWall = new Rectangle(0, 0, wallThickness, Raylib.GetScreenHeight());
            rightWall = new Rectangle(Raylib.GetScreenWidth() - wallThickness, 0, wallThickness, Raylib.GetScreenHeight());
            midWall = new Rectangle(Raylib.GetScreenWidth() * 0.5f - 50f, Raylib.GetScreenHeight() - 250f, 100f, 200f);

            // --- moving platforms ---
            // parameters: rect, axisMin, axisMax, velocity, horizontalMovement (true = horizontal, false = vertical)
            platforms.Add(new MovingPlatform(new Rectangle(200f, 450f, 240f, 16f), 200f, 600f, new Vector2(80f, 0f), true));  // horizontal platform
            platforms.Add(new MovingPlatform(new Rectangle(520f, 350f, 140f, 16f), 300f, 520f, new Vector2(-60f, 0f), true)); // horizontal moving left
            platforms.Add(new MovingPlatform(new Rectangle(800f, 400f, 160f, 16f), 380f, 520f, new Vector2(0f, 40f), false)); // vertical platform
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.OffWhite);

            // delta time for frame-independent movement
            float dt = Raylib.GetFrameTime();

            // radius used for drawing and collision with window bounds
            float radius = 20f;

            // update walls if window resized (keep them at edges)
            float wallThickness = 20f;
            leftWall = new Rectangle(0, 0, wallThickness, Raylib.GetScreenHeight());
            rightWall = new Rectangle(Raylib.GetScreenWidth() - wallThickness, 0, wallThickness, Raylib.GetScreenHeight());
            midWall = new Rectangle(Raylib.GetScreenWidth() * 0.5f - 50f, Raylib.GetScreenHeight() - 250f, 100f, 200f);

            // Update platforms (move them) before collision checks
            foreach (var p in platforms)
            {
                p.Update(dt);
            }

            // Save previous Y for accurate platform landing detection
            float prevY = playerY;

            // Horizontal input (A/D) — direct control plus any existing horizontal velocity (from wall-jump)
            float inputX = 0f;
            if (Raylib.IsKeyDown(KeyboardKey.A)) inputX -= 1f;
            if (Raylib.IsKeyDown(KeyboardKey.D)) inputX += 1f;

            // apply direct input movement
            playerX += inputX * speed * dt;

            // apply horizontal velocity (wall-jump impulse), then decay it
            playerX += velocityX * dt;
            // apply friction to velocityX so impulse decays
            float frictionFactor = 1f - MathF.Min(horizontalFriction * dt, 1f);
            velocityX *= frictionFactor;

            // Apply gravity
            velocityY += gravity * dt;
            playerY += velocityY * dt;

            // Ground Y position and on-ground test
            float groundY = Raylib.GetScreenHeight() - radius;
            bool onGround = false;
            if (playerY >= groundY)
            {
                playerY = groundY;
                velocityY = 0f;
                jumpCount = 0; // reset jump counter when touching the ground
                onGround = true;
            }

            // Platform collisions (landing from above or side separation)
            foreach (var plat in platforms)
            {
                if (Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, plat.Rect))
                {
                    float platformTop = plat.Rect.Y;
                    float platformLeft = plat.Rect.X;
                    float platformRight = plat.Rect.X + plat.Rect.Width;

                    // Landing from above: previous bottom was above platform top
                    if (prevY + radius <= platformTop + 1f && playerY + radius >= platformTop)
                    {
                        playerY = platformTop - radius;
                        velocityY = 0f;
                        jumpCount = 0; // reset jumps when landing on platform
                        onGround = true;

                        // carry player with platform horizontally if platform moves
                        playerX += plat.Velocity.X * dt;
                    }
                    else
                    {
                        // Side collision: push player out horizontally
                        if (playerX < platformLeft)
                        {
                            playerX = platformLeft - radius;
                            velocityX = 0f;
                        }
                        else if (playerX > platformRight)
                        {
                            playerX = platformRight + radius;
                            velocityX = 0f;
                        }
                        else
                        {
                            // If inside vertically (rare), push up
                            playerY = platformTop - radius;
                            velocityY = 0f;
                            onGround = true;
                        }
                    }
                }
            }

            // Check wall contacts (circle vs rect) for wall-jump detection
            bool touchingLeftWall = Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, leftWall);
            bool touchingRightWall = Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, rightWall);
            bool touchingMidWall = Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, midWall);
            bool onWall = (touchingLeftWall || touchingRightWall || touchingMidWall) && !onGround;

            // Jump input
            bool jumpPressed = Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.Space);

            // Jump behavior: normal jumps (up to maxJumps) OR wall-jump when touching wall
            if (jumpPressed && (jumpCount < maxJumps || onWall))
            {
                velocityY = -jumpSpeed;
                jumpCount++;

                // if jumping from a wall, apply horizontal impulse away from the wall
                if (onWall)
                {
                    if (touchingLeftWall)
                    {
                        velocityX = wallJumpImpulse; // push right
                        // nudge player out of wall so collision resolution below won't immediately re-snap
                        playerX = leftWall.X + leftWall.Width + radius + 0.5f;
                    }
                    else if (touchingRightWall)
                    {
                        velocityX = -wallJumpImpulse; // push left
                        playerX = rightWall.X - radius - 0.5f;
                    }
                    else if (touchingMidWall)
                    {
                        // determine which side of midWall we're closer to and push opposite
                        float midCenterX = midWall.X + midWall.Width / 2f;
                        if (playerX < midCenterX)
                        {
                            velocityX = -wallJumpImpulse; // push left if on left side of mid wall
                            playerX = midWall.X - radius - 0.5f;
                        }
                        else
                        {
                            velocityX = wallJumpImpulse; // push right
                            playerX = midWall.X + midWall.Width + radius + 0.5f;
                        }
                    }
                }
            }

            // Simple wall collision resolution: prevent penetrating vertical walls, zero horizontal impulse when colliding
            // Left wall
            if (Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, leftWall))
            {
                playerX = leftWall.X + leftWall.Width + radius;
                velocityX = 0f;
            }
            // Right wall
            if (Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, rightWall))
            {
                playerX = rightWall.X - radius;
                velocityX = 0f;
            }
            // Mid wall
            if (Raylib.CheckCollisionCircleRec(new Vector2(playerX, playerY), radius, midWall))
            {
                // decide side and push out
                float midCenterX = midWall.X + midWall.Width / 2f;
                if (playerX < midCenterX)
                {
                    playerX = midWall.X - radius;
                }
                else
                {
                    playerX = midWall.X + midWall.Width + radius;
                }
                velocityX = 0f;
            }

            // Clamp horizontal position so the circle stays visible
            playerX = Math.Clamp(playerX, radius, Raylib.GetScreenWidth() - radius);

            // Draw walls (visual aid)
            Raylib.DrawRectangleRec(leftWall, Color.DarkGray);
            Raylib.DrawRectangleRec(rightWall, Color.DarkGray);
            Raylib.DrawRectangleRec(midWall, Color.DarkGray);

            // Draw moving platforms
            foreach (var p in platforms)
            {
                Raylib.DrawRectangleRec(p.Rect, Color.Red);
            }

            // Draw player
            Draw.FillColor = Color.Red;
            Draw.Circle(playerX, playerY, radius);
        }

        // Nested helper class for moving platforms
        class MovingPlatform
        {
            public Rectangle Rect;
            public Vector2 Velocity;
            float minAxis;
            float maxAxis;
            bool horizontal; // true: move on X axis, false: move on Y axis

            public MovingPlatform(Rectangle rect, float minAxis, float maxAxis, Vector2 velocity, bool horizontal)
            {
                Rect = rect;
                this.minAxis = minAxis;
                this.maxAxis = maxAxis;
                Velocity = velocity;
                this.horizontal = horizontal;
            }

            public void Update(float dt)
            {
                if (horizontal)
                {
                    Rect.X += Velocity.X * dt;
                    if (Rect.X < minAxis)
                    {
                        Rect.X = minAxis;
                        Velocity.X = -Velocity.X;
                    }
                    else if (Rect.X > maxAxis)
                    {
                        Rect.X = maxAxis;
                        Velocity.X = -Velocity.X;
                    }
                }
                else
                {
                    Rect.Y += Velocity.Y * dt;
                    if (Rect.Y < minAxis)
                    {
                        Rect.Y = minAxis;
                        Velocity.Y = -Velocity.Y;
                    }
                    else if (Rect.Y > maxAxis)
                    {
                        Rect.Y = maxAxis;
                        Velocity.Y = -Velocity.Y;
                    }
                }
            }
        }
    }
}