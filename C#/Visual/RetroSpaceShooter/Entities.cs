using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RetroSpaceShooter;

public static class GameConfig
{
    public const int CanvasWidth = 800;
    public const int CanvasHeight = 700;
}

public static class Projection3D
{
    // Camera position: (0, 160, -260)
    // Camera pitch angle: 16 degrees (0.28 rad)
    // cos(16 deg) = 0.9613f, sin(16 deg) = 0.2756f
    public const float CameraY = 160f;
    public const float CameraZ = -260f;
    public const float CosPitch = 0.9613f;
    public const float SinPitch = 0.2756f;
    public const float Fov = 500f;

    public static PointF Project(float x, float y, float z, out bool visible)
    {
        // Translate relative to camera
        float dx = x;
        float dy = y - CameraY;
        float dz = z - CameraZ;

        // Rotate around X-axis (Pitch)
        float rx = dx;
        float ry = dy * CosPitch + dz * SinPitch;
        float rz = -dy * SinPitch + dz * CosPitch;

        // Clip points too close or behind the camera
        if (rz < 10f)
        {
            visible = false;
            return PointF.Empty;
        }

        visible = true;
        float sx = (GameConfig.CanvasWidth / 2f) + (rx * Fov) / rz;
        float sy = (GameConfig.CanvasHeight * 0.56f) - (ry * Fov) / rz;
        return new PointF(sx, sy);
    }
}

public abstract class GameEntity
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    
    public float SpeedX { get; set; }
    public float SpeedY { get; set; }
    public float SpeedZ { get; set; }
    
    public bool IsActive { get; set; } = true;

    protected GameEntity(float x, float y, float z, float width, float height, float depth)
    {
        X = x;
        Y = y;
        Z = z;
        Width = width;
        Height = height;
        Depth = depth;
    }

    public virtual void Update(float deltaTime)
    {
        X += SpeedX * deltaTime;
        Y += SpeedY * deltaTime;
        Z += SpeedZ * deltaTime;
    }

    public abstract void Draw(Graphics g);

    public bool IntersectsWith(GameEntity other)
    {
        float dx = Math.Abs(X - other.X);
        float dy = Math.Abs(Y - other.Y);
        float dz = Math.Abs(Z - other.Z);

        // Standard 3D collision check
        return dx < (Width + other.Width) / 2f &&
               dy < (Height + other.Height) / 2f &&
               dz < (Depth + other.Depth) / 2f;
    }

    protected void DrawLine3D(Graphics g, Pen pen, float x1, float y1, float z1, float x2, float y2, float z2)
    {
        PointF p1 = Projection3D.Project(x1, y1, z1, out bool v1);
        PointF p2 = Projection3D.Project(x2, y2, z2, out bool v2);
        if (v1 && v2)
        {
            g.DrawLine(pen, p1, p2);
        }
    }
}

public enum WeaponType
{
    Single,
    Double,
    Triple,
    Overcharge
}

public enum PowerUpType
{
    Shield,
    DoubleShot,
    TripleShot,
    Overcharge
}


public class Player : GameEntity
{
    public float MaxHealth { get; } = 100f;
    public float Health { get; set; } = 100f;
    public float MaxShield { get; } = 100f;
    public float Shield { get; set; } = 100f;
    public int Lives { get; set; } = 3;
    
    public WeaponType Weapon { get; set; } = WeaponType.Single;
    public float WeaponTimer { get; set; } = 0f;
    public float FireCooldown { get; set; } = 0f;
    public float BaseFireInterval { get; } = 0.2f;
    
    public bool ShieldJustHit { get; set; } = false;
    public float ShieldHitAnimTimer { get; set; } = 0f;

    public Player(float x, float y, float z) : base(x, y, z, 35f, 15f, 35f)
    {
    }

