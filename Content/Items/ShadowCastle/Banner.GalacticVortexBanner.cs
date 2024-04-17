﻿using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;

namespace Coralite.Content.Items.ShadowCastle
{
    public class GalacticVortexBanner : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadowCastleBannerTile>(), 1);
        }
    }
}
