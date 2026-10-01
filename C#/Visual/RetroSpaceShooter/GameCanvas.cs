using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace RetroSpaceShooter;

public class GameCanvas : Control
{
    private readonly GameEngine _engine;
    private float _titlePulse = 0f;
    private bool _pulseDirection = true;

    public GameCanvas(GameEngine engine)
    {
        _engine = engine;
        DoubleBuffered = true;
        Size = new Size(GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        BackColor = Color.Black;

        // Set styles for high-performance double buffered painting
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.Selectable, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        // 1. Draw 3D Starfield in background
        DrawStarfield(g);

        // 2. Draw 3D Scrolling Ground Grid
        DrawGroundGrid(g);

        // 3. Draw Game World or Overlays depending on state
        switch (_engine.State)
        {
            case GameState.MainMenu:
                DrawMainMenu(g);
                break;

            case GameState.Playing:
                DrawGameWorld3D(g);
                DrawHUD(g);
                break;

            case GameState.Paused:
                DrawGameWorld3D(g);
                DrawHUD(g);
                DrawPauseOverlay(g);
                break;

            case GameState.GameOver:
                DrawGameWorld3D(g);
                DrawGameOverScreen(g);
                break;
        }

        // Pulse timer for menu glows
        UpdatePulse();
    }

    private void UpdatePulse()
    {
        if (_pulseDirection)
        {
            _titlePulse += 0.02f;
            if (_titlePulse >= 1f) _pulseDirection = false;
        }
        else
        {
            _titlePulse -= 0.02f;
            if (_titlePulse <= 0f) _pulseDirection = true;
        }
    }

    private void DrawStarfield(Graphics g)
    {
        foreach (var star in _engine.Stars)
        {
            PointF screenPos = Projection3D.Project(star.X, star.Y, star.Z, out bool visible);
            if (visible)
            {
                // Size scale based on depth
                float distance = Math.Max(10f, star.Z - Projection3D.CameraZ);
                float starSize = Math.Max(1f, star.Size * (1500f / distance));
                
                using (var brush = new SolidBrush(star.Color))
                {
                    g.FillEllipse(brush, screenPos.X - starSize / 2f, screenPos.Y - starSize / 2f, starSize, starSize);
                }
            }
        }
    }

    private void DrawGroundGrid(Graphics g)
    {
        float gridY = -40f; // Align below player altitude
        float offset = _engine.GridOffset;

        // 1. Draw horizontal scrolling lines (fading towards the horizon)
        for (float z = offset - 100f; z <= 1200f; z += 100f)
        {
            if (z < -100f) continue;
            int alpha = Math.Clamp((int)(110f * (1f - z / 1200f)), 0, 255);
            using (var pen = new Pen(Color.FromArgb(alpha, 0, 255, 255), 1f))
            {
                PointF p1 = Projection3D.Project(-380f, gridY, z, out bool v1);
                PointF p2 = Projection3D.Project(380f, gridY, z, out bool v2);
                if (v1 && v2)
                {
                    g.DrawLine(pen, p1, p2);
                }
            }
        }

        // 2. Draw longitudinal lines in fading segments
        for (float x = -350f; x <= 350f; x += 70f)
        {
            for (float z = -100f; z < 1200f; z += 100f)
            {
                float zStart = z;
                float zEnd = z + 100f;
                int alpha = Math.Clamp((int)(110f * (1f - zStart / 1200f)), 0, 255);
                using (var pen = new Pen(Color.FromArgb(alpha, 0, 255, 255), 1f))
                {
                    PointF p1 = Projection3D.Project(x, gridY, zStart, out bool v1);
                    PointF p2 = Projection3D.Project(x, gridY, zEnd, out bool v2);
                    if (v1 && v2)
                    {
                        g.DrawLine(pen, p1, p2);
                    }
                }
            }
        }
    }

    private void DrawGameWorld3D(Graphics g)
    {
        // Gather all visible game entities
        var entities = new List<GameEntity>();
        entities.AddRange(_engine.PowerUps);
        entities.AddRange(_engine.Lasers);
        entities.AddRange(_engine.Enemies);
        entities.AddRange(_engine.Particles);
        
        if (_engine.PlayerShip.Health > 0)
        {
            entities.Add(_engine.PlayerShip);
        }

        // Sort by Z descending (Painter's Algorithm: draw back-to-front)
        var sortedEntities = entities.OrderByDescending(e => e.Z).ToList();

        foreach (var entity in sortedEntities)
        {
            entity.Draw(g);
        }
    }

    private void DrawMainMenu(Graphics g)
    {
        int centerX = GameConfig.CanvasWidth / 2;

        // Draw Title with Cyber neon glow
        using (var fontTitle = new Font("Impact", 54f, FontStyle.Italic))
        {
            string titleText = "NEON VECTOR 3D";
            
            // Glow shadow layers
            int glowOffset = (int)(3 + _titlePulse * 5);
            using (var glowBrush1 = new SolidBrush(Color.FromArgb(50, 255, 0, 100)))
            {
                g.DrawString(titleText, fontTitle, glowBrush1, centerX - 210 + glowOffset, 150 + glowOffset);
                g.DrawString(titleText, fontTitle, glowBrush1, centerX - 210 - glowOffset, 150 - glowOffset);
            }
            using (var glowBrush2 = new SolidBrush(Color.FromArgb(80, 0, 255, 255)))
            {
                g.DrawString(titleText, fontTitle, glowBrush2, centerX - 210 - glowOffset, 150 + glowOffset);
                g.DrawString(titleText, fontTitle, glowBrush2, centerX - 210 + glowOffset, 150 - glowOffset);
            }

            // Foreground Text
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(titleText, fontTitle, textBrush, centerX - 210, 150);
            }
        }

        // Subtitle
        using (var fontSub = new Font("Courier New", 14f, FontStyle.Bold))
        using (var brushSub = new SolidBrush(Color.FromArgb(0, 255, 255)))
        {
            string subText = "--- THREE-DIMENSIONAL CYBER ARCADE ---";
            var size = g.MeasureString(subText, fontSub);
            g.DrawString(subText, fontSub, brushSub, centerX - size.Width / 2, 230);
        }

        // Blinking Prompt
        int promptAlpha = Math.Clamp((int)(100 + _titlePulse * 155), 0, 255);
        using (var fontPrompt = new Font("Courier New", 16f, FontStyle.Bold))
        using (var brushPrompt = new SolidBrush(Color.FromArgb(promptAlpha, 255, 255, 255)))
        {
            string promptText = "PRESS ENTER TO DEPLOY INTRUSION";
            var size = g.MeasureString(promptText, fontPrompt);
            g.DrawString(promptText, fontPrompt, brushPrompt, centerX - size.Width / 2, 360);
        }

        // How to Play
        DrawInstructionBox(g, centerX, 440);

        // High Scores Display
        DrawHighScoreList(g, centerX, 550);
    }

