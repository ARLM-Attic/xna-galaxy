using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;



namespace Galaxy.Core
{
    public class cPlayerPlane
    {
        public Model model = null;
        
        public Vector3 position = new Vector3(0,25,0); // 임시로 보이게 하기 위해 높게 띄워놈.
        public Vector3 rotation = Vector3.Zero;

        public float scale = 0.005f;
        public float rotation_rate = 0.05f;
        public float velocity = 0.0f;
        public float acceleration = 0.01f;      // 가속도
        public float friction = 0.0025f;           // 마찰
        public float max_veloctiy = 250.0f;     // 최대 속도
        
        public Vector3 direction = Vector3.Zero;
        public Vector3 up = Vector3.Up;

        private Matrix cameraProjectionMatrix;
        private Matrix cameraViewMatrix;

        // 플레이어 비행기를 생성합니다.
        public cPlayerPlane()
        {

        }

        public bool Load(ContentManager contentManager, string modelName)
        {
            try
            {
                model = contentManager.Load<Model>(modelName);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public void Update(GameTime gameTime, Matrix projMat, Matrix viewMat)
        {
            cameraProjectionMatrix = projMat;
            cameraViewMatrix = viewMat;

            if (Input.IsKeyDown(Keys.Left))
            {
                rotation.X += rotation_rate;
                rotation.Z += rotation_rate;
                if (rotation.Z > MathHelper.PiOver2)
                    rotation.Z = MathHelper.PiOver2;
            }
            else if (Input.IsKeyDown(Keys.Right))
            {
                rotation.X -= rotation_rate;
                rotation.Z -= rotation_rate;
                if (rotation.Z < -MathHelper.PiOver2)
                    rotation.Z = -MathHelper.PiOver2;
            }
            else if (Input.IsKeyDown(Keys.Up))
            {
                rotation.Y += rotation_rate;
            }
            else if (Input.IsKeyDown(Keys.Down))
            {
                rotation.Y -= rotation_rate;
            }
            else
            {
                //아무 키 안누르면 다시 원각도로 돌아옮.
                if (rotation.Z > 0)
                    rotation.Z -= rotation_rate;
                if (rotation.Z < 0)
                    rotation.Z += rotation_rate;
            }

            if (rotation.X > MathHelper.TwoPi)
                rotation.X -= MathHelper.TwoPi;
            if (rotation.X < -MathHelper.TwoPi)
                rotation.X += MathHelper.TwoPi;
            if (rotation.Y > MathHelper.TwoPi)
                rotation.Y -= MathHelper.TwoPi;
            if (rotation.Y < -MathHelper.TwoPi)
                rotation.Y += MathHelper.TwoPi;
            if (rotation.Z > MathHelper.TwoPi)
                rotation.Z -= MathHelper.TwoPi;
            if (rotation.Z < -MathHelper.TwoPi)
                rotation.Z += MathHelper.TwoPi;


            #region Move
            
            Vector3 vOrg = new Vector3(0, 0, -1);
            Vector3 vOrg_up = new Vector3(0, 1, 0);
            if (Input.IsKeyDown(Keys.Space))
            {
                TimeSpan timespan = new TimeSpan(0, 0, 0, 0, 500);  // 0.5초
                if (gameTime.ElapsedGameTime < timespan)            // 0.5초이내에 다시 스페이스를 누른다면?
                {
                    velocity += acceleration;
                    if (velocity > max_veloctiy)
                    {
                        velocity = max_veloctiy;
                    }
                }
            }
            else //아닐떄;
            {
                velocity -= friction;
                if (velocity < 0.0f)
                {
                    velocity = 0.0f;
                }
            }
            Matrix rotationMatrixX = Matrix.CreateRotationX(rotation.Y);
            Matrix rotationMatrixY = Matrix.CreateRotationY(rotation.X);
            Matrix rotationMatrixZ = Matrix.CreateRotationZ(rotation.Z);

            vOrg = Vector3.Transform(vOrg, rotationMatrixZ * rotationMatrixX * rotationMatrixY);
            vOrg_up = Vector3.Transform(vOrg_up, rotationMatrixZ * rotationMatrixX * rotationMatrixY);
            
            position += (velocity) * vOrg;
            direction = vOrg;   //비행기의 진행방향
            up = vOrg_up;       //비행기의 윗방향 
            
            #endregion
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = false;

                    effect.World = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z) *
                                   Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    
                    effect.Projection = cameraProjectionMatrix;
                    effect.View = cameraViewMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
