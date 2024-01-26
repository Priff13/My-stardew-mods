using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace FourCorners_Improved
{
    internal class ModPatches
    {
        // Used to log errors
        private static IMonitor? Monitor;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        /*
         * Many of the changes that are not listed here are simply stated in the map file.
         * Thankfully, a lot of the attributes of the map can be controlled by adding a property
         * to the map file.
         * 
         * Note: 
         * Internally, the farm types are represented as an integer ID as such:
         * 
         * |----------------------------------------------------|
         * | In-game name       | ID    | Internal name         |
         * |====================================================|
         * | Standard Farm      | 0     | "Farm"                |
         * |----------------------------------------------------|
         * | Riverland Farm     | 1     | "Farm_Fishing"        |
         * |----------------------------------------------------|
         * | Forest Farm        | 2     | "Farm_Foraging"       |
         * |----------------------------------------------------|
         * | Hilltop Farm       | 3     | "Farm_Mining"         |
         * |----------------------------------------------------|
         * | Wilderness Farm    | 4     | "Farm_Combat"         |
         * |----------------------------------------------------|
         * | Four Corners Farm  | 5     | "Farm_FourCorners"    |
         * |----------------------------------------------------|
         * | Beach Farm         | 6     | "Farm_Beach"          |
         * |----------------------------------------------------|
         * 
         * From:
         *      public static string StardewValley.Farm.getMapNameFromTypeInt(int type)
         *      
         * To make this work, a few attributes had to be added into the map file.
         * These include:
         *      "FishingRect": "{x0} {y0} {x1} {y1}"
         *      "RespawnStumpsRect": "{x0} {y0} {x1} {y1}"
         */

        /*
         * Because of the specifics of the fish spawns in the base game, I had to implement a sort of "hack" to make it to work consistently
         * What I did, is that I made sure that the fishing tile was in the riverland region,
         * and then changed the farm type to the riverland farm, thus spawning the good fish.
         * Immediately afterwards, I set the farm type back to the original.
         */
        public static void getFishPrefix(ref StardewValley.Object __result, out bool __state, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string? location = null)
        {
            __state = false;
            if (!Game1.getFarm().map.Properties.ContainsKey("FourCorners_Improved"))
            {
                return;
            }
            if (Game1.player.currentLocation.Name != "Farm")
            {
                return;
            }
            if (Game1.whichFarm != 5)
            {
                return;
            }
            if (!Game1.getFarm().map.Properties.ContainsKey("FishingRect"))
            {
                return;
            }

            int x0, y0, x1, y1;
            string[] rectsplit = Game1.getFarm().map.Properties["FishingRect"].ToString().Split(' ');
            if (
                rectsplit.Length == 4 &&
                int.TryParse(rectsplit[0], out x0) &&
                int.TryParse(rectsplit[1], out y0) &&
                int.TryParse(rectsplit[2], out x1) &&
                int.TryParse(rectsplit[3], out y1))
            {
                if ((bobberTile.X > x0) && (x1 > bobberTile.X))
                {
                    if ((bobberTile.Y > y0) && (y1 > bobberTile.Y))
                    {
                        Game1.whichFarm = 1; // Switch to Riverland Farm to catch good fish 
                        __state = true;      // Only set if and only if the farm type was temporatily changed
                    }
                }
            }
        }
        public static void getFishPostfix(bool __state)
        {
            if (__state)
            {
                Game1.whichFarm = 5;    // Switch back to FourCorners Farm
            }
        }

        // Used to respawn stumps on the forest region within a specific rectangle
        public static void DayUpdatePostfix(int dayOfMonth)
        {
            if (Game1.whichFarm != 5)
            {
                return;
            }
            if (!Game1.getFarm().map.Properties.ContainsKey("RespawnStumpsRect"))
            {
                return;
            }


            int x0, y0, x1, y1;
            string[] rectsplit = Game1.getFarm().map.Properties["RespawnStumpsRect"].ToString().Split(' ');
            if (rectsplit.Length == 4 &&
                int.TryParse(rectsplit[0], out x0) &&
                int.TryParse(rectsplit[1], out y0) &&
                int.TryParse(rectsplit[2], out x1) &&
                int.TryParse(rectsplit[3], out y1))
            {

                for (int x = x0; x < x1; x++)
                {
                    for (int y = y0; y < y1; y++)
                    {
                        if (Game1.getFarm().map.GetLayer("Paths").Tiles[x, y] != null &&
                            Game1.getFarm().map.GetLayer("Paths").Tiles[x, y].TileIndex == 21 &&
                            Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x, y) &&
                            Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x + 1, y) &&
                            Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x + 1, y + 1) &&
                            Game1.getFarm().isTileLocationTotallyClearAndPlaceable(x, y + 1))
                        {
                            Game1.getFarm().resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2(x, y))); // Stump
                        }
                    }
                }
            }
        }


        /*
         * In the base game, the warp locations for entering the farm are always hardcoded.
         * This caused issues that you would not be placed in the correct location when entering the farm.
         * I solved this by overriding the warp locations before the warp is performed.
         */
        public static void WarpFarmerPrefix(LocationRequest locationRequest, ref int tileX, ref int tileY, int facingDirectionAfterWarp)
        {
            if (Game1.whichFarm != 5)
            {
                return;
            }
            if (locationRequest.Name != "Farm")
            {
                return;
            }
            switch (Game1.player.currentLocation.Name)
            {
                case "FarmCave":
                    tileX = 79;
                    tileY = 98;
                    break;
                case "Forest":
                    tileX = 55;
                    tileY = 109;
                    break;
                case "Backwoods":
                    tileX = 45;
                    tileY = 1;
                    break;
                case "BusStop":
                    tileX = 109;
                    tileY = 18;
                    break;
                default:
                    break;
            }
        }
    }
}
