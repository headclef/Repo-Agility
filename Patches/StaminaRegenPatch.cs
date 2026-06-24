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

            // Calculate the base regen rate from the formula
            float regenPerSecond = Agility.BaseRegenPerSecond.Value
                                 + (Agility.RegenPerCombinedLevel.Value * combinedLevel);

            // The cap applies to the base (standing-still) rate, before the
            // movement multiplier — so it acts as the "natural" regen ceiling.
            float maxRegen = Agility.MaxRegenPerSecond.Value;
            if (maxRegen > 0f)
                regenPerSecond = Math.Min(regenPerSecond, maxRegen);

            if (regenPerSecond <= 0f)
                return;

            // Scale the base rate by the player's current movement state.
            float movementMultiplier = GetMovementMultiplier(__instance, out string movementState);
            float effectiveRegen = regenPerSecond * movementMultiplier;

            // No regen this state (e.g. running) — nothing to do.
            if (effectiveRegen <= 0f)
                return;

            // Apply regen this frame
            float regenThisFrame = effectiveRegen * Time.deltaTime;
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
                    $"Stamina regen: {effectiveRegen:F2}/sec ({movementState} x{movementMultiplier:F2}, " +
                    $"base {regenPerSecond:F2}/sec, combined level: {combinedLevel}, " +
                    $"stamina: {staminaLevel}, crouch: {crouchRestLevel}, speed: {speedLevel})");
            }
        }
        catch (Exception ex)
        {
            Agility.Logger.LogError($"StaminaRegen exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Returns the stamina-regen multiplier for the player's current movement state.
    /// Priority is most-active first: sprinting → walking → standing still, with a
    /// crouch bonus layered on top of standing still ("resting").
    ///   • sprinting/running         → SprintingMultiplier      (default 0   — no regen)
    ///   • walking (moving on foot)  → WalkingMultiplier        (default 0.5 — half)
    ///   • standing still            → StandingMultiplier       (default 1   — full)
    ///   • crouching + standing still→ CrouchingStillMultiplier (default 2   — double)
    /// </summary>
    private static float GetMovementMultiplier(PlayerController player, out string movementState)
    {
        // Running / sprinting — exerting, so no passive regen by default.
        if (player.sprinting)
        {
            movementState = "sprinting";
            return Agility.SprintingMultiplier.Value;
        }

        // Walking — moving on foot but not sprinting.
        if (player.moving)
        {
            movementState = "walking";
            return Agility.WalkingMultiplier.Value;
        }

        // Standing still and crouching — resting, the strongest regen.
        if (player.Crouching)
        {
            movementState = "crouching-still";
            return Agility.CrouchingStillMultiplier.Value;
        }

        // Standing still, upright.
        movementState = "standing";
        return Agility.StandingMultiplier.Value;
    }
}
