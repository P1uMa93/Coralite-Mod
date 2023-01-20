﻿using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Tiles
{
    public abstract class BasePlantTile:ModTile
    {
        public readonly short FrameWidth;
        public readonly int FrameCount;

        public BasePlantTile(short FrameWidth,int FrameCount)
        {
            this.FrameWidth = FrameWidth;
            this.FrameCount = FrameCount;
        }
    }
}
