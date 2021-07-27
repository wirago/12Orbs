using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Orbs.LootSystem;
using Orbs.Animations;
using Microsoft.Xna.Framework.Graphics;
using Orbs.Interface;
using System.Linq;
using Orbs.Effects;
using System;

namespace Orbs.World
{
    public static class WorldObjects
    {
        public static List<ItemContainer> lootableContainers = new List<ItemContainer>();
        public static List<ObjectAnimation> animatedObjects = new List<ObjectAnimation>();
        public static List<Transit> transits = new List<Transit>();
        public static List<EnemyDrop> enemyDrops = new List<EnemyDrop>();
        public static List<Glyph> placedGlyphs = new List<Glyph>();

        public static Dictionary<string, Rectangle> interactionObjects = new Dictionary<string, Rectangle>();

        private const int SECONDSBETWEENGLYPHEFFECT = 1;
        private static TimeSpan lastGlyphEffect = Orbs.currentTimeSpan;

        //For Debug Purpose
        public static void DrawObjectStatus()
        {
            foreach (var o in lootableContainers)
                o.DrawStatus();
        }

        public static bool CheckMousClick(Vector2 mouseWorldPos)
        {
            foreach(var o in lootableContainers)
            {
                if (o.positionRectangle.Contains(mouseWorldPos))
                {
                    o.LootContainer();
                    return true;
                }
            }

            foreach(var d in enemyDrops)
            {
                if (d.BoundingBox.Contains(mouseWorldPos))
                {
                    d.CollectDrop(d);
                    return true;
                }
            }

            foreach(KeyValuePair<string, Rectangle> i in interactionObjects)
            {
                if(i.Value.Contains(mouseWorldPos))
                {
                    QuestSystem.QuestManager.InteractWithObject(i.Key);
                    return true;
                }
            }

            for(int i = animatedObjects.Count-1; i >= 0; i--)
            {
                if (!animatedObjects[i].IsLootable)
                    return false;

                if(animatedObjects[i].BoundingBox.Contains(mouseWorldPos))
                {
                    if (!Player.PlayerInteractionRectangle.Intersects(animatedObjects[i].BoundingBox)) //object not in range
                        return false;

                    Item itemToAdd = null;
                    itemToAdd = LootTable.questItems.Where(item => item.Id == animatedObjects[i].item.Id).First();

                    if (itemToAdd != null)
                        UIManager.charWindow.AddItemToInventory(itemToAdd);

                    animatedObjects.RemoveAt(i);
                }
            }
            return false;
        }

        public static void UpdateWorldObjects(GameTime gameTime)
        {
            foreach (ObjectAnimation ao in animatedObjects)
                ao.Update(gameTime);

            EffectManager.lightning.Update(Player.PlayerPositionRectangle, gameTime);
            ApplyGlyphEffects();

            GlyphManager.RemoveExpiredGlyphs();
        }

        public static void ApplyGlyphEffects()
        {
            if (Orbs.currentTimeSpan.Subtract(lastGlyphEffect).Seconds >= SECONDSBETWEENGLYPHEFFECT)
            {
                foreach(Glyph gl in placedGlyphs)
                {
                    //Water Orb
                    if (gl.Id == "rn06")
                        GlyphManager.ApplyWaterGlyphs();
                }
            }

            foreach(Glyph glyph in placedGlyphs)
            {
                if ((Orbs.currentTimeSpan - glyph.PlacedAt).Seconds >= 10)
                    glyph.expired = true;
            }
        }

        public static void DrawEnemyDrops(SpriteBatch spriteBatch)
        {
            foreach (EnemyDrop ed in enemyDrops)
                ed.Draw(spriteBatch);
        }

        public static void DrawAnimatedObjects(SpriteBatch spriteBatch)
        {
            foreach (ObjectAnimation oa in animatedObjects)
                oa.Draw(spriteBatch);
        }

        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            foreach (ItemContainer ic in lootableContainers)
                ic.Draw(spriteBatch);
        }

        public static void DrawPlacedGlyphes()
        {
            foreach (Glyph g in placedGlyphs)
                g.Draw(Orbs.spriteBatch);
        }

        /// <summary> 
        /// Changes the ambientColor (not the lights itself) to given color 
        /// </summary> 
        public static void ChangeAmbienteColor(Color color)
        {
            EffectManager.lightning.SwitchAmbientColor(color);
        }
    }
}
