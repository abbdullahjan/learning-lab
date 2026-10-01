using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RetroSpaceShooter;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class Star
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float SpeedZ { get; set; }
    public float Size { get; set; }
    public Color Color { get; set; }
}

public class GameEngine
{
    public GameState State { get; private set; } = GameState.MainMenu;
    public Player PlayerShip { get; private set; } = null!;
    public List<Laser> Lasers { get; } = new();
    public List<Enemy> Enemies { get; } = new();
    public List<PowerUp> PowerUps { get; } = new();
    public List<Particle> Particles { get; } = new();
    public List<Star> Stars { get; } = new();

    public int Score { get; private set; }
    public int Multiplier { get; private set; } = 1;
    public float MultiplierTimer { get; private set; } = 0f;
    public int Wave { get; private set; } = 1;
    
    // 3D Grid scrolling
    public float GridOffset { get; private set; } = 0f;

    // Targeting Lock-On
    public Enemy? CurrentTarget { get; private set; }

    // Wave Spawning variables
    private int _enemiesToSpawn;
    private int _enemiesSpawned;
    private float _spawnCooldown;
    private float _spawnInterval = 2.0f;
    private bool _bossSpawned = false;
    private float _waveTransitionTimer = 0f;
    private bool _isTransitioning = false;

    // High Score List
    public List<int> HighScores { get; private set; } = new();
    private readonly string _highScoreFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.txt");

    private readonly Random _rand = new();

    public GameEngine()
    {
        InitializeStars();
        LoadHighScores();
        ResetGame();
    }

    public void StartGame()
    {
        ResetGame();
        State = GameState.Playing;
    }

    public void ResetGame()
    {
        PlayerShip = new Player(0f, 0f, -50f);
        Lasers.Clear();
        Enemies.Clear();
        PowerUps.Clear();
        Particles.Clear();
        Score = 0;
        Multiplier = 1;
        MultiplierTimer = 0f;
        Wave = 1;
        _enemiesSpawned = 0;
        _enemiesToSpawn = 6;
        _spawnInterval = 2.0f;
        _spawnCooldown = 1.0f;
        _bossSpawned = false;
        _isTransitioning = false;
        _waveTransitionTimer = 0f;
        GridOffset = 0f;
        CurrentTarget = null;
        InputManager.Clear();
    }

    private void InitializeStars()
    {
        Stars.Clear();
        for (int i = 0; i < 100; i++)
        {
            float speedZ = 120f + (float)_rand.NextDouble() * 300f;
            int brightness = (int)(80 + (speedZ / 420f) * 175);
            Color starColor;
            if (_rand.Next(3) == 0)
                starColor = Color.FromArgb(brightness, 170 + _rand.Next(85), 210 + _rand.Next(45), 255); // Cyanish
            else if (_rand.Next(4) == 0)
                starColor = Color.FromArgb(brightness, 255, 180 + _rand.Next(75), 180 + _rand.Next(75)); // Pinkish
            else
                starColor = Color.FromArgb(brightness, brightness, brightness); // White

            Stars.Add(new Star
            {
                X = -450f + (float)(_rand.NextDouble() * 900f),
                Y = -150f + (float)(_rand.NextDouble() * 450f),
                Z = 10f + (float)(_rand.NextDouble() * 1490f),
                SpeedZ = speedZ,
                Size = 1.5f,
                Color = starColor
            });
        }
    }

