using System;
using System.Collections.Generic;
using System.Drawing;

namespace CyberStrike;

public class GameEngine
{
    public Player Player1 { get; }
    public Player Player2 { get; }
    public List<Bullet> Bullets { get; } = new();
    public List<Grenade> Grenades { get; } = new();
    public List<Barrel> Barrels { get; } = new();
    public List<Wall> Walls { get; } = new();
    public List<WeaponCrate> WeaponCrates { get; } = new();
    public List<Particle> Particles { get; } = new();
    public List<SoundWave> SoundWaves { get; } = new();
    public List<Debris> DebrisList { get; } = new();

    private float _p1FootstepTimer = 0f;
    private float _p2FootstepTimer = 0f;

    // Screen Shake
    public float ScreenShakeIntensity { get; private set; } = 0f;
    public float ShakeX { get; private set; } = 0f;
    public float ShakeY { get; private set; } = 0f;

    // Callbacks for Visual FX (to notify canvas to draw permanent stains)
    public event Action<float, float, Color, float>? OnSpawnPermanentSplatter;

    // Arena Boundaries
    public float ArenaWidth { get; } = 900f;
    public float ArenaHeight { get; } = 665f; // Leaves 35px for custom header and 100px for HUD at 900x800 Form

    private readonly Random _rand = new();

    public GameEngine()
    {
        // Initialize Players
        Player1 = new Player(1, 100f, 350f, Color.FromArgb(0, 200, 255)); // cyan
        Player2 = new Player(2, 800f, 350f, Color.FromArgb(255, 60, 60));  // red/orange

        Player1.Angle = 0f; // facing right
        Player2.Angle = (float)Math.PI; // facing left

        InitializeMap();
    }

    private void InitializeMap()
    {
        // 1. Boundary Walls
        Walls.Add(new Wall(0, 0, ArenaWidth, 20, Color.FromArgb(40, 45, 50))); // Top
        Walls.Add(new Wall(0, ArenaHeight - 20, ArenaWidth, 20, Color.FromArgb(40, 45, 50))); // Bottom
        Walls.Add(new Wall(0, 0, 20, ArenaHeight, Color.FromArgb(40, 45, 50))); // Left
        Walls.Add(new Wall(ArenaWidth - 20, 0, 20, ArenaHeight, Color.FromArgb(40, 45, 50))); // Right

        // 2. Inner Tactical Obstacles
        Walls.Add(new Wall(200, 150, 40, 180, Color.FromArgb(50, 55, 60))); // Left vertical pillar
        Walls.Add(new Wall(200, 370, 40, 180, Color.FromArgb(50, 55, 60))); // Left vertical pillar lower
        Walls.Add(new Wall(660, 150, 40, 180, Color.FromArgb(50, 55, 60))); // Right vertical pillar
        Walls.Add(new Wall(660, 370, 40, 180, Color.FromArgb(50, 55, 60))); // Right vertical pillar lower

        Walls.Add(new Wall(370, 330, 160, 40, Color.FromArgb(60, 65, 70)));  // Center horizontal divider
        Walls.Add(new Wall(430, 80, 40, 100, Color.FromArgb(50, 55, 60)));   // Center top barrier
        Walls.Add(new Wall(430, 520, 40, 100, Color.FromArgb(50, 55, 60)));  // Center bottom barrier

        // 3. Destructible Fuel Barrels
        Barrels.Add(new Barrel(110, 100));
        Barrels.Add(new Barrel(790, 100));
        Barrels.Add(new Barrel(110, 600));
        Barrels.Add(new Barrel(790, 600));
        Barrels.Add(new Barrel(450, 240));
        Barrels.Add(new Barrel(450, 460));
        Barrels.Add(new Barrel(300, 350));
        Barrels.Add(new Barrel(600, 350));

        // 4. Weapon Crates
        WeaponCrates.Add(new WeaponCrate(110, 350, WeaponType.Shotgun));
        WeaponCrates.Add(new WeaponCrate(790, 350, WeaponType.Sniper));
    }

    public void Reset()
    {
        Player1.X = 100f;
        Player1.Y = 350f;
        Player1.Vx = 0f;
        Player1.Vy = 0f;
        Player1.Angle = 0f;
        Player1.Health = 100f;
        Player1.ResetWeapon(WeaponType.AssaultRifle);
        Player1.GrenadeCount = 3;
        Player1.Score = 0;

        Player2.X = 800f;
        Player2.Y = 350f;
        Player2.Vx = 0f;
        Player2.Vy = 0f;
        Player2.Angle = (float)Math.PI;
        Player2.Health = 100f;
        Player2.ResetWeapon(WeaponType.AssaultRifle);
        Player2.GrenadeCount = 3;
        Player2.Score = 0;

        Bullets.Clear();
        Grenades.Clear();
        Barrels.Clear();
        WeaponCrates.Clear();
        Particles.Clear();
        SoundWaves.Clear();
        DebrisList.Clear();
        Walls.Clear();

        _p1FootstepTimer = 0f;
        _p2FootstepTimer = 0f;

        InitializeMap();
    }