    public override void Update(float deltaTime)
    {
        if (Weapon != WeaponType.Single)
        {
            WeaponTimer -= deltaTime;
            if (WeaponTimer <= 0)
            {
                Weapon = WeaponType.Single;
            }
        }

        if (FireCooldown > 0)
            FireCooldown -= deltaTime;

        if (ShieldJustHit)
        {
            ShieldHitAnimTimer -= deltaTime;
            if (ShieldHitAnimTimer <= 0)
                ShieldJustHit = false;
        }

        if (Shield < MaxShield && Health > 0)
        {
            Shield = Math.Min(MaxShield, Shield + 8f * deltaTime);
        }

        // 3D Player movement (WASD / Arrows)
        float speed = 250f;
        if (Weapon == WeaponType.Overcharge)
            speed = 360f;

        float dx = 0;
        float dy = 0;

        if (InputManager.IsKeyPressed(Keys.Left) || InputManager.IsKeyPressed(Keys.A))
            dx -= 1;
        if (InputManager.IsKeyPressed(Keys.Right) || InputManager.IsKeyPressed(Keys.D))
            dx += 1;
        if (InputManager.IsKeyPressed(Keys.Up) || InputManager.IsKeyPressed(Keys.W))
            dy += 1; // Move up (altitude)
        if (InputManager.IsKeyPressed(Keys.Down) || InputManager.IsKeyPressed(Keys.S))
            dy -= 1; // Move down (altitude)

        if (dx != 0 && dy != 0)
        {
            dx *= 0.7071f;
            dy *= 0.7071f;
        }

        X += dx * speed * deltaTime;
        Y += dy * speed * deltaTime;

        // Clamp inside 3D bounds
        X = Math.Clamp(X, -280f, 280f);
        Y = Math.Clamp(Y, -40f, 180f); // Ground level is 0
    }

    public void Hit(float damage)
    {
        if (Shield > 0)
        {
            ShieldJustHit = true;
            ShieldHitAnimTimer = 0.15f;
            Shield -= damage;
            if (Shield < 0)
            {
                Health += Shield;
                Shield = 0;
            }
        }
        else
        {
            Health -= damage;
        }

        if (Health <= 0)
        {
            Lives--;
            if (Lives > 0)
            {
                Health = MaxHealth;
                Shield = MaxShield;
                X = 0f;
                Y = 0f;
                Z = -50f;
                Weapon = WeaponType.Single;
            }
        }
    }

    public override void Draw(Graphics g)
    {
        if (Health <= 0) return;

        // 3D local points of Player Ship
        float noseZ = 25f;
        float wingX = 22f;
        float wingY = -6f;
        float wingZ = -15f;
        float tailY = 10f;
        float tailZ = -15f;
        float bodyY = -4f;
        float bodyZ = -15f;

        // Draw wireframe player ship
        using (var pen = new Pen(Color.FromArgb(0, 255, 255), 2f))
        {
            // Nose to wingtips and tail
            DrawLine3D(g, pen, X, Y, Z + noseZ, X - wingX, Y + wingY, Z + wingZ); // nose to left wing
            DrawLine3D(g, pen, X, Y, Z + noseZ, X + wingX, Y + wingY, Z + wingZ); // nose to right wing
            DrawLine3D(g, pen, X, Y, Z + noseZ, X, Y + tailY, Z + tailZ);         // nose to tail top
            DrawLine3D(g, pen, X, Y, Z + noseZ, X, Y + bodyY, Z + bodyZ);         // nose to cockpit base

            // Back connections
            DrawLine3D(g, pen, X - wingX, Y + wingY, Z + wingZ, X, Y + bodyY, Z + bodyZ); // left wing to base
            DrawLine3D(g, pen, X + wingX, Y + wingY, Z + wingZ, X, Y + bodyY, Z + bodyZ); // right wing to base
            DrawLine3D(g, pen, X - wingX, Y + wingY, Z + wingZ, X, Y + tailY, Z + tailZ); // left wing to tail top
            DrawLine3D(g, pen, X + wingX, Y + wingY, Z + wingZ, X, Y + tailY, Z + tailZ); // right wing to tail top
            DrawLine3D(g, pen, X, Y + bodyY, Z + bodyZ, X, Y + tailY, Z + tailZ);         // base to tail top

            // Wings cross line
            DrawLine3D(g, pen, X - wingX, Y + wingY, Z + wingZ, X + wingX, Y + wingY, Z + wingZ);
        }

        // Draw thruster plume in 3D
        var rand = new Random();
        float plumeLength = 12f + (float)rand.NextDouble() * 14f;
        using (var plumePen = new Pen(Color.FromArgb(200, 255, 100, 0), 1.5f))
        {
            DrawLine3D(g, plumePen, X - 6f, Y - 2f, Z + wingZ, X, Y - 2f, Z + wingZ - plumeLength);
            DrawLine3D(g, plumePen, X + 6f, Y - 2f, Z + wingZ, X, Y - 2f, Z + wingZ - plumeLength);
        }

        // Draw 3D holographic shield rings if hit or shield is low
        if (Shield > 0 && (ShieldJustHit || Shield < MaxShield))
        {
            int alpha = ShieldJustHit ? 180 : Math.Clamp((int)(30 + (Shield / MaxShield) * 40), 0, 255);
            using (var shieldPen = new Pen(Color.FromArgb(alpha, 0, 255, 255), 1.5f))
            {
                float radius = Width * 0.85f;
                // Draw 3 orthogonal rings
                DrawCircularRing(g, shieldPen, X, Y, Z, radius, Axis.X);
                DrawCircularRing(g, shieldPen, X, Y, Z, radius, Axis.Y);
                DrawCircularRing(g, shieldPen, X, Y, Z, radius, Axis.Z);
            }
        }
    }

