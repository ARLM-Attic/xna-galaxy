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
        // 3D 관련 오브젝트들을 선언
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

            #region 지형 생성

            const string heightMap_Name = "content\\map\\smile.bmp";

            m_Terrain = new cTerrain();
            m_Terrain.Init(GraphicsDevice, heightMap_Name);
            
            #endregion

            #region 주인공 비행기 생성
            
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


            // 3D Map용 Texture 로드
            const string textureName = "Map\\texture";
            m_Terrain.LoadTexture(Content, textureName);

            // 주인공 비행기용 Model 로드(FBX파일에서 자동으로 텍스쳐를 읽어 들임)
            const string planeModelName = "3D_Object\\ship";
            m_PlayerPlane.Load(Content, planeModelName);

        }

        protected override void UnloadContent()
        {

        }


        // 임시로 카메라 Update 함수 만듬.
        private void UpdateCamera(ref Matrix projectionMatrix, ref Matrix viewMatrix)
        {
            Vector3 cameraPosition = new Vector3(200.0f, 200.0f, 200.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 cameraUpvector = Vector3.Up;

            float aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;    //화면의 종횡비를 저장
            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,    //카메라의 앵글
                                                                            aspectRatio,         //카메라의 세로 비율
                                                                          0.1f, 1000.0f);        //카메라의 최소, 최대 구간

            viewMatrix = Matrix.CreateLookAt(cameraPosition,      // 카메라의 위치 
                                                    cameraTarget,        // 카메라의 볼위치 
                                                    cameraUpvector);     // 카메라의 업벡터  
        }

        // 임시로 카메라 행렬 변수 선언.
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
            /* Z 버퍼 기능을 킨다. */
            Graphics.ClearScreen(ClearOptions.Target | ClearOptions.DepthBuffer,
                                 Color.CornflowerBlue, 1.0f, 0);

            m_Terrain.Draw(GraphicsDevice);
            m_PlayerPlane.Draw();

            Graphics.UpdateScreen(gameTime);

            base.Draw(gameTime);
        }
    }
}