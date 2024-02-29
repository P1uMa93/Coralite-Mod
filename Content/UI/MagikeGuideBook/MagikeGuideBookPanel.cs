﻿using Coralite.Content.UI.MagikeGuideBook.Chapter1;
using Coralite.Content.UI.MagikeGuideBook.Chapter2;
using Coralite.Content.UI.MagikeGuideBook.Chapter3;
using Coralite.Content.UI.MagikeGuideBook.Chapter4;
using Coralite.Content.UI.MagikeGuideBook.Introduce;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook
{
    public class MagikeGuideBookPanel : UI_BookPanel
    {
        public MagikeGuideBookPanel() : base(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 38, 100, 50, 50, 1)
        {

        }

        public override void InitPageGroups()
        {
            pageGroups = new BookUI.UIPageGroup[5]
            {
                new IntroduceGroup(),
                new Chapter1Group(),
                new Chapter2Group(),
                new Chapter3Group(),
                new Chapter4Group(),
            };

            InitGroups();
        }
    }
}
