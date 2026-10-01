using System;
using System.Drawing;
using System.Windows.Forms;

namespace RetroSpaceShooter;

public class MainForm : Form
{
    private readonly GameEngine _engine;
    private readonly GameCanvas _canvas;
    private readonly System.Windows.Forms.Timer _gameTimer;
    private DateTime _lastTime;

    public MainForm()
    {
        // 1. Setup Form properties
        Text = "NEON VECTOR // CYBER SHIELD";
        ClientSize = new Size(GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;
        KeyPreview = true; // Essential for intercepting keys before child controls

        // 2. Initialize Game Components
        _engine = new GameEngine();
        _canvas = new GameCanvas(_engine)
        {
            Dock = DockStyle.Fill
        };
        Controls.Add(_canvas);

        // 3. Setup Game Loop Timer (Targeting ~60 FPS)
        _gameTimer = new System.Windows.Forms.Timer
        {
            Interval = 16 // ~62.5 FPS
        };
        _gameTimer.Tick += GameLoopTick;

        // 4. Hook Key Events
        KeyDown += OnFormKeyDown;
        KeyUp += OnFormKeyUp;
        FormClosing += OnFormClosing;

        // 5. Start the Engine Loop
        _lastTime = DateTime.Now;
        _gameTimer.Start();
    }

    private void GameLoopTick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        float deltaTime = (float)(now - _lastTime).TotalSeconds;
        
        // Clamp deltaTime to prevent huge jumps if the window is moved
        if (deltaTime > 0.1f) deltaTime = 0.1f;
        
        _lastTime = now;

        // Update and Render
        _engine.Update(deltaTime);
        _canvas.Invalidate();
    }

    private void OnFormKeyDown(object? sender, KeyEventArgs e)
    {
        // Intercept global state control keys
        switch (e.KeyCode)
        {
            case Keys.Enter:
                if (_engine.State == GameState.MainMenu)
                {
                    _engine.StartGame();
                    e.Handled = true;
                    return;
                }
                else if (_engine.State == GameState.GameOver)
                {
                    _engine.ResetGame();
                    _engine.StartGame();
                    e.Handled = true;
                    return;
                }
                break;

            case Keys.Escape:
                if (_engine.State == GameState.Playing || _engine.State == GameState.Paused || _engine.State == GameState.GameOver)
                {
                    _engine.ReturnToMenu();
                    e.Handled = true;
                    return;
                }
                else if (_engine.State == GameState.MainMenu)
                {
                    Application.Exit();
                }
                break;

            case Keys.P:
                if (_engine.State == GameState.Playing || _engine.State == GameState.Paused)
                {
                    _engine.PauseUnpause();
                    e.Handled = true;
                    return;
                }
                break;
        }

        // Forward normal gameplay inputs to the InputManager
        InputManager.SetKeyState(e.KeyCode, true);
    }

    private void OnFormKeyUp(object? sender, KeyEventArgs e)
    {
        // Forward key releases to the InputManager
        InputManager.SetKeyState(e.KeyCode, false);
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        _gameTimer.Stop();
        _gameTimer.Dispose();
    }
}