    private enum Axis { X, Y, Z }

    private void DrawCircularRing(Graphics g, Pen pen, float cx, float cy, float cz, float r, Axis axis)
    {
        int segments = 12;
        PointF prevPoint = PointF.Empty;
        bool prevVisible = false;

        for (int i = 0; i <= segments; i++)
        {
            double angle = (i * Math.PI * 2.0) / segments;
            float cos = (float)Math.Cos(angle) * r;
            float sin = (float)Math.Sin(angle) * r;

            float px = cx, py = cy, pz = cz;
            if (axis == Axis.X)
            {
                py += cos;
                pz += sin;
            }
            else if (axis == Axis.Y)
            {
                px += cos;
                pz += sin;
            }
            else
            {
                px += cos;
                py += sin;
            }

            PointF proj = Projection3D.Project(px, py, pz, out bool visible);
            if (i > 0 && prevVisible && visible)
            {
                g.DrawLine(pen, prevPoint, proj);
            }
            prevPoint = proj;
            prevVisible = visible;
        }
    }
}

public enum EnemyType
{
    Scout,
    Fighter,
    Bomber,
    Boss
}

public class Enemy : GameEntity
{
    public EnemyType Type { get; }
    public float Health { get; set; }
    public float MaxHealth { get; }
    public int ScoreValue { get; }
    
    private float _shootCooldown;
    private float _shootInterval;
    private float _timeActive = 0f;
    private float _startX;
    private float _startY;

    public Enemy(EnemyType type, float x, float y, float z) : base(x, y, z, 30f, 20f, 30f)
    {
        Type = type;
        _startX = x;
        _startY = y;
        var rand = new Random();

        switch (type)
        {
            case EnemyType.Scout:
                Width = 26f;
                Height = 26f;
                Depth = 26f;
                MaxHealth = 15f;
                Health = 15f;
                SpeedZ = -140f; // Moves towards camera
                SpeedY = 0f;
                SpeedX = 0f;
                ScoreValue = 100;
                _shootInterval = 1.6f + (float)rand.NextDouble() * 1.6f;
                _shootCooldown = (float)rand.NextDouble() * _shootInterval;
                break;

            case EnemyType.Fighter:
                Width = 34f;
                Height = 15f;
                Depth = 34f;
                MaxHealth = 35f;
                Health = 35f;
                SpeedZ = -180f;
                SpeedY = 0f;
                SpeedX = 0f;
                ScoreValue = 250;
                _shootInterval = 1.2f + (float)rand.NextDouble() * 1.2f;
                _shootCooldown = (float)rand.NextDouble() * _shootInterval;
                break;

            case EnemyType.Bomber:
                Width = 42f;
                Height = 35f;
                Depth = 42f;
                MaxHealth = 80f;
                Health = 80f;
                SpeedZ = -90f;
                SpeedY = 0f;
                SpeedX = 0f;
                ScoreValue = 450;
                _shootInterval = 2.0f + (float)rand.NextDouble() * 1.0f;
                _shootCooldown = (float)rand.NextDouble() * _shootInterval;
                break;

            case EnemyType.Boss:
                Width = 140f;
                Height = 70f;
                Depth = 120f;
                MaxHealth = 1200f;
                Health = 1200f;
                SpeedZ = -60f; // Slowly flies forward
                SpeedX = 80f;
                ScoreValue = 3000;
                _shootInterval = 0.7f;
                _shootCooldown = 1.2f;
                break;
        }
    }

