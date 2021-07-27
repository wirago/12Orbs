namespace Orbs.LootSystem
{

    public enum EnemyType { Creep, Elite, Boss };
    public enum ElementType { none, nature, earth, water, fire, arcane };
    public class EnemyTemplate
    {
        public EnemyType EnemyType { get; set; }
        public ElementType ElementType { get; set; }

        public int minDmg;
        public int maxDmg;
        public int attackSpeed;
        public int movementSpeed;
        public int maxHealth;

        public int textureHeight;
        public int textureWidth;

        public string uid;
        public string texture;
        public string attackAnimation;
        public string name;
    }
}
