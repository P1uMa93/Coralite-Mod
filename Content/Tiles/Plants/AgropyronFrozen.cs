﻿using Coralite.Content.Items.BotanicalItems.Plants;
using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class AgropyronFrozen : BasePlantTileWithSeed<NormalPlantTileEntity>
    {
        public AgropyronFrozen() : base(AssetDirectory.PlantTiles, 24, 3, ItemType<AgropyronSeed>(), ItemType<AgropyronFreezer>()) { }

        public override void SetStaticDefaults()
        {
            (this).PlantPrefab<NormalPlantTileEntity>(new int[] { TileID.IceBlock,TileID.SnowBlock }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 22 }, -6, SoundID.Grass, DustID.Grass, Color.Green, 24, "寒霜冰草", 1, 0);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(2);
            seedItemStack = Main.rand.Next(3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(4);

            seedItemStack = Main.rand.Next(3);
        }
    }
}