    public void Update(float dt)
    {
        // 1. Process Screen Shake Decay
        if (ScreenShakeIntensity > 0f)
        {
            ScreenShakeIntensity = Math.Max(0f, ScreenShakeIntensity - 40f * dt);
            ShakeX = (float)((_rand.NextDouble() - 0.5) * ScreenShakeIntensity);
            ShakeY = (float)((_rand.NextDouble() - 0.5) * ScreenShakeIntensity);
        }
        else
        {
            ShakeX = 0f;
            ShakeY = 0f;
        }

        // 2. Update Players (input mapping & timers)
        UpdatePlayerState(Player1, dt);
        UpdatePlayerState(Player2, dt);

        // Process Player 1 Footsteps Noise
        if (!Player1.IsDead && (Math.Abs(Player1.Vx) > 5f || Math.Abs(Player1.Vy) > 5f))
        {
            _p1FootstepTimer -= dt;
            if (_p1FootstepTimer <= 0f)
            {
                _p1FootstepTimer = 0.35f;
                TriggerSoundWave(Player1.X, Player1.Y, 65f, 0.4f, Color.FromArgb(70, Player1.PlayerColor));
            }
        }
        else
        {
            _p1FootstepTimer = 0f;
        }

        // Process Player 2 Footsteps Noise
        if (!Player2.IsDead && (Math.Abs(Player2.Vx) > 5f || Math.Abs(Player2.Vy) > 5f))
        {
            _p2FootstepTimer -= dt;
            if (_p2FootstepTimer <= 0f)
            {
                _p2FootstepTimer = 0.35f;
                TriggerSoundWave(Player2.X, Player2.Y, 65f, 0.4f, Color.FromArgb(70, Player2.PlayerColor));
            }
        }
        else
        {
            _p2FootstepTimer = 0f;
        }

        // 3. Handle Physics / Collision Resolution
        ResolvePlayerCollisions(Player1);
        ResolvePlayerCollisions(Player2);
        ResolvePlayerToPlayerCollision();

        // 4. Update Bullets
        for (int i = Bullets.Count - 1; i >= 0; i--)
        {
            var b = Bullets[i];
            b.Update(dt);

            // Check boundaries
            if (b.X < 0 || b.X > ArenaWidth || b.Y < 0 || b.Y > ArenaHeight)
            {
                b.IsActive = false;
            }

            if (!b.IsActive)
            {
                Bullets.RemoveAt(i);
                continue;
            }

            // Bullet vs. Walls
            foreach (var wall in Walls)
            {
                if (Intersects(b, wall))
                {
                    b.IsActive = false;
                    SpawnSparks(b.X, b.Y, -b.Vx * 0.2f, -b.Vy * 0.2f, Color.Gold, 6);
                    break;
                }
            }

            if (!b.IsActive)
            {
                Bullets.RemoveAt(i);
                continue;
            }

            // Bullet vs. Barrels
            foreach (var barrel in Barrels)
            {
                if (barrel.IsActive && Intersects(b, barrel))
                {
                    b.IsActive = false;
                    barrel.Health -= b.Damage;
                    SpawnSparks(b.X, b.Y, -b.Vx * 0.2f, -b.Vy * 0.2f, Color.OrangeRed, 8);
                    if (!barrel.IsActive)
                    {
                        TriggerExplosion(barrel.X, barrel.Y, 150f, 110f);
                    }
                    break;
                }
            }

            if (!b.IsActive)
            {
                Bullets.RemoveAt(i);
                continue;
            }

            // Bullet vs. Players
            if (b.OwnerIndex == 2 && !Player1.IsDead && Intersects(b, Player1))
            {
                b.IsActive = false;
                Player1.TakeDamage(b.Damage);
                SpawnBloodSplatters(b.X, b.Y, b.Vx * 0.15f, b.Vy * 0.15f, 15);
                TriggerScreenShake(5f);
            }
            else if (b.OwnerIndex == 1 && !Player2.IsDead && Intersects(b, Player2))
            {
                b.IsActive = false;
                Player2.TakeDamage(b.Damage);
                SpawnBloodSplatters(b.X, b.Y, b.Vx * 0.15f, b.Vy * 0.15f, 15);
                TriggerScreenShake(5f);
            }

            if (!b.IsActive)
            {
                Bullets.RemoveAt(i);
            }
        }

        // 5. Update Grenades
        for (int i = Grenades.Count - 1; i >= 0; i--)
        {
            var g = Grenades[i];
            g.Update(dt);

            if (g.FuseTimer <= 0f)
            {
                TriggerExplosion(g.X, g.Y, 160f, 120f);
                g.IsActive = false;
                Grenades.RemoveAt(i);
                continue;
            }

            // Grenade Wall bounces
            foreach (var wall in Walls)
            {
                ResolveGrenadeWallBounce(g, wall);
            }

            // Grenade Barrel bounces
            foreach (var barrel in Barrels)
            {
                if (barrel.IsActive)
                {
                    ResolveGrenadeBarrelBounce(g, barrel);
                }
            }

            // Keep within arena bounds
            g.X = Math.Clamp(g.X, g.Radius + 20f, ArenaWidth - g.Radius - 20f);
            g.Y = Math.Clamp(g.Y, g.Radius + 20f, ArenaHeight - g.Radius - 20f);
        }

        // 6. Update Particles
        for (int i = Particles.Count - 1; i >= 0; i--)
        {
            var p = Particles[i];
            p.Update(dt);

            // Wall bounce for physical casings and sparks/shrapnel
            if (p.Type == ParticleType.Casing || p.Type == ParticleType.Spark || p.Type == ParticleType.Shrapnel)
            {
                foreach (var wall in Walls)
                {
                    ResolveParticleWallBounce(p, wall);
                }
            }

            if (!p.IsActive)
            {
                // If it's blood and it dies, render a permanent blood stain on the floor!
                if (p.Type == ParticleType.Blood)
                {
                    OnSpawnPermanentSplatter?.Invoke(p.X, p.Y, p.ParticleColor, p.Size);
                }
                Particles.RemoveAt(i);
            }
        }

        // 6b. Update SoundWaves
        for (int i = SoundWaves.Count - 1; i >= 0; i--)
        {
            var sw = SoundWaves[i];
            sw.Update(dt);
            if (!sw.IsActive)
            {
                SoundWaves.RemoveAt(i);
            }
        }

        // 6c. Update DebrisList
        for (int i = DebrisList.Count - 1; i >= 0; i--)
        {
            var deb = DebrisList[i];
            deb.Update(dt);
            foreach (var wall in Walls)
            {
                ResolveDebrisWallBounce(deb, wall);
            }
            if (!deb.IsActive)
            {
                DebrisList.RemoveAt(i);
            }
        }

        // 7. Update Weapon Crates (Respawn mechanics)
        foreach (var crate in WeaponCrates)
        {
            if (crate.RespawnTimer > 0f)
            {
                crate.RespawnTimer -= dt;
                if (crate.RespawnTimer <= 0f)
                {
                    // Select new random weapon
                    var weapons = (WeaponType[])Enum.GetValues(typeof(WeaponType));
                    crate.SpawnedWeapon = weapons[_rand.Next(weapons.Length)];
                }
            }
            else
            {
                // Intersect with active player
                if (!Player1.IsDead && Intersects(Player1, crate.Rect))
                {
                    Player1.ResetWeapon(crate.SpawnedWeapon);
                    crate.RespawnTimer = 12f; // 12 seconds respawn
                    TriggerScreenShake(3f);
                }
                else if (!Player2.IsDead && Intersects(Player2, crate.Rect))
                {
                    Player2.ResetWeapon(crate.SpawnedWeapon);
                    crate.RespawnTimer = 12f; // 12 seconds respawn
                    TriggerScreenShake(3f);
                }
            }
        }

        // Clean up dead barrels
        Barrels.RemoveAll(b => !b.IsActive);
    }

