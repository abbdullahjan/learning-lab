using System;
using System.Collections.Generic;
using System.Drawing;

namespace CyberStrike;

public enum WeaponType
{
    AssaultRifle,
    Shotgun,
    Sniper
}

public enum ParticleType
{
    Blood,
    Casing,
    Smoke,
    Spark,
    Shrapnel
}

public class WeaponStats
{
    public string Name { get; }
    public int MaxClip { get; }
    public float FireInterval { get; }
    public float ReloadDuration { get; }
    public float BulletSpeed { get; }
    public float Damage { get; }

    public WeaponStats(string name, int maxClip, float fireInterval, float reloadDuration, float bulletSpeed, float damage)
    {
        Name = name;
        MaxClip = maxClip;
        FireInterval = fireInterval;
        ReloadDuration = reloadDuration;
        BulletSpeed = bulletSpeed;
        Damage = damage;
    }

    public static WeaponStats GetStats(WeaponType type) => type switch
    {
        WeaponType.AssaultRifle => new WeaponStats("Assault Rifle", 30, 0.12f, 1.5f, 900f, 15f),
        WeaponType.Shotgun => new WeaponStats("Shotgun", 8, 0.8f, 2.0f, 750f, 12f), // 7 pellets
        WeaponType.Sniper => new WeaponStats("Sniper Rifle", 5, 1.5f, 2.5f, 1600f, 100f),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}

public class Player
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Radius { get; set; } = 18f;
    public float Angle { get; set; } // in Radians
    public float Health { get; set; } = 100f;
    public float MaxHealth { get; } = 100f;
    public int PlayerIndex { get; } // 1 or 2
    public bool IsSpecialOps { get; } // true for P1, false for P2
    public Color PlayerColor { get; }

    // Weapon State
    public WeaponType Weapon { get; private set; } = WeaponType.AssaultRifle;
    public int AmmoClip { get; set; }
    public int AmmoReserve { get; set; }

    // Actions & Timers
    public int GrenadeCount { get; set; } = 3;
    public int Score { get; set; } = 0;
    public float FireCooldown { get; set; } = 0f;
    public float ReloadTimer { get; set; } = 0f;
    public float MeleeCooldown { get; set; } = 0f;
    public float MeleeVisualTimer { get; set; } = 0f; // For slash arc drawing
    public float TimeSinceLastDamage { get; set; } = 0f;
    
    public bool IsReloading => ReloadTimer > 0f;
    public bool IsDead => Health <= 0f;

    public Player(int playerIndex, float startX, float startY, Color color)
    {
        PlayerIndex = playerIndex;
        X = startX;
        Y = startY;
        PlayerColor = color;
        IsSpecialOps = playerIndex == 1;
        ResetWeapon(WeaponType.AssaultRifle);
    }

    public void ResetWeapon(WeaponType newWeapon)
    {
        Weapon = newWeapon;
        var stats = WeaponStats.GetStats(newWeapon);
        AmmoClip = stats.MaxClip;
        AmmoReserve = newWeapon switch
        {
            WeaponType.AssaultRifle => 90,
            WeaponType.Shotgun => 24,
            WeaponType.Sniper => 10,
            _ => 60
        };
        ReloadTimer = 0f;
    }

    public void RefillAmmo()
    {
        var stats = WeaponStats.GetStats(Weapon);
        AmmoClip = stats.MaxClip;
        AmmoReserve = Weapon switch
        {
            WeaponType.AssaultRifle => 120,
            WeaponType.Shotgun => 32,
            WeaponType.Sniper => 15,
            _ => 60
        };
    }

    public void TakeDamage(float damage)
    {
        Health = Math.Max(0f, Health - damage);
        TimeSinceLastDamage = 0f;
    }