    public override void Update(float deltaTime)
    {
        _timeActive += deltaTime;

        if (Type == EnemyType.Boss)
        {
            // Hover at Z=650 and slide left/right, slowly moving Z forward if too far back
            if (Z > 650f)
            {
                Z += SpeedZ * deltaTime;
            }
            else
            {
                X += SpeedX * deltaTime;
                if (X < -220f || X > 220f)
                {
                    SpeedX = -SpeedX;
                    X = Math.Clamp(X, -220f, 220f);
                }
                // Hover sinusoidal Y oscillation
                Y = _startY + (float)Math.Sin(_timeActive * 2.0f) * 30f;
            }
        }
        else
        {
            if (Type == EnemyType.Fighter)
            {
                // Sine wave horizontal drift + altitude swoop
                X = _startX + (float)Math.Sin(_timeActive * 3.5f) * 90f;
                Y = _startY + (float)Math.Cos(_timeActive * 2.5f) * 40f;
                Z += SpeedZ * deltaTime;
            }
            else
            {
                base.Update(deltaTime);
            }
        }

        // Deactivate when behind the camera/player
        if (Type != EnemyType.Boss && Z < -150f)
        {
            IsActive = false;
        }
    }

    public bool ShouldShoot(float deltaTime, out List<Laser> spawnedLasers)
    {
        spawnedLasers = new List<Laser>();
        if (!IsActive || Health <= 0) return false;

        // Don't shoot if way in the background or past the player
        if (Z > 1100f || Z < 50f) return false;

        _shootCooldown -= deltaTime;
        if (_shootCooldown <= 0)
        {
            _shootCooldown = _shootInterval;

            switch (Type)
            {
                case EnemyType.Scout:
                    spawnedLasers.Add(new Laser(X, Y, Z, 0f, 0f, -500f, isPlayerOwned: false, damage: 10f));
                    break;

                case EnemyType.Fighter:
                    spawnedLasers.Add(new Laser(X - 12f, Y, Z, -40f, 0f, -550f, isPlayerOwned: false, damage: 12f));
                    spawnedLasers.Add(new Laser(X + 12f, Y, Z, 40f, 0f, -550f, isPlayerOwned: false, damage: 12f));
                    break;

                case EnemyType.Bomber:
                    spawnedLasers.Add(new Laser(X, Y, Z, 0f, -50f, -380f, isPlayerOwned: false, damage: 25f, isPlasma: true));
                    break;

                case EnemyType.Boss:
                    float healthPct = Health / MaxHealth;
                    if (healthPct > 0.6f)
                    {
                        // Twin heavy shots
                        spawnedLasers.Add(new Laser(X - 35f, Y, Z - 20f, -50f, 0f, -500f, isPlayerOwned: false, damage: 15f));
                        spawnedLasers.Add(new Laser(X + 35f, Y, Z - 20f, 50f, 0f, -500f, isPlayerOwned: false, damage: 15f));
                    }
                    else if (healthPct > 0.3f)
                    {
                        // 4-spread shot
                        for (int i = 0; i < 4; i++)
                        {
                            float vx = (i - 1.5f) * 70f;
                            float vy = (float)(Math.Sin(i * 1.5) * 30f);
                            spawnedLasers.Add(new Laser(X, Y, Z - 10f, vx, vy, -480f, isPlayerOwned: false, damage: 15f));
                        }
                    }
                    else
                    {
                        // Phase 3: Spiral rings
                        _shootCooldown = _shootInterval * 1.5f;
                        for (int i = 0; i < 6; i++)
                        {
                            double angle = i * Math.PI / 3.0;
                            float vx = (float)(Math.Cos(angle) * 120f);
                            float vy = (float)(Math.Sin(angle) * 120f);
                            spawnedLasers.Add(new Laser(X, Y, Z, vx, vy, -420f, isPlayerOwned: false, damage: 15f));
                        }
                    }
                    break;
            }
            return true;
        }

        return false;
    }

