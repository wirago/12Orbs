using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbs.LootSystem;
using System.Collections.Generic;

namespace Orbs.Animations
{
    public class ObjectAnimation
    {
        private Texture2D spriteTexture;

        private Color tintColor = Color.White;

        protected Vector2 spritePosition = Vector2.Zero;
        protected Vector2 lastSpritePosition = Vector2.Zero;
        private Vector2 spriteCenter;
        private int spriteWidth;
        private int spriteHeight;
        public Item item;
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsLootable { get; set; }

        // Dictionary holding all of the FrameAnimation objects
        Dictionary<string, FrameAnimation> frameAnimations = new Dictionary<string, FrameAnimation>();

        string currentAnimation = null;

        //Simple Animated Objects
        public ObjectAnimation(string textureName, int posX, int posY, int width, int height, string name, string type)
        {
            spriteTexture = Orbs.content.Load<Texture2D>(@"Animations\" + textureName);
            Position = new Vector2(posX, posY);
            Name = name;
            Type = type;
            IsLootable = false;

            AddAnimation("default", 0, 0, width, height, 4, 0.2f);
            CurrentAnimation = "default";
        }

        //Lootable Animated Objects
        public ObjectAnimation(string itemId, string textureName, int posX, int posY, int width, int height, string name, string type, string icon)
        {
            spriteTexture = Orbs.content.Load<Texture2D>(@"Animations\" + textureName);
            Position = new Vector2(posX, posY);
            Name = name;
            Type = type;
            IsLootable = true;

            item = new Item();
            item.Id = itemId;
            item.Name = name;

            switch (type)
            {
                case "QItem":
                    item.Type = Item.type.QuestItem;
                    item.IconTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Questitems\" + icon);
                    break;
                case "Material":
                    item.Type = Item.type.Material;
                    item.IconTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Material\" + icon);
                    break;
                case "Weapon":
                    item.Type = Item.type.Weapon;
                    item.IconTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Weapons\" + icon);
                    break;
                case "Consumable":
                    item.Type = Item.type.Consumable;
                    item.IconTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Consumables\" + icon);
                    break;
                default:
                    item.Type = Item.type.Material;
                    item.IconTexture = Orbs.content.Load<Texture2D>(@"UserInterface\Icons\Material\" + icon);
                    break;
            }

            AddAnimation("default", 0, 0, width, height, 4, 0.2f);
            CurrentAnimation = "default";
        }

        public Vector2 Position
        {
            get { return spritePosition; }
            set
            {
                lastSpritePosition = spritePosition;
                spritePosition = value;
            }
        }

        public int X
        {
            get { return (int)spritePosition.X; }
            set
            {
                lastSpritePosition.X = spritePosition.X;
                spritePosition.X = value;
            }
        }

        public int Y
        {
            get { return (int)spritePosition.Y; }
            set
            {
                lastSpritePosition.Y = spritePosition.Y;
                spritePosition.Y = value;
            }
        }

        public int Width
        {
            get { return spriteWidth; }
        }

        public int Height
        {
            get { return spriteHeight; }
        }

        //Screen coordinates of the bounding box surrounding this sprite
        public Rectangle BoundingBox
        {
            get { return new Rectangle(X, Y, spriteWidth, spriteHeight); }
        }

        public Texture2D Texture
        {
            get { return spriteTexture; }
        }

        //FrameAnimation object of the currently playing animation
        public FrameAnimation CurrentFrameAnimation
        {
            get
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return frameAnimations[currentAnimation];
                else
                    return null;
            }
        }

        public string CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                if (frameAnimations.ContainsKey(value))
                    currentAnimation = value;
            }
        }

        public void AddAnimation(string Name, int X, int Y, int Width, int Height, int Frames, float FrameLength)
        {
            frameAnimations.Add(Name, new FrameAnimation(X, Y, Width, Height, Frames, FrameLength));
            spriteWidth = Width;
            spriteHeight = Height;
            spriteCenter = new Vector2(spriteWidth / 2, spriteHeight / 2);
        }

        public void Update(GameTime gameTime)
        {
            CurrentFrameAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(IsVisible())
                spriteBatch.Draw(spriteTexture, spritePosition, CurrentFrameAnimation.FrameRectangle, tintColor);

            //For Debugging
            //int bw = 2; // Border width
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Left
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Right, BoundingBox.Top, bw, BoundingBox.Height), Color.Black); // Right
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Top, BoundingBox.Width, bw), Color.Black); // Top
            //spriteBatch.Draw(Game1.pixel, new Rectangle(BoundingBox.Left, BoundingBox.Bottom, BoundingBox.Width, bw), Color.Black); // Bottom

        }

        private bool IsVisible()
        {
            if (Position.X < Orbs.camera.Position.X - 50 || Position.Y < Orbs.camera.Position.Y - 50 ||
                    Position.X > Orbs.camera.Position.X + Orbs.camera.viewWidth + 50 || Position.Y > Orbs.camera.Position.Y + Orbs.camera.viewHeight + 50)
                return false;

            return true;
        }
    }
}
