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

## Configuration

Settings are in `BepInEx/config/headclef.Agility.cfg`:

| Key | Default | Description |
|-----|---------|-------------|
| Enable | `true` | Toggle stamina regen on/off |
| Base Regen Per Second | `0` | Flat regen regardless of stats (0 = none) |
| Regen Per Combined Level | `0.3` | Extra regen per combined agility level |
| Max Regen Per Second | `0` | Cap the regen rate (0 = no cap) |

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

## Development

### Project Structure
```
├── Agility.cs                     # Plugin entry point & config
├── Patches/
│   └── StaminaRegenPatch.cs       # Harmony postfix — stamina regen
└── README.md
```

### Building
```bash
dotnet build "../Character Stats/Character Stats.csproj"
dotnet build
```

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
