﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Tile
{
    public abstract class BaseRefractorTile<T>(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile<T>(width, height, mapColor, dustType, minPick, topSoild)
        where T : BaseMagikeTileEntity
    {
        public override void DrawExtraTex(SpriteBatch spriteBatch,Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, BaseMagikeTileEntity entity)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = tileRect.Height / 2;
            float rotation = 0;

            //虽然一般不会没有 但是还是检测一下
            if (!(entity as IEntity).TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender senderComponent))
                return;

            if (senderComponent.IsEmpty())
                drawPos += new Vector2(0, halfHeight - 8);
            else
            {
                Point16 p = senderComponent.FirstConnector();
                Vector2 targetPos = Helper.GetTileCenter(p);
                rotation = (targetPos - selfCenter).ToRotation() + MathHelper.PiOver2;
            }

            // 绘制主帖图
            spriteBatch.Draw(tex, drawPos, null, lightColor, rotation, tex.Size() / 2, 1f, 0, 0f);
        }
    }
}
