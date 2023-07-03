﻿using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class HardBasaltWall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Walls.Magike.HardBasaltWall>());
        }
    }
}
