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

                float newFallValue = __instance.fallValueUncapped;

                // screw you *caps your fallValueUncapped*
                if (newFallValue > fallValueRangeMax)
                    newFallValue = fallValueRangeMax;

                // i dont even know whats happening here i stole from stack overflow 
                int fallDamageAmount = (int)((Math.Abs(newFallValue) - fallValueRangeMin) * damageRange / fallRange) + fallDamageMin;

                string msg = String.Format("Fall Damage (the better one): {0} Fall Value: {1}, Capped Fall Value: {2}", 
                                            fallDamageAmount, __instance.fallValueUncapped, newFallValue);

                fallLogger.Log(msg);

                __instance.DamagePlayer(fallDamageAmount, true, true, CauseOfDeath.Gravity, 0, false, default(Vector3));

            }

            __instance.takingFallDamage = false;
        }
    }
}
