using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using xTile;

namespace FourCorners_Improved
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            ModPatches.Initialize(Monitor); 
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo performWarpFarmer = this.Helper.Reflection.GetMethod(typeof(Game1), "performWarpFarmer").MethodInfo;

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(performWarpFarmer)),
                prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.WarpFarmerPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.DayUpdate)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.DayUpdatePostfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.getFish)),
                prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.getFishPrefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.getFish)),
               postfix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.getFishPostfix))
           );
        
        }
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/Farm_FourCorners"))
            {
                e.LoadFromModFile<Map>("assets/Farm_FourCorners.tmx", AssetLoadPriority.Medium);
            }
        }
    }

}