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
        // 3D ���� ������Ʈ���� ����
        cTerrain        m_Terrain;
        cPlayerPlane    m_PlayerPlane;

        Sprite   demoSpr  = null;
        int      zoom     = 100;
        int      zoomStep = 5;
        
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

            #region ���ΰ� ����� ����
            
            m_PlayerPlane = new cPlayerPlane();
            
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture2D   texture;

            Graphics.SetGraphicDevice(this.GraphicsDevice);
            Graphics.Init2DSystem();

            texture = Content.Load<Texture2D>("images\\sample");
            demoSpr = Graphics.PutSprite(1, texture, new Vector2(250, 250));


            // 3D Map�� Texture �ε�
            const string textureName = "Map\\texture";
            m_Terrain.LoadTexture(Content, textureName);

            // ���ΰ� ������ Model �ε�(FBX���Ͽ��� �ڵ����� �ؽ��ĸ� �о� ����)
            const string planeModelName = "3D_Object\\ship";
            m_PlayerPlane.Load(Content, planeModelName);

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
                        Input.IsButtonPressed(Buttons.Start) )
            {
                this.Exit();
            }

            /* Just test code to move, zoom and rotation the demo sprite */
            if ( demoSpr != null )
            {
                demoSpr.Rotate(1);
                demoSpr.SetZoom((uint)zoom, (uint)zoom);
                zoom += zoomStep;
                if ( zoom > 500 )
                {
                    zoomStep = -5;
                    zoom = 500;
                }
                if ( zoom < 0 )
                {
                    zoomStep = 5;
                    zoom = 0;
                }

                if ( Input.IsKeyDown(Keys.Left) ||
                                Input.IsButtonDown(Buttons.DPadLeft) )
                {
                    demoSpr.Move(-2.0f, 0.0f);
                }
                if ( Input.IsKeyDown(Keys.Right) ||
                                Input.IsButtonDown(Buttons.DPadRight) )
                {
                    demoSpr.Move(2.0f, 0.0f);
                }
                if ( Input.IsKeyDown(Keys.Up) ||
                                Input.IsButtonDown(Buttons.DPadUp) )
                {
                    demoSpr.Move(0.0f, -2.0f);
                }
                if ( Input.IsKeyDown(Keys.Down) ||
                                Input.IsButtonDown(Buttons.DPadDown) )
                {
                    demoSpr.Move(.0f, 2.0f);
                }

                if ( Input.IsKeyPressed(Keys.Space) ||
                                Input.IsButtonPressed(Buttons.A) )
                {
                    Graphics.RemoveImage(1, demoSpr);
                    demoSpr = null;
                }
            }

            UpdateCamera(ref projectionMatrix, ref viewMatrix);
            
            m_Terrain.Update(projectionMatrix, viewMatrix);
            m_PlayerPlane.Update(gameTime, projectionMatrix, viewMatrix);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.ClearScreen(Color.CornflowerBlue);

            m_Terrain.Draw(GraphicsDevice);
            m_PlayerPlane.Draw();

            Graphics.UpdateScreen(gameTime);

            base.Draw(gameTime);
        }
    }
}