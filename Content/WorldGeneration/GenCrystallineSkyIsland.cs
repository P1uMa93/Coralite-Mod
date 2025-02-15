﻿using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Content.Walls.Magike;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 蕴魔空岛的范围
        /// </summary>
        public static Rectangle CrystallineSkyIslandArea { get; set; }
        /// <summary>
        /// 是否放置光明之魂
        /// </summary>
        public static bool PlaceLightSoul { get; set; }
        /// <summary>
        /// 是否放置暗影之魂
        /// </summary>
        public static bool PlaceNightSoul { get; set; }
        /// <summary>
        /// 是否有权限进入蕴魔空岛
        /// </summary>
        public static bool HasPermission { get; set; }

        public void GenCrystallineSkyIsland(GenerationProgress progress, GameConfiguration configuration)
        {
            //生成地表结构
            GenGroundLock(out Point altarPoint);
            //生成主体空岛
            GenMainSkyIsland(altarPoint);
        }

        public void GenGroundLock(out Point altarPoint)
        {
            //找到丛林，在地表处选择一个地方
            //Point searchOrigin = new Point((int)(Main.LocalPlayer.Center.X / 16), 0);
            Point p = new Point(0, 0);

            //在丛林中心寻找一个位置
            p = new Point(PickAltarX(), (int)(Main.worldSurface * 0.4f));

            for (int j = 0; j < 1000; j++)//向下遍历，找到地面
            {
                Tile t = Main.tile[p.X, p.Y];
                if (t.HasTile && Main.tileSolid[t.TileType])//找到实心方块
                    break;

                p.Y++;
            }

            altarPoint = p+new Point(0,-8);

            ushort skarn = (ushort)ModContent.TileType<SkarnTile>();
            ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();

            ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "SkarnAltars", AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "SkarnAltarsClear", AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "SkarnAltarsWall", AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallClearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "SkarnAltarsWallClear", AssetRequestMode.ImmediateLoad).Value;

            p -= new Point(shrineTex.Width / 2, shrineTex.Height / 2);

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = skarn,//334c75
                [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
                [new Color(184, 230, 207)] = skarnBrick,//b8e6cf

                [new Color(105, 97, 90)] = ModContent.TileType<BasaltBeamTile>(),//69615a
                [new Color(63, 76, 73)] = ModContent.TileType<BasaltTile>(),//3f4c49
                [new Color(166, 166, 166)] = ModContent.TileType<HardBasaltTile>(),//a6a6a6

                [new Color(71, 56, 53)] = TileID.Mud,//473835

                [new Color(241, 130, 255)] = crystallineBrick,//f182ff
                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = ModContent.WallType<SmoothSkarnWallUnsafe>(),//55b7ce
                [new Color(29, 30, 28)] = ModContent.WallType<HardBasaltWall>(),//1d1e1c
                [Color.Black] = -1
            };

            GenByTexture(clearTex, shrineTex, wallClearTex, wallTex, clearDic, mainDic, clearDic, wallDic, p.X, p.Y);

            WorldGen.PlaceObject(p.X + 7, p.Y + 6, ModContent.TileType<SoulOfNightAltarTile>());
            WorldGen.PlaceObject(p.X + 19, p.Y + 4, ModContent.TileType<PremissionAltarTile>());
            WorldGen.PlaceObject(p.X + 33, p.Y + 6, ModContent.TileType<SoulOfLightAltarTile>());

            GenVars.structures.AddProtectedStructure(new Rectangle(p.X, p.Y, shrineTex.Width, shrineTex.Height));

            //p就是中心点，放置主祭坛
            //altarPoint = p;

            //ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            //ushort beam = (ushort)ModContent.TileType<BasaltBeamTile>();

            //for (int j = -1; j < 2; j += 2)//防止底部空
            //    if (!Main.tile[p.X + j, p.Y + 2].HasTile)
            //    {
            //        WorldGen.KillTile(p.X - 1, p.Y + 2);
            //        Main.tile[p.X - 1, p.Y + 2].ResetToType(basalt);
            //    }

            //int h = WorldGen.genRand.Next(3, 5);

            //for (int i = -1; i < h; i++)//放置两条玄武岩柱子
            //{
            //    Main.tile[p.X - 1, p.Y - i].ResetToType(beam);
            //    Main.tile[p.X + 1, p.Y - i].ResetToType(beam);
            //}

            ////放置一条玄武岩
            //for (int i = -2; i < 3; i++)
            //    Main.tile[p.X + i, p.Y - h + 1].ResetToType(basalt);

            //Point topP = p + new Point(-1, -h - 2);
            //for (int i = 0; i < 3; i++)
            //    for (int j = 0; j < 3; j++)
            //    {
            //        WorldGen.KillTile(topP.X + i, topP.Y + j);
            //    }

            ////放置主要祭坛
            //WorldGen.PlaceTile(topP.X + 1, topP.Y + 2, ModContent.TileType<PremissionAltarTile>(), true);
        }

        public void GenMainSkyIsland(Point altarPos)
        {
            //矽卡岩
            ushort skarn = (ushort)ModContent.TileType<SkarnTile>();
            ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();

            //光滑矽卡岩墙
            ushort skarnWall = (ushort)ModContent.WallType<SmoothSkarnWallUnsafe>();
            ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            //随机一下主岛的尺寸
            int baseX = 50;
            int xPlus = 15;

            int baseY = 60;
            int yPlus = 22;

            Point mainIslandSize = Main.maxTilesY switch
            {
                //小世界
                < 6000 => new Point(WorldGen.genRand.Next(baseX, baseX + xPlus), WorldGen.genRand.Next(baseY, baseY + yPlus)),
                //中世界
                > 6000 and < 8000 => new Point(WorldGen.genRand.Next(baseX + xPlus, baseX + xPlus * 2), WorldGen.genRand.Next(baseY + yPlus, baseY + yPlus * 2)),
                //大世界
                _ => new Point(WorldGen.genRand.Next(baseX + xPlus * 2, baseX + xPlus * 3), WorldGen.genRand.Next(baseY + yPlus * 2, baseY + yPlus * 3))
            };

            //主岛的中心点和左上角
            Point mainIslandTopLeft = new Point(altarPos.X - mainIslandSize.X / 2, WorldGen.genRand.Next(60, 80));
            Point mainIslandCenter = new Point(altarPos.X, mainIslandTopLeft.Y + mainIslandSize.Y / 2);

            #region 生成主体

            ShapeData mainData = new ShapeData();
            ShapeData OutlineData = new ShapeData();

            //生成主体矩形
            WorldUtils.Gen(
                mainIslandTopLeft,
                new Shapes.Rectangle(mainIslandSize.X, mainIslandSize.Y),
                Actions.Chain(
                     new Modifiers.Blotches(4)
                     , new Actions.SetTile(skarn)
                     , new Actions.SetFrames().Output(mainData)));

            //获取外边缘的形状
            WorldUtils.Gen(
                mainIslandTopLeft,
                new ModShapes.InnerOutline(mainData),
                Actions.Chain(
                    new Modifiers.Expand(3, 3)  //扩展这个边缘线，得到一个圆环
                    , new Modifiers.OnlyTiles(skarn).Output(OutlineData)));

            //两个相减，得到内部的形状，并放置背景墙
            WorldUtils.Gen(
                mainIslandTopLeft,
                new ModShapes.All(mainData),
                Actions.Chain(
                    new Modifiers.NotInShape(OutlineData)
                    , new Modifiers.Blotches(4)
                    , new Actions.PlaceWall(skarnWall)));

            #endregion

            #region 随机加一些突起和凹坑
            int count = ValueByWorldSize(16, 20, 26);

            for (int i = 0; i < count; i++)
            {
                bool add = WorldGen.genRand.NextBool();
                //随机在边缘上取点
                Point p1 = WorldGen.genRand.NextInRectangleEdge(mainIslandTopLeft, mainIslandSize);

                int r = WorldGen.genRand.Next(2, 5);

                WorldUtils.Gen(
                    p1,
                    new Shapes.Circle(r),
                    Actions.Chain(
                         new Modifiers.Blotches(2)
                         //new Modifiers.Dither()
                         , add ? new Actions.SetTile(skarn) : new Actions.ClearTile()
                         , new Actions.SetFrames(true)));
            }

            #endregion

            int xExpand = 8;
            int yExpand = 8;
            Point mainIslandOutTopLeft = mainIslandTopLeft - new Point(xExpand, yExpand);
            Point mainIslandOutSize = mainIslandSize + new Point(xExpand * 2, yExpand * 2);
            //空岛外缘的矩形
            Rectangle outerRect = new Rectangle(mainIslandOutTopLeft.X, mainIslandOutTopLeft.Y, mainIslandOutSize.X, mainIslandOutSize.Y);

            #region 用平滑矽卡岩画条纹

            int x = (int)(WorldGen.genRand.NextFloat() * mainIslandOutSize.X * 5);
            int y = (int)(WorldGen.genRand.NextFloat() * mainIslandOutSize.Y * 5);
            int size = Math.Max(mainIslandOutSize.X, mainIslandOutSize.Y);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    Tile t = Main.tile[mainIslandCenter.X - size / 2 + i, mainIslandCenter.Y - size / 2 + j];
                    if (!t.HasTile)
                        continue;

                    float mainNoise = MainNoise(new Vector2(x + i, y + j), new Vector2(size) * 5);
                    if (mainNoise > 0.8f)
                        t.ResetToType(smoothSkarn);
                }

            #endregion

            #region 随机挖通道，与中心小遗迹生成

            //主要通道，用于生成小遗迹
            int type = WorldGen.genRand.Next(0, 1);

            Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MainSkyIslandShrine" + type, AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MainSkyIslandShrineClear" + type, AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MainSkyIslandShrineWall" + type, AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallClearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MainSkyIslandShrineWallClear" + type, AssetRequestMode.ImmediateLoad).Value;

            int width = shrineTex.Width;
            int height = shrineTex.Height;
            /* 一一一一一一一一一一一
             * |    一一一一      |
             * |    |      |     |
             * |    |      |     |
             * |    |      |     |
             * |    |      |     |
             * |    一一一一      |
             * 一一一一一一一一一一一
             * 
             * 获取里面这个矩形
             */
            Rectangle innerRect = new Rectangle(mainIslandTopLeft.X + width, mainIslandTopLeft.Y + height, mainIslandSize.X - width * 2, mainIslandSize.Y - height * 2);

            //遗迹的中心
            Point shrineCenter = WorldGen.genRand.NextVector2FromRectangle(innerRect).ToPoint();
            Point shrineTopLeft = new Point(shrineCenter.X - width / 2, shrineCenter.Y - height / 2);
            //遗迹的尺寸
            Rectangle shrineRect = new Rectangle(shrineTopLeft.X, shrineTopLeft.Y, width, height);

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = skarn,//334c75
                [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
                [new Color(184, 230, 207)] = skarnBrick,//b8e6cf

                [new Color(255, 239, 219)] = ModContent.TileType<ChalcedonyTile>(),//ffefdb
                [new Color(170, 228, 143)] = ModContent.TileType<LeafChalcedonyTile>(),//aae48f

                [new Color(241, 130, 255)] = crystallineBrick,//f182ff
                [new Color(90, 100, 80)] = TileID.Chain,//5a6450

                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = skarnWall,//55b7ce
                [new Color(188, 171, 150)] = ModContent.WallType<ChalcedonyWallUnsafe>(),//bcab96
                [new Color(113, 128, 131)] = ModContent.WallType<SkarnBrickWallUnsafe>(),//718083
                [Color.Black] = -1
            };

            //生成遗迹
            GenByTexture(clearTex, shrineTex, wallClearTex, wallTex, clearDic, mainDic, clearDic, wallDic, shrineTopLeft.X, shrineTopLeft.Y);

            //向哪边开通道
            int tunnelDir = WorldGen.genRand.NextFromList(-1, 1);

            //挖通道的中心点
            Point tunnelCenter = tunnelDir > 0 ? (shrineTopLeft + new Point(27, 12)) : (shrineTopLeft + new Point(0, 12));

            DigSkyIslandTunnel(outerRect, shrineRect, tunnelCenter, 1, GetTunnelLength, true);

            #endregion



            //ushort crystallineSkarn = (ushort)ModContent.TileType<CrystallineSkarnTile>();
            //ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            //ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();

            //ushort chalcedony = (ushort)ModContent.TileType<ChalcedonyTile>();
            //ushort leafChalcedony = (ushort)ModContent.TileType<LeafChalcedonyTile>();

            //ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            //ushort skarnBrickPlatform = (ushort)ModContent.TileType<SkarnBrickPlatformTile>();

            //Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIsland" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;
            //Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIslandClear" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;
            //Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIslandWall" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;

            //int genOrigin_x = Main.maxTilesX / 2 - (clearTex.Width / 2);
            //int genOrigin_y = 200 - (clearTex.Height / 2);

            //Dictionary<Color, int> clearDic = new()
            //{
            //    [Color.White] = -2,
            //    [Color.Black] = -1
            //};
            //Dictionary<Color, int> mainDic = new()
            //{
            //    [new Color(51, 76, 117)] = skarn,//334c75
            //    [new Color(165, 58, 255)] = crystallineSkarn,//a53aff
            //    [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
            //    [new Color(184, 230, 207)] = skarnBrick,//b8e6cf

            //    [new Color(255, 239, 219)] = chalcedony,//ffefdb
            //    [new Color(170, 228, 143)] = leafChalcedony,//aae48f

            //    [new Color(241, 130, 255)] = crystallineBrick,//f182ff
            //    [new Color(90, 100, 80)] = TileID.Chain,//5a6450
            //    [Color.Black] = -1
            //};
            //Dictionary<Color, int> wallDic = new()
            //{
            //    [new Color(85, 183, 206)] = ModContent.WallType<Walls.Magike.SmoothSkarnWallUnsafe>(),//55b7ce
            //    [new Color(188, 171, 150)] = ModContent.WallType<Walls.Magike.ChalcedonyWallUnsafe>(),//bcab96
            //    [Color.Black] = -1
            //};

            //GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, genOrigin_x, genOrigin_y);


            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<CrystallineStalactite>(), () => 1, 3, 0);
            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<CrystallineStalactite2x2>(), () => 1, 3, 0);

            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles1x1>(), () => 1, 3, 0);
            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles2x1>(), () => 1, 3, 0);
            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles2x2>(), () => 1, 3, 0);
            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles3x2>(), () => 1, 3, 0);
            //WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
            //    , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles4x2>(), () => 1, 3, 0);
        }

        private int GetTunnelLength()
        {
            return ValueByWorldSize(
                            WorldGen.genRand.Next(8, 16)
                            , WorldGen.genRand.Next(12, 20)
                            , WorldGen.genRand.Next(12, 28));
        }

        /// <summary>
        /// 从指定点向外挖出通道
        /// </summary>
        /// <param name="outerRect">最外围的矩形</param>
        /// <param name="shrineRect">需要避让的矩形</param>
        /// <param name="tunnelCenter">从哪开始挖</param>
        /// <param name="digType">初始挖掘类型， 横向或竖向，0为竖向，1为横向</param>
        /// <param name="tunnelLength">初始挖多长</param>
        public void DigSkyIslandTunnel(Rectangle outerRect,Rectangle shrineRect, Point tunnelCenter,int digType,Func<int> getTunnelLength,bool ignoreFirstDig=false)
        {
            int tunnelDir = WorldGen.genRand.NextFromList(-1, 1);
            int tunnelWidth = WorldGen.genRand.Next(5, 8);
            int tunnelLength = getTunnelLength();
            int digCount = 0;

            while (outerRect.Contains(tunnelCenter))
            {
                switch (digType)
                {
                    default:
                    case 0:
                        {
                            //
                            for (; tunnelLength > 0; tunnelLength--)
                            {
                                int yTop = tunnelCenter.Y - tunnelWidth / 2 + WorldGen.genRand.Next(-1, 2);

                                //遇到遗迹了就退出
                                if (!(ignoreFirstDig && digCount == 0))
                                    if (shrineRect.Intersects(new Rectangle(tunnelCenter.X, yTop, 1, tunnelWidth)))
                                        return;

                                for (int i = 0; i < tunnelWidth; i++)
                                    Main.tile[tunnelCenter.X, yTop + i].Clear(TileDataType.Tile);

                                tunnelCenter.X += tunnelDir;
                            }

                            //竖向挖完就横向挖
                            digType = 1;
                            tunnelDir = WorldGen.genRand.NextFromList(-1, 1);
                            tunnelLength = getTunnelLength();
                            digCount++;
                        }
                        break;
                    case 1:
                        {
                            for (; tunnelLength > 0; tunnelLength--)
                            {
                                int xLeft = tunnelCenter.X - tunnelWidth / 2 + WorldGen.genRand.Next(-1, 2);

                                if (!(ignoreFirstDig && digCount == 0))
                                    if (shrineRect.Intersects(new Rectangle(xLeft, tunnelCenter.Y, tunnelWidth, 1)))
                                        return;

                                for (int i = 0; i < tunnelWidth; i++)
                                    Main.tile[xLeft + i, tunnelCenter.Y].Clear(TileDataType.Tile);

                                tunnelCenter.X += tunnelDir;
                            }

                            //横向挖完就竖向挖
                            digType = 1;
                            tunnelDir = WorldGen.genRand.NextFromList(-1, 1);
                            tunnelLength = getTunnelLength();
                            digCount++;
                        }

                        break;
                }
            }
        }

        public T ValueByWorldSize<T>(T smallWorld,T middleWorld,T bigWorld)
        {
            return Main.maxTilesY switch
            {
                //小世界
                < 6000 => smallWorld,
                //中世界
                > 6000 and < 8000 => middleWorld,
                //大世界
                _ => bigWorld
            };
        }

        /// <summary>
        /// 传入位置，并找到最高点，实心块向上找，空心块向下找
        /// </summary>
        /// <param name="point"></param>
        public void FindTop(ref Point point)
        {
            Tile selfTile = Framing.GetTileSafely(point);

            if (selfTile.HasTile && Main.tileSolid[selfTile.TileType])//实心块，向上查找
            {
                for (int i = 0; i < 100; i++)
                {
                    point.Y--;
                    Tile upTile = Framing.GetTileSafely(point);
                    if (!upTile.HasTile || !Main.tileSolid[upTile.TileType])//找到没有物块的地方了，返回
                        return;
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    point.Y++;
                    Tile upTile = Framing.GetTileSafely(point);
                    if (upTile.HasTile && Main.tileSolid[upTile.TileType])//找到有物块的地方了，返回
                        return;
                }
            }
        }

        /// <summary>
        /// 随机找丛林中心附近的X
        /// </summary>
        /// <returns></returns>
        public int PickAltarX()
        {
            return GenVars.jungleOriginX + WorldGen.genRand.Next(-60, 60);
        }


        #region 蕴魔空岛使用到的噪声方法

        float Random(Vector2 st)
        {
            float a = MathF.Sin(Vector2.Dot(st, new Vector2(12.9898f, 78.233f))) * 43758.5453123f;
            int a2 = (int)a;
            return a2 - a;
        }

        // Value noise by Inigo Quilez - iq/2013
        // https://www.shadertoy.com/view/lsf3WH
        float Noise(Vector2 st)
        {
            Vector2 i = new Vector2((float)Math.Floor(st.X), (float)Math.Floor(st.Y));
            Vector2 f = new Vector2(st.X - i.X, st.Y - i.Y);
            Vector2 u = f * f * (new Vector2(3f) - 2f * f);

            float a = Random(i);
            float b = Random(i + new Vector2(1f, 0f));
            float c = Random(i + new Vector2(0f, 1f));
            float d = Random(i + new Vector2(1f, 1f));

            float x1 = Helper.Lerp(a, b, u.X);
            float x2 = Helper.Lerp(c, d, u.X);

            return Helper.Lerp(x1, x2, u.Y);
        }

        // 2D旋转矩阵
        private Matrix Rotate2D(Vector2 pos, float angle)
        {
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);
            return new Matrix(
                cosTheta, -sinTheta, 0f, 0f,
                sinTheta, cosTheta, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f
            );
        }

        private float Lines(Vector2 pos, float b)
        {
            float scale = 10f;
            pos *= scale;
            return SmoothStep(0f, 0.5f + b * 0.5f, Math.Abs((float)Math.Sin(pos.X * Math.PI) + b * 2f) * 0.5f);
        }

        private float SmoothStep(float edge0, float edge1, float x)
        {
            float t = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        public float MainNoise(Vector2 selfPos, Vector2 size)
        {
            Vector2 st = selfPos / size;
            st.Y *= size.Y / size.X;

            Vector2 pos = new Vector2(st.Y, st.X) * new Vector2(7f, 8f);

            float pattern = pos.X;

            // 添加噪声并旋转
            float noiseValue = Noise(pos);
            pos = Vector2.Transform(pos, Rotate2D(pos, noiseValue));
            
            // 绘制线条
            return pattern = Lines(pos, 0.5f);
        }

        #endregion


        public void SaveSkyIsland(TagCompound tag)
        {
            if (PlaceLightSoul)
                tag.Add(nameof(PlaceLightSoul), true);
            if (PlaceNightSoul)
                tag.Add(nameof(PlaceNightSoul), true);
            if (HasPermission)
                tag.Add(nameof(HasPermission), true);
        }

        public void LoadSkyIsland(TagCompound tag)
        {
            PlaceLightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            PlaceNightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            HasPermission = tag.ContainsKey(nameof(PlaceLightSoul));
        }
    }
}
