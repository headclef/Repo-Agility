using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Agility;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("headclef.CharacterStats", BepInDependency.DependencyFlags.HardDependency)]
public class Agility : BaseUnityPlugin
{
    private const string PluginGuid = "headclef.Agility";
    private const string PluginName = "Agility";
    private const string PluginVersion = "1.2.1";

    internal static Agility Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    // ── Config ──
    internal static ConfigEntry<bool> EnableStaminaRegen = null!;
    internal static ConfigEntry<float> BaseRegenPerSecond = null!;
    internal static ConfigEntry<float> RegenPerCombinedLevel = null!;
    internal static ConfigEntry<float> MaxRegenPerSecond = null!;

    // ── Movement Multipliers ──
    internal static ConfigEntry<float> SprintingMultiplier = null!;
    internal static ConfigEntry<float> WalkingMultiplier = null!;
    internal static ConfigEntry<float> StandingMultiplier = null!;
    internal static ConfigEntry<float> CrouchingStillMultiplier = null!;

    private void Awake()
    {
        Instance = this;
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        BindConfiguration();
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private void OnDestroy()
    {
        Harmony?.UnpatchSelf();
    }

    private void BindConfiguration()
    {
        const string section = "Stamina Regeneration";

        EnableStaminaRegen = Config.Bind(section, "Enable", true,
            "Enable passive stamina regeneration based on agility stats.");

        BaseRegenPerSecond = Config.Bind(section, "Base Regen Per Second", 0f,
            new ConfigDescription(
                "Base stamina regeneration per second regardless of stats. 0 = no base regen.",
                new AcceptableValueRange<float>(0f, 10f)));

        RegenPerCombinedLevel = Config.Bind(section, "Regen Per Combined Level", 0.3f,
            new ConfigDescription(
                "Additional stamina regeneration per second for each combined level of Stamina + Crouch Rest + Speed upgrades.",
                new AcceptableValueRange<float>(0f, 5f)));

        MaxRegenPerSecond = Config.Bind(section, "Max Regen Per Second", 0f,
            new ConfigDescription(
                "Maximum stamina regeneration per second. Caps the base (standing-still) rate before the movement multiplier is applied. 0 = no cap.",
                new AcceptableValueRange<float>(0f, 50f)));

        const string movementSection = "Movement Multipliers";

        SprintingMultiplier = Config.Bind(movementSection, "Sprinting Multiplier", 0f,
            new ConfigDescription(
                "Stamina regen multiplier while sprinting/running. Default 0 = no regen while running.",
                new AcceptableValueRange<float>(0f, 5f)));

        WalkingMultiplier = Config.Bind(movementSection, "Walking Multiplier", 0.5f,
            new ConfigDescription(
                "Stamina regen multiplier while walking (moving on foot but not sprinting). Default 0.5 = half regen.",
                new AcceptableValueRange<float>(0f, 5f)));

        StandingMultiplier = Config.Bind(movementSection, "Standing Multiplier", 1f,
            new ConfigDescription(
                "Stamina regen multiplier while standing still. Default 1 = full regen.",
                new AcceptableValueRange<float>(0f, 5f)));

        CrouchingStillMultiplier = Config.Bind(movementSection, "Crouching Still Multiplier", 2f,
            new ConfigDescription(
                "Stamina regen multiplier while crouching and standing still (resting). Default 2 = double regen.",
                new AcceptableValueRange<float>(0f, 5f)));
    }
}
