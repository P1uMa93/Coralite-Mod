﻿using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Coralite.Content.Items.ShadowItems
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadowHead : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子兜帽");
            Tooltip.SetDefault("最大魔力值增加40\n魔法伤害增加8%");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<ShadowChest>() && legs.type == ItemType<ShadowLegs>();
        }


    }

    [AutoloadEquip(EquipType.Body)]
    public class ShadowChest : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子胸甲");
            Tooltip.SetDefault("最大魔力值增加20\n魔法暴击率增加4%");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 8;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class ShadowLegs : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子护腿");
            Tooltip.SetDefault("魔力消耗减少10%\n魔法暴击率增加6%");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6;
        }
    }
}
