﻿using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Crimson
{
    public class VertebraeKey : ModItem
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.width = 16;
            Item.height = 16;
            Item.placeStyle = 0;
            Item.maxStack = 99;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
