using FashionSense.Framework.Models.Appearances.Pants;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace FashionSense.Framework.Patches.Entities
{
    internal class FarmerPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Farmer);

        internal FarmerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Farmer.DrawShadow), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawShadowPrefix)));
        }

        private static bool DrawShadowPrefix(Farmer __instance, SpriteBatch b)
        {
            if (__instance is not null && AppearanceHelpers.GetCurrentlyEquippedModels(__instance, __instance.FacingDirection).Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