    private void DrawInstructionBox(Graphics g, int centerX, int y)
    {
        string[] instructions = new string[]
        {
            "MOVE: Arrow Keys or WASD (Up/Down controls altitude)",
            "FIRE: Spacebar (Hold to rapid fire)",
            "PAUSE: P Key | EXIT: Esc Key",
            "POWERUPS: (S) Shield, (D) Double, (T) Triple, (O) Overcharge"
        };

        using (var borderPen = new Pen(Color.FromArgb(100, 0, 255, 255), 1f))
        using (var bgBrush = new SolidBrush(Color.FromArgb(30, 0, 30, 30)))
        using (var textFont = new Font("Courier New", 9f, FontStyle.Regular))
        using (var textBrush = new SolidBrush(Color.FromArgb(200, 200, 255)))
        {
            g.FillRectangle(bgBrush, centerX - 260, y, 520, 95);
            g.DrawRectangle(borderPen, centerX - 260, y, 520, 95);

            for (int i = 0; i < instructions.Length; i++)
            {
                var size = g.MeasureString(instructions[i], textFont);
                g.DrawString(instructions[i], textFont, textBrush, centerX - size.Width / 2, y + 8 + i * 20);
            }
        }
    }

    private void DrawHighScoreList(Graphics g, int centerX, int y)
    {
        using (var textFont = new Font("Courier New", 10f, FontStyle.Bold))
        using (var titleBrush = new SolidBrush(Color.FromArgb(255, 165, 0)))
        using (var scoreBrush = new SolidBrush(Color.FromArgb(180, 180, 180)))
        {
            string title = "SYSTEM TOP SCORES";
            var tSize = g.MeasureString(title, textFont);
            g.DrawString(title, textFont, titleBrush, centerX - tSize.Width / 2, y);

            if (_engine.HighScores.Count == 0)
            {
                string empty = "NO RECORDED CORES";
                var eSize = g.MeasureString(empty, textFont);
                g.DrawString(empty, textFont, scoreBrush, centerX - eSize.Width / 2, y + 22);
            }
            else
            {
                for (int i = 0; i < _engine.HighScores.Count; i++)
                {
                    string scoreLine = $"#{i + 1} - {_engine.HighScores[i]:000000}";
                    var sSize = g.MeasureString(scoreLine, textFont);
                    g.DrawString(scoreLine, textFont, scoreBrush, centerX - sSize.Width / 2, y + 20 + i * 16);
                }
            }
        }
    }