    public void Update(float deltaTime)
    {
        UpdateStars(deltaTime);

        // Update ground grid offset to simulate flying forward
        float speedMult = (State == GameState.Playing && PlayerShip.Weapon == WeaponType.Overcharge) ? 2.5f : 1f;
        GridOffset -= 150f * speedMult * deltaTime;
        if (GridOffset < -100f)
        {
            GridOffset += 100f;
        }

        if (State != GameState.Playing)
            return;

        // Player actions
        if (PlayerShip.Health > 0)
        {
            PlayerShip.Update(deltaTime);
            CurrentTarget = FindAutoAimTarget(); // Soft lock targeting
            HandlePlayerShooting(deltaTime);
        }
        else
        {
            EndGame();
            return;
        }

        // Wave Spawning & Transitions
        if (_isTransitioning)
        {
            _waveTransitionTimer -= deltaTime;
            if (_waveTransitionTimer <= 0)
            {
                _isTransitioning = false;
                StartNewWave();
            }
        }
        else
        {
            HandleSpawning(deltaTime);
        }

        // Update active entities in 3D space
        foreach (var laser in Lasers) laser.Update(deltaTime);
        foreach (var enemy in Enemies) enemy.Update(deltaTime);
        foreach (var powerUp in PowerUps) powerUp.Update(deltaTime);
        foreach (var particle in Particles) particle.Update(deltaTime);

        // Enemies shooting logic
        foreach (var enemy in Enemies)
        {
            if (enemy.ShouldShoot(deltaTime, out var newLasers))
            {
                Lasers.AddRange(newLasers);
            }
        }

        // Handle Collisions in 3D with tunneling checks
        CheckCollisions(deltaTime);

        // Manage Multiplier decay
        if (Multiplier > 1)
        {
            MultiplierTimer -= deltaTime;
            if (MultiplierTimer <= 0f)
            {
                Multiplier--;
                if (Multiplier > 1)
                    MultiplierTimer = 3.5f;
            }
        }

        // Cleanup inactive entities
        Lasers.RemoveAll(l => !l.IsActive);
        Enemies.RemoveAll(e => !e.IsActive);
        PowerUps.RemoveAll(p => !p.IsActive);
        Particles.RemoveAll(p => !p.IsActive);

        // Check for wave completion
        if (_enemiesSpawned >= _enemiesToSpawn && Enemies.Count == 0 && !_isTransitioning)
        {
            _isTransitioning = true;
            _waveTransitionTimer = 3.0f;
            SpawnExplosion(0f, 50f, 600f, Color.Cyan, 60);
        }
    }

    private void UpdateStars(float deltaTime)
    {
        float speedMult = (State == GameState.Playing && PlayerShip.Weapon == WeaponType.Overcharge) ? 2.5f : 1f;
        foreach (var star in Stars)
        {
            star.Z -= star.SpeedZ * speedMult * deltaTime;
            if (star.Z < 10f)
            {
                star.Z = 1500f;
                star.X = -450f + (float)(_rand.NextDouble() * 900f);
                star.Y = -150f + (float)(_rand.NextDouble() * 450f);
            }
        }
    }

