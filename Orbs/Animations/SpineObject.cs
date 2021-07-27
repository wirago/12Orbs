using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;


namespace Orbs.Animations
{
    public class SpineObject
    {
        private SkeletonRenderer skeletonRenderer;
        public Skeleton skeleton;
        private AnimationState state;
        private SkeletonBounds bounds = new SkeletonBounds();
        private string assetsFolder = "Content/Animations/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetPath">Path within Content/Animations/ including name</param>
        /// <param name="position"></param>
        public SpineObject(string assetPath, Vector2 position)
        {
            skeletonRenderer = new SkeletonRenderer(Orbs.graphics.GraphicsDevice);
            skeletonRenderer.PremultipliedAlpha = false;

            String fullPath = assetsFolder + assetPath;

            Atlas atlas = new Atlas(fullPath + ".atlas", new XnaTextureLoader(Orbs.graphics.GraphicsDevice));

            SkeletonJson json = new SkeletonJson(atlas);
            json.Scale = 1;
            SkeletonData skeletonData = json.ReadSkeletonData(fullPath + ".json");
            skeleton = new Skeleton(skeletonData);
            AnimationStateData stateData = new AnimationStateData(skeleton.Data);
            state = new AnimationState(stateData);
            state.SetAnimation(0, "idleb", true);

            skeleton.X = position.X;
            skeleton.Y = position.Y;
            skeleton.ScaleY = 0.5f;
            skeleton.ScaleX = 0.5f;
        }

        public void Update(GameTime gameTime)
        {
            state.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            skeleton.UpdateWorldTransform();
            state.Apply(skeleton);
        }

        public void SetPosition(Vector2 position)
        {
            skeleton.X = position.X;
            skeleton.Y = position.Y;
        }

        public void SetAnimation(string animationName)
        {
            if (state.GetCurrent(0).Animation.Name != animationName)
                state.SetAnimation(0, animationName, true);

            //state.TimeScale = 2f;
        }

        public void Draw()
        {
            ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(0, Orbs.graphics.PreferredBackBufferWidth, Orbs.graphics.PreferredBackBufferHeight, 0, 1, 0);
            ((BasicEffect)skeletonRenderer.Effect).World = Orbs.camera.GetViewMatrix();

            skeletonRenderer.Begin();
            skeletonRenderer.Draw(skeleton);
            skeletonRenderer.End();
        }
    }
}