    public override void Draw(Graphics g)
    {
        if (Health <= 0) return;

        var rand = new Random();
        if (Type == EnemyType.Boss)
        {
            // Draw 3D wireframe Boss Cruiser
            float length = Depth / 2f;
            float width = Width / 2f;
            float height = Height / 2f;

            Color neonPink = Color.FromArgb(255, 0, 128 + rand.Next(127));
            using (var pen = new Pen(neonPink, 2.5f))
            {
                // Nose
                float nz = length;
                // Wings back corners
                float wx = width;
                float wy = -height * 0.5f;
                float wz = -length * 0.8f;
                // Top fin
                float tx = 0f;
                float ty = height;
                float tz = -length * 0.4f;
                // Bottom hull
                float bx = 0f;
                float by = -height;
                float bz = -length * 0.4f;
                // Rear plate
                float rx = width * 0.6f;
                float ry = height * 0.5f;
                float rz = -length;

                // Nose to points
                DrawLine3D(g, pen, X, Y, Z + nz, X - wx, Y + wy, Z + wz);
                DrawLine3D(g, pen, X, Y, Z + nz, X + wx, Y + wy, Z + wz);
                DrawLine3D(g, pen, X, Y, Z + nz, X + tx, Y + ty, Z + tz);
                DrawLine3D(g, pen, X, Y, Z + nz, X + bx, Y + by, Z + bz);

                // Body structure
                DrawLine3D(g, pen, X - wx, Y + wy, Z + wz, X + tx, Y + ty, Z + tz);
                DrawLine3D(g, pen, X + wx, Y + wy, Z + wz, X + tx, Y + ty, Z + tz);
                DrawLine3D(g, pen, X - wx, Y + wy, Z + wz, X + bx, Y + by, Z + bz);
                DrawLine3D(g, pen, X + wx, Y + wy, Z + wz, X + bx, Y + by, Z + bz);

                // Back engine bulkhead box
                DrawLine3D(g, pen, X - rx, Y + ry, Z + rz, X + rx, Y + ry, Z + rz);
                DrawLine3D(g, pen, X - rx, Y - ry, Z + rz, X + rx, Y - ry, Z + rz);
                DrawLine3D(g, pen, X - rx, Y + ry, Z + rz, X - rx, Y - ry, Z + rz);
                DrawLine3D(g, pen, X + rx, Y + ry, Z + rz, X + rx, Y - ry, Z + rz);

                // Connect wings to back bulkhead
                DrawLine3D(g, pen, X - wx, Y + wy, Z + wz, X - rx, Y - ry, Z + rz);
                DrawLine3D(g, pen, X + wx, Y + wy, Z + wz, X + rx, Y - ry, Z + rz);
                DrawLine3D(g, pen, X + tx, Y + ty, Z + tz, X, Y + ry, Z + rz);
                DrawLine3D(g, pen, X + bx, Y + by, Z + bz, X, Y - ry, Z + rz);
            }

            // Draw glowing core inside Boss
            using (var corePen = new Pen(Color.FromArgb(200, 255, 255, 0), 2f))
            {
                // Simple 3D diamond inside
                float cs = 16f;
                DrawLine3D(g, corePen, X, Y + cs, Z, X - cs, Y, Z);
                DrawLine3D(g, corePen, X, Y + cs, Z, X + cs, Y, Z);
                DrawLine3D(g, corePen, X, Y - cs, Z, X - cs, Y, Z);
                DrawLine3D(g, corePen, X, Y - cs, Z, X + cs, Y, Z);
                DrawLine3D(g, corePen, X, Y, Z - cs, X - cs, Y, Z);
                DrawLine3D(g, corePen, X, Y, Z - cs, X + cs, Y, Z);
                DrawLine3D(g, corePen, X, Y, Z + cs, X - cs, Y, Z);
                DrawLine3D(g, corePen, X, Y, Z + cs, X + cs, Y, Z);
            }

            // Draw boss health bar (3D projected above boss center)
            PointF barCenter = Projection3D.Project(X, Y + height + 35f, Z, out bool bVis);
            if (bVis)
            {
                float distFactor = Math.Max(0.1f, Z - Projection3D.CameraZ);
                float barW = 22000f / distFactor; // Scales with distance
                float barH = 5f;
                float barX = barCenter.X - barW / 2f;
                float barY = barCenter.Y;

                using (var bgBrush = new SolidBrush(Color.FromArgb(80, 50, 50, 50)))
                {
                    g.FillRectangle(bgBrush, barX, barY, barW, barH);
                }
                float healthRatio = Health / MaxHealth;
                using (var healthBrush = new SolidBrush(Color.Red))
                {
                    g.FillRectangle(healthBrush, barX, barY, barW * healthRatio, barH);
                }
                using (var borderPen = new Pen(Color.White, 1f))
                {
                    g.DrawRectangle(borderPen, barX, barY, barW, barH);
                }
            }
        }
        else
        {
            Color primaryColor = Color.Red;
            switch (Type)
            {
                case EnemyType.Scout:
                    primaryColor = Color.FromArgb(255, 80, 80);
                    using (var pen = new Pen(primaryColor, 1.5f))
                    {
                        float r = Width / 2f;
                        // Octahedron wireframe
                        DrawLine3D(g, pen, X, Y + r, Z, X - r, Y, Z);
                        DrawLine3D(g, pen, X, Y + r, Z, X + r, Y, Z);
                        DrawLine3D(g, pen, X, Y - r, Z, X - r, Y, Z);
                        DrawLine3D(g, pen, X, Y - r, Z, X + r, Y, Z);

                        DrawLine3D(g, pen, X, Y + r, Z, X, Y, Z + r);
                        DrawLine3D(g, pen, X, Y + r, Z, X, Y, Z - r);
                        DrawLine3D(g, pen, X, Y - r, Z, X, Y, Z + r);
                        DrawLine3D(g, pen, X, Y - r, Z, X, Y, Z - r);

                        DrawLine3D(g, pen, X - r, Y, Z, X, Y, Z + r);
                        DrawLine3D(g, pen, X - r, Y, Z, X, Y, Z - r);
                        DrawLine3D(g, pen, X + r, Y, Z, X, Y, Z + r);
                        DrawLine3D(g, pen, X + r, Y, Z, X, Y, Z - r);
                    }
                    break;

                case EnemyType.Fighter:
                    primaryColor = Color.FromArgb(255, 0, 150);
                    using (var pen = new Pen(primaryColor, 1.8f))
                    {
                        // Sleek delta fighter wireframe
                        float nx = 0f, ny = 0f, nz = Height;
                        float wx = Width / 2f, wy = -3f, wz = -Depth / 2f;
                        float ty = Height, tz = -Depth / 3f;
                        float cx = 0f, cy = -4f, cz = -Depth / 2f;

                        DrawLine3D(g, pen, X + nx, Y + ny, Z + nz, X - wx, Y + wy, Z + wz); // nose to left wing
                        DrawLine3D(g, pen, X + nx, Y + ny, Z + nz, X + wx, Y + wy, Z + wz); // nose to right wing
                        DrawLine3D(g, pen, X + nx, Y + ny, Z + nz, X + cx, Y + cy, Z + cz); // nose to center back
                        DrawLine3D(g, pen, X + nx, Y + ny, Z + nz, X, Y + ty, Z + tz);      // nose to tail

                        DrawLine3D(g, pen, X - wx, Y + wy, Z + wz, X + cx, Y + cy, Z + cz);
                        DrawLine3D(g, pen, X + wx, Y + wy, Z + wz, X + cx, Y + cy, Z + cz);
                        DrawLine3D(g, pen, X - wx, Y + wy, Z + wz, X, Y + ty, Z + tz);
                        DrawLine3D(g, pen, X + wx, Y + wy, Z + wz, X, Y + ty, Z + tz);
                        DrawLine3D(g, pen, X + cx, Y + cy, Z + cz, X, Y + ty, Z + tz);
                    }
                    break;

                case EnemyType.Bomber:
                default:
                    primaryColor = Color.FromArgb(200, 0, 200);
                    using (var pen = new Pen(primaryColor, 2f))
                    {
                        // 3D Hexagonal prism or double-box wireframe
                        float w = Width / 2f;
                        float h = Height / 2f;
                        float d = Depth / 2f;

                        // Front Face
                        DrawLine3D(g, pen, X - w, Y - h, Z + d, X + w, Y - h, Z + d);
                        DrawLine3D(g, pen, X + w, Y - h, Z + d, X + w, Y + h, Z + d);
                        DrawLine3D(g, pen, X + w, Y + h, Z + d, X - w, Y + h, Z + d);
                        DrawLine3D(g, pen, X - w, Y + h, Z + d, X - w, Y - h, Z + d);

                        // Back Face
                        DrawLine3D(g, pen, X - w, Y - h, Z - d, X + w, Y - h, Z - d);
                        DrawLine3D(g, pen, X + w, Y - h, Z - d, X + w, Y + h, Z - d);
                        DrawLine3D(g, pen, X + w, Y + h, Z - d, X - w, Y + h, Z - d);
                        DrawLine3D(g, pen, X - w, Y + h, Z - d, X - w, Y - h, Z - d);

                        // Connectors
                        DrawLine3D(g, pen, X - w, Y - h, Z + d, X - w, Y - h, Z - d);
                        DrawLine3D(g, pen, X + w, Y - h, Z + d, X + w, Y - h, Z - d);
                        DrawLine3D(g, pen, X + w, Y + h, Z + d, X + w, Y + h, Z - d);
                        DrawLine3D(g, pen, X - w, Y + h, Z + d, X - w, Y + h, Z - d);
                    }
                    break;
            }
        }
    }
}

