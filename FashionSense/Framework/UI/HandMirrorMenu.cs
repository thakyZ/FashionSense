﻿using FashionSense.Framework.Models;
using FashionSense.Framework.Models.Accessory;
using FashionSense.Framework.Models.Hair;
using FashionSense.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.UI
{
    public class HandMirrorMenu : IClickableMenu
    {
        private Rectangle _portraitBox;
        private Farmer _displayFarmer;
        private string hoverText = "";

        private ClickableComponent descriptionLabel;
        private ClickableComponent colorLabel;
        private ClickableComponent authorLabel;
        public List<ClickableComponent> labels = new List<ClickableComponent>();
        public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> filterButtons = new List<ClickableComponent>();
        public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();

        public ColorPicker hairColorPicker;
        private ColorPicker _lastHeldColorPicker;
        public ClickableTextureComponent okButton;

        public HandMirrorMenu() : base(0, 0, 350, 550, showUpperRightCloseButton: true)
        {
            // Set up menu structure
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y;

            // Set up portrait and farmer
            topLeft = Utility.getTopLeftPositionForCenteringOnScreen(128, 192);
            _portraitBox = new Rectangle((int)topLeft.X, base.yPositionOnScreen + 64, 128, 192);
            _displayFarmer = Game1.player;

            // Add author label
            var authorName = "Author's Hair Name";
            labels.Add(authorLabel = new ClickableComponent(new Rectangle((int)(_portraitBox.X - Game1.smallFont.MeasureString(authorName).X / 2) + 64, base.yPositionOnScreen + 32, 1, 1), authorName));

            // Add buttons
            int yOffset = 144;
            leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(_portraitBox.X - 32, _portraitBox.Y + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
            {
                myID = 520,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                leftNeighborImmutable = true,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });
            rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(_portraitBox.Right - 32, _portraitBox.Y + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
            {
                myID = 521,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });

            yOffset += 64;
            leftSelectionButtons.Add(new ClickableTextureComponent("Appearance", new Rectangle(_portraitBox.X - 64, _portraitBox.Y + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
            {
                myID = 514,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });
            labels.Add(descriptionLabel = new ClickableComponent(new Rectangle(_portraitBox.Right - 86, _portraitBox.Y + yOffset + 16, 1, 1), FashionSense.modHelper.Translation.Get("ui.fashion_sense.title.hair")));
            rightSelectionButtons.Add(new ClickableTextureComponent("Appearance", new Rectangle(_portraitBox.Right, _portraitBox.Y + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
            {
                myID = 515,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });

            filterButtons.Add(new ClickableTextureComponent("HairFilter", new Rectangle(base.xPositionOnScreen + 50, base.yPositionOnScreen + 70, 64, 64), null, "enabled", FashionSense.assetManager.scissorsButtonTexture, new Rectangle(0, 0, 15, 15), 3f)
            {
                myID = 601,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });

            filterButtons.Add(new ClickableTextureComponent("AccessoryFilter", new Rectangle(base.xPositionOnScreen + 50, base.yPositionOnScreen + 125, 64, 64), null, "disabled", FashionSense.assetManager.accessoryButtonTexture, new Rectangle(0, 0, 15, 15), 3f)
            {
                myID = 601,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });

            okButton = new ClickableTextureComponent("OK", new Rectangle(base.xPositionOnScreen + base.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 32, base.yPositionOnScreen + base.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 32, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 505,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };

            // Add color picker
            yOffset += 48;
            var measuredStringSize = Game1.smallFont.MeasureString(GetColorPickerLabel(true));
            labels.Add(colorLabel = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 48, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, (int)measuredStringSize.X, (int)measuredStringSize.Y), GetColorPickerLabel(false)));
            yOffset += 32;
            var top = new Point(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            hairColorPicker = new ColorPicker("Hair", top.X, top.Y);
            hairColorPicker.setColor(Game1.player.hairstyleColor);
            colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
            {
                myID = 525,
                downNeighborID = -99998,
                upNeighborID = -99998,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
            {
                myID = 526,
                upNeighborID = -99998,
                downNeighborID = -99998,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
            {
                myID = 527,
                upNeighborID = -99998,
                downNeighborID = -99998,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
        }

        internal static string GetColorPickerLabel(bool isDisabled = false, bool isCompact = false)
        {
            string labelName = FashionSense.modHelper.Translation.Get("ui.fashion_sense.color_active.hair");
            if (isDisabled)
            {
                var separator = isCompact ? "\n" : " ";
                labelName += $"{separator}{FashionSense.modHelper.Translation.Get("ui.fashion_sense.color_disabled.generic")}";
            }

            return $"{labelName}:";
        }

        private string GetNameOfEnabledFilter()
        {
            var enabledButton = filterButtons.FirstOrDefault(f => (f as ClickableTextureComponent).hoverText == "enabled");
            if (enabledButton is null)
            {
                return null;
            }

            return enabledButton.name;
        }

        private void UpdateAppearance(int change)
        {
            string modDataKey = null;
            AppearanceContentPack currentAppearance = null;
            List<AppearanceContentPack> appearanceModels = new List<AppearanceContentPack>();
            switch (GetNameOfEnabledFilter())
            {
                case "HairFilter":
                    modDataKey = ModDataKeys.CUSTOM_HAIR_ID;
                    currentAppearance = FashionSense.textureManager.GetSpecificAppearanceModel<HairContentPack>(Game1.player.modData[ModDataKeys.CUSTOM_HAIR_ID]);
                    appearanceModels = FashionSense.textureManager.GetAllAppearanceModels().Where(m => m is HairContentPack).ToList();
                    break;
                case "AccessoryFilter":
                    modDataKey = ModDataKeys.CUSTOM_ACCESSORY_ID;
                    currentAppearance = FashionSense.textureManager.GetSpecificAppearanceModel<AccessoryContentPack>(Game1.player.modData[ModDataKeys.CUSTOM_ACCESSORY_ID]);
                    appearanceModels = FashionSense.textureManager.GetAllAppearanceModels().Where(m => m is AccessoryContentPack).ToList();
                    break;
            }

            int current_index = -1;
            if (currentAppearance != null)
            {
                current_index = appearanceModels.IndexOf(currentAppearance);
            }
            current_index += change;
            if (current_index >= appearanceModels.Count)
            {
                current_index = -1;
            }
            else if (current_index < -1)
            {
                current_index = appearanceModels.Count() - 1;
            }

            Game1.player.modData[modDataKey] = current_index == -1 ? "None" : appearanceModels[current_index].Id;
            FashionSense.ResetAnimationModDataFields(Game1.player, 0, AnimationModel.Type.Idle, Game1.player.facingDirection);
            Game1.playSound("grassyStep");
        }

        private void selectionClick(string name, int change)
        {
            switch (name)
            {
                case "Appearance":
                    {
                        UpdateAppearance(change);
                        break;
                    }
                case "Direction":
                    _displayFarmer.faceDirection((_displayFarmer.FacingDirection - change + 4) % 4);
                    _displayFarmer.FarmerSprite.StopAnimation();
                    _displayFarmer.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (leftSelectionButtons.Count > 0)
            {
                foreach (ClickableComponent c2 in leftSelectionButtons)
                {
                    if (c2.containsPoint(x, y))
                    {
                        selectionClick(c2.name, -1);
                        if (c2.scale != 0f)
                        {
                            c2.scale -= 0.25f;
                            c2.scale = Math.Max(0.75f, c2.scale);
                        }
                    }
                }
            }

            if (rightSelectionButtons.Count > 0)
            {
                foreach (ClickableComponent c in rightSelectionButtons)
                {
                    if (c.containsPoint(x, y))
                    {
                        selectionClick(c.name, 1);
                        if (c.scale != 0f)
                        {
                            c.scale -= 0.25f;
                            c.scale = Math.Max(0.75f, c.scale);
                        }
                    }
                }
            }

            foreach (ClickableTextureComponent c in filterButtons)
            {
                if (c.containsPoint(x, y))
                {
                    var enabledButton = filterButtons.FirstOrDefault(b => (b as ClickableTextureComponent).hoverText == "enabled");
                    if (enabledButton != null)
                    {
                        (enabledButton as ClickableTextureComponent).hoverText = "disabled";
                    }

                    c.hoverText = "enabled";
                    switch (c.name)
                    {
                        case "HairFilter":
                            descriptionLabel.name = FashionSense.modHelper.Translation.Get("ui.fashion_sense.title.hair");
                            break;
                        case "AccessoryFilter":
                            descriptionLabel.name = FashionSense.modHelper.Translation.Get("ui.fashion_sense.title.accessory");
                            break;
                    }

                    if (c.scale != 0f)
                    {
                        c.scale -= 0.25f;
                        c.scale = Math.Max(0.75f, c.scale);
                    }
                }
            }

            if (hairColorPicker != null && hairColorPicker.containsPoint(x, y))
            {
                Color color2 = hairColorPicker.click(x, y);
                Game1.player.changeHairColor(color2);
                _lastHeldColorPicker = hairColorPicker;
            }

            if (okButton.containsPoint(x, y))
            {
                okButton.scale -= 0.25f;
                okButton.scale = Math.Max(0.75f, okButton.scale);
                exitThisMenu();
                Game1.playSound("coin");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            foreach (ClickableTextureComponent c6 in leftSelectionButtons)
            {
                if (c6.containsPoint(x, y))
                {
                    c6.scale = Math.Min(c6.scale + 0.02f, c6.baseScale + 0.1f);
                }
                else
                {
                    c6.scale = Math.Max(c6.scale - 0.02f, c6.baseScale);
                }
            }
            foreach (ClickableTextureComponent c5 in rightSelectionButtons)
            {
                if (c5.containsPoint(x, y))
                {
                    c5.scale = Math.Min(c5.scale + 0.02f, c5.baseScale + 0.1f);
                }
                else
                {
                    c5.scale = Math.Max(c5.scale - 0.02f, c5.baseScale);
                }
            }
            foreach (ClickableTextureComponent c5 in filterButtons)
            {
                if (c5.containsPoint(x, y))
                {
                    if (c5.hoverText == "disabled")
                    {
                        c5.scale = Math.Min(c5.scale + 0.02f, c5.baseScale + 0.1f);
                    }

                    switch (c5.name)
                    {
                        case "HairFilter":
                            hoverText = FashionSense.modHelper.Translation.Get("ui.fashion_sense.title.hair");
                            break;
                        case "AccessoryFilter":
                            hoverText = FashionSense.modHelper.Translation.Get("ui.fashion_sense.title.accessory");
                            break;
                        default:
                            continue;
                    }
                }
                else
                {
                    c5.scale = Math.Max(c5.scale - 0.02f, c5.baseScale);
                }
            }
            foreach (ClickableComponent label in labels)
            {
                if (label == colorLabel && label.name == GetColorPickerLabel(true) && colorLabel.containsPoint(x, y))
                {
                    hoverText = FashionSense.modHelper.Translation.Get("ui.fashion_sense.color_info.hair");
                }
            }

            if (okButton.containsPoint(x, y))
            {
                okButton.scale = Math.Min(okButton.scale + 0.02f, okButton.baseScale + 0.1f);
            }
            else
            {
                okButton.scale = Math.Max(okButton.scale - 0.02f, okButton.baseScale);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.dialogueUp || Game1.IsFading())
            {
                return;
            }

            // Get the custom hair object, if it exists
            AppearanceContentPack contentPack = null;
            switch (GetNameOfEnabledFilter())
            {
                case "HairFilter":
                    contentPack = FashionSense.textureManager.GetSpecificAppearanceModel<HairContentPack>(Game1.player.modData[ModDataKeys.CUSTOM_HAIR_ID]);
                    break;
                case "AccessoryFilter":
                    contentPack = FashionSense.textureManager.GetSpecificAppearanceModel<AccessoryContentPack>(Game1.player.modData[ModDataKeys.CUSTOM_ACCESSORY_ID]);
                    break;
            }

            // General UI (title, background)
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, FashionSense.modHelper.Translation.Get("tools.name.hand_mirror"), base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);

            // Farmer portrait
            b.Draw(Game1.daybg, new Vector2(_portraitBox.X, _portraitBox.Y), Color.White);
            FarmerRenderer.isDrawingForUI = true;
            _displayFarmer.FarmerRenderer.draw(b, _displayFarmer.FarmerSprite.CurrentAnimationFrame, _displayFarmer.FarmerSprite.CurrentFrame, _displayFarmer.FarmerSprite.SourceRect, new Vector2(_portraitBox.Center.X - 32, _portraitBox.Bottom - 160), Vector2.Zero, 0.8f, Color.White, 0f, 1f, _displayFarmer);
            FarmerRenderer.isDrawingForUI = false;

            // Draw buttons
            foreach (ClickableTextureComponent leftSelectionButton in leftSelectionButtons)
            {
                leftSelectionButton.draw(b);
            }
            foreach (ClickableTextureComponent rightSelectionButton in rightSelectionButtons)
            {
                rightSelectionButton.draw(b);
            }
            foreach (ClickableTextureComponent filterButton in filterButtons)
            {
                filterButton.draw(b, filterButton.hoverText == "enabled" ? Color.White : Color.Gray, 1f);
            }
            okButton.draw(b);

            // Draw labels
            foreach (ClickableComponent c in labels)
            {
                if (!c.visible)
                {
                    continue;
                }
                string sub = "";
                float offset = 0f;
                float subYOffset = 0f;
                Color color = Game1.textColor;
                if (c == descriptionLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c.name).X / 2f;
                    if (!c.name.Contains("Color"))
                    {
                        sub = "None";
                        if (contentPack != null)
                        {
                            sub = contentPack.Name;
                        }
                    }
                }
                else if (c == authorLabel)
                {
                    authorLabel.name = "";
                    if (contentPack != null)
                    {
                        authorLabel.name = contentPack.Author;
                    }

                    authorLabel.bounds.X = (int)(_portraitBox.X - Game1.smallFont.MeasureString(authorLabel.name).X / 2) + 64;
                }
                else if (c == colorLabel)
                {
                    var name = GetColorPickerLabel(false);
                    if (contentPack != null)
                    {
                        if (contentPack is HairContentPack hairPack && hairPack.GetHairFromFacingDirection(Game1.player.facingDirection) is HairModel hModel && hModel != null && hModel.DisableGrayscale)
                        {
                            name = GetColorPickerLabel(true);
                        }
                        else if (contentPack is AccessoryContentPack accessoryPack && accessoryPack.GetAccessoryFromFacingDirection(Game1.player.facingDirection) is AccessoryModel aModel && aModel != null && aModel.DisableGrayscale)
                        {
                            name = GetColorPickerLabel(true);
                        }
                    }

                    colorLabel.name = name;
                }
                else
                {
                    color = Game1.textColor;
                }
                Utility.drawTextWithShadow(b, c.name, Game1.smallFont, new Vector2((float)c.bounds.X + offset, c.bounds.Y), color);
                if (sub.Length > 0)
                {
                    Utility.drawTextWithShadow(b, sub, Game1.smallFont, new Vector2((float)(c.bounds.X + 21) - Game1.smallFont.MeasureString(sub).X / 2f, (float)(c.bounds.Y + 32) + subYOffset), color);
                }
            }

            // Draw color selector
            if (hairColorPicker != null)
            {
                hairColorPicker.draw(b);
            }

            // Draw hover text
            if (!hoverText.Equals(""))
            {
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}
