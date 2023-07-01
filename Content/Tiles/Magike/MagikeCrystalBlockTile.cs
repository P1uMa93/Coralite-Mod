﻿using Coralite.Core;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Magike
{
    public class MagikeCrystalBlockTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 600;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<BasaltTile>()] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(Coralite.Instance.MagicCrystalPink);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<BasaltTile>(), true, true, false);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.05f;
            b = 0.1f;
        }

    }
}