public class Laser : GameEntity
{
    public bool IsPlayerOwned { get; }
    public float Damage { get; }
    public bool IsPlasma { get; }
    private readonly Random _rand = new();

    public Laser(float x, float y, float z, float speedX, float speedY, float speedZ, bool isPlayerOwned, float damage, bool isPlasma = false) 
        : base(x, y, z, isPlasma ? 16f : 4f, isPlasma ? 16f : 4f, isPlasma ? 20f : 30f)
    {
        SpeedX = speedX;
        SpeedY = speedY;
        SpeedZ = speedZ;
        IsPlayerOwned = isPlayerOwned;
        Damage = damage;
        IsPlasma = isPlasma;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Deactivate if out of range
        if (Z < -150f || Z > 1400f || Math.Abs(X) > 600f || Y < -150f || Y > 400f)
        {
            IsActive = false;
        }
    }

    public override void Draw(Graphics g)
    {
        if (IsPlasma)
        {
            // Draw a spinning 3D plasma ball (a wireframe octahedron)
            Color plasmaColor = Color.FromArgb(255, 120 + _rand.Next(100), 0);
            using (var pen = new Pen(plasmaColor, 2f))
            {
                float r = Width / 2f;
                DrawLine3D(g, pen, X, Y + r, Z, X - r, Y, Z);
                DrawLine3D(g, pen, X, Y + r, Z, X + r, Y, Z);
                DrawLine3D(g, pen, X, Y - r, Z, X - r, Y, Z);
                DrawLine3D(g, pen, X, Y - r, Z, X + r, Y, Z);

                DrawLine3D(g, pen, X, Y + r, Z, X, Y, Z + r);
                DrawLine3D(g, pen, X, Y + r, Z, X, Y, Z - r);
                DrawLine3D(g, pen, X, Y - r, Z, X, Y, Z + r);
                DrawLine3D(g, pen, X, Y - r, Z, X, Y, Z - r);
            }
        }
        else
        {
            Color c = IsPlayerOwned ? Color.FromArgb(0, 255, 255) : Color.FromArgb(255, 0, 100);
            using (var pen = new Pen(c, 2.5f))
            {
                // Draw a 3D line from past position to current position
                float trailZ = Z - (SpeedZ * 0.02f);
                DrawLine3D(g, pen, X, Y, Z, X - (SpeedX * 0.02f), Y - (SpeedY * 0.02f), trailZ);
            }
        }
    }
}

