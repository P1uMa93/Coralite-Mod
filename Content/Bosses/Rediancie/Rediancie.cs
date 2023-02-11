﻿using Coralite.Content.Dusts;
using Coralite.Content.Items.RedJadeItems;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.Rediancie
{
    //简单写下Rediancie这个英文名由来
    //其实就是Red+Diancie ,红色+蒂安希，捏他自宝可梦中的钻石公主蒂安希
    //另外它的召唤物，小赤玉灵名字捏他的是小碎钻。
    //
    //                                            饿啊  ， 吃我钻石风暴！！！
    //
    //                      💎💎                                       💎💎                                      💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    // 💎💎💎💎💎💎💎💎         💎💎💎💎💎💎💎💎        💎💎💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //                      💎💎                                       💎💎                                      💎💎
    //
    //MAGA蒂安希 160物攻 本系100威力群攻技能，你接的下?
    //你十万条命都接不下
    //打完还50%概率上升2段物防，又硬又能打（虽然maga后物防还低了）

    [AutoloadBossHead]
    public class Rediancie : ModNPC
    {
        public override string Texture => AssetDirectory.Rediancie + Name;

        Player target => Main.player[NPC.target];

        internal ref float yFrame => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float Timer => ref NPC.ai[2];

        public bool exchangeState = true;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉灵");

            Main.npcFrameCount[Type] = 13;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 70;
            NPC.damage = 25;
            NPC.defense = 8;
            NPC.lifeMax = 1500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = GetInstance<RediancieBossBar>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(1700 * bossLifeScale) + numPlayers * 350;
            NPC.damage = 30;
            NPC.defense = 10;

            if (Main.masterMode)
            {
                NPC.lifeMax = (int)(2200 * bossLifeScale) + numPlayers * 550;
                NPC.damage = 45;
                NPC.defense = 12;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 20, 24));
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            for (int j = 0; j < 3; j++)
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(2, 2), Mod.Find<ModGore>("StoneImitator_Gore0").Type);

            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(2, 2), Mod.Find<ModGore>("StoneImitator_Gore1").Type);
        }

        public override void Load()
        {
            for (int i = 0; i < 2; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.Gores + "StoneImitator_Gore" + i);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
        }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
            if (Main.netMode!=NetmodeID.MultiplayerClient)
            {
                if (Main.masterMode)
                    State = (int)AIStates.dash;
                else
                    State = (int)AIStates.explosion;

                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            #region frame
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                yFrame += 1;
                if (yFrame == 13)
                    yFrame = 0;
                NPC.frameCounter = 0;
            }
            #endregion

            if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active || target.Distance(NPC.Center) > 3000)
                NPC.TargetClosest();

            if (target.dead || !target.active || target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
            {
                State = -1;
                NPC.velocity.X *= 0.97f;
                NPC.velocity.Y += 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }

            NPC.direction = NPC.spriteDirection = target.position.X > NPC.position.X ? 1 : -1;
            NPC.directionY = target.Center.Y > NPC.Center.Y ? 1 : -1;
            switch (State)
            {
                case (int)AIStates.Accumulate:          //追逐玩家并蓄力爆炸
                    {
                        //控制X方向的移动
                        if (Timer < 225)
                        {
                            Helper.NPCMovment_OneLine(ref NPC.velocity.X, NPC.direction, 7f, 0.1f, 0.15f, 0.97f);

                            //控制Y方向的移动
                            float yLenth = Math.Abs(target.Center.Y - NPC.Center.Y);
                            if (yLenth > 30)
                                Helper.NPCMovment_OneLine(ref NPC.velocity.Y, NPC.directionY, 4.5f, 0.06f, 0.08f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.96f;
                        }
                        else
                        {
                            NPC.velocity *= 0.99f;
                        }

                        if (Timer % 5 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            int count = (int)Timer / 20;
                            for (int i = 0; i < count; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                dust.noGravity = true;
                            }
                        }

                        if (Timer == 235)       //生成弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_BigBoom>(), 55, 8f);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                var modifier = new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10, 6f, 20, 1000f);
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                        }

                        if (Timer > 250)
                            ResetState();

                    }
                    break;
                case (int)AIStates.dash:            //连续3次冲刺攻击
                    {
                        int realTime = (int)Timer % 100;

                        do
                        {
                            if (realTime == 18 && Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                                Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) - new Vector2(63, 46.5f), DustType<HorizontalStar>(), null, 0, Coralite.Instance.RedJadeRed, 1.5f);
                            }

                            if (realTime < 20)
                            {
                                Helper.NPCMovment_OneLine(ref NPC.velocity.X, NPC.direction, 2f, 0.1f, 0.1f, 0.97f);

                                //控制Y方向的移动
                                float yLenth = Math.Abs(target.Center.Y - NPC.Center.Y);
                                if (yLenth > 30)
                                    Helper.NPCMovment_OneLine(ref NPC.velocity.Y, NPC.directionY, 2f, 0.1f, 0.1f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.96f;
                                break;
                            }

                            if (realTime == 22)
                                NPC.velocity = (target.Center + new Vector2(0, (int)Timer / 100 % 2 == 0 ? 250 : -250) - NPC.Center).SafeNormalize(Vector2.One) * 10f;

                            //边冲边炸
                            if (realTime < 71)
                            {
                                if (realTime % 10 == 0)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_Explosion>(), 20, 5f);

                                break;
                            }

                            NPC.velocity *= 0.98f;
                        } while (false);

                        if (Timer > 300)
                            ResetState();
                    }
                    break;
                default:
                case (int)AIStates.explosion:       //追着玩家并爆炸
                    {
                        //控制X方向的移动
                        Helper.NPCMovment_OneLine(ref NPC.velocity.X, NPC.direction, 6f, 0.09f, 0.22f, 0.97f);

                        //控制Y方向的移动
                        float yLenth2 = Math.Abs(target.Center.Y - NPC.Center.Y);
                        if (yLenth2 > 30)
                            Helper.NPCMovment_OneLine(ref NPC.velocity.Y, NPC.directionY, 4.5f, 0.06f, 0.06f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer % 3 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            int count = ((int)Timer % 80) / 10;
                            for (int i = 0; i < count; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.1f);
                                dust.noGravity = true;
                            }
                        }

                        if (Timer % 80 == 0)//生成弹幕
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_Explosion>(), 30, 5f);

                        if (Timer > 250)
                            ResetState();
                    }
                    break;
                case (int)AIStates.upShoot:         //朝上连续射击
                    {
                        SlowDownAndGoUp();

                        //隔固定时间射弹幕
                        if (Timer % 25 == 0)
                        {
                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 20);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, -8).RotatedBy(((Timer / 20) - 3) * 0.08f), ProjectileType<Rediancie_Strike>(), damage, 5f);
                            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                        }

                        if (Timer > 220)
                            ResetState();
                    }
                    break;
                case (int)AIStates.magicShoot:      //蓄力射3发魔法弹幕
                    {
                        SlowDownAndGoUp();

                        if (Main.netMode != NetmodeID.Server && Timer % 3 == 0)
                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                                dust.velocity = (NPC.Center - dust.position).SafeNormalize(Vector2.UnitY) * 3f;
                                dust.noGravity = true;
                            }

                        if (Timer < 36)
                            break;

                        if (Timer % 35 == 0)//生成弹幕
                        {
                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 20);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (target.Center - NPC.Center + new Vector2(0, 60 * (Timer / 30) == 1 ? 1 : -1)).SafeNormalize(Vector2.UnitY) * 10f, ProjectileType<Rediancie_Beam>(), damage, 5f);
                            Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                        }

                        if (Timer > 155)
                            ResetState();
                    }
                    break;
                case (int)AIStates.summon:          //召唤小赤玉灵
                    {
                        SlowDownAndGoUp();

                        if (Main.netMode != NetmodeID.Server && Timer % 10 == 0)
                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(20, 20), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                                dust.noGravity = true;
                            }

                        if (Timer % 40 == 0)
                        {
                            if (Main.npc.Count((n) => n.active && n.type == NPCType<RediancieMinion>()) < OwnedMinionMax())
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<RediancieMinion>());
                            else
                                ResetState();

                        }

                        if (Timer > 200)//防止出BUG
                            ResetState();
                    }
                    break;

            }

            NPC.rotation = NPC.velocity.ToRotation() * 0.05f * NPC.direction;
            Timer++;
        }

        public void ResetState()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            do
            {
                if (exchangeState && NPC.life < NPC.lifeMax / 2)    //血量低于一半固定放小弟
                {
                    State = (int)AIStates.summon;
                    exchangeState = false;
                    break;
                }

                if (NPC.life < NPC.lifeMax / 2)//血量低于一半的攻击动作，加入了放小弟的招式
                {
                    if (State <= 1)
                        State = Main.rand.Next(3) switch
                        {
                            0 => (int)AIStates.upShoot,
                            1 => (int)AIStates.magicShoot,
                            2 => (int)AIStates.summon,
                            _ => (int)AIStates.upShoot
                        };
                    else if (Main.masterMode)
                        State = (int)(Main.rand.NextBool() ? AIStates.Accumulate : AIStates.dash);
                    else
                        State = (int)AIStates.explosion;

                    break;
                }

                //正常时的招式
                if (State <= 1)
                    State = Main.rand.NextBool() ? (int)AIStates.upShoot : (int)AIStates.magicShoot;
                else if(Main.masterMode)
                    State = Main.rand.NextBool() ? (int)AIStates.explosion : (int)AIStates.Accumulate;
                else
                    State = (int)AIStates.explosion;

            } while (false);

            Timer = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        public void SlowDownAndGoUp()
        {
            NPC.velocity.X *= 0.98f;
            NPC.velocity.Y += -0.1f;
            if (NPC.velocity.Y < -1f)
                NPC.velocity.Y = -1f;
        }

        public static int OwnedMinionMax()
        {
            if (Main.masterMode)
                return 3;

            if (Main.expertMode)
                return 3;

            return 2;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            Rectangle frameBox = new Rectangle(0, (int)yFrame * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        #endregion
    }

    public enum AIStates : int
    {
        Accumulate = -1,
        dash = 0,
        explosion = 1,
        upShoot = 2,
        magicShoot = 3,
        summon = 4
    }
}