    private void DrawHUD(Graphics g)
    {
        // Draw 3D Crosshairs & Target Lock brackets first
        DrawAimAssistReticles(g);

        // 1. Score & Multiplier
        using (var scoreFont = new Font("Courier New", 12f, FontStyle.Bold))
        using (var greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0)))
        using (var whiteBrush = new SolidBrush(Color.White))
        {
            string scoreStr = $"SCORE: {_engine.Score:000000}";
            g.DrawString(scoreStr, scoreFont, whiteBrush, 20, 20);

            if (_engine.Multiplier > 1)
            {
                string multStr = $"x{_engine.Multiplier} ({_engine.MultiplierTimer:0.0}s)";
                g.DrawString(multStr, scoreFont, greenBrush, 20, 38);
            }
        }

        // 2. Wave indicator
        using (var waveFont = new Font("Courier New", 12f, FontStyle.Bold))
        using (var waveBrush = new SolidBrush(Color.FromArgb(255, 0, 150)))
        {
            string waveStr = $"WAVE: {_engine.Wave}";
            var size = g.MeasureString(waveStr, waveFont);
            g.DrawString(waveStr, waveFont, waveBrush, GameConfig.CanvasWidth - size.Width - 20, 20);
        }

        // 3. Lives Icons
        int livesLeft = _engine.PlayerShip.Lives - 1;
        float iconW = 14f;
        float iconH = 14f;
        float startX = GameConfig.CanvasWidth - 25f;
        float startY = 42f;

        for (int i = 0; i < livesLeft; i++)
        {
            float ix = startX - i * 20f;
            PointF[] lifePoints = new PointF[]
            {
                new PointF(ix, startY - iconH / 2f),
                new PointF(ix - iconW / 2f, startY + iconH / 2f),
                new PointF(ix + iconW / 2f, startY + iconH / 2f)
            };
            using (var lifeBrush = new SolidBrush(Color.FromArgb(0, 200, 200)))
            {
                g.FillPolygon(lifeBrush, lifePoints);
            }
        }

        // 4. Status Bars (Bottom Left)
        float barY = GameConfig.CanvasHeight - 35f;
        DrawBar(g, 20, barY, 150, 12, _engine.PlayerShip.Shield, _engine.PlayerShip.MaxShield, Color.FromArgb(0, 255, 255), "SHIELD");
        DrawBar(g, 190, barY, 150, 12, _engine.PlayerShip.Health, _engine.PlayerShip.MaxHealth, Color.FromArgb(255, 0, 100), "HEALTH");

        // 5. Active Weapon / Overcharge Timer
        if (_engine.PlayerShip.Weapon != WeaponType.Single)
        {
            string wpName = _engine.PlayerShip.Weapon.ToString().ToUpper();
            float wpTimer = _engine.PlayerShip.WeaponTimer;
            DrawBar(g, 360, barY, 180, 12, wpTimer, wpName == "OVERCHARGE" ? 6f : (wpName == "TRIPLE" ? 9f : 12f), Color.FromArgb(255, 200, 0), wpName);
        }
    }

    private void DrawAimAssistReticles(Graphics g)
    {
        if (_engine.PlayerShip == null || _engine.PlayerShip.Health <= 0) return;

        // 1. Draw central aim reticle (projected in front of ship)
        PointF crosshairScreen = Projection3D.Project(_engine.PlayerShip.X, _engine.PlayerShip.Y, _engine.PlayerShip.Z + 320f, out bool cVisible);
        if (cVisible)
        {
            using (var dotPen = new Pen(Color.FromArgb(120, 0, 255, 255), 1f))
            {
                dotPen.DashStyle = DashStyle.Dot;
                g.DrawEllipse(dotPen, crosshairScreen.X - 18f, crosshairScreen.Y - 18f, 36f, 36f);
                g.DrawLine(dotPen, crosshairScreen.X - 25f, crosshairScreen.Y, crosshairScreen.X - 6f, crosshairScreen.Y);
                g.DrawLine(dotPen, crosshairScreen.X + 6f, crosshairScreen.Y, crosshairScreen.X + 25f, crosshairScreen.Y);
                g.DrawLine(dotPen, crosshairScreen.X, crosshairScreen.Y - 25f, crosshairScreen.X, crosshairScreen.Y - 6f);
                g.DrawLine(dotPen, crosshairScreen.X, crosshairScreen.Y + 6f, crosshairScreen.X, crosshairScreen.Y + 25f);
            }
        }

        // 2. Draw bracket locked target
        if (_engine.CurrentTarget != null)
        {
            PointF targetScreen = Projection3D.Project(_engine.CurrentTarget.X, _engine.CurrentTarget.Y, _engine.CurrentTarget.Z, out bool visible);
            if (visible)
            {
                using (var lockPen = new Pen(Color.FromArgb(0, 255, 0), 1.5f))
                {
                    float dist = Math.Max(10f, _engine.CurrentTarget.Z - Projection3D.CameraZ);
                    float size = Math.Max(10f, 16000f / dist); // Brackets size based on depth distance

                    // Bracket corner offsets
                    g.DrawLine(lockPen, targetScreen.X - size, targetScreen.Y - size, targetScreen.X - size + 6f, targetScreen.Y - size);
                    g.DrawLine(lockPen, targetScreen.X - size, targetScreen.Y - size, targetScreen.X - size, targetScreen.Y - size + 6f);

                    g.DrawLine(lockPen, targetScreen.X + size, targetScreen.Y - size, targetScreen.X + size - 6f, targetScreen.Y - size);
                    g.DrawLine(lockPen, targetScreen.X + size, targetScreen.Y - size, targetScreen.X + size, targetScreen.Y - size + 6f);

                    g.DrawLine(lockPen, targetScreen.X - size, targetScreen.Y + size, targetScreen.X - size + 6f, targetScreen.Y + size);
                    g.DrawLine(lockPen, targetScreen.X - size, targetScreen.Y + size, targetScreen.X - size, targetScreen.Y + size - 6f);

                    g.DrawLine(lockPen, targetScreen.X + size, targetScreen.Y + size, targetScreen.X + size - 6f, targetScreen.Y + size);
                    g.DrawLine(lockPen, targetScreen.X + size, targetScreen.Y + size, targetScreen.X + size, targetScreen.Y + size - 6f);

                    // LOCKED neon tag
                    using (var lockFont = new Font("Courier New", 7f, FontStyle.Bold))
                    using (var lockBrush = new SolidBrush(Color.FromArgb(200, 0, 255, 0)))
                    {
                        string lbl = "LOCKED";
                        var sz = g.MeasureString(lbl, lockFont);
                        g.DrawString(lbl, lockFont, lockBrush, targetScreen.X - sz.Width / 2f, targetScreen.Y - size - 12f);
                    }
                }
            }
        }
    }

    private void DrawBar(Graphics g, float x, float y, float w, float h, float val, float maxVal, Color themeColor, string label)
    {
        using (var bgBrush = new SolidBrush(Color.FromArgb(60, 40, 40, 40)))
        {
            g.FillRectangle(bgBrush, x, y, w, h);
        }

        float ratio = Math.Clamp(val / maxVal, 0f, 1f);
        if (ratio > 0)
        {
            using (var fillBrush = new SolidBrush(Color.FromArgb(120, themeColor)))
            {
                g.FillRectangle(fillBrush, x, y, w * ratio, h);
            }
        }

        using (var borderPen = new Pen(themeColor, 1f))
        {
            g.DrawRectangle(borderPen, x, y, w, h);
        }

        using (var font = new Font("Courier New", 7f, FontStyle.Bold))
        using (var brush = new SolidBrush(Color.White))
        {
            g.DrawString(label, font, brush, x, y - 11f);
        }
    }

    private void DrawPauseOverlay(Graphics g)
    {
        using (var overlayBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
        {
            g.FillRectangle(overlayBrush, 0, 0, GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        }

        int centerX = GameConfig.CanvasWidth / 2;
        int centerY = GameConfig.CanvasHeight / 2;

        using (var font = new Font("Impact", 36f))
        using (var textBrush = new SolidBrush(Color.FromArgb(0, 255, 255)))
        {
            string pausedText = "WAVE INTERCEPT PAUSED";
            var size = g.MeasureString(pausedText, font);
            g.DrawString(pausedText, font, textBrush, centerX - size.Width / 2, centerY - 40);
        }

        using (var subFont = new Font("Courier New", 12f, FontStyle.Bold))
        using (var subBrush = new SolidBrush(Color.White))
        {
            string promptText = "PRESS 'P' TO RESUME SIMULATION";
            var size = g.MeasureString(promptText, subFont);
            g.DrawString(promptText, subFont, subBrush, centerX - size.Width / 2, centerY + 20);
        }
    }

    private void DrawGameOverScreen(Graphics g)
    {
        using (var overlayBrush = new SolidBrush(Color.FromArgb(180, 20, 0, 0)))
        {
            g.FillRectangle(overlayBrush, 0, 0, GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        }

        int centerX = GameConfig.CanvasWidth / 2;
        int centerY = GameConfig.CanvasHeight / 2;

        using (var fontTitle = new Font("Impact", 46f))
        using (var titleBrush = new SolidBrush(Color.Red))
        {
            string text = "SYSTEM INTEGRITY LOST";
            var size = g.MeasureString(text, fontTitle);
            g.DrawString(text, fontTitle, titleBrush, centerX - size.Width / 2, centerY - 140);
        }

        using (var fontScore = new Font("Courier New", 16f, FontStyle.Bold))
        using (var brushScore = new SolidBrush(Color.White))
        {
            string text = $"FINAL DEFECTION CORE: {_engine.Score:000000}";
            var size = g.MeasureString(text, fontScore);
            g.DrawString(text, fontScore, brushScore, centerX - size.Width / 2, centerY - 60);

            string waveText = $"WAVES DEFENDED: {_engine.Wave}";
            var wSize = g.MeasureString(waveText, fontScore);
            g.DrawString(waveText, fontScore, brushScore, centerX - wSize.Width / 2, centerY - 35);
        }

        DrawHighScoreList(g, centerX, centerY + 15);

        using (var fontRestart = new Font("Courier New", 12f, FontStyle.Bold))
        using (var brushRestart = new SolidBrush(Color.FromArgb(0, 255, 255)))
        {
            string text = "PRESS [ENTER] TO REBOOT | [ESC] FOR MAIN MENU";
            var size = g.MeasureString(text, fontRestart);
            g.DrawString(text, fontRestart, brushRestart, centerX - size.Width / 2, centerY + 140);
        }
    }
}
