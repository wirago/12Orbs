using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Orbs.NPC;
using Orbs.Tiled;

namespace Orbs.World
{
    public static class WorldCharacters
    {

        public static List<EnemyNPC> enemyWorldCharacters = new List<EnemyNPC>();
        public static List<StandardNPC> standardWorldCharacters = new List<StandardNPC>();

        private const string SOURCE_CHARANIMATIONS = @"Animations\Characters\";
        public static void GenerateWorldNPC(TmxObject o)
        {
            //Moving NPC
            if (o.Type.EndsWith("_m"))
            {
                string imageSource = o.Type.Remove(o.Type.Length - 2);

                new StandardNPC(
                    Orbs.content.Load<Texture2D>(SOURCE_CHARANIMATIONS + imageSource),
                    GetCoordFromMap(o),
                    (int)o.Height,
                    (int)o.Width,
                    o.Name,
                    o.Properties["id"],
                    (int)o.Rotation,
                    o.Properties.ContainsKey("portrait") ? o.Properties["portrait"] : "",
                    true
                    );
            }
            else //Static NPC
            {
                new StandardNPC(
                    Orbs.content.Load<Texture2D>(SOURCE_CHARANIMATIONS + o.Type),
                    GetCoordFromMap(o),
                    (int)o.Height,
                    (int)o.Width,
                    o.Name,
                    o.Properties["id"],
                    (int)o.Rotation,
                    o.Properties.ContainsKey("portrait") ? o.Properties["portrait"] : "",
                    o.Properties.ContainsKey("offeredItems") ? o.Properties["offeredItems"] : "",
                    o.Properties.ContainsKey("soundfile") ? o.Properties["soundfile"] : ""
                    );
            }
        }

        static List<Waypoint> GetCoordFromMap(TmxObject o)
        {
            List<Waypoint> waypoints = new List<Waypoint>();

            waypoints.Add(new Waypoint((float)o.X, (float)o.Y));

            int currentID = 1;
            bool isNextID = o.Properties.ContainsKey("targetX" + currentID);

            while (isNextID)
            {
                float x = float.Parse(o.Properties["targetX" + currentID]);
                float y = float.Parse(o.Properties["targetY" + currentID]);

                waypoints.Add(new Waypoint(x, y));

                currentID++;

                if (!o.Properties.ContainsKey("targetX"+currentID))
                {
                    isNextID = false;
                }
            }

            return waypoints;
        }

        public static void GenerateWorldEnemy(TmxObject o, LootSystem.EnemyTemplate enemy)
        {
            new EnemyNPC(
                Orbs.content.Load<Texture2D>(SOURCE_CHARANIMATIONS + enemy.texture),
                GetCoordFromMap(o),
                bool.Parse(o.Properties["isHostile"]),
                enemy
                );
        }

        //public static void GenerateWorldEnemy(TmxObject o)
        //{
        //    //Moving Enemy
        //    if(o.Type.EndsWith("_m"))
        //    {
        //        string imageSource = o.Type.Remove(o.Type.Length - 2);

        //        new EnemyNPC(
        //            Orbs.content.Load<Texture2D>(SOURCE_CHARANIMATIONS + imageSource),
        //            getCoordFromMap(o),
        //            (int)o.Height,
        //            (int)o.Width,
        //            2,
        //            o.Type,
        //            bool.Parse(o.Properties["isHostile"]),
        //            o.Name,
        //            o.Properties.ContainsKey("attackAnimation") ? o.Properties["attackAnimation"] : null
        //            );
        //    }
        //    else //Static Enemy
        //    {
        //        new EnemyNPC(
        //            Orbs.content.Load<Texture2D>(SOURCE_CHARANIMATIONS + o.Type),
        //            getCoordFromMap(o),
        //            (int)o.Height,
        //            (int)o.Width,
        //            o.Type,
        //            bool.Parse(o.Properties["isHostile"]),
        //            o.Name,
        //            o.Properties.ContainsKey("attackanimation") ? o.Properties["attackAnimation"] : null
        //            );
        //    }
        //}

        public static void DrawWorldCharacters()
        {
            foreach (StandardNPC npc in standardWorldCharacters)
            {
                npc.Draw(Orbs.spriteBatch, null);
                npc.DrawName(Orbs.spriteBatch);
            }

            foreach (EnemyNPC eNPC in enemyWorldCharacters)
            {
                eNPC.Draw(Orbs.spriteBatch);
                //eNPC.DrawStatus(Orbs.spriteBatch);
            }
        }

        public static void UpdateWorldCharacters(GameTime gameTime)
        {
            foreach (StandardNPC npc in standardWorldCharacters)
                npc.UpdateNPC(gameTime);

            for (int i = 0; i <= enemyWorldCharacters.Count - 1; i++)
                enemyWorldCharacters[i].UpdateEnemyNPC(gameTime);
        }

        public static string CheckLeftMouseClick(Vector2 mouseWorldPos, Vector2 mouseWindowPos)
        {
            foreach (StandardNPC npc in standardWorldCharacters)
            {
                if (npc.BoundingBox.Contains(mouseWorldPos))
                {
                    npc.Interact(npc.Name);
                    return "npc";
                }
            }

            foreach (EnemyNPC enpc in enemyWorldCharacters)
            {
                if(enpc.BoundingBox.Contains(mouseWorldPos))
                    return "enemy";
            }

            return string.Empty;
        }

        public static string CheckRightMouseClick(Vector2 mouseWorldPos)
        {
            foreach (EnemyNPC enpc in enemyWorldCharacters)
            {
                if (enpc.BoundingBox.Contains(mouseWorldPos))
                    return "enemy";
            }

            return string.Empty;
        }
    }
}
