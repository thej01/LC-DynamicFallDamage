using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DynamicFallDamage.DynamicFallDamageMod;
using static DynamicFallDamage.Logger;
using BepInEx.Logging;


namespace DynamicFallDamage.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
        [HarmonyPrefix]
        public static void PreHitGround(ref PlayerControllerB __instance)
        {

            if (__instance.takingFallDamage)
            {
                float fallRange = fallValueRangeMax - fallValueRangeMin;
                float damageRange = fallDamageMax - fallDamageMin;

                float newFallValue = Math.Abs(__instance.fallValueUncapped);

                // screw you *caps your fallValueUncapped*
                if (newFallValue > fallValueRangeMax)
                    newFallValue = fallValueRangeMax;

                // soo for some reason, takingFallDamage is set to true if fallValueUncapped is <= -35
                // which is weird because the minimum fall damage is -38
                // so we just... don't take fall damage if we shouldn't (shocking)
                if (newFallValue < fallValueRangeMin)
                    return;
                
                // i dont even know whats happening here i stole from stack overflow 
                int fallDamageAmount = (int)((newFallValue - fallValueRangeMin) * damageRange / fallRange) + fallDamageMin;

                if (fallDamageAmount < 0)
                {
                    fallDamageAmount = 0;
                    fallLogger.Log("Fall damage was less than zero! This should never happen!", LogLevel.Warning, LogLevelConfig.Important);
                }

                string msg = String.Format("Fall Damage (the better one): {0} Fall Value: {1}, Capped Fall Value: {2}", 
                                            fallDamageAmount, __instance.fallValueUncapped, newFallValue);

                fallLogger.Log(msg, LogLevel.Info, LogLevelConfig.Everything);

                __instance.DamagePlayer(fallDamageAmount, true, true, CauseOfDeath.Gravity, 0, false, default(Vector3));

            }

            __instance.takingFallDamage = false;
        }
    }
}