public class PowerUp : GameEntity
{
    public PowerUpType Type { get; }
    private float _rotation = 0f;

    public PowerUp(PowerUpType type, float x, float y, float z) : base(x, y, z, 24f, 24f, 24f)
    {
        Type = type;
        SpeedX = 0f;
        SpeedY = -20f; // Sinks slightly
        SpeedZ = -100f; // Drifts towards player
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        _rotation += deltaTime * 2.5f;

        if (Z < -150f)
        {
            IsActive = false;
        }
    }

    public override void Draw(Graphics g)
    {
        Color primaryColor = Color.Yellow;
        string symbol = "?";

        switch (Type)
        {
            case PowerUpType.Shield:
                primaryColor = Color.FromArgb(0, 255, 255);
                symbol = "S";
                break;
            case PowerUpType.DoubleShot:
                primaryColor = Color.FromArgb(0, 255, 0);
                symbol = "D";
                break;
            case PowerUpType.TripleShot:
                primaryColor = Color.FromArgb(255, 0, 255);
                symbol = "T";
                break;
            case PowerUpType.Overcharge:
                primaryColor = Color.FromArgb(255, 165, 0);
                symbol = "O";
                break;
        }

        // Draw spinning 3D wireframe diamond (gem)
        using (var pen = new Pen(primaryColor, 1.8f))
        {
            float r = Width / 2f;
            float cos = (float)Math.Cos(_rotation) * r;
            float sin = (float)Math.Sin(_rotation) * r;

            // Rotating corners
            float x1 = X + cos, z1 = Z + sin;
            float x2 = X - cos, z2 = Z - sin;
            float x3 = X - sin, z3 = Z + cos;
            float x4 = X + sin, z4 = Z - cos;

            float ty = Y + r;
            float by = Y - r;

            // Connect top
            DrawLine3D(g, pen, x1, Y, z1, X, ty, Z);
            DrawLine3D(g, pen, x2, Y, z2, X, ty, Z);
            DrawLine3D(g, pen, x3, Y, z3, X, ty, Z);
            DrawLine3D(g, pen, x4, Y, z4, X, ty, Z);

            // Connect bottom
            DrawLine3D(g, pen, x1, Y, z1, X, by, Z);
            DrawLine3D(g, pen, x2, Y, z2, X, by, Z);
            DrawLine3D(g, pen, x3, Y, z3, X, by, Z);
            DrawLine3D(g, pen, x4, Y, z4, X, by, Z);

            // Connect middle loop
            DrawLine3D(g, pen, x1, Y, z1, x3, Y, z3);
            DrawLine3D(g, pen, x3, Y, z3, x2, Y, z2);
            DrawLine3D(g, pen, x2, Y, z2, x4, Y, z4);
            DrawLine3D(g, pen, x4, Y, z4, x1, Y, z1);
        }

        // Draw symbol projected at center
        PointF screenPos = Projection3D.Project(X, Y, Z, out bool visible);
        if (visible)
        {
            using (var font = new Font("Courier New", 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(primaryColor))
            {
                var size = g.MeasureString(symbol, font);
                g.DrawString(symbol, font, brush, screenPos.X - size.Width / 2f, screenPos.Y - size.Height / 2f);
            }
        }
    }
}

public class Particle : GameEntity
{
    public Color Color { get; }
    public float MaxLifespan { get; }
    public float Lifespan { get; set; }
    public float Friction { get; }

