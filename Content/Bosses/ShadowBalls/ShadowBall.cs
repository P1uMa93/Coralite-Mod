﻿using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    ///                                               马赛克
    ///           ○○○○○○○○○○ ○                        l   l  
    ///       ○○○○○○○○○○○○○○○○○ ○                     l   l
    ///     ○○○○○○○○○○○○○○○○○○○○○ ○              _ _  l   l_  
    ///    ○○○○○{影}○○○○○○○○○○○{球}○ ○          !  !  l   l l ˉl
    ///   ○○○○○{影影影}○○子○○○{球球球}○ ○        l               l
    /// ○○○○○○○○{影}○○○○○○○○○○○{球}○○○ ○        l               l
    /// ○○○○○○○○○○○○○○○○○○○○○○○○○○○○○ ○          l             l
    ///  ○○○○○○==○○○○○○○○○○○○○○○○○○○ ○           l            l
    ///    ○○○○○○==○○○○○○○○○○○○○○○○ ○            l           l
    ///     ○○○○○○○=========○○○○○ ○              l           l
    ///       ○○○○○○○○○○○○○○○○○ ○
    ///           ○○○○○○○○○○ ○
    /// 
    ///             就贼搁赤玉灵嗷，别让我在影之城看见你嗷，
    ///                 抓到你，指定没你好果汁吃
    ///                     你记住我说的话嗷！
    /// 
    /// </summary>
    public partial class ShadowBall : ModNPC
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        internal ref float Phase => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];

        public Player Target => Main.player[NPC.target];

        public bool SpawnedSmallBalls;
        public List<NPC> smallBalls = new List<NPC>();
        public int smallBallCount;

        public Rectangle MovementLimitRect;
        /// <summary>
        /// 生成时自下而上出现的高度
        /// </summary>
        public float SpawnOverflowHeight;
        //public bool CanDamage; 

        private Player ShadowPlayer;

        private static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            //NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            //NPC.BossBar = GetInstance<BabyIceDragonBossBar>();

            //BGM：冰结寒流
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((3820 + numPlayers * 1750) / journeyScale);
                    NPC.damage = 35;
                    NPC.defense = 12;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((4720 + numPlayers * 2100) / journeyScale);
                    NPC.damage = 60;
                    NPC.defense = 15;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 15;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 3820 + numPlayers * 1750;
            NPC.damage = 35;
            NPC.defense = 12;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + numPlayers * 2100;
                NPC.damage = 60;
                NPC.defense = 15;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + numPlayers * 2200;
                NPC.damage = 80;
                NPC.defense = 15;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override bool CheckDead()
        {
            //if ((int)State != (int)AIStates.onKillAnim)
            //{
            //    State = (int)AIStates.onKillAnim;
            //    Timer = 0;
            //    NPC.dontTakeDamage = true;
            //    NPC.life = 1;
            //    return false;
            //}

            return true;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            //npcLoot.Add(notExpertRule);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        #endregion

        #region AI

        public enum AIPhases
        {
            WithSmallBalls,
            ShadowPlayer,
            BigBallSmash
        }

        public enum AIStates
        {
            OnSpawnAnmi,
            /// <summary> 狂暴，为出框惩罚 </summary>
            Rampage,
            /// <summary> 一阶段招式：小球转转转后射激光 </summary>
            RollingLaser,
            /// <summary> 一阶段招式：小球瞄准玩家后射激光 </summary>
            ConvergeLaser,
            /// <summary> 一阶段招式：一个小球射激光，其他射弹幕 </summary>
            LaserWithBeam,
            /// <summary> 一阶段招式：小球到场地左右两边射激光 </summary>
            LeftRightLaser,
            /// <summary> 一阶段招式：旋转后释放影子玩家 </summary>
            RollingShadowPlayer,
            /// <summary> 一阶段招式：随便射点激光 </summary>
            RandomLaser,
            /// <summary> 一阶段招式：依次射激光 </summary>
            RandomLaser_Master,

            /// <summary> 一阶段和2阶段的切换，使用在2阶段 </summary>
            P1ToP2Exchange,
            /// <summary> 二阶段招式，跳起后斜向下冲刺之后玩家在头顶就升龙拳宰回旋砍，不在就只回旋砍 </summary>
            SmashDownh,
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.Center = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();
            NPC.dontTakeDamage = true;
            State = (int)AIStates.OnSpawnAnmi;

            MovementLimitRect = CoraliteWorld.shadowBallsFightArea;
            MovementLimitRect.X += 200;
            MovementLimitRect.Y += 200;
            MovementLimitRect.Width -= 400;
            MovementLimitRect.Height -= 400;

            //CanDamage = false;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || /*Target.Distance(NPC.Center) > 4800 ||*/ Main.dayTime) //世花也是4800
            {
                NPC.TargetClosest();

                do
                {
                    if (!Main.dayTime)
                    {
                        State = (int)AIStates.Rampage; //狂暴的AI
                        break;
                    }

                    if (Target.dead || !Target.active)
                    {
                        NPC.EncourageDespawn(10);
                        NPC.dontTakeDamage = true;  //脱战无敌
                        NPC.velocity.Y += 0.25f;

                        return;
                    }
                    //else
                    //    ResetStates();
                } while (false);
            }

            StarsBackSky sky = ((StarsBackSky)SkyManager.Instance["StarsBackSky"]);
            if (sky.Timeleft < 100)
                sky.Timeleft += 2;
            if (sky.Timeleft>100)
                sky.Timeleft = 100;

            switch (Phase)
            {
                default:
                case (int)AIPhases.WithSmallBalls:
                    {
                        //SpawnSmallBalls();

                        if (State != (int)AIStates.OnSpawnAnmi && !GetSmallBalls())
                        {
                            //切换状态
                            ExchangeToPhase2();
                            return;
                        }

                        switch (State)
                        {
                            default:
                            case (int)AIStates.OnSpawnAnmi:
                                {
                                    OnSpawnAnmi();
                                    Timer++;
                                }
                                break;
                            case (int)AIStates.RollingLaser:
                                {
                                    RollingLaser();
                                    Timer++;
                                }
                                break;
                            case (int)AIStates.ConvergeLaser:
                                {
                                    ConvergeLaser();
                                    Timer++;
                                }
                                break;
                            case (int)AIStates.LaserWithBeam:
                                {
                                    LaserWithBeam();
                                }
                                break;
                            case (int)AIStates.LeftRightLaser:
                                {
                                    LeftRightLaser();
                                }
                                break;
                            case (int)AIStates.RollingShadowPlayer:
                                {
                                    RollingShadowPlayer();
                                    Timer++;
                                }
                                break;
                            case (int)AIStates.RandomLaser:
                                {
                                    RandomLaser();
                                }
                                break;

                        }
                    }
                    break;
                case (int)AIPhases.ShadowPlayer:
                    {
                        switch (SonState)
                        {
                            default:
                            case (int)AIStates.P1ToP2Exchange:
                                {

                                }
                                break;
                            case (int)AIStates.SmashDownh:
                                {

                                }
                                break;
                        }
                    }
                    break;
                case (int)AIPhases.BigBallSmash:
                    break;
            }
        }

        #endregion

        #region States

        public void ResetState()
        {
            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            switch (Phase)
            {
                default:
                case (int)AIPhases.WithSmallBalls:
                    {
                        State = Main.rand.Next(6) switch
                        {
                            0 => (int)AIStates.RollingLaser,
                            1 => (int)AIStates.ConvergeLaser,
                            2 => (int)AIStates.LaserWithBeam,
                            3 => (int)AIStates.LeftRightLaser,
                            4 => (int)AIStates.RollingShadowPlayer,
                            _ => (int)AIStates.RandomLaser,
                        };

                        //State = State == (int)AIStates.ConvergeLaser ? (int)AIStates.RollingLaser : (int)AIStates.ConvergeLaser;
                        //State = (int)AIStates.RollingLaser;
                    }
                    break;
                case (int)AIPhases.ShadowPlayer:
                    {

                    }
                    break;
                case (int)AIPhases.BigBallSmash:
                    {

                    }
                    break;

            }
        }

        public void ExchangeToPhase2()
        {
            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            Phase = (int)AIPhases.ShadowPlayer;
            State = (int)AIStates.P1ToP2Exchange;

            NPC.TargetClosest();
            ShadowPlayer=Target.clientClone();

        }

        #endregion

        #region HelperMethods

        public bool GetSmallBalls()
        {
            smallBalls.Clear();
            int count = 0;
            for (int i = 0; i < 200; i++)
                if (Main.npc[i].active &&
                    Main.npc[i].type == ModContent.NPCType<SmallShadowBall>() &&
                    Main.npc[i].ai[0] == NPC.whoAmI &&//小球主人是自己
                    Main.npc[i].ai[1] != (int)SmallShadowBall.AIStates.OnKillAnmi)//小球不在死亡动画
                {
                    smallBalls.Add(Main.npc[i]);
                    count++;
                    if (count >= 5)
                    {
                        break;
                    }
                }

            smallBallCount = count;
            if (count == 0)
                return false;

            return true;
        }

        public bool CheckSmallBallsReady()
        {
            int howManyReady = 0;

            foreach (var ball in smallBalls)
            {
                if ((ball.ModNPC as SmallShadowBall).Sign == (int)SmallShadowBall.SignType.Ready)
                    howManyReady++;
                else
                    break;
            }

            return howManyReady == smallBallCount;//全准备好了
        }

        public void SetDirection(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            NPC.direction = xLength > 0 ? -1 : 1;
            NPC.directionY = yLength > 0 ? -1 : 1;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        public void SpawnSmallBalls()
        {
            if (!SpawnedSmallBalls)
            {
                for (int i = 0; i < 5; i++)
                {
                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
                        ModContent.NPCType<SmallShadowBall>(), NPC.whoAmI, NPC.whoAmI);
                    //Main.npc[index].realLife = NPC.whoAmI;
                    //NPC.lifeMax += Main.npc[index].lifeMax;
                }

                //NPC.life = NPC.lifeMax;
                SpawnedSmallBalls = true;
            }
        }

        public void MovementLimit()
        {
            Vector2 center = NPC.Center;
            center.X = Math.Clamp(center.X, MovementLimitRect.X, MovementLimitRect.X + MovementLimitRect.Width);
            center.Y = Math.Clamp(center.Y, MovementLimitRect.Y, MovementLimitRect.Y + MovementLimitRect.Height);
            NPC.Center = center;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (State == (int)AIStates.OnSpawnAnmi)
            {
                Texture2D mainTex = NPC.GetTexture();

                var pos = NPC.Center - screenPos;
                var frameBox = mainTex.Frame();
                var origin = frameBox.Size() / 2;

                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

                spriteBatch.End();
                Rectangle scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch, pos, frameBox), spriteBatch.GraphicsDevice.ScissorRectangle);
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
                spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null);

                spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, 0, 0);

                rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                spriteBatch.End();
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null);

                return false;
            }

            DrawSelf(spriteBatch, screenPos, drawColor);

            return false;
        }

        public void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame();
            var origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, 0, 0);
        }

        public Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Vector2 center, Rectangle frameBox)
        {
            float height = SpawnOverflowHeight * frameBox.Height;
            Vector2 position = center + new Vector2(-frameBox.Width / 2, frameBox.Height / 2 - height);
            Vector2 size = new Vector2(frameBox.Width, height);
            position = Vector2.Transform(position, Main.Transform);
            size = Vector2.Transform(size, Main.Transform);
            Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            int screenWidth = Main.screenWidth;
            int screenHeight = Main.screenHeight;
            rectangle.X = Utils.Clamp(rectangle.X, 0, screenWidth);
            rectangle.Y = Utils.Clamp(rectangle.Y, 0, screenHeight);
            rectangle.Width = Utils.Clamp(rectangle.Width, 0, screenWidth - rectangle.X);
            rectangle.Height = Utils.Clamp(rectangle.Height, 0, screenHeight - rectangle.Y);
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            int num3 = Utils.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
            int num4 = Utils.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
            int num5 = Utils.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
            int num6 = Utils.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);
            return new Rectangle(num3, num4, num5 - num3, num6 - num4);
        }

        #endregion
    }
}
