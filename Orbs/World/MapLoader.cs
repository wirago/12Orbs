using Orbs.Tiled;

namespace Orbs.World
{
    public static class MapLoader
    {
        public static TileMap loadMap(string map)
        {
            Orbs.curLevel = map;
            return new TileMap(@"Content/Maps/" + map + ".tmx");
        }

        public static void changeMap(string map, string spawnTag)
        {
            TileMap newMap = new TileMap(@"Content/Maps/" + map + ".tmx");
            Orbs.curLevel = map;

            Orbs.curMap = newMap;
            Orbs.musicManager.ChangeBackgroundMusic(newMap.GetBackgroundMusicFromMap());
            Orbs.camera.worldWidth = newMap.worldWidth;
            Orbs.camera.worldHeight = newMap.worldHeight;

            if (spawnTag == "")
                Orbs.respawnPlayer = true;
            else
                Player.respawnAt = newMap.transitionTags[spawnTag];
            
            QuestSystem.QuestManager.InitializeLevel();
        }
    }
}
