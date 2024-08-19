﻿using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class SandlitePowder : BaseMaterial, IMagikeCraftable
    {
        public SandlitePowder() : base(9999, Item.sellPrice(0, 0, 2, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<SandlitePowder>(15)
                .SetMainItem<MagicalPowder>()
                .AddIngredient(ItemID.SandBlock, 6)
                .Register();
        }
    }
}