    public void Update(float dt)
    {
        if (IsDead) return;

        TimeSinceLastDamage += dt;

        // Health Regeneration: If avoid damage for 3.5 seconds
        if (TimeSinceLastDamage >= 3.5f && Health < MaxHealth)
        {
            Health = Math.Min(MaxHealth, Health + 25f * dt); // Regenerates 25 HP/sec
        }

        // Fire Cooldown
        if (FireCooldown > 0f)
            FireCooldown -= dt;

        // Reload
        if (ReloadTimer > 0f)
        {
            ReloadTimer -= dt;
            if (ReloadTimer <= 0f)
            {
                var stats = WeaponStats.GetStats(Weapon);
                int needed = stats.MaxClip - AmmoClip;
                int transfer = Math.Min(needed, AmmoReserve);
                AmmoClip += transfer;
                AmmoReserve -= transfer;
            }
        }

        // Melee cooldown
        if (MeleeCooldown > 0f)
            MeleeCooldown -= dt;

        // Melee animation timer
        if (MeleeVisualTimer > 0f)
            MeleeVisualTimer -= dt;
    }
}

public class Bullet
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Radius { get; set; } = 3f;
    public float Damage { get; set; }
    public int OwnerIndex { get; set; }
    public bool IsActive { get; set; } = true;
    public Queue<PointF> Trail { get; } = new();

    public Bullet(float x, float y, float vx, float vy, float damage, int ownerIndex)
    {
        X = x;
        Y = y;
        Vx = vx;
        Vy = vy;
        Damage = damage;
        OwnerIndex = ownerIndex;
    }

    public void Update(float dt)
    {
        // Store trail point before moving
        Trail.Enqueue(new PointF(X, Y));
        if (Trail.Count > 6)
            Trail.Dequeue();

        X += Vx * dt;
        Y += Vy * dt;
    }
}

public class Grenade
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Radius { get; set; } = 6f;
    public float FuseTimer { get; set; } = 2.5f; // 2.5 seconds fuse
    public int OwnerIndex { get; set; }
    public bool IsActive { get; set; } = true;
    public int BounceCount { get; set; } = 0;
    public float Rotation { get; set; }

    public Grenade(float x, float y, float vx, float vy, int ownerIndex)
    {
        X = x;
        Y = y;
        Vx = vx;
        Vy = vy;
        OwnerIndex = ownerIndex;
    }

    public void Update(float dt)
    {
        X += Vx * dt;
        Y += Vy * dt;
        FuseTimer -= dt;

        // Apply friction to sliding grenade
        Vx *= (1f - 0.5f * dt);
        Vy *= (1f - 0.5f * dt);

        // Rotation spin based on velocity
        float speed = (float)Math.Sqrt(Vx * Vx + Vy * Vy);
        Rotation += speed * 0.05f * dt;
    }
}

public class Barrel
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Radius { get; set; } = 20f;
    public float Health { get; set; } = 30f;
    public float MaxHealth { get; } = 30f;
    public bool IsActive => Health > 0f;

    public Barrel(float x, float y)
    {
        X = x;
        Y = y;
    }
}

public class Wall
{
    public RectangleF Rect { get; set; }
    public Color WallColor { get; set; }

    public Wall(float x, float y, float w, float h, Color color)
    {
        Rect = new RectangleF(x, y, w, h);
        WallColor = color;
    }
}

public class WeaponCrate
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; } = 32f;
    public float Height { get; set; } = 32f;
    public WeaponType SpawnedWeapon { get; set; }
    public float RespawnTimer { get; set; } = 0f;
    public bool IsActive => RespawnTimer <= 0f;

    public RectangleF Rect => new RectangleF(X - Width / 2f, Y - Height / 2f, Width, Height);

    public WeaponCrate(float x, float y, WeaponType weapon)
    {
        X = x;
        Y = y;
        SpawnedWeapon = weapon;
    }
}

