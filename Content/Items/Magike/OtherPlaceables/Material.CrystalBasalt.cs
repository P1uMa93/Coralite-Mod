﻿using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class CrystalBasalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystalBasaltTile>());
            Item.rare = ModContent.RarityType<MagikeCrystalRarity>();
        }

    }
}
