# Agility

A [BepInEx](https://github.com/BepInEx/BepInEx) mod for **R.E.P.O.** that adds passive stamina regeneration based on your agility-related upgrade levels.

## What This Mod Does

Your **Stamina**, **Crouch Rest**, and **Sprint Speed** upgrades now contribute to a passive stamina regeneration rate. The more you invest in these agility stats, the faster your stamina recovers — making high-mobility builds even more rewarding in longer runs.

### How It Works

The combined level of your three agility stats determines your regen rate:

| Stamina | Crouch Rest | Speed | Combined | Regen/sec (default) |
|---------|-------------|-------|----------|---------------------|
| 1       | 0           | 0     | 1        | 0.3/sec             |
| 2       | 1           | 1     | 4        | 1.2/sec             |
| 3       | 2           | 2     | 7        | 2.1/sec             |
| 5       | 3           | 3     | 11       | 3.3/sec             |

> The regeneration is subtle and designed to be useful over time — it won't make you invincible, but it rewards investing in agility stats.

### Movement States

Regen now depends on what you're doing. The rate from the table above is your **base** rate; your current movement state multiplies it, so you recover fastest while resting and not at all while running:

| State                       | Multiplier (default) | Effect                     |
|-----------------------------|----------------------|----------------------------|
| Running (sprinting)         | `x0`                 | No regen while sprinting   |
| Walking (moving)            | `x0.5`               | Half regen on the move     |
| Standing still              | `x1`                 | Full base regen            |
| Crouching + standing still  | `x2`                 | Double regen while resting |

> Example: with a 3.3/sec base rate, you regen **0/sec** while sprinting, **1.65/sec** while walking, **3.3/sec** standing still, and **6.6/sec** crouched and still. All four multipliers are configurable.

## Configuration

Settings are in `BepInEx/config/headclef.Agility.cfg`:

**[Stamina Regeneration]**

| Key | Default | Description |
|-----|---------|-------------|
| Enable | `true` | Toggle stamina regen on/off |
| Base Regen Per Second | `0` | Flat regen regardless of stats (0 = none) |
| Regen Per Combined Level | `0.3` | Extra regen per combined agility level |
| Max Regen Per Second | `0` | Cap the base (standing-still) rate before the movement multiplier (0 = no cap) |

**[Movement Multipliers]**

| Key | Default | Description |
|-----|---------|-------------|
| Sprinting Multiplier | `0` | Regen multiplier while running |
| Walking Multiplier | `0.5` | Regen multiplier while walking |
| Standing Multiplier | `1` | Regen multiplier while standing still |
| Crouching Still Multiplier | `2` | Regen multiplier while crouching and still |

## Requirements

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx) installed for R.E.P.O.
- [Character Stats](https://github.com/headclef/Repo-CharacterStats) — required dependency

## Installation

1. Install via **Thunderstore** (recommended) — Character Stats will be installed automatically.
2. Or manually: place both `Character Stats.dll` and `Agility.dll` into your `BepInEx/plugins` folder.
3. Launch the game — config file is generated on first run.

## Multiplayer

- Stamina regeneration runs **per-client** — only players with the mod installed get the regen.
- Does not affect other players or enemies.

## Changelog

### v1.2.0

- **Movement-aware regen** — stamina now recovers based on what you're doing: none while running, half while walking, full while standing still, and double while crouched and still.
- Added a **[Movement Multipliers]** config section so each state's multiplier can be tuned.
- The **Max Regen Per Second** cap now applies to the base (standing-still) rate before the movement multiplier.

### v1.1.0

- Passive stamina regen scaling from the combined level of Stamina + Crouch Rest + Speed upgrades (via Character Stats).

## Development

### Project Structure
```
├── Agility.cs                     # Plugin entry point & config
├── Patches/
│   └── StaminaRegenPatch.cs       # Harmony postfix — stamina regen
└── README.md
```

### Building

This project is part of the `Repo.slnx` solution and references Character Stats at compile time. Build the **whole solution** so dependencies build in the correct order:

```bash
dotnet build ../Repo.slnx
```

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
