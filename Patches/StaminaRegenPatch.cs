using System;
using static Character_Stats.Character_Stats;
using HarmonyLib;
using UnityEngine;

namespace Agility.Patches;

[HarmonyPatch]
internal static class StaminaRegenPatch
{
    private static float _lastLogTime;

    /// <summary>
    /// Postfix on PlayerController.Update — adds passive stamina regen each frame
    /// based on the combined level of Stamina + Crouch Rest + Speed upgrades.
    /// </summary>
    [HarmonyPatch(typeof(PlayerController), "Update")]
    [HarmonyPostfix]
    private static void PlayerController_Update_Postfix(PlayerController __instance)
    {
        if (!Agility.EnableStaminaRegen.Value)
            return;

        if (!AreStatsReady)
            return;

        try
        {
            // Only apply to the local player
            if (__instance != PlayerController.instance)
                return;

            string? steamId = GetLocalSteamId();
            if (steamId == null)
                return;

            // Read stat levels from Character Stats
            int staminaLevel = GetUpgradeLevel(steamId, "Stamina");
            int crouchRestLevel = GetUpgradeLevel(steamId, "Crouch Rest");
            int speedLevel = GetUpgradeLevel(steamId, "Speed");
            int combinedLevel = staminaLevel + crouchRestLevel + speedLevel;

            if (combinedLevel <= 0 && Agility.BaseRegenPerSecond.Value <= 0f)
                return;

            // Calculate regen rate
            float regenPerSecond = Agility.BaseRegenPerSecond.Value
                                 + (Agility.RegenPerCombinedLevel.Value * combinedLevel);

            float maxRegen = Agility.MaxRegenPerSecond.Value;
            if (maxRegen > 0f)
                regenPerSecond = Math.Min(regenPerSecond, maxRegen);

            if (regenPerSecond <= 0f)
                return;

            // Apply regen this frame
            float regenThisFrame = regenPerSecond * Time.deltaTime;
            float currentEnergy = __instance.EnergyCurrent;
            float maxEnergy = __instance.EnergyStart;

            // Don't regen if already at max
            if (currentEnergy >= maxEnergy)
                return;

            __instance.EnergyCurrent = Math.Min(currentEnergy + regenThisFrame, maxEnergy);

            // Log periodically (every 10 seconds) to avoid spam
            if (Time.time - _lastLogTime > 10f)
            {
                _lastLogTime = Time.time;
                Agility.Logger.LogDebug(
                    $"Stamina regen: {regenPerSecond:F2}/sec (combined level: {combinedLevel}, " +
                    $"stamina: {staminaLevel}, crouch: {crouchRestLevel}, speed: {speedLevel})");
            }
        }
        catch (Exception ex)
        {
            Agility.Logger.LogError($"StaminaRegen exception: {ex.Message}");
        }
    }
}