    private void UpdatePlayerState(Player p, float dt)
    {
        p.Update(dt);
        if (p.IsDead) return;

        // Apply input controls
        float vx = 0f;
        float vy = 0f;
        float speed = 230f;

        if (p.PlayerIndex == 1)
        {
            // Player 1 Movement (WASD)
            if (InputManager.IsKeyPressed(Keys.W)) vy -= 1f;
            if (InputManager.IsKeyPressed(Keys.S)) vy += 1f;
            if (InputManager.IsKeyPressed(Keys.A)) vx -= 1f;
            if (InputManager.IsKeyPressed(Keys.D)) vx += 1f;

            // Player 1 Rotation (Q / E)
            if (InputManager.IsKeyPressed(Keys.Q)) p.Angle -= 3.8f * dt;
            if (InputManager.IsKeyPressed(Keys.E)) p.Angle += 3.8f * dt;

            // Player 1 Shoot (Space)
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                TryShoot(p);
            }

            // Player 1 Reload (R)
            if (InputManager.IsKeyPressed(Keys.R))
            {
                TryReload(p);
            }

            // Player 1 Melee (F)
            if (InputManager.IsKeyPressed(Keys.F))
            {
                TryMelee(p);
            }

            // Player 1 Grenade (G)
            if (InputManager.IsKeyPressed(Keys.G))
            {
                TryThrowGrenade(p);
            }
        }
        else
        {
            // Player 2 Movement (Arrows)
            if (InputManager.IsKeyPressed(Keys.Up)) vy -= 1f;
            if (InputManager.IsKeyPressed(Keys.Down)) vy += 1f;
            if (InputManager.IsKeyPressed(Keys.Left)) vx -= 1f;
            if (InputManager.IsKeyPressed(Keys.Right)) vx += 1f;

            // Player 2 Rotation (Numpad 4/6 or [ / ] as fallback)
            if (InputManager.IsKeyPressed(Keys.NumPad4) || InputManager.IsKeyPressed(Keys.OemOpenBrackets)) p.Angle -= 3.8f * dt;
            if (InputManager.IsKeyPressed(Keys.NumPad6) || InputManager.IsKeyPressed(Keys.OemCloseBrackets)) p.Angle += 3.8f * dt;

            // Player 2 Shoot (NumpadEnter or standard Enter)
            if (InputManager.IsKeyPressed(Keys.Enter) || InputManager.IsKeyPressed(Keys.Return))
            {
                TryShoot(p);
            }

            // Player 2 Reload (Numpad 0 or Insert or I)
            if (InputManager.IsKeyPressed(Keys.NumPad0) || InputManager.IsKeyPressed(Keys.Insert) || InputManager.IsKeyPressed(Keys.I))
            {
                TryReload(p);
            }

            // Player 2 Melee (NumpadDecimal or Delete or K)
            if (InputManager.IsKeyPressed(Keys.Decimal) || InputManager.IsKeyPressed(Keys.Delete) || InputManager.IsKeyPressed(Keys.K))
            {
                TryMelee(p);
            }

            // Player 2 Grenade (NumpadPlus or Add or O)
            if (InputManager.IsKeyPressed(Keys.Add) || InputManager.IsKeyPressed(Keys.Oemplus) || InputManager.IsKeyPressed(Keys.O))
            {
                TryThrowGrenade(p);
            }
        }

        // Normalize velocity
        if (vx != 0f || vy != 0f)
        {
            float len = (float)Math.Sqrt(vx * vx + vy * vy);
            p.Vx = (vx / len) * speed;
            p.Vy = (vy / len) * speed;
        }
        else
        {
            p.Vx = 0f;
            p.Vy = 0f;
        }

