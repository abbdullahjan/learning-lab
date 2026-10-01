using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberStrike;

public class MainForm : Form
{
    private readonly GameEngine _engine;
    private readonly GameCanvas _canvas;
    private readonly System.Windows.Forms.Timer _gameTimer;
    private DateTime _lastTime;

    public MainForm()
    {
        // 1. Setup Window Properties
        Text = "CYBERSTRIKE: Tactical Combat Arena";
        ClientSize = new Size(900, 800);
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(10, 10, 12);
        KeyPreview = true; // Captures keys before controls process them

        // 2. Initialize Game Engine & Rendering Canvas
        _engine = new GameEngine();
        _canvas = new GameCanvas(_engine)
        {
            Dock = DockStyle.Fill
        };
        Controls.Add(_canvas);

        // 3. Setup Loop Timer (Targeting ~60 FPS / 16ms)
        _gameTimer = new System.Windows.Forms.Timer
        {
            Interval = 16
        };
        _gameTimer.Tick += GameLoopTick;

        // 4. Hook Key Events
        KeyDown += OnFormKeyDown;
        KeyUp += OnFormKeyUp;
        FormClosing += OnFormClosing;

        // 5. Start Game loop
        _lastTime = DateTime.Now;
        _gameTimer.Start();
    }

    private void GameLoopTick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        float deltaTime = (float)(now - _lastTime).TotalSeconds;

        // Clamp delta time to avoid large physics updates on window drag/lag spikes
        if (deltaTime > 0.1f) deltaTime = 0.1f;
        _lastTime = now;

        // Process physics/logic & redraw canvas
        _engine.Update(deltaTime);
        _canvas.Invalidate();
    }

    private void OnFormKeyDown(object? sender, KeyEventArgs e)
    {
        // Global escape key to exit
        if (e.KeyCode == Keys.Escape)
        {
            Application.Exit();
            return;
        }

        // Reboot / Reset game state when Enter is pressed after round end
        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        {
            if (_engine.Player1.IsDead || _engine.Player2.IsDead)
            {
                _engine.Reset();
                _canvas.Invalidate();
                e.Handled = true;
                return;
            }
        }

        // Set key status in InputManager
        InputManager.SetKeyState(e.KeyCode, true);
    }

    private void OnFormKeyUp(object? sender, KeyEventArgs e)
    {
        InputManager.SetKeyState(e.KeyCode, false);
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        _gameTimer.Stop();
        _gameTimer.Dispose();
        _canvas.Dispose();
    }
}