public class Particle
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public Color ParticleColor { get; set; }
    public float Size { get; set; }
    public float Lifespan { get; set; }
    public float MaxLifespan { get; }
    public ParticleType Type { get; }
    public float Angle { get; set; }
    public float RotationSpeed { get; set; }

    public bool IsActive => Lifespan > 0f;

    public Particle(float x, float y, float vx, float vy, Color color, float size, float lifespan, ParticleType type)
    {
        X = x;
        Y = y;
        Vx = vx;
        Vy = vy;
        ParticleColor = color;
        Size = size;
        Lifespan = lifespan;
        MaxLifespan = lifespan;
        Type = type;

        if (type == ParticleType.Casing)
        {
            var rand = new Random();
            Angle = (float)(rand.NextDouble() * Math.PI * 2);
            RotationSpeed = (float)((rand.NextDouble() - 0.5) * 30.0);
        }
    }

    public void Update(float dt)
    {
        X += Vx * dt;
        Y += Vy * dt;
        Lifespan -= dt;

        if (Type == ParticleType.Casing)
        {
            // Ejected casings slide with friction and bounce/spin
            Vx *= (1f - 4f * dt);
            Vy *= (1f - 4f * dt);
            Angle += RotationSpeed * dt;
            RotationSpeed *= (1f - 2f * dt);
        }
        else if (Type == ParticleType.Smoke)
        {
            // Smoke floats slowly, expands, and rises/spreads
            Vx *= (1f - 1.5f * dt);
            Vy *= (1f - 1.5f * dt);
            Size += 8f * dt; // Expansion
        }
        else if (Type == ParticleType.Blood)
        {
            // Blood drops slow down rapidly on concrete
            Vx *= (1f - 5f * dt);
            Vy *= (1f - 5f * dt);
        }
        else if (Type == ParticleType.Spark || Type == ParticleType.Shrapnel)
        {
            Vx *= (1f - 2f * dt);
            Vy *= (1f - 2f * dt);
        }
    }
}

public class SoundWave
{
    public float X { get; set; }
    public float Y { get; set; }
    public float CurrentRadius { get; set; }
    public float MaxRadius { get; set; }
    public float Lifespan { get; set; }
    public float MaxLifespan { get; }
    public Color WaveColor { get; set; }

    public bool IsActive => Lifespan > 0f;

    public SoundWave(float x, float y, float maxRadius, float lifespan, Color color)
    {
        X = x;
        Y = y;
        CurrentRadius = 0f;
        MaxRadius = maxRadius;
        Lifespan = lifespan;
        MaxLifespan = lifespan;
        WaveColor = color;
    }

    public void Update(float dt)
    {
        Lifespan -= dt;
        if (Lifespan < 0f) Lifespan = 0f;
        float ageRatio = 1f - (Lifespan / MaxLifespan);
        CurrentRadius = MaxRadius * ageRatio;
    }
}

public class Debris
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Vx { get; set; }
    public float Vy { get; set; }
    public float Size { get; set; }
    public float Lifespan { get; set; }
    public float MaxLifespan { get; }
    public float Angle { get; set; }
    public float RotationSpeed { get; set; }
    public Color DebrisColor { get; set; }

    public bool IsActive => Lifespan > 0f;

    public Debris(float x, float y, float vx, float vy, float size, float lifespan, Color color)
    {
        X = x;
        Y = y;
        Vx = vx;
        Vy = vy;
        Size = size;
        Lifespan = lifespan;
        MaxLifespan = lifespan;
        DebrisColor = color;

        var rand = new Random();
        Angle = (float)(rand.NextDouble() * Math.PI * 2);
        RotationSpeed = (float)((rand.NextDouble() - 0.5) * 20.0);
    }

    public void Update(float dt)
    {
        X += Vx * dt;
        Y += Vy * dt;
        Lifespan -= dt;

        // Decelerate over time
        Vx *= (1f - 2f * dt);
        Vy *= (1f - 2f * dt);

        Angle += RotationSpeed * dt;
        RotationSpeed *= (1f - dt);
    }
}