        // Move player
        p.X += p.Vx * dt;
        p.Y += p.Vy * dt;
    }

    private void TryShoot(Player p)
    {
        if (p.FireCooldown > 0f || p.IsReloading) return;

        if (p.AmmoClip <= 0)
        {
            // Auto reload
            TryReload(p);
            return;
        }

        var stats = WeaponStats.GetStats(p.Weapon);
        p.FireCooldown = stats.FireInterval;
        p.AmmoClip--;

        // Gun barrel tip calculation
        float barrelLen = p.Radius + 12f;
        float spawnX = p.X + (float)Math.Cos(p.Angle) * barrelLen;
        float spawnY = p.Y + (float)Math.Sin(p.Angle) * barrelLen;

        // Spawn gun noise sound wave
        float soundRadius = p.Weapon == WeaponType.Sniper ? 390f : (p.Weapon == WeaponType.Shotgun ? 330f : 240f);
        TriggerSoundWave(spawnX, spawnY, soundRadius, 0.45f, p.PlayerColor);

        // Spawn muzzle flash sparks & smoke
        SpawnMuzzleFlash(spawnX, spawnY, p.Angle);

        // Spawn Ejected Bullet Casing
        float casingAngle = p.Angle + (float)(Math.PI / 2f + (_rand.NextDouble() - 0.5f) * 0.2f);
        float casingSpeed = 100f + (float)_rand.NextDouble() * 100f;
        float cvx = (float)Math.Cos(casingAngle) * casingSpeed + p.Vx * 0.3f;
        float cvy = (float)Math.Sin(casingAngle) * casingSpeed + p.Vy * 0.3f;
        Particles.Add(new Particle(spawnX - (float)Math.Cos(p.Angle)*10f, spawnY - (float)Math.Sin(p.Angle)*10f, cvx, cvy, Color.Gold, 2.5f, 1.2f, ParticleType.Casing));

        // Fire projectiles
        if (p.Weapon == WeaponType.Shotgun)
        {
            // 7 spreading pellets
            int pelletCount = 7;
            for (int i = 0; i < pelletCount; i++)
            {
                float spread = (float)((_rand.NextDouble() - 0.5) * 0.28); // spread angle
                float bulletAngle = p.Angle + spread;
                float speed = stats.BulletSpeed * (0.9f + (float)_rand.NextDouble() * 0.15f);
                float bvx = (float)Math.Cos(bulletAngle) * speed;
                float bvy = (float)Math.Sin(bulletAngle) * speed;
                Bullets.Add(new Bullet(spawnX, spawnY, bvx, bvy, stats.Damage, p.PlayerIndex));
            }
            TriggerScreenShake(12f);
            // Pushback recoil
            p.X -= (float)Math.Cos(p.Angle) * 8f;
            p.Y -= (float)Math.Sin(p.Angle) * 8f;
        }
        else if (p.Weapon == WeaponType.Sniper)
        {
            float bvx = (float)Math.Cos(p.Angle) * stats.BulletSpeed;
            float bvy = (float)Math.Sin(p.Angle) * stats.BulletSpeed;
            Bullets.Add(new Bullet(spawnX, spawnY, bvx, bvy, stats.Damage, p.PlayerIndex));
            TriggerScreenShake(22f);
            // Strong pushback recoil
            p.X -= (float)Math.Cos(p.Angle) * 15f;
            p.Y -= (float)Math.Sin(p.Angle) * 15f;
        }
        else
        {
            // Assault Rifle
            float spread = (float)((_rand.NextDouble() - 0.5) * 0.06);
            float bulletAngle = p.Angle + spread;
            float bvx = (float)Math.Cos(bulletAngle) * stats.BulletSpeed;
            float bvy = (float)Math.Sin(bulletAngle) * stats.BulletSpeed;
            Bullets.Add(new Bullet(spawnX, spawnY, bvx, bvy, stats.Damage, p.PlayerIndex));
            TriggerScreenShake(2.5f);
            // Tiny recoil
            p.X -= (float)Math.Cos(p.Angle) * 1.5f;
            p.Y -= (float)Math.Sin(p.Angle) * 1.5f;
        }
    }

    private void TryReload(Player p)
    {
        if (p.IsReloading || p.AmmoReserve <= 0) return;
        var stats = WeaponStats.GetStats(p.Weapon);
        if (p.AmmoClip >= stats.MaxClip) return;

        p.ReloadTimer = stats.ReloadDuration;

        // Trigger reloading sound wave
        TriggerSoundWave(p.X, p.Y, 130f, 0.45f, Color.FromArgb(150, p.PlayerColor));
    }

    private void TryMelee(Player p)
    {
        if (p.MeleeCooldown > 0f) return;
        p.MeleeCooldown = 0.5f;
        p.MeleeVisualTimer = 0.15f; // Slash arc visible for 0.15 seconds

        // Trigger melee slice sound wave
        TriggerSoundWave(p.X, p.Y, 80f, 0.3f, Color.FromArgb(120, p.PlayerColor));

        // Check if other player is hit
        Player target = p.PlayerIndex == 1 ? Player2 : Player1;
        float meleeReach = p.Radius + 30f;

        if (!target.IsDead)
        {
            float dx = target.X - p.X;
            float dy = target.Y - p.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist <= meleeReach + target.Radius)
            {
                // Check if angle is in front of slashing player (+/- 65 degrees)
                float targetAngle = (float)Math.Atan2(dy, dx);
                float angleDiff = GetAngleDifference(p.Angle, targetAngle);
                if (Math.Abs(angleDiff) <= Math.PI * 0.36)
                {
                    target.TakeDamage(100f); // Heavy/instant kill melee
                    p.Score += 200;
                    SpawnBloodSplatters(target.X, target.Y, (float)Math.Cos(p.Angle) * 150f, (float)Math.Sin(p.Angle) * 150f, 25);
                    TriggerScreenShake(12f);
                }
            }
        }

        // Check if barrels are hit
        foreach (var barrel in Barrels)
        {
            if (barrel.IsActive)
            {
                float dx = barrel.X - p.X;
                float dy = barrel.Y - p.Y;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist <= meleeReach + barrel.Radius)
                {
                    float targetAngle = (float)Math.Atan2(dy, dx);
                    float angleDiff = GetAngleDifference(p.Angle, targetAngle);
                    if (Math.Abs(angleDiff) <= Math.PI * 0.36)
                    {
                        barrel.Health -= 30f; // Instantly blows up
                        SpawnSparks(barrel.X, barrel.Y, (float)Math.Cos(p.Angle) * 80f, (float)Math.Sin(p.Angle) * 80f, Color.OrangeRed, 12);
                        if (!barrel.IsActive)
                        {
                            TriggerExplosion(barrel.X, barrel.Y, 150f, 110f);
                        }
                    }
                }
            }
        }
    }

    private void TryThrowGrenade(Player p)
    {
        if (p.GrenadeCount <= 0 || p.IsReloading) return;
        p.GrenadeCount--;

        float speed = 400f;
        float gvx = (float)Math.Cos(p.Angle) * speed + p.Vx * 0.5f;
        float gvy = (float)Math.Sin(p.Angle) * speed + p.Vy * 0.5f;

        float spawnX = p.X + (float)Math.Cos(p.Angle) * (p.Radius + 8f);
        float spawnY = p.Y + (float)Math.Sin(p.Angle) * (p.Radius + 8f);

        Grenades.Add(new Grenade(spawnX, spawnY, gvx, gvy, p.PlayerIndex));
    }

    private void TriggerExplosion(float ex, float ey, float radius, float maxDamage)
    {
        TriggerScreenShake(30f);

        // Spawn massive explosion sound wave
        TriggerSoundWave(ex, ey, 480f, 0.55f, Color.FromArgb(180, 255, 180, 0));

        // Spawn flying physical debris barrel fragments
        int debrisCount = 14;
        for (int i = 0; i < debrisCount; i++)
        {
            float angle = (float)(_rand.NextDouble() * Math.PI * 2);
            float speed = 80f + (float)_rand.NextDouble() * 240f;
            float vx = (float)Math.Cos(angle) * speed;
            float vy = (float)Math.Sin(angle) * speed;
            float size = 4f + (float)_rand.NextDouble() * 6f;
            float lifespan = 1.0f + (float)_rand.NextDouble() * 1.5f;
            Color debColor = _rand.Next(2) == 0 ? Color.FromArgb(120, 90, 60) : Color.FromArgb(160, 40, 40);
            DebrisList.Add(new Debris(ex, ey, vx, vy, size, lifespan, debColor));
        }

        // Spawn lots of fire, smoke, and shrapnel particles
        int particleCount = 45;
        for (int i = 0; i < particleCount; i++)
        {
            float angle = (float)(_rand.NextDouble() * Math.PI * 2);
            float speed = 100f + (float)_rand.NextDouble() * 320f;
            float px = ex + (float)(_rand.NextDouble() - 0.5) * 15;
            float py = ey + (float)(_rand.NextDouble() - 0.5) * 15;
            float vx = (float)Math.Cos(angle) * speed;
            float vy = (float)Math.Sin(angle) * speed;

            Color pColor = _rand.Next(3) switch
            {
                0 => Color.FromArgb(255, 60, 0),    // Dark Red/Orange
                1 => Color.FromArgb(255, 180, 0),   // Orange/Yellow
                _ => Color.FromArgb(80, 80, 80)     // Grey smoke
            };

            ParticleType type = pColor.R == 80 ? ParticleType.Smoke : ParticleType.Spark;
            float lifespan = 0.5f + (float)_rand.NextDouble() * 0.6f;
            float size = type == ParticleType.Smoke ? 8f + (float)_rand.NextDouble() * 8f : 3f + (float)_rand.NextDouble() * 3f;

            Particles.Add(new Particle(px, py, vx, vy, pColor, size, lifespan, type));
        }

        // Deal radial splash damage & apply push force to players
        ApplySplashDamageToPlayer(Player1, ex, ey, radius, maxDamage);
        ApplySplashDamageToPlayer(Player2, ex, ey, radius, maxDamage);

        // Chain reaction: damage other barrels
        foreach (var b in Barrels)
        {
            if (b.IsActive)
            {
                float dx = b.X - ex;
                float dy = b.Y - ey;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist < radius && dist > 0f)
                {
                    float factor = 1f - (dist / radius);
                    float damage = maxDamage * factor;
                    b.Health -= damage;
                    if (!b.IsActive)
                    {
                        // Explode in the next update loop, or trigger immediately
                        // Triggering immediately creates a nice chain cascade
                        TriggerExplosion(b.X, b.Y, 150f, 110f);
                    }
                }
            }
        }

        // Push nearby grenades
        foreach (var g in Grenades)
        {
            float dx = g.X - ex;
            float dy = g.Y - ey;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist < radius && dist > 0f)
            {
                float factor = 1f - (dist / radius);
                float pushForce = 400f * factor;
                g.Vx += (dx / dist) * pushForce;
                g.Vy += (dy / dist) * pushForce;
            }
        }
    }

    private void ApplySplashDamageToPlayer(Player p, float ex, float ey, float radius, float maxDamage)
    {
        if (p.IsDead) return;

        float dx = p.X - ex;
        float dy = p.Y - ey;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);

        if (dist < radius)
        {
            float factor = 1f - (dist / radius);
            float damage = maxDamage * factor;
            p.TakeDamage(damage);

            // Push player back
            if (dist > 0f)
            {
                float pushForce = 350f * factor;
                p.X += (dx / dist) * pushForce * 0.05f; // apply direct position push
                p.Y += (dy / dist) * pushForce * 0.05f;
            }

            // Spawn blood
            SpawnBloodSplatters(p.X, p.Y, (dx / (dist == 0 ? 1 : dist)) * 120f, (dy / (dist == 0 ? 1 : dist)) * 120f, (int)(15 * factor));
        }
    }

    private void ResolvePlayerCollisions(Player p)
    {
        // 1. Resolve collisions against Walls (sliding out)
        foreach (var wall in Walls)
        {
            float closestX = Math.Clamp(p.X, wall.Rect.Left, wall.Rect.Right);
            float closestY = Math.Clamp(p.Y, wall.Rect.Top, wall.Rect.Bottom);

            float dx = p.X - closestX;
            float dy = p.Y - closestY;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dist < p.Radius)
            {
                // Resolve collision by pushing player back along the normal
                if (dist > 0f)
                {
                    float overlap = p.Radius - dist;
                    p.X += (dx / dist) * overlap;
                    p.Y += (dy / dist) * overlap;
                }
                else
                {
                    // Circle center is inside the rectangle! Push towards the closest edge.
                    float distL = p.X - wall.Rect.Left;
                    float distR = wall.Rect.Right - p.X;
                    float distT = p.Y - wall.Rect.Top;
                    float distB = wall.Rect.Bottom - p.Y;

                    float minDist = Math.Min(Math.Min(distL, distR), Math.Min(distT, distB));

                    if (minDist == distL) p.X -= p.Radius;
                    else if (minDist == distR) p.X += p.Radius;
                    else if (minDist == distT) p.Y -= p.Radius;
                    else p.Y += p.Radius;
                }
            }
        }

        // 2. Resolve collisions against Barrels
        foreach (var barrel in Barrels)
        {
            if (!barrel.IsActive) continue;

            float dx = p.X - barrel.X;
            float dy = p.Y - barrel.Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            float minDist = p.Radius + barrel.Radius;

            if (dist < minDist)
            {
                if (dist > 0f)
                {
                    float overlap = minDist - dist;
                    p.X += (dx / dist) * overlap;
                    p.Y += (dy / dist) * overlap;
                }
                else
                {
                    p.X += p.Radius; // Fallback
                }
            }
        }
    }

    private void ResolvePlayerToPlayerCollision()
    {
        if (Player1.IsDead || Player2.IsDead) return;

        float dx = Player2.X - Player1.X;
        float dy = Player2.Y - Player1.Y;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
        float minDist = Player1.Radius + Player2.Radius;

        if (dist < minDist)
        {
            float overlap = minDist - dist;
            float pushX = (dx / (dist == 0f ? 1f : dist)) * overlap * 0.5f;
            float pushY = (dy / (dist == 0f ? 1f : dist)) * overlap * 0.5f;

            Player1.X -= pushX;
            Player1.Y -= pushY;
            Player2.X += pushX;
            Player2.Y += pushY;
        }
    }

    private void ResolveGrenadeWallBounce(Grenade g, Wall wall)
    {
        float closestX = Math.Clamp(g.X, wall.Rect.Left, wall.Rect.Right);
        float closestY = Math.Clamp(g.Y, wall.Rect.Top, wall.Rect.Bottom);

        float dx = g.X - closestX;
        float dy = g.Y - closestY;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);

        if (dist < g.Radius)
        {
            // Bounce it! Find which edge we hit by checking normal vector from closest point
            float overlap = g.Radius - dist;

            float nx = 0f;
            float ny = 0f;

            if (dist > 0.1f)
            {
                nx = dx / dist;
                ny = dy / dist;
            }
            else
            {
                // Center inside wall. Compute side
                float distL = g.X - wall.Rect.Left;
                float distR = wall.Rect.Right - g.X;
                float distT = g.Y - wall.Rect.Top;
                float distB = wall.Rect.Bottom - g.Y;
                float minDist = Math.Min(Math.Min(distL, distR), Math.Min(distT, distB));

                if (minDist == distL) nx = -1f;
                else if (minDist == distR) nx = 1f;
                else if (minDist == distT) ny = -1f;
                else ny = 1f;
            }

            // Adjust position out of wall
            g.X += nx * overlap;
            g.Y += ny * overlap;

            // Reflect velocity: V' = V - 2*(V.N)*N
            float dot = g.Vx * nx + g.Vy * ny;
            if (dot < 0f) // heading into the wall
            {
                g.Vx = (g.Vx - 2f * dot * nx) * 0.55f; // Bounciness friction
                g.Vy = (g.Vy - 2f * dot * ny) * 0.55f;
                g.BounceCount++;

                // Spawn bounce sparks
                SpawnSparks(closestX, closestY, nx * 30f, ny * 30f, Color.LightGray, 4);
                TriggerScreenShake(1.5f);
            }
        }
    }

    private void ResolveGrenadeBarrelBounce(Grenade g, Barrel b)
    {
        float dx = g.X - b.X;
        float dy = g.Y - b.Y;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
        float minDist = g.Radius + b.Radius;

        if (dist < minDist)
        {
            float overlap = minDist - dist;
            float nx = dx / (dist == 0f ? 1f : dist);
            float ny = dy / (dist == 0f ? 1f : dist);

            g.X += nx * overlap;

            float dot = g.Vx * nx + g.Vy * ny;
            if (dot < 0f)
            {
                g.Vx = (g.Vx - 2f * dot * nx) * 0.55f;
                g.Vy = (g.Vy - 2f * dot * ny) * 0.55f;
                g.BounceCount++;

                SpawnSparks(g.X - nx * g.Radius, g.Y - ny * g.Radius, nx * 30f, ny * 30f, Color.OrangeRed, 4);
                TriggerScreenShake(1.5f);
            }
        }
    }

    private void SpawnMuzzleFlash(float x, float y, float angle)
    {
        // Add fire sparks in the direction of the barrel
        int sparks = 8;
        for (int i = 0; i < sparks; i++)
        {
            float spAngle = angle + (float)((_rand.NextDouble() - 0.5) * 0.4);
            float speed = 120f + (float)_rand.NextDouble() * 200f;
            float vx = (float)Math.Cos(spAngle) * speed;
            float vy = (float)Math.Sin(spAngle) * speed;
            Color flashColor = Color.FromArgb(255, 200 + _rand.Next(55), 50 + _rand.Next(100), 0);
            Particles.Add(new Particle(x, y, vx, vy, flashColor, 2f + (float)_rand.NextDouble() * 2f, 0.08f + (float)_rand.NextDouble() * 0.08f, ParticleType.Spark));
        }

        // Add a small puff of smoke at barrel tip
        float smSpeed = 20f + (float)_rand.NextDouble() * 30f;
        float smAngle = angle + (float)((_rand.NextDouble() - 0.5) * 0.8);
        float svx = (float)Math.Cos(smAngle) * smSpeed;
        float svy = (float)Math.Sin(smAngle) * smSpeed;
        Particles.Add(new Particle(x, y, svx, svy, Color.FromArgb(60, 100, 100, 100), 6f, 0.4f, ParticleType.Smoke));
    }

    private void SpawnBloodSplatters(float x, float y, float baseVx, float baseVy, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Spray blood in the direction of bullet movement, with some random scattering
            float angle = (float)(Math.Atan2(baseVy, baseVx) + (_rand.NextDouble() - 0.5) * 1.2);
            float baseSpeed = (float)Math.Sqrt(baseVx * baseVx + baseVy * baseVy);
            float speed = (baseSpeed * 0.2f) + (float)_rand.NextDouble() * 120f;
            float vx = (float)Math.Cos(angle) * speed;
            float vy = (float)Math.Sin(angle) * speed;

            // Random sizes for blood drops
            float size = 2f + (float)_rand.NextDouble() * 4.5f;
            float lifespan = 0.3f + (float)_rand.NextDouble() * 0.4f;

            Color bloodColor = _rand.Next(3) == 0 
                ? Color.FromArgb(140, 0, 0) // Dark dried blood
                : Color.FromArgb(190, 10, 10); // Bright red fresh blood

            Particles.Add(new Particle(x, y, vx, vy, bloodColor, size, lifespan, ParticleType.Blood));
        }
    }

    private void SpawnSparks(float x, float y, float baseVx, float baseVy, Color color, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(Math.Atan2(baseVy, baseVx) + (_rand.NextDouble() - 0.5) * 1.5);
            float baseSpeed = (float)Math.Sqrt(baseVx * baseVx + baseVy * baseVy);
            float speed = (baseSpeed * 0.4f) + (float)_rand.NextDouble() * 160f;
            float vx = (float)Math.Cos(angle) * speed;
            float vy = (float)Math.Sin(angle) * speed;
            float size = 1.5f + (float)_rand.NextDouble() * 2f;
            float lifespan = 0.15f + (float)_rand.NextDouble() * 0.2f;

            Particles.Add(new Particle(x, y, vx, vy, color, size, lifespan, ParticleType.Spark));
        }
    }

    public void TriggerScreenShake(float intensity)
    {
        ScreenShakeIntensity = Math.Max(ScreenShakeIntensity, intensity);
    }

    private float GetAngleDifference(float a1, float a2)
    {
        float diff = a2 - a1;
        while (diff < -Math.PI) diff += (float)(Math.PI * 2);
        while (diff > Math.PI) diff -= (float)(Math.PI * 2);
        return diff;
    }

    private bool Intersects(Bullet b, Wall w)
    {
        return b.X >= w.Rect.Left && b.X <= w.Rect.Right &&
               b.Y >= w.Rect.Top && b.Y <= w.Rect.Bottom;
    }

    private bool Intersects(Bullet b, Barrel br)
    {
        float dx = b.X - br.X;
        float dy = b.Y - br.Y;
        return (dx * dx + dy * dy) <= br.Radius * br.Radius;
    }

    private bool Intersects(Bullet b, Player p)
    {
        float dx = b.X - p.X;
        float dy = b.Y - p.Y;
        return (dx * dx + dy * dy) <= p.Radius * p.Radius;
    }

    private bool Intersects(Player p, RectangleF rect)
    {
        float closestX = Math.Clamp(p.X, rect.Left, rect.Right);
        float closestY = Math.Clamp(p.Y, rect.Top, rect.Bottom);
        float dx = p.X - closestX;
        float dy = p.Y - closestY;
        return (dx * dx + dy * dy) <= p.Radius * p.Radius;
    }

    public struct LineSegment
    {
        public PointF A;
        public PointF B;
        public LineSegment(PointF a, PointF b) { A = a; B = b; }
    }

    public List<LineSegment> GetWallSegments()
    {
        var segments = new List<LineSegment>();
        foreach (var w in Walls)
        {
            segments.Add(new LineSegment(new PointF(w.Rect.Left, w.Rect.Top), new PointF(w.Rect.Right, w.Rect.Top)));
            segments.Add(new LineSegment(new PointF(w.Rect.Left, w.Rect.Bottom), new PointF(w.Rect.Right, w.Rect.Bottom)));
            segments.Add(new LineSegment(new PointF(w.Rect.Left, w.Rect.Top), new PointF(w.Rect.Left, w.Rect.Bottom)));
            segments.Add(new LineSegment(new PointF(w.Rect.Right, w.Rect.Top), new PointF(w.Rect.Right, w.Rect.Bottom)));
        }
        return segments;
    }

    public PointF[] CalculateVisionPolygon(float px, float py, float angle, int numRays = 50, float maxDist = 480f)
    {
        var segments = GetWallSegments();
        var points = new List<PointF>();
        
        // Vision cone is 120 degrees
        float fov = (float)(120.0 * Math.PI / 180.0);
        float startAngle = angle - fov / 2f;
        float step = fov / (numRays - 1);

        for (int i = 0; i < numRays; i++)
        {
            float a = startAngle + step * i;
            float dx = (float)Math.Cos(a);
            float dy = (float)Math.Sin(a);

            float closestT = 1f; // relative parameter along the maxDist ray
            float hitX = px + dx * maxDist;
            float hitY = py + dy * maxDist;

            foreach (var seg in segments)
            {
                float rx = dx * maxDist;
                float ry = dy * maxDist;
                float sx = seg.B.X - seg.A.X;
                float sy = seg.B.Y - seg.A.Y;

                float denom = rx * sy - ry * sx;
                if (Math.Abs(denom) > 0.0001f)
                {
                    float t = ((seg.A.X - px) * sy - (seg.A.Y - py) * sx) / denom;
                    float u = ((seg.A.X - px) * ry - (seg.A.Y - py) * rx) / denom;

                    if (t >= 0f && t <= closestT && u >= 0f && u <= 1f)
                    {
                        closestT = t;
                        hitX = px + rx * t;
                        hitY = py + ry * t;
                    }
                }
            }

            points.Add(new PointF(hitX, hitY));
        }

        points.Insert(0, new PointF(px, py));
        points.Add(new PointF(px, py));

        return points.ToArray();
    }

    public void TriggerSoundWave(float x, float y, float maxRadius, float lifespan, Color color)
    {
        SoundWaves.Add(new SoundWave(x, y, maxRadius, lifespan, color));
    }

    private void ResolveParticleWallBounce(Particle p, Wall wall)
    {
        float closestX = Math.Clamp(p.X, wall.Rect.Left, wall.Rect.Right);
        float closestY = Math.Clamp(p.Y, wall.Rect.Top, wall.Rect.Bottom);

        float dx = p.X - closestX;
        float dy = p.Y - closestY;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
        float radius = p.Size / 2f;

        if (dist < radius)
        {
            float overlap = radius - dist;
            float nx = 0f;
            float ny = 0f;

            if (dist > 0.05f)
            {
                nx = dx / dist;
                ny = dy / dist;
            }
            else
            {
                float distL = p.X - wall.Rect.Left;
                float distR = wall.Rect.Right - p.X;
                float distT = p.Y - wall.Rect.Top;
                float distB = wall.Rect.Bottom - p.Y;
                float minDist = Math.Min(Math.Min(distL, distR), Math.Min(distT, distB));

                if (minDist == distL) nx = -1f;
                else if (minDist == distR) nx = 1f;
                else if (minDist == distT) ny = -1f;
                else ny = 1f;
            }

            p.X += nx * overlap;
            float dot = p.Vx * nx + p.Vy * ny;
            if (dot < 0f)
            {
                float coeff = p.Type == ParticleType.Casing ? 0.35f : 0.6f;
                p.Vx = (p.Vx - 2f * dot * nx) * coeff;
                p.Vy = (p.Vy - 2f * dot * ny) * coeff;
                if (p.Type == ParticleType.Casing)
                {
                    p.RotationSpeed = (float)((_rand.NextDouble() - 0.5) * 15f);
                }
            }
        }
    }

    private void ResolveDebrisWallBounce(Debris d, Wall wall)
    {
        float closestX = Math.Clamp(d.X, wall.Rect.Left, wall.Rect.Right);
        float closestY = Math.Clamp(d.Y, wall.Rect.Top, wall.Rect.Bottom);

        float dx = d.X - closestX;
        float dy = d.Y - closestY;
        float dist = (float)Math.Sqrt(dx * dx + dy * dy);
        float radius = d.Size / 2f;

        if (dist < radius)
        {
            float overlap = radius - dist;
            float nx = 0f;
            float ny = 0f;

            if (dist > 0.05f)
            {
                nx = dx / dist;
                ny = dy / dist;
            }
            else
            {
                float distL = d.X - wall.Rect.Left;
                float distR = wall.Rect.Right - d.X;
                float distT = d.Y - wall.Rect.Top;
                float distB = wall.Rect.Bottom - d.Y;
                float minDist = Math.Min(Math.Min(distL, distR), Math.Min(distT, distB));

                if (minDist == distL) nx = -1f;
                else if (minDist == distR) nx = 1f;
                else if (minDist == distT) ny = -1f;
                else ny = 1f;
            }

            d.X += nx * overlap;
            float dot = d.Vx * nx + d.Vy * ny;
            if (dot < 0f)
            {
                d.Vx = (d.Vx - 2f * dot * nx) * 0.4f;
                d.Vy = (d.Vy - 2f * dot * ny) * 0.4f;
                d.RotationSpeed = (float)((_rand.NextDouble() - 0.5) * 10f);
            }
        }
    }
}