    private Enemy? FindAutoAimTarget()
    {
        Enemy? bestTarget = null;
        float bestScore = float.MaxValue;

        foreach (var enemy in Enemies)
        {
            if (enemy.Health <= 0 || !enemy.IsActive) continue;
            if (enemy.Z <= PlayerShip.Z + 25f) continue; // Must be in front

            float dx = enemy.X - PlayerShip.X;
            float dy = enemy.Y - PlayerShip.Y;
            float dz = enemy.Z - PlayerShip.Z;

            // Distance in X/Y plane
            float projDist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Lock onto targets that are reasonably near our crosshair field
            if (projDist < 300f)
            {
                // Score favors closer Z and smaller angular offset
                float score = projDist + dz * 0.15f;
                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = enemy;
                }
            }
        }
        return bestTarget;
    }

    private void HandlePlayerShooting(float deltaTime)
    {
        if (InputManager.IsKeyPressed(Keys.Space) && PlayerShip.FireCooldown <= 0)
        {
            float fireInterval = PlayerShip.BaseFireInterval;
            if (PlayerShip.Weapon == WeaponType.Overcharge)
                fireInterval = 0.08f;

            PlayerShip.FireCooldown = fireInterval;

            // Firing from player nose (+15f Z offset)
            float launchZ = PlayerShip.Z + 15f;
            float vz = 1350f;
            float vx = 0f;
            float vy = 0f;

            // Apply soft lock guidance towards current target
            if (CurrentTarget != null)
            {
                float t = (CurrentTarget.Z - launchZ) / vz;
                if (t > 0.05f)
                {
                    vx = Math.Clamp((CurrentTarget.X - PlayerShip.X) / t, -320f, 320f);
                    vy = Math.Clamp((CurrentTarget.Y - PlayerShip.Y) / t, -220f, 220f);
                }
            }

            switch (PlayerShip.Weapon)
            {
                case WeaponType.Single:
                    Lasers.Add(new Laser(PlayerShip.X, PlayerShip.Y, launchZ, vx, vy, vz, isPlayerOwned: true, damage: 10f));
                    break;
                case WeaponType.Double:
                    Lasers.Add(new Laser(PlayerShip.X - 10f, PlayerShip.Y - 2f, launchZ, vx, vy, vz, isPlayerOwned: true, damage: 10f));
                    Lasers.Add(new Laser(PlayerShip.X + 10f, PlayerShip.Y - 2f, launchZ, vx, vy, vz, isPlayerOwned: true, damage: 10f));
                    break;
                case WeaponType.Triple:
                    Lasers.Add(new Laser(PlayerShip.X, PlayerShip.Y, launchZ, vx, vy, vz + 100f, isPlayerOwned: true, damage: 10f));
                    Lasers.Add(new Laser(PlayerShip.X - 8f, PlayerShip.Y - 2f, launchZ, vx - 100f, vy + 20f, vz, isPlayerOwned: true, damage: 10f));
                    Lasers.Add(new Laser(PlayerShip.X + 8f, PlayerShip.Y - 2f, launchZ, vx + 100f, vy + 20f, vz, isPlayerOwned: true, damage: 10f));
                    break;
                case WeaponType.Overcharge:
                    Lasers.Add(new Laser(PlayerShip.X - 6f, PlayerShip.Y - 2f, launchZ, vx - 30f, vy, vz + 450f, isPlayerOwned: true, damage: 12f));
                    Lasers.Add(new Laser(PlayerShip.X + 6f, PlayerShip.Y - 2f, launchZ, vx + 30f, vy, vz + 450f, isPlayerOwned: true, damage: 12f));
                    break;
            }

            SpawnRecoilSparks();
        }
    }

    private void SpawnRecoilSparks()
    {
        for (int i = 0; i < 3; i++)
        {
            float vx = (float)(_rand.NextDouble() * 60 - 30);
            float vy = (float)(_rand.NextDouble() * 30 - 15);
            float vz = -180f - (float)_rand.NextDouble() * 100f;
            Particles.Add(new Particle(PlayerShip.X, PlayerShip.Y - 2f, PlayerShip.Z - 12f, vx, vy, vz, Color.Cyan, 0.2f + (float)_rand.NextDouble() * 0.15f));
        }
    }

    private void HandleSpawning(float deltaTime)
    {
        _spawnCooldown -= deltaTime;
        if (_spawnCooldown <= 0)
        {
            _spawnCooldown = _spawnInterval;

            if (_enemiesSpawned < _enemiesToSpawn)
            {
                EnemyType typeToSpawn = EnemyType.Scout;
                double roll = _rand.NextDouble();

                if (Wave >= 4 && roll < 0.25)
                    typeToSpawn = EnemyType.Bomber;
                else if (Wave >= 2 && roll < 0.5)
                    typeToSpawn = EnemyType.Fighter;

                float spawnX = -200f + (float)(_rand.NextDouble() * 400f);
                float spawnY = 0f + (float)(_rand.NextDouble() * 100f);
                Enemies.Add(new Enemy(typeToSpawn, spawnX, spawnY, 1200f));
                _enemiesSpawned++;
            }
            else if (!_bossSpawned && Enemies.Count == 0)
            {
                Enemies.Add(new Enemy(EnemyType.Boss, 0f, 60f, 1200f));
                _bossSpawned = true;
            }
        }
    }

    private void StartNewWave()
    {
        Wave++;
        _enemiesSpawned = 0;
        _enemiesToSpawn = 5 + Wave * 3;
        _spawnInterval = Math.Max(0.7f, 2.0f - (Wave * 0.15f));
        _bossSpawned = false;
    }

    private void CheckCollisions(float deltaTime)
    {
        // 1. Lasers vs. Ships (3D AABB with Sweep/Tunneling Prevention)
        foreach (var laser in Lasers)
        {
            if (!laser.IsActive) continue;

            if (laser.IsPlayerOwned)
            {
                foreach (var enemy in Enemies)
                {
                    if (enemy.Health > 0)
                    {
                        // Calculate lookback window based on current relative speed
                        float travel = Math.Abs(laser.SpeedZ - enemy.SpeedZ) * deltaTime;
                        float zTolerance = (laser.Depth + enemy.Depth) / 2f + travel + 10f; // Add padding buffer
                        float dz = Math.Abs(enemy.Z - laser.Z);

                        if (dz < zTolerance)
                        {
                            float dx = Math.Abs(laser.X - enemy.X);
                            float dy = Math.Abs(laser.Y - enemy.Y);

                            if (dx < (laser.Width + enemy.Width) / 2f + 5f &&
                                dy < (laser.Height + enemy.Height) / 2f + 5f)
                            {
                                laser.IsActive = false;
                                enemy.Health -= laser.Damage;
                                
                                Color impactColor = (enemy.Type == EnemyType.Boss) ? Color.HotPink : Color.Red;
                                SpawnExplosion(laser.X, laser.Y, laser.Z, impactColor, 8);

                                if (enemy.Health <= 0)
                                {
                                    enemy.IsActive = false;
                                    OnEnemyDefeated(enemy);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (PlayerShip.Health > 0)
                {
                    float travel = Math.Abs(laser.SpeedZ - PlayerShip.SpeedZ) * deltaTime;
                    float zTolerance = (laser.Depth + PlayerShip.Depth) / 2f + travel + 10f;
                    float dz = Math.Abs(PlayerShip.Z - laser.Z);

                    if (dz < zTolerance)
                    {
                        float dx = Math.Abs(laser.X - PlayerShip.X);
                        float dy = Math.Abs(laser.Y - PlayerShip.Y);

                        if (dx < (laser.Width + PlayerShip.Width) / 2f + 5f &&
                            dy < (laser.Height + PlayerShip.Height) / 2f + 5f)
                        {
                            laser.IsActive = false;
                            PlayerShip.Hit(laser.Damage);
                            
                            SpawnExplosion(laser.X, laser.Y, laser.Z, Color.Cyan, 12);
                        }
                    }
                }
            }
        }

        // 2. Player vs. Enemies (Ramming in 3D)
        if (PlayerShip.Health > 0)
        {
            foreach (var enemy in Enemies)
            {
                if (enemy.Health > 0 && PlayerShip.IntersectsWith(enemy))
                {
                    float enemyHealth = enemy.Health;
                    enemy.Health -= 100f;
                    PlayerShip.Hit(enemyHealth * 0.6f + 15f);

                    if (enemy.Health <= 0)
                    {
                        enemy.IsActive = false;
                        OnEnemyDefeated(enemy);
                    }

                    SpawnExplosion((PlayerShip.X + enemy.X) / 2f, (PlayerShip.Y + enemy.Y) / 2f, (PlayerShip.Z + enemy.Z) / 2f, Color.Orange, 30);
                    break;
                }
            }
        }

        // 3. Player vs. PowerUps in 3D
        if (PlayerShip.Health > 0)
        {
            foreach (var powerUp in PowerUps)
            {
                if (powerUp.IsActive && PlayerShip.IntersectsWith(powerUp))
                {
                    powerUp.IsActive = false;
                    ApplyPowerUp(powerUp.Type);
                    
                    SpawnExplosion(powerUp.X, powerUp.Y, powerUp.Z, Color.Yellow, 20);
                }
            }
        }
    }

    private void OnEnemyDefeated(Enemy enemy)
    {
        int rawScore = enemy.ScoreValue;
        Score += rawScore * Multiplier;

        Multiplier = Math.Min(10, Multiplier + 1);
        MultiplierTimer = 3.5f;

        Color explosionColor = enemy.Type switch
        {
            EnemyType.Scout => Color.FromArgb(255, 100, 100),
            EnemyType.Fighter => Color.FromArgb(255, 0, 150),
            EnemyType.Bomber => Color.FromArgb(200, 0, 200),
            EnemyType.Boss => Color.FromArgb(255, 0, 255),
            _ => Color.Red
        };
        int particleCount = enemy.Type == EnemyType.Boss ? 100 : 20;
        SpawnExplosion(enemy.X, enemy.Y, enemy.Z, explosionColor, particleCount);

        // Power-Up drops
        double dropChance = enemy.Type switch
        {
            EnemyType.Scout => 0.10,
            EnemyType.Fighter => 0.18,
            EnemyType.Bomber => 0.35,
            EnemyType.Boss => 1.0,
            _ => 0.05
        };

        if (_rand.NextDouble() < dropChance)
        {
            var powerupTypes = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToArray();
            PowerUpType chosen = powerupTypes[_rand.Next(powerupTypes.Length)];
            
            if (enemy.Type == EnemyType.Boss)
            {
                chosen = _rand.Next(2) == 0 ? PowerUpType.Overcharge : PowerUpType.TripleShot;
            }

            PowerUps.Add(new PowerUp(chosen, enemy.X, enemy.Y, enemy.Z));
        }
    }

    private void ApplyPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                if (PlayerShip.Shield >= PlayerShip.MaxShield)
                    PlayerShip.Health = Math.Min(PlayerShip.MaxHealth, PlayerShip.Health + 25f);
                else
                    PlayerShip.Shield = PlayerShip.MaxShield;
                break;

            case PowerUpType.DoubleShot:
                PlayerShip.Weapon = WeaponType.Double;
                PlayerShip.WeaponTimer = 12f;
                break;

            case PowerUpType.TripleShot:
                PlayerShip.Weapon = WeaponType.Triple;
                PlayerShip.WeaponTimer = 9f;
                break;

            case PowerUpType.Overcharge:
                PlayerShip.Weapon = WeaponType.Overcharge;
                PlayerShip.WeaponTimer = 6f;
                break;
        }
    }

    private void SpawnExplosion(float x, float y, float z, Color color, int particleCount)
    {
        for (int i = 0; i < particleCount; i++)
        {
            double theta = _rand.NextDouble() * Math.PI * 2.0;
            double phi = _rand.NextDouble() * Math.PI;
            float speed = 80f + (float)(_rand.NextDouble() * 260f);

            float vx = (float)(Math.Sin(phi) * Math.Cos(theta) * speed);
            float vy = (float)(Math.Sin(phi) * Math.Sin(theta) * speed);
            float vz = (float)(Math.Cos(phi) * speed);

            float lifespan = 0.4f + (float)(_rand.NextDouble() * 0.5f);

            Particles.Add(new Particle(x, y, z, vx, vy, vz, color, lifespan));
        }
    }

    public void PauseUnpause()
    {
        if (State == GameState.Playing)
            State = GameState.Paused;
        else if (State == GameState.Paused)
            State = GameState.Playing;
    }

    private void EndGame()
    {
        State = GameState.GameOver;
        SaveHighScore(Score);
    }

    public void ReturnToMenu()
    {
        State = GameState.MainMenu;
    }

    private void LoadHighScores()
    {
        HighScores.Clear();
        try
        {
            if (File.Exists(_highScoreFile))
            {
                var lines = File.ReadAllLines(_highScoreFile);
                foreach (var line in lines)
                {
                    if (int.TryParse(line, out int s))
                    {
                        HighScores.Add(s);
                    }
                }
            }
        }
        catch { }

        HighScores = HighScores.OrderByDescending(s => s).Take(5).ToList();
    }

    private void SaveHighScore(int newScore)
    {
        if (newScore <= 0) return;
        HighScores.Add(newScore);
        HighScores = HighScores.OrderByDescending(s => s).Take(5).ToList();

        try
        {
            File.WriteAllLines(_highScoreFile, HighScores.Select(s => s.ToString()));
        }
        catch { }
    }
}
