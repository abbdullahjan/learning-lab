using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CyberStrike;

public class GameCanvas : Control
{
    private readonly GameEngine _engine;
    private Bitmap? _floorBitmap;
    private Graphics? _floorGraphics;
    
    // UI Pulse details
    private float _glowPulse = 0f;
    private bool _pulseDir = true;

    // Font cache
    private readonly Font _titleFont = new("Segoe UI", 36f, FontStyle.Bold | FontStyle.Italic);
    private readonly Font _hudHeaderFont = new("Courier New", 11f, FontStyle.Bold);
    private readonly Font _hudBodyFont = new("Courier New", 13f, FontStyle.Bold);
    private readonly Font _ammoFont = new("Impact", 18f);
    private readonly Font _msgFont = new("Courier New", 14f, FontStyle.Bold);

    public GameCanvas(GameEngine engine)
    {
        _engine = engine;
        DoubleBuffered = true;
        Size = new Size((int)engine.ArenaWidth, (int)engine.ArenaHeight + 100); // 900x800
        BackColor = Color.FromArgb(15, 15, 18);

        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.Selectable, true);

        InitializeFloor();

        // Bind callback for permanent blood splatters
        _engine.OnSpawnPermanentSplatter += AddPermanentBloodSplatter;
    }

    private void InitializeFloor()
    {
        _floorBitmap = new Bitmap((int)_engine.ArenaWidth, (int)_engine.ArenaHeight);
        _floorGraphics = Graphics.FromImage(_floorBitmap);
        _floorGraphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Fill background with slate metal theme
        _floorGraphics.Clear(Color.FromArgb(20, 24, 28));

        // Draw tiling lines (50x50 tile grid)
        using var gridPen = new Pen(Color.FromArgb(28, 32, 38), 1.5f);
        int tileSize = 60;
        for (int x = 0; x < _engine.ArenaWidth; x += tileSize)
        {
            _floorGraphics.DrawLine(gridPen, x, 0, x, _engine.ArenaHeight);
        }
        for (int y = 0; y < _engine.ArenaHeight; y += tileSize)
        {
            _floorGraphics.DrawLine(gridPen, 0, y, _engine.ArenaWidth, y);
        }

        // Add concrete surface noise & imperfections
        var rand = new Random();
        using var noiseBrush = new SolidBrush(Color.FromArgb(8, 255, 255, 255));
        using var scratchPen = new Pen(Color.FromArgb(6, 0, 0, 0), 1f);

        for (int i = 0; i < 180; i++)
        {
            int rx = rand.Next((int)_engine.ArenaWidth);
            int ry = rand.Next((int)_engine.ArenaHeight);
            int size = rand.Next(2, 6);
            _floorGraphics.FillEllipse(noiseBrush, rx, ry, size, size);
        }

        for (int i = 0; i < 40; i++)
        {
            int rx = rand.Next((int)_engine.ArenaWidth);
            int ry = rand.Next((int)_engine.ArenaHeight);
            int length = rand.Next(15, 45);
            float angle = (float)(rand.NextDouble() * Math.PI * 2);
            _floorGraphics.DrawLine(scratchPen, rx, ry, rx + (float)Math.Cos(angle) * length, ry + (float)Math.Sin(angle) * length);
        }
    }

    private void AddPermanentBloodSplatter(float x, float y, Color color, float size)
    {
        if (_floorGraphics == null || _floorBitmap == null) return;

        // Draw permanent blood stain on background
        using var brush = new SolidBrush(Color.FromArgb(170, color.R, color.G, color.B));
        _floorGraphics.FillEllipse(brush, x - size / 2f, y - size / 2f, size, size);

        // Sometimes draw tiny splatter drops nearby
        var rand = new Random();
        if (rand.Next(3) == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                float ox = (float)((rand.NextDouble() - 0.5) * size * 2f);
                float oy = (float)((rand.NextDouble() - 0.5) * size * 2f);
                float osize = size * (0.2f + (float)rand.NextDouble() * 0.3f);
                _floorGraphics.FillEllipse(brush, x + ox - osize / 2f, y + oy - osize / 2f, osize, osize);
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        // Update Pulse Timer
        UpdateGlowPulse();

        // 1. Draw World with Screen Shake and translated arena offset (down by 35px)
        var state = g.Save();
        g.TranslateTransform(_engine.ShakeX, _engine.ShakeY + 35f);

        // Draw tiled floor with blood stains
        if (_floorBitmap != null)
        {
            g.DrawImage(_floorBitmap, 0, 0);
        }

        // Draw Weapon Crates
        DrawWeaponCrates(g);

        // Draw Debris
        DrawDebris(g);

        // Draw Barrels
        DrawBarrels(g);

        // Draw Players
        DrawPlayers(g);

        // Draw Bullets
        DrawBullets(g);

        // Draw Grenades
        DrawGrenades(g);

        // Draw Particles
        DrawParticles(g);

        // Draw Walls
        DrawWalls(g);

        // 2. Draw Fog of War (Raycasted sight cones)
        DrawFogOfWar(g);

        // 3. Draw Sound Waves (drawn after Fog of War so they slice through the dark)
        DrawSoundWaves(g);

        // 4. Draw Vignettes (Low Health) inside translated arena
        DrawLowHealthVignettes(g);

        g.Restore(state);

        // 5. Draw Custom Futuristic Titlebar Header at the top
        DrawCustomHeader(g);

        // 6. Draw HUD Panel at the bottom
        DrawHUDPanel(g);

        // 7. Draw Overlay Screens (Victory / Game Over)
        DrawOverlayScreens(g);
    }

    private void UpdateGlowPulse()
    {
        if (_pulseDir)
        {
            _glowPulse += 0.03f;
            if (_glowPulse >= 1f) _pulseDir = false;
        }
        else
        {
            _glowPulse -= 0.03f;
            if (_glowPulse <= 0f) _pulseDir = true;
        }
    }

    private void DrawWalls(Graphics g)
    {
        foreach (var w in _engine.Walls)
        {
            // Draw wall shadows/ambient occlusion outline
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, w.Rect.X - 4, w.Rect.Y - 4, w.Rect.Width + 8, w.Rect.Height + 8);
            }

            // Fill wall block (metallic look)
            using (var wallBrush = new LinearGradientBrush(w.Rect, Color.FromArgb(55, 60, 65), Color.FromArgb(30, 32, 35), 45f))
            {
                g.FillRectangle(wallBrush, w.Rect);
            }

            // Draw inner panel lines to look structural
            using (var borderPen = new Pen(Color.FromArgb(75, 82, 90), 1.5f))
            using (var darkPen = new Pen(Color.FromArgb(15, 18, 20), 1.5f))
            {
                g.DrawRectangle(borderPen, w.Rect.X, w.Rect.Y, w.Rect.Width, w.Rect.Height);
                g.DrawRectangle(darkPen, w.Rect.X + 2, w.Rect.Y + 2, w.Rect.Width - 4, w.Rect.Height - 4);
            }
        }
    }

    private void DrawBarrels(Graphics g)
    {
        foreach (var b in _engine.Barrels)
        {
            if (!b.IsActive) continue;

            float r = b.Radius;
            var rect = new RectangleF(b.X - r, b.Y - r, r * 2, r * 2);

            // Shadow
            using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, b.X - r - 2, b.Y - r - 2, r * 2 + 4, r * 2 + 4);
            }

            // Barrel Body (Red/Orange fuel theme)
            using (var barrelBrush = new PathGradientBrush(GetCirclePath(b.X, b.Y, r)))
            {
                barrelBrush.CenterPoint = new PointF(b.X - r * 0.3f, b.Y - r * 0.3f);
                barrelBrush.CenterColor = Color.FromArgb(255, 120, 120);
                barrelBrush.SurroundColors = new Color[] { Color.FromArgb(180, 20, 20) };
                g.FillEllipse(barrelBrush, rect);
            }

            // Details: Steel straps & yellow hazard label
            using (var strapPen = new Pen(Color.FromArgb(35, 35, 35), 2f))
            using (var hazardBrush = new SolidBrush(Color.Gold))
            using (var borderPen = new Pen(Color.FromArgb(40, 0, 0), 1f))
            {
                // Metal rims
                g.DrawEllipse(strapPen, rect);
                g.DrawEllipse(borderPen, b.X - r + 3, b.Y - r + 3, r * 2 - 6, r * 2 - 6);

                // Hazard symbol (Yellow diamond)
                PointF[] points = {
                    new PointF(b.X, b.Y - 6),
                    new PointF(b.X + 6, b.Y),
                    new PointF(b.X, b.Y + 6),
                    new PointF(b.X - 6, b.Y)
                };
                g.FillPolygon(hazardBrush, points);
            }

            // Draw damage cracks if health is low
            if (b.Health < b.MaxHealth)
            {
                float damagePct = b.Health / b.MaxHealth;
                int cracks = (int)((1f - damagePct) * 5);
                var rand = new Random((int)b.X); // stable seed for barrel cracks
                using var crackPen = new Pen(Color.FromArgb(180, 0, 0, 0), 1.5f);
                for (int i = 0; i < cracks; i++)
                {
                    float angle = (float)(rand.NextDouble() * Math.PI * 2);
                    float length = r * (0.3f + (float)rand.NextDouble() * 0.5f);
                    g.DrawLine(crackPen, b.X, b.Y, b.X + (float)Math.Cos(angle) * length, b.Y + (float)Math.Sin(angle) * length);
                }
            }
        }
    }

    private void DrawWeaponCrates(Graphics g)
    {
        foreach (var c in _engine.WeaponCrates)
        {
            if (c.IsActive)
            {
                // Glow effect around active crate
                int alpha = (int)(40 + _glowPulse * 40);
                using (var glowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 255, 100)))
                {
                    g.FillRectangle(glowBrush, c.X - c.Width / 2f - 4, c.Y - c.Height / 2f - 4, c.Width + 8, c.Height + 8);
                }

                // Crate base (military green/orange)
                var crateRect = c.Rect;
                using (var crateBrush = new LinearGradientBrush(crateRect, Color.FromArgb(70, 95, 60), Color.FromArgb(40, 55, 35), 45f))
                {
                    g.FillRectangle(crateBrush, crateRect);
                }

                // Yellow metal brackets on corners & diagonals
                using (var borderPen = new Pen(Color.FromArgb(200, 180, 50), 2f))
                using (var textFont = new Font("Courier New", 8f, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.Gold))
                {
                    g.DrawRectangle(borderPen, crateRect.X, crateRect.Y, crateRect.Width, crateRect.Height);
                    // Cross planks
                    g.DrawLine(borderPen, crateRect.X, crateRect.Y, crateRect.Right, crateRect.Bottom);

                    // Text display of weapon type
                    string wpCode = c.SpawnedWeapon switch
                    {
                        WeaponType.AssaultRifle => "AR",
                        WeaponType.Shotgun => "SG",
                        WeaponType.Sniper => "SR",
                        _ => "??"
                    };
                    var size = g.MeasureString(wpCode, textFont);
                    g.FillRectangle(Brushes.Black, c.X - size.Width / 2f - 1, c.Y - size.Height / 2f - 1, size.Width + 2, size.Height + 2);
                    g.DrawString(wpCode, textFont, textBrush, c.X - size.Width / 2f, c.Y - size.Height / 2f);
                }
            }
            else
            {
                // Draw inactive/wireframe crate (transparent respawn look)
                var crateRect = c.Rect;
                using (var wirePen = new Pen(Color.FromArgb(50, 255, 255, 255), 1f))
                {
                    wirePen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(wirePen, crateRect.X, crateRect.Y, crateRect.Width, crateRect.Height);
                    g.DrawLine(wirePen, crateRect.X, crateRect.Y, crateRect.Right, crateRect.Bottom);
                }

                // Respawn progress timer circular outline
                float progress = 1f - (c.RespawnTimer / 12f);
                using (var progressPen = new Pen(Color.FromArgb(120, 0, 255, 100), 2f))
                {
                    g.DrawArc(progressPen, c.X - 10, c.Y - 10, 20, 20, -90, progress * 360f);
                }
            }
        }
    }

    private void DrawPlayers(Graphics g)
    {
        DrawSinglePlayer(g, _engine.Player1);
        DrawSinglePlayer(g, _engine.Player2);
    }

    private void DrawSinglePlayer(Graphics g, Player p)
    {
        if (p.IsDead) return;

        float r = p.Radius;

        // 1. Draw Aiming Laser Pointer (Tactical red/blue thin dotted line)
        float laserLen = 300f;
        float lx = p.X + (float)Math.Cos(p.Angle) * laserLen;
        float ly = p.Y + (float)Math.Sin(p.Angle) * laserLen;
        using (var laserPen = new Pen(Color.FromArgb(100, p.PlayerColor), 1f))
        {
            laserPen.DashStyle = DashStyle.Dot;
            g.DrawLine(laserPen, p.X, p.Y, lx, ly);
        }

        // 2. Gun Model Draw (rotating with Player Angle)
        var state = g.Save();
        g.TranslateTransform(p.X, p.Y);
        g.RotateTransform(p.Angle * 180f / (float)Math.PI);

        // Gun dimensions based on Weapon Class
        float barrelW = 24f;
        float barrelH = 6f;
        Color weaponColor = Color.FromArgb(70, 75, 80);

        if (p.Weapon == WeaponType.Shotgun)
        {
            barrelW = 20f;
            barrelH = 10f; // Fat dual-barrel
        }
        else if (p.Weapon == WeaponType.Sniper)
        {
            barrelW = 34f;
            barrelH = 4f; // Long barrel with scope block
        }

        // Draw Gun Barrel
        using (var gunBrush = new SolidBrush(weaponColor))
        using (var gunPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
        {
            g.FillRectangle(gunBrush, 0, -barrelH / 2f, barrelW, barrelH);
            g.DrawRectangle(gunPen, 0, -barrelH / 2f, barrelW, barrelH);

            if (p.Weapon == WeaponType.Sniper)
            {
                // Draw Scope block on sniper
                g.FillRectangle(Brushes.Black, 8, -barrelH / 2f - 4, 8, 4);
            }
        }

        // Muzzle Flash visual
        if (p.FireCooldown > 0 && p.FireCooldown >= WeaponStats.GetStats(p.Weapon).FireInterval - 0.05f)
        {
            using (var flashBrush = new SolidBrush(Color.FromArgb(220, 255, 200, 0)))
            {
                PointF[] points = {
                    new PointF(barrelW, -8),
                    new PointF(barrelW + 18, 0),
                    new PointF(barrelW, 8)
                };
                g.FillPolygon(flashBrush, points);
            }
        }

        // Knife Melee Slash animation (glowing slash arc in front)
        if (p.MeleeVisualTimer > 0f)
        {
            float sweepAngle = 120f;
            float startAngle = -sweepAngle / 2f;
            float slashRadius = r + 24f;
            int alpha = (int)(255 * (p.MeleeVisualTimer / 0.15f));
            using (var meleePen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 3f))
            {
                g.DrawArc(meleePen, -slashRadius, -slashRadius, slashRadius * 2, slashRadius * 2, startAngle, sweepAngle);
            }
        }

        g.Restore(state);

        // 3. Draw Player Body Chassis
        var bodyRect = new RectangleF(p.X - r, p.Y - r, r * 2, r * 2);

        // Player ambient shadow
        using (var shadowBrush = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
        {
            g.FillEllipse(shadowBrush, p.X - r - 2, p.Y - r - 2, r * 2 + 4, r * 2 + 4);
        }

        // Body gradient (Metallic sphere style)
        using (var bodyBrush = new PathGradientBrush(GetCirclePath(p.X, p.Y, r)))
        {
            bodyBrush.CenterPoint = new PointF(p.X - r * 0.3f, p.Y - r * 0.3f);
            bodyBrush.CenterColor = Color.White;
            bodyBrush.SurroundColors = new Color[] { p.PlayerColor };
            g.FillEllipse(bodyBrush, bodyRect);
        }

        // Outer armor plates/rim
        using (var rimPen = new Pen(Color.FromArgb(200, 30, 30, 30), 2f))
        {
            g.DrawEllipse(rimPen, bodyRect);
        }

        // Draw Player Index / Team Text (e.g. "P1" or "P2")
        using (var nameBrush = new SolidBrush(Color.White))
        using (var labelFont = new Font("Segoe UI", 8f, FontStyle.Bold))
        {
            string label = p.IsSpecialOps ? "OPS" : "MERC";
            var size = g.MeasureString(label, labelFont);
            g.DrawString(label, labelFont, nameBrush, p.X - size.Width / 2f, p.Y - r - 16f);
        }

        // 4. Draw Small Health Bar above Player
        float healthBarW = 34f;
        float healthBarH = 4f;
        float hbX = p.X - healthBarW / 2f;
        float hbY = p.Y - r - 22f;

        using (var hbBg = new SolidBrush(Color.FromArgb(100, 40, 0, 0)))
        using (var hbFg = new SolidBrush(p.Health > 30f ? Color.FromArgb(0, 255, 100) : Color.FromArgb(255, 30, 30)))
        using (var hbBorder = new Pen(Color.Black, 1f))
        {
            g.FillRectangle(hbBg, hbX, hbY, healthBarW, healthBarH);
            float hpRatio = p.Health / p.MaxHealth;
            g.FillRectangle(hbFg, hbX, hbY, healthBarW * hpRatio, healthBarH);
            g.DrawRectangle(hbBorder, hbX, hbY, healthBarW, healthBarH);
        }

        // 5. Draw Reload circular progress ring directly around Player Chassis
        if (p.IsReloading)
        {
            var stats = WeaponStats.GetStats(p.Weapon);
            float reloadProgress = 1f - (p.ReloadTimer / stats.ReloadDuration); // 0 to 1
            float meterR = r + 7f;
            using (var reloadPen = new Pen(Color.FromArgb(200, 0, 255, 100), 2.5f))
            {
                g.DrawArc(reloadPen, p.X - meterR, p.Y - meterR, meterR * 2, meterR * 2, -90, reloadProgress * 360f);
            }
        }
    }

    private void DrawBullets(Graphics g)
    {
        foreach (var b in _engine.Bullets)
        {
            if (!b.IsActive) continue;

            // Draw bullet trail lines (fade out)
            if (b.Trail.Count > 1)
            {
                var points = b.Trail.ToArray();
                using (var trailPath = new GraphicsPath())
                {
                    trailPath.AddLines(points);
                    using (var trailPen = new Pen(Color.FromArgb(80, 255, 230, 100), 1.5f))
                    {
                        g.DrawPath(trailPen, trailPath);
                    }
                }
            }

            // Draw Bullet Head (bright yellow/white dot)
            using (var bulletBrush = new SolidBrush(Color.White))
            using (var glowPen = new Pen(Color.Gold, 1f))
            {
                g.FillEllipse(bulletBrush, b.X - b.Radius, b.Y - b.Radius, b.Radius * 2, b.Radius * 2);
                g.DrawEllipse(glowPen, b.X - b.Radius, b.Y - b.Radius, b.Radius * 2, b.Radius * 2);
            }
        }
    }

    private void DrawGrenades(Graphics g)
    {
        foreach (var gr in _engine.Grenades)
        {
            if (!gr.IsActive) continue;

            float r = gr.Radius;

            // Grenade shadow
            using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, gr.X - r - 1, gr.Y - r - 1, r * 2 + 2, r * 2 + 2);
            }

            // Rotated Grenade rendering
            var state = g.Save();
            g.TranslateTransform(gr.X, gr.Y);
            g.RotateTransform(gr.Rotation * 180f / (float)Math.PI);

            // Draw ribbed grenade circle
            using (var grenadeBrush = new SolidBrush(Color.FromArgb(45, 65, 45)))
            using (var rimPen = new Pen(Color.FromArgb(80, 100, 80), 1f))
            {
                g.FillEllipse(grenadeBrush, -r, -r, r * 2, r * 2);
                g.DrawEllipse(rimPen, -r, -r, r * 2, r * 2);

                // Draw ribbing line segments
                g.DrawLine(rimPen, -r, 0, r, 0);
                g.DrawLine(rimPen, 0, -r, 0, r);
            }

            g.Restore(state);

            // Blinking fuse dot in the center (red glow, blinks faster as fuse burns down)
            float flashFreq = gr.FuseTimer > 1.0f ? 0.3f : 0.08f;
            bool isLightOn = (gr.FuseTimer % (flashFreq * 2f)) < flashFreq;

            if (isLightOn)
            {
                using (var fuseGlow = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(fuseGlow, gr.X - 2, gr.Y - 2, 4, 4);
                }
            }
        }
    }

    private void DrawParticles(Graphics g)
    {
        foreach (var p in _engine.Particles)
        {
            if (!p.IsActive) continue;

            float ageRatio = p.Lifespan / p.MaxLifespan;
            int alpha = (int)(255 * ageRatio);
            if (alpha <= 0) continue;

            using (var brush = new SolidBrush(Color.FromArgb(alpha, p.ParticleColor)))
            {
                if (p.Type == ParticleType.Casing)
                {
                    // Draw rotated rectangle for bullet casings
                    var state = g.Save();
                    g.TranslateTransform(p.X, p.Y);
                    g.RotateTransform(p.Angle * 180f / (float)Math.PI);
                    g.FillRectangle(brush, -p.Size, -p.Size / 2f, p.Size * 2, p.Size);
                    g.Restore(state);
                }
                else if (p.Type == ParticleType.Spark || p.Type == ParticleType.Shrapnel)
                {
                    // Draw streak spark in direction of velocity
                    float len = 0.04f; // 4% of speed length
                    using (var sparkPen = new Pen(Color.FromArgb(alpha, p.ParticleColor), p.Size))
                    {
                        g.DrawLine(sparkPen, p.X, p.Y, p.X + p.Vx * len, p.Y + p.Vy * len);
                    }
                }
                else
                {
                    // Standard circle for smoke, blood splat
                    g.FillEllipse(brush, p.X - p.Size / 2f, p.Y - p.Size / 2f, p.Size, p.Size);
                }
            }
        }
    }

    private void DrawLowHealthVignettes(Graphics g)
    {
        DrawSinglePlayerVignette(g, _engine.Player1);
        DrawSinglePlayerVignette(g, _engine.Player2);
    }

    private void DrawSinglePlayerVignette(Graphics g, Player p)
    {
        if (p.IsDead || p.Health >= 35f) return;

        float hpIntensity = (35f - p.Health) / 35f; // 0 to 1
        int alpha = (int)(hpIntensity * 130); // Max opacity 130

        // Create path representing the vignette
        using (var path = new GraphicsPath())
        {
            path.AddEllipse(0, 0, _engine.ArenaWidth, _engine.ArenaHeight);
            using (var pgb = new PathGradientBrush(path))
            {
                pgb.CenterColor = Color.FromArgb(0, 0, 0, 0);
                pgb.SurroundColors = new Color[] { Color.FromArgb(alpha, 150, 0, 0) };
                g.FillRectangle(pgb, 0, 0, _engine.ArenaWidth, _engine.ArenaHeight);
            }
        }
    }

    private void DrawHUDPanel(Graphics g)
    {
        int hudY = (int)_engine.ArenaHeight + 35;
        int hudH = 100;
        var hudRect = new Rectangle(0, hudY, (int)_engine.ArenaWidth, hudH);

        // Fill HUD background (dark metallic slate)
        using (var hudBg = new LinearGradientBrush(hudRect, Color.FromArgb(12, 14, 16), Color.FromArgb(6, 7, 8), 90f))
        {
            g.FillRectangle(hudBg, hudRect);
        }

        // Draw neon separation line
        using (var linePen = new Pen(Color.FromArgb(40, 50, 60), 2f))
        {
            g.DrawLine(linePen, 0, hudY, _engine.ArenaWidth, hudY);
        }

        // Center divider line
        using (var divPen = new Pen(Color.FromArgb(20, 24, 28), 1.5f))
        {
            g.DrawLine(divPen, _engine.ArenaWidth / 2f, hudY + 10, _engine.ArenaWidth / 2f, hudY + 90);
        }

        // 1. Draw Player 1 HUD (Left side)
        DrawPlayerHUD(g, _engine.Player1, 40, hudY + 12);

        // 2. Draw Player 2 HUD (Right side)
        DrawPlayerHUD(g, _engine.Player2, (int)_engine.ArenaWidth / 2 + 40, hudY + 12);
    }

    private void DrawPlayerHUD(Graphics g, Player p, int x, int y)
    {
        // Name & Profile Header
        string title = p.PlayerIndex == 1 ? "PLAYER 1: SPECIAL OPS" : "PLAYER 2: MERCENARY";
        Color teamColor = p.PlayerColor;
        using (var headerBrush = new SolidBrush(teamColor))
        {
            g.DrawString(title, _hudHeaderFont, headerBrush, x, y);
        }

        if (p.IsDead)
        {
            using (var deadBrush = new SolidBrush(Color.FromArgb(200, 255, 0, 0)))
            {
                g.DrawString("K.I.A. (ELIMINATED)", _hudBodyFont, deadBrush, x, y + 20);
            }
            return;
        }

        // Health Bar Draw
        float barW = 160f;
        float barH = 12f;
        float bx = x;
        float by = y + 22;

        using (var barBg = new SolidBrush(Color.FromArgb(30, 40, 40, 40)))
        using (var barBorder = new Pen(Color.FromArgb(50, 60, 70), 1f))
        using (var barFill = new SolidBrush(p.Health > 30f ? Color.FromArgb(0, 255, 100) : Color.FromArgb(255, 30, 30)))
        {
            g.FillRectangle(barBg, bx, by, barW, barH);
            float hpRatio = p.Health / p.MaxHealth;
            g.FillRectangle(barFill, bx, by, barW * hpRatio, barH);
            g.DrawRectangle(barBorder, bx, by, barW, barH);

            // HP text overlay
            string hpText = $"HP: {(int)p.Health}";
            using (var textFont = new Font("Courier New", 8f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(hpText, textFont, textBrush, bx + 4, by - 1);
            }
        }

        // Weapon details & Ammo
        string wpName = WeaponStats.GetStats(p.Weapon).Name.ToUpper();
        using (var wpBrush = new SolidBrush(Color.LightGray))
        {
            g.DrawString(wpName, _hudHeaderFont, wpBrush, x, y + 42);
        }

        // Ammo Display (Blinking if reloading)
        string ammoStr;
        Brush ammoBrush = Brushes.White;

        if (p.IsReloading)
        {
            ammoStr = "RELOADING...";
            // Blinking red
            bool reloadBlink = (p.ReloadTimer % 0.4f) < 0.2f;
            ammoBrush = reloadBlink ? Brushes.Red : Brushes.DarkRed;
        }
        else
        {
            var stats = WeaponStats.GetStats(p.Weapon);
            ammoStr = $"{p.AmmoClip}/{p.AmmoReserve}";
            if (p.AmmoClip == 0 && p.AmmoReserve > 0)
            {
                ammoStr = "PRESS RELOAD";
                ammoBrush = Brushes.Orange;
            }
            else if (p.AmmoClip == 0 && p.AmmoReserve == 0)
            {
                ammoStr = "OUT OF AMMO";
                ammoBrush = Brushes.Red;
            }
        }
        g.DrawString(ammoStr, _hudBodyFont, ammoBrush, x, y + 58);

        // Tactical Grenade count & Melee Indicator
        int iconStartX = x + 240;
        int iconStartY = y + 16;
        using (var greBrush = new SolidBrush(Color.FromArgb(40, 100, 50)))
        using (var greBorder = new Pen(Color.FromArgb(80, 180, 90), 1f))
        {
            g.DrawString("GRENADES:", _hudHeaderFont, Brushes.Gray, iconStartX - 70, iconStartY - 10);
            for (int i = 0; i < p.GrenadeCount; i++)
            {
                g.FillEllipse(greBrush, iconStartX + i * 16, iconStartY - 10, 10, 12);
                g.DrawEllipse(greBorder, iconStartX + i * 16, iconStartY - 10, 10, 12);
            }
        }

        // Score display
        using (var scoreBrush = new SolidBrush(Color.Gold))
        {
            g.DrawString($"SCORE: {p.Score:D5}", _hudHeaderFont, scoreBrush, iconStartX - 70, iconStartY + 15);
        }
    }

    private void DrawOverlayScreens(Graphics g)
    {
        // 1. Victory / Game Over Overlay
        if (_engine.Player1.IsDead || _engine.Player2.IsDead)
        {
            using (var overlayBrush = new SolidBrush(Color.FromArgb(190, 10, 10, 12)))
            {
                g.FillRectangle(overlayBrush, 0, 0, _engine.ArenaWidth, 800);
            }

            string resultTitle = "MATCH CONCLUDED";
            string victoryMsg = "";

            if (_engine.Player1.IsDead && _engine.Player2.IsDead)
            {
                victoryMsg = "MUTUAL ELIMINATION - BOTH PLAYERS K.I.A.";
            }
            else if (_engine.Player1.IsDead)
            {
                victoryMsg = "PLAYER 2 (MERCENARY) WINS THE COMBAT!";
            }
            else
            {
                victoryMsg = "PLAYER 1 (SPECIAL OPS) WINS THE COMBAT!";
            }

            int centerX = (int)_engine.ArenaWidth / 2;
            int centerY = 400;

            // Draw Header with neon glow
            using (var titleBrush = new SolidBrush(Color.Red))
            using (var subBrush = new SolidBrush(Color.White))
            using (var promptBrush = new SolidBrush(Color.Gold))
            {
                // Glow effect
                int offset = (int)(2 + _glowPulse * 3);
                using (var glowB = new SolidBrush(Color.FromArgb(70, 255, 0, 0)))
                {
                    g.DrawString(resultTitle, _titleFont, glowB, centerX - g.MeasureString(resultTitle, _titleFont).Width / 2 + offset, centerY - 120 + offset);
                    g.DrawString(resultTitle, _titleFont, glowB, centerX - g.MeasureString(resultTitle, _titleFont).Width / 2 - offset, centerY - 120 - offset);
                }

                g.DrawString(resultTitle, _titleFont, titleBrush, centerX - g.MeasureString(resultTitle, _titleFont).Width / 2, centerY - 120);
                g.DrawString(victoryMsg, _msgFont, subBrush, centerX - g.MeasureString(victoryMsg, _msgFont).Width / 2, centerY - 30);

                string p1Score = $"P1 SCORE: {_engine.Player1.Score}";
                string p2Score = $"P2 SCORE: {_engine.Player2.Score}";
                g.DrawString(p1Score, _hudBodyFont, Brushes.LightSkyBlue, centerX - 120, centerY + 20);
                g.DrawString(p2Score, _hudBodyFont, Brushes.Salmon, centerX + 20, centerY + 20);

                string prompt = "PRESS [ENTER] TO REBOOT SYSTEMS";
                g.DrawString(prompt, _msgFont, promptBrush, centerX - g.MeasureString(prompt, _msgFont).Width / 2, centerY + 90);
            }
        }
    }

    private GraphicsPath GetCirclePath(float x, float y, float r)
    {
        var path = new GraphicsPath();
        path.AddEllipse(x - r, y - r, r * 2, r * 2);
        return path;
    }

    protected override bool IsInputKey(Keys keyData)
    {
        switch (keyData & Keys.KeyCode)
        {
            case Keys.Up:
            case Keys.Down:
            case Keys.Left:
            case Keys.Right:
            case Keys.Enter:
            case Keys.Space:
                return true;
            default:
                return base.IsInputKey(keyData);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _floorGraphics?.Dispose();
            _floorBitmap?.Dispose();
            _titleFont.Dispose();
            _hudHeaderFont.Dispose();
            _hudBodyFont.Dispose();
            _ammoFont.Dispose();
            _msgFont.Dispose();
        }
        base.Dispose(disposing);
    }

    private void DrawDebris(Graphics g)
    {
        foreach (var deb in _engine.DebrisList)
        {
            if (!deb.IsActive) continue;

            var state = g.Save();
            g.TranslateTransform(deb.X, deb.Y);
            g.RotateTransform(deb.Angle * 180f / (float)Math.PI);

            using (var brush = new SolidBrush(deb.DebrisColor))
            using (var borderPen = new Pen(Color.FromArgb(80, 0, 0, 0), 1f))
            {
                g.FillRectangle(brush, -deb.Size / 2f, -deb.Size / 2f, deb.Size, deb.Size);
                g.DrawRectangle(borderPen, -deb.Size / 2f, -deb.Size / 2f, deb.Size, deb.Size);
            }

            g.Restore(state);
        }
    }

    private void DrawSoundWaves(Graphics g)
    {
        foreach (var sw in _engine.SoundWaves)
        {
            if (!sw.IsActive) continue;

            float ageRatio = sw.Lifespan / sw.MaxLifespan;
            int alpha = (int)(sw.WaveColor.A * ageRatio);
            if (alpha <= 0) continue;

            using (var pen = new Pen(Color.FromArgb(alpha, sw.WaveColor), 2f))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawEllipse(pen, sw.X - sw.CurrentRadius, sw.Y - sw.CurrentRadius, sw.CurrentRadius * 2, sw.CurrentRadius * 2);
            }
        }
    }

    private void DrawFogOfWar(Graphics g)
    {
        using (var fowBrush = new SolidBrush(Color.FromArgb(238, 8, 9, 12)))
        {
            var oldClip = g.Clip;
            var fowRegion = new Region(new RectangleF(0, 0, _engine.ArenaWidth, _engine.ArenaHeight));

            if (!_engine.Player1.IsDead)
            {
                using (var p1Path = new GraphicsPath())
                {
                    PointF[] pts = _engine.CalculateVisionPolygon(_engine.Player1.X, _engine.Player1.Y, _engine.Player1.Angle, 45, 520f);
                    p1Path.AddPolygon(pts);
                    fowRegion.Exclude(p1Path);
                }
            }

            if (!_engine.Player2.IsDead)
            {
                using (var p2Path = new GraphicsPath())
                {
                    PointF[] pts = _engine.CalculateVisionPolygon(_engine.Player2.X, _engine.Player2.Y, _engine.Player2.Angle, 45, 520f);
                    p2Path.AddPolygon(pts);
                    fowRegion.Exclude(p2Path);
                }
            }

            g.FillRegion(fowBrush, fowRegion);
            fowRegion.Dispose();
        }
    }

    private void DrawCustomHeader(Graphics g)
    {
        var headerRect = new Rectangle(0, 0, (int)_engine.ArenaWidth, 35);

        using (var brush = new LinearGradientBrush(headerRect, Color.FromArgb(22, 24, 28), Color.FromArgb(12, 13, 15), 90f))
        {
            g.FillRectangle(brush, headerRect);
        }

        using (var linePen = new Pen(Color.FromArgb(80, 0, 255, 200), 1.5f))
        {
            g.DrawLine(linePen, 0, 35, _engine.ArenaWidth, 35);
        }

        string titleStr = "CYBERSTRIKE // LOCAL COMBAT ENGINE v1.1.0";
        using (var font = new Font("Courier New", 9f, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.FromArgb(200, 0, 255, 200)))
        {
            var size = g.MeasureString(titleStr, font);
            g.DrawString(titleStr, font, brush, (_engine.ArenaWidth - size.Width) / 2f, (35f - size.Height) / 2f);
        }

        int minX = (int)_engine.ArenaWidth - 75;
        g.DrawString("_", _hudHeaderFont, Brushes.Gray, minX, 6);

        int closeX = (int)_engine.ArenaWidth - 40;
        using (var closeBrush = new SolidBrush(Color.FromArgb(220, 200, 50, 50)))
        {
            g.DrawString("X", _hudHeaderFont, closeBrush, closeX, 8);
        }
    }

    private Point _dragStart = Point.Empty;

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Y <= 35)
        {
            int closeX = (int)_engine.ArenaWidth - 40;
            int minX = (int)_engine.ArenaWidth - 75;

            if (e.X >= closeX - 10 && e.X <= closeX + 20)
            {
                Application.Exit();
                return;
            }
            if (e.X >= minX - 10 && e.X <= minX + 20)
            {
                var form = FindForm();
                if (form != null) form.WindowState = FormWindowState.Minimized;
                return;
            }

            _dragStart = new Point(e.X, e.Y);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.Button == MouseButtons.Left && _dragStart != Point.Empty)
        {
            var form = FindForm();
            if (form != null)
            {
                form.Location = new Point(
                    form.Location.X + e.X - _dragStart.X,
                    form.Location.Y + e.Y - _dragStart.Y
                );
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragStart = Point.Empty;
    }
}
