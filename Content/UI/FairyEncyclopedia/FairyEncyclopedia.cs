﻿using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyEncyclopedia : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static bool visible;
        public override bool Visible => visible;

        public static UIPanel BackGround;

        /// <summary>
        /// 排列的按钮
        /// </summary>
        public SortButton[] sortButtons;
        public PageText PageText;
        public UIGrid FairyGrid;
        public static float Timer;

        public UIImageButton SelectButton;
        public UIPanel SelectButtonsPanel;

        /// <summary>
        /// 当前所在的页数
        /// </summary>
        public static int PageIndex;
        public static int PageCount;

        public static UpdateState State;
        public static SortStyle CurrentSortStyle;

        public static FairyAttempt.Rarity? selectType = null;

        private List<Fairy> fairies;

        public enum SortStyle
        {
            ByType,
            ByRarity,
            ShowCaught,
        }

        public enum UpdateState
        {
            /// <summary>
            /// 显示所有的
            /// </summary>
            ShowAll,
            /// <summary>
            /// 收集换奖励界面
            /// </summary>
            CollectPanel,
            /// <summary>
            /// 单个的描述
            /// </summary>
            Details,
        }

        #region 各类初始化

        public override void OnInitialize()
        {
            InitBackground();
            InitPageText();
            InitFairyGrid();
            SelectButtonsPanel = new UIPanel();

            MakeExitButton(this);

            SelectButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Filtering"));
            SelectButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Filtering"));
            SelectButton.Left.Set(PageText.Width.Pixels + 10, 0);

            SelectButton.OnLeftClick += SelectButton_OnLeftClick;

            //SelectButtonsPanel.VAlign = 1;
            SelectButtonsPanel.Top.Set(40, 0);
            SelectButtonsPanel.Left.Set(SelectButton.Left.Pixels, 0);
            SelectButtonsPanel.Width.Set(BackGround.Width.Pixels / 4, 0);
            SelectButtonsPanel.Height.Set(BackGround.Height.Pixels / 3, 0);

            UIGrid buttonsGrid= new UIGrid();
            buttonsGrid.Width.Set(SelectButtonsPanel.Width.Pixels, 0);
            buttonsGrid.Height.Set(SelectButtonsPanel.Height.Pixels, 0);
            Asset<Texture2D> circleButtonTex = TextureAssets.WireUi[0];
            Asset<Texture2D> circleButtonHoverTex = TextureAssets.WireUi[1];

            SelectButton AllButton = new SelectButton(circleButtonTex, null);
            AllButton.SetHoverImage(circleButtonHoverTex);
            buttonsGrid.Add(AllButton);
            SelectButton SPButton = new SelectButton(circleButtonTex, (FairyAttempt.Rarity)(-1));
            SPButton.SetHoverImage(circleButtonHoverTex);
            buttonsGrid.Add(SPButton);

            FairyAttempt.Rarity[] rarities = Enum.GetValues<FairyAttempt.Rarity>();

            foreach (var rairty in rarities)
            {
                SelectButton rarityButton = new SelectButton(circleButtonTex, rairty);
                rarityButton.SetHoverImage(circleButtonHoverTex);
                buttonsGrid.Add(rarityButton);
            }

            SelectButtonsPanel.Append(buttonsGrid);

            Append(BackGround);
        }

        private void SelectButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (BackGround.HasChild(SelectButtonsPanel))//有就关闭
            {
                BackGround.RemoveChild(SelectButtonsPanel);
            }
            else//没有就打开
            {
                BackGround.Append(SelectButtonsPanel);
            }
        }

        private static void InitBackground()
        {
            BackGround = new UIPanel();

            //设置到屏幕中心
            BackGround.HAlign = 0.5f;
            BackGround.VAlign = 0.5f;
            BackGround.Width.Set(Main.screenWidth * 0.66f, 0);
            BackGround.Height.Set(Main.screenHeight * 0.7f, 0);

            BackGround.BackgroundColor = new Color(63, 107, 151) * 0.85f;
        }

        private void InitPageText()
        {
            PageText = new PageText();
            UIImageButton leftButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "FairyEncyclopediaLeftButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            UIImageButton rightButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "FairyEncyclopediaRightButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            PageText.Left.Set(0, 0);
            PageText.Width.Set(100, 0);
            PageText.Height.Set(40, 0);

            leftButton.Left.Set(0, 0);
            rightButton.Left.Set(PageText.Width.Pixels - 32, 0);

            leftButton.OnLeftClick += LeftButton_OnLeftClick;
            rightButton.OnLeftClick += RightButton_OnLeftClick;

            PageText.Append(leftButton);
            PageText.Append(rightButton);
        }

        private void InitFairyGrid()
        {
            FairyGrid ??= new UIGrid();

            FairyGrid.Top.Set(40, 0);
            FairyGrid.Width.Set(BackGround.Width.Pixels - 18, 0);
            FairyGrid.Height.Set(BackGround.Height.Pixels - 70, 0);

            fairies = new List<Fairy>();
            for (int i = 0; i < FairyLoader.FairyCount; i++)
                fairies.Add(FairyLoader.fairys[i]);
        }

        private void MakeExitButton(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true)
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 0.3f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 0.95f,
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(-25f)
            };

            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.SetSnapPoint("ExitButton", 0);
            outerContainer.Append(uITextPanel);
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(CoraliteSoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(CoraliteSoundID.MenuClose);
            visible = false;
        }

        #endregion


        private void LeftButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SetShowGrid(-1);
            State = UpdateState.ShowAll;
            base.Recalculate();
        }

        private void RightButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SetShowGrid(1);
            State = UpdateState.ShowAll;
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (BackGround.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            if (Main.LocalPlayer.controlInv || Main.playerInventory)
            {
                visible = false;
                return;
            }

            switch (State)
            {
                case UpdateState.ShowAll:
                    {
                        if (Timer < FairySlot.XCount * FairySlot.YCount)//显示过渡小动画
                        {
                            Timer += 0.2f;
                        }
                    }
                    break;
                case UpdateState.CollectPanel:
                    break;
                case UpdateState.Details:
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        public override void Recalculate()
        {
            if (BackGround != null)
            {
                BackGround.Width.Set(Main.screenWidth * 0.66f, 0);
                BackGround.Height.Set(Main.screenHeight * 0.7f, 0);

                FairyGrid.Top.Set(40, 0);
                FairyGrid.Width.Set(BackGround.Width.Pixels - 18, 0);
                FairyGrid.Height.Set(BackGround.Height.Pixels - 70, 0);
            }

            Main.playerInventory = false;
            PageIndex = 0;
            base.Recalculate();
        }

        public void SetToAllShow()
        {
            BackGround.RemoveAllChildren();
            BackGround.Append(FairyGrid);
            BackGround.Append(PageText);
            BackGround.Append(SelectButton);

            SetShowGrid(0);

            State = UpdateState.ShowAll;
            selectType = null;
            Recalculate();
            Recalculate();
        }

        /// <summary>
        /// 传入的值是增或者减少页数
        /// </summary>
        /// <param name="page"></param>
        public void SetShowGrid(int page)
        {
            int currentPage = page + PageIndex;
            PageCount = FairyLoader.FairyCount / (FairySlot.XCount * FairySlot.YCount);
            PageIndex = Math.Clamp(currentPage, 0, PageCount);

            FairyGrid.Clear();
            Timer = 0;
            for (int i = 0; i < FairySlot.XCount * FairySlot.YCount; i++)
            {
                int index = PageIndex * FairySlot.XCount * FairySlot.YCount + i;

                if (index < fairies.Count)
                {
                    FairySlot slot = new FairySlot(fairies[index].Type, i);
                    slot.SetSize(FairyGrid);
                    FairyGrid.Add(slot);
                }
                else
                    break;
            }

            base.Recalculate();
        }

        /// <summary>
        /// 传入null时为选择全部
        /// 传入小于0的数时为选择除了已有的稀有度以外的特别稀有度
        /// </summary>
        /// <param name="targetRarity"></param>
        public void Select(FairyAttempt.Rarity? targetRarity)
        {
            selectType= targetRarity;
            fairies.Clear();

            if (!targetRarity.HasValue)
            {
                for (int i = 0; i < FairyLoader.FairyCount; i++)
                    fairies.Add(FairyLoader.fairys[i]);

                goto CheckOver;
            }

            if (targetRarity.Value < 0)//查找所有特殊的稀有度
            {
                for (int i = 0; i < FairyLoader.FairyCount; i++)
                    if (!Enum.IsDefined(FairyLoader.fairys[i].Rarity))
                        fairies.Add(FairyLoader.fairys[i]);
                goto CheckOver;
            }

            for (int i = 0; i < FairyLoader.FairyCount; i++)
                if (FairyLoader.fairys[i].Rarity == targetRarity)
                    fairies.Add(FairyLoader.fairys[i]);

            CheckOver:

            Sort(CurrentSortStyle);
            SetShowGrid(0);
            Recalculate();
        }

        public void Sort(SortStyle sortStyle)
        {
            Timer = 0;

            switch (sortStyle)
            {
                default:
                case SortStyle.ByType:
                    fairies.Sort((f1, f2) => f1.Type.CompareTo(f2.Type));
                    break;
                case SortStyle.ByRarity:
                    fairies.Sort((f1, f2) => f1.Rarity.CompareTo(f2.Rarity));
                    break;
                case SortStyle.ShowCaught:
                    fairies = [.. fairies.OrderBy(f => FairySystem.FairyCaught[f.Type])];
                    break;
            }
        }
    }
}
