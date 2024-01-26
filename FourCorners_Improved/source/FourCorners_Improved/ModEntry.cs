using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;

namespace FourCorners_Improved
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            ModPatches.Initialize(Monitor); 
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            // DANGER: I had to patch a private method in order to fix the warp targets to the farm
            // Will most likely break the game in a future patch, BWARE
            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo performWarpFarmer = this.Helper.Reflection.GetMethod(typeof(Game1), "performWarpFarmer").MethodInfo;

            /*
             * To be honest, there is probably a better way to do this, but this is what I did, and it works.
             * The whole schtick of the Four Corners Farm is that every corner includes toned-down gimmicks from the other maps.
             * This farm however, does not actually have corners, but is more like distinct regions.
             * Because the gimmicks were hardcoded into the base game's source code, I had to include these code patches.
             */

            // Changes where you warp to on the farm when entering
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(performWarpFarmer)),
                prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.WarpFarmerPrefix))
            ); 

            // Controls the respawning stumps in the forest region, and the quarry in the mountain region
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.DayUpdate)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.DayUpdatePostfix))
            );

            // Controls the adjusted fish spawns in the riverland region
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.getFish)),
                prefix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.getFishPrefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.getFish)),
               postfix: new HarmonyMethod(typeof(ModPatches), nameof(ModPatches.getFishPostfix))
            );
        
        }

        // Used to load the map file when requested
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/Farm_FourCorners"))
            {
                e.LoadFromModFile<Map>("assets/Farm_FourCorners.tmx", AssetLoadPriority.Medium);
            }
        }
    }

}