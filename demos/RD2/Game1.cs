using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Galaxy.Core;

namespace RD2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        cTerrain m_Terrain;
        Image2D  demoImage = null;
        
        public Game1()
        {
            Graphics.Initialize(this, 5);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            #region ���� ����

            const string heightMap_Name = "content\\map\\smile.bmp";

            m_Terrain = new cTerrain();
            m_Terrain.Init(GraphicsDevice, heightMap_Name);
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture2D   texture;

            Graphics.SetGraphicDevice(this.GraphicsDevice);
            Graphics.Init2DSystem();

            texture = Content.Load<Texture2D>("images\\sample");
            demoImage = Graphics.PutImage(1, texture, new Vector2(50, 50));


            // 3D Map�� Texture �ε�
            const string textureName = "Map\\texture";
            m_Terrain.LoadTexture(Content, textureName);
        }

        protected override void UnloadContent()
        {

        }


        // �ӽ÷� ī�޶� Update �Լ� ����.
        private void UpdateCamera(ref Matrix projectionMatrix, ref Matrix viewMatrix)
        {
            Vector3 cameraPosition = new Vector3(80.0f, 80.0f, 80.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 cameraUpvector = Vector3.Up;

            float aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;    //ȭ���� ��Ⱦ�� ����
            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,    //ī�޶��� �ޱ�
                                                                            aspectRatio,         //ī�޶��� ���� ����
                                                                          0.1f, 1000.0f);        //ī�޶��� �ּ�, �ִ� ����

            viewMatrix = Matrix.CreateLookAt(cameraPosition,      // ī�޶��� ��ġ 
                                                    cameraTarget,        // ī�޶��� ����ġ 
                                                    cameraUpvector);     // ī�޶��� ������  
        }

        // �ӽ÷� ī�޶� ��� ���� ����.
        Matrix projectionMatrix = new Matrix();
        Matrix viewMatrix = new Matrix();

        protected override void Update(GameTime gameTime)
        {
            Galaxy.Core.Input.Update();

            // Allows the game to exit
            if ( Input.IsKeyPressed(Keys.Escape) ||
                Input.IsKeyPressed(Keys.Enter) ||
                Input.IsKeyPressed(Keys.Space) ||
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed )
            {
                this.Exit();
            }

            /* Just test code to moving the demo image */
            if ( Input.IsKeyPressed(Keys.Left) )
            {
                demoImage.Move(-2.0f, 0.0f);
            }
            if ( Input.IsKeyPressed(Keys.Right) )
            {
                demoImage.Move(2.0f, 0.0f);
            }
            if ( Input.IsKeyPressed(Keys.Up) )
            {
                demoImage.Move(0.0f, -2.0f);
            }
            if ( Input.IsKeyPressed(Keys.Down) )
            {
                demoImage.Move(.0f, 2.0f);
            }

            UpdateCamera(ref projectionMatrix, ref viewMatrix);
            
            m_Terrain.Update(projectionMatrix, viewMatrix);           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.ClearScreen(Color.CornflowerBlue);

            m_Terrain.Draw(GraphicsDevice);

            Graphics.UpdateScreen(gameTime);

            base.Draw(gameTime);
        }
    }
}