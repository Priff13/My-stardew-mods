using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Threading.Channels;
using StardewValley.TerrainFeatures;

namespace FourCorners_Improved
{
    internal class ModPatches
    {
        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }


        //quarry location: (3,3) --> (14,8)

        //warps needed:
        /*
         * forest below farm -->            (55, 109)   {tiles}         (864,1744)  {actual position}   "Forest"
         * woods above farm (backwoods) --> (45, 1)     {tiles}         (720, 16)   {actual position}   "Backwoods"
         * bus stop -->                     (109,18)    {tiles}         (1744, 288) {actual position}   "BusStop"
         * 
         * farm called:     "Farm"
         * farm cave --> (79, 98) called "FarmCave"
         */
        /*
        public const int default_layout = 0;

        public const int riverlands_layout = 1;

        public const int forest_layout = 2;

        public const int mountains_layout = 3;

        publ4ic const int combat_layout = 4;

        public const int fourCorners_layout = 5;

        public const int beach_layout = 6;

        public const int mod_layout = 7;
        */
        //public override Object getFish(float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string location = null)

        public static void getFishPrefix(ref StardewValley.Object __result, out bool __state, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string? location = null)
        {
            __state = false;
            if (Game1.player.currentLocation.Name != "Farm")
            {
                return;
            }
            if (Game1.whichFarm != 5)
            {
                return;
            }
            Game1.whichFarm = 1;
            __state = true;
        }
        public static void getFishPostfix(bool __state)
        {
            if (__state)
            {
                Game1.whichFarm = 5;
            }
        }
        public static void DayUpdatePostfix(int dayOfMonth)
        {
            if (Game1.whichFarm != 5)
            {
                return;
            }
            //replenish those big stumps
            //beach farm crates

            //replenish the stumps in the forest section (heheoasudhfpiuashdlifuahsilejfsaildjnfasidhfasdjf)
            //add "RespawnStumpsRect" to map file <int x0, int y0, int x1, int y1>
            if (Game1.getFarm().map.Properties.ContainsKey("RespawnStumpsRect"))
            {
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
                                Game1.getFarm().resourceClumps.Add(new ResourceClump(600, 2, 2, new Vector2(x, y)));
                            }
                        }
                    }
                }
            }
        
            //beach farm crates thingiesisieisieise

        }
        public static void WarpFarmerPrefix(LocationRequest locationRequest, ref int tileX, ref int tileY, int facingDirectionAfterWarp)
        {
            if (Game1.whichFarm == 5) //four corners farm layout = 5, internally
            {
                if (locationRequest.Name == "Farm")
                {
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


    }
}