    public Particle(float x, float y, float z, float speedX, float speedY, float speedZ, Color color, float lifespan, float friction = 0.98f) 
        : base(x, y, z, 3f, 3f, 3f)
    {
        SpeedX = speedX;
        SpeedY = speedY;
        SpeedZ = speedZ;
        Color = color;
        MaxLifespan = lifespan;
        Lifespan = lifespan;
        Friction = friction;
    }

    public override void Update(float deltaTime)
    {
        Lifespan -= deltaTime;
        if (Lifespan <= 0)
        {
            IsActive = false;
            return;
        }

        SpeedX *= Friction;
        SpeedY *= Friction;
        SpeedZ *= Friction;
        base.Update(deltaTime);
    }

    public override void Draw(Graphics g)
    {
        PointF screenPos = Projection3D.Project(X, Y, Z, out bool visible);
        if (visible)
        {
            float ratio = Lifespan / MaxLifespan;
            int alpha = Math.Clamp((int)(255 * ratio), 0, 255);
            
            // Scale size with distance
            float distanceZ = Math.Max(10f, Z - Projection3D.CameraZ);
            float size = Math.Max(1f, 1500f / distanceZ);

            using (var brush = new SolidBrush(Color.FromArgb(alpha, Color)))
            {
                g.FillRectangle(brush, screenPos.X - size / 2f, screenPos.Y - size / 2f, size, size);
            }
        }
    }
}
