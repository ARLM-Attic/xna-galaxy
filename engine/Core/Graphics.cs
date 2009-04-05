/**
 * Graphics.cs
 * 
 * 2D / 3D Graphics Implementation.
 * 
 * @file
 * @author
 *        2D Part: YongChul [Chris] Jin / id: anecia
 *                 [Please remove this line and put your name/id here instead]
 *        3D Part:
 *
 * Copyright (C) XNA Naver cafe, 2009. All rights reserved.
 * $Id: $
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;


namespace Galaxy.Core
{
    public sealed class Sprite
    {
        public  UInt32      id;
        public  Vector2     position;
        public  Texture2D   srcTexture;
        public  Texture2D   texture;
        public  Vector2     origin;
        public  Vector2     scale;
        public  float       rotation;
        public  bool        holdLife;

        public Sprite(UInt32 id, Texture2D image, Vector2 pos,
                       Vector2 origin, uint zoomX, uint zoomY,
                       float rotation, bool holdLife)
        {
            this.id       = id;
            position      = pos;
            srcTexture    = image;
            texture       = srcTexture;
            this.origin   = origin;
            scale.X       = zoomX / 100.0f;
            scale.Y       = zoomY / 100.0f;
            this.rotation = rotation;
            this.holdLife = holdLife;
        }

        ~Sprite()
        {
            Dispose();
        }

        public void Dispose()
        {
            texture     = null;
            srcTexture  = null;
        }

        public void Move(float x, float y)
        {
            position.X += x;
            position.Y += y;
        }

        public void SetZoom(uint zoomX, uint zoomY)
        {
            scale.X = zoomX / 100.0f;
            scale.Y = zoomY / 100.0f;
        }

        public void SetScale(Vector2 scale)
        {
            this.scale = scale;
        }

        public void SetRotation(float degrees)
        {
            rotation = MathHelper.ToRadians(degrees);
        }

        public void Rotate(float degrees)
        {
            float r360 = MathHelper.ToRadians(360);

            rotation += MathHelper.ToRadians(degrees);
            if ( rotation > r360 )
                rotation -= r360;
            if ( rotation < -r360 )
                rotation += r360;
        }
    };

    public static class Graphics
    {
        private static GraphicsDeviceManager grpDevMgr   = null;
        private static SpriteBatch           sprBatch    = null;
        private static LinkedList<Sprite>[]  layers      = null;
        private static uint                  maxLayerNum = 0;

        private enum SpriteLayer
        {
            LimitNumber    = 50

        };

        //
        // Summary:
        //     Gets or sets the current Framework.Graphics.GraphicsDevice.
        //
        // Returns:
        //     The current Framework.Graphics.GraphicsDevice.
        private static GraphicsDevice GraphicsDevice { get; set; }

        /**
         * Initializes Graphics Engine.
         * 
         * Initializes Graphics Engine, which provides
         * GraphicsDeviceManager and Sprite layer.
         * 
         * @param game          Game object
         * @param layerNum      The number of Sprite layers
         * 
         * @return  'true' if sucess, 'false' otherwise.
         */
        public static bool Initialize(Game game, uint layerNum)
        {
            Debug.Assert(game != null);
            if ( game == null || layerNum <= 0 )
                return false;
            if ( layerNum > (uint)SpriteLayer.LimitNumber )
            {
                Debug.Fail(
                    "Sprite Layer number you requested is over the limit.\n"
                    + "Please set the number under " + SpriteLayer.LimitNumber
                    + " and less " + SpriteLayer.LimitNumber + ".");
                return false;
            }

            grpDevMgr = new GraphicsDeviceManager(game);

            layers = new LinkedList<Sprite>[layerNum];
            Debug.Assert(layers != null);
            if ( layers == null )
            {
                Debug.Fail("Failed to create Sprite Layers. layerNum = "
                            + layerNum + ".");
                return false;
            }

            if ( layers != null )
            {
                uint     i;

                for ( i = 0; i < layerNum; i++ )
                {
                    layers[i] = new LinkedList<Sprite>();
                    if ( layers[i] == null )
                    {
                        Debug.Fail("Failed to create Sprite List. ("
                                    + i + " / " + layerNum  + ")");
                        break;
                    }
                }
                maxLayerNum = i;
            }

            return (maxLayerNum == layerNum);
        }

        /**
         * Finalize Graphics Engine.
         * 
         */
        public static void Finalize(Game game, uint layerNum)
        {
            Console.WriteLine("Finalize Galaxy Graphics Engine...");
        }

        /**
         * Clears specific layer.
         * 
         * All Sprites in layer will be removed.
         * 
         * @param layerNo       Layer index to be cleared
         */
        public static void ClearLayer(uint layerNo)
        {
            if ( layerNo < 1 || layerNo > maxLayerNum )
                return;
            layers[layerNo - 1].Clear();
        }

        /**
         * Clears All layers.
         * 
         * All Spritess in all layers will be removed.
         * 
         */
        public static void ClearAllLayer()
        {
            uint i;

            if ( maxLayerNum < 1 )
                return;
            for ( i = 0; i < maxLayerNum; i++ )
                ClearLayer(i);
        }

        /**
         * Sets the GraphicsDevice.
         * 
         * @param grpDevice     Graphics Device
         */
        public static void SetGraphicDevice(GraphicsDevice grpDevice)
        {
            GraphicsDevice = grpDevice;
            Debug.Assert(GraphicsDevice != null);
        }

        /**
         * Updates the screen, it draws 2D Sprites and 3D models to
         * the target screen.
         * 
         * @param gameTime     Provides a snapshot of timing values.
         */
        public static void UpdateScreen(GameTime gameTime)
        {
            int                     i;
            LinkedList<Sprite>     list;
            LinkedListNode<Sprite> node;
            Sprite                 spr;

            /* Draws 2D Sprites in the layers */
            
            // Gomdong : 3D와 혼용해서 그리기 위해서,  SaveStateMode 설정
            // 아네시아님께서 보시고, 다시 설정해 주시면 됩니다. 임시로 되게끔 바꿨습니다.
            // >> anecia: 네, 일단 곰동님이 수정 해 주신 방법을 이용하고 나중에 성능에 대해
            //            더 좋은 방안이 생기면 수정하는 것으로 하겠습니다.
            //
            sprBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            
            for ( i = 0; i < maxLayerNum; i++ )
            {
                list = layers[i];
                if ( list.Count() > 0 )
                {
                    node = list.First;
                    while ( node != null )
                    {
                        spr = node.Value;
                        sprBatch.Draw(spr.texture,
                                      spr.position,
                                      null,
                                      Color.White,
                                      spr.rotation,
                                      spr.origin,
                                      spr.scale,
                                      SpriteEffects.None,
                                      0);
                        if ( spr.holdLife == false )
                        {
                            list.Remove(node.Value);
                        }
                        node = node.Next;
                    }
                }
            }
            sprBatch.End();
        }

        //===================================================================
        // 2D Graphic Implimentation
        //===================================================================
        #region 2DGraphic

        /**
         * Initializes 2D Graphic system.
         * 
         * It should be called once before using 2D Graphic methods.
         * 
         * @return  true if the initialization of 2D system is succeed,
         *          false otherwise.
         */
        public static bool Init2DSystem()
        {
            Debug.Assert(GraphicsDevice != null);

            sprBatch = new SpriteBatch(GraphicsDevice);
            Debug.Assert(sprBatch != null);
            return (sprBatch != null);
        }

        /**
         * Clears the screen with the given color.
         * 
         * @param color     Color to clear the screen
         */
        public static void ClearScreen(Color color)
        {
            Debug.Assert(GraphicsDevice != null);
            GraphicsDevice.Clear(color);
        }

        public static void ClearScreen(ClearOptions options, Color color, float depth, int stencil)
        {
            Debug.Assert(GraphicsDevice != null);
            GraphicsDevice.Clear(options, color, depth, stencil);
        }

        /**
         * Puts a sprite to specific Sprite layer.
         * 
         * The sprite to be added to the Sprite layer is not drawn
         * immediately, it will be drawn when Update() method is called.
         * 
         * @param layerNo       Layer number which will have the Sprite,
         *                      it must be 1 or above.
         * @param image         Image, 2D texture
         * @param position      The location, in screen coordinates,
         *                      where the Sprite will be put.
         * 
         * @return  Sprite if success, null otherwise.
         */
        public static Sprite PutSprite(uint layerNo, Texture2D image,
                                       Vector2 position)
        {
            return PutSprite(layerNo, image, position,
                             new Vector2(image.Width/2, image.Height/2),
                             100, 100, 0, true);
        }

        /**
         * Puts a sprite to specific Sprite layer, specifying origin,
         * scale and rotation.
         * 
         * The sprite to be added to the Sprite layer is not drawn
         * immediately, it will be drawn when Update() method is called.
         * 
         * @param layerNo       Layer number which will have the Sprite,
         *                      it must be 1 or above.
         * @param image         Image, 2D texture
         * @param position      The location, in screen coordinates,
         *                      where the Sprite will be put.
         * @param origin        The origin of the sprite. Specify (0,0) for
         *                      the upper-left corner.
         * @param zoomX         Zoom value(scale) for the x-axis of the
         *                      sprite, 100 means 100%.
         * @param zoomY         Zoom value(scale) for the y-axis of the
         *                      sprite, 100 means 100%.
         * @param rotation      The angle, in radians, to rotate the sprite
         *                      around the origin.
         * 
         * @return  Sprite if success, null otherwise.
         */
        public static Sprite PutSprite(uint layerNo, Texture2D image,
                                       Vector2 position, Vector2 origin,
                                       uint zoomX, uint zoomY,
                                       float rotation,
                                       bool holdLife)
        {
            Sprite mySpr = null;

            Debug.Assert(layerNo <= maxLayerNum);
            if ( layerNo > maxLayerNum )
                return null;

            mySpr = new Sprite(0 /* not used yet */, image, position,
                               origin, zoomX, zoomY, rotation, holdLife);
            if ( mySpr != null )
            {
                if ( layers[layerNo - 1].AddLast(mySpr) == null )
                {
                    mySpr = null;
                }
            }
            return mySpr;
        }

        /**
         * Removed a sprite in specfic Sprite layer.
         * 
         * The image added to the image layer is not drawn immediately,
         * it will be drawn when Update() method is called.
         * 
         * @param layerNo       Layer number which has image.
         * @param image         Image to be removed.
         * 
         * @return  true if the image is removed, false otherwise.
         */
        public static bool RemoveImage(uint layerNo, Sprite image)
        {
            Debug.Assert(layerNo <= maxLayerNum);
            if ( layerNo <= maxLayerNum )
            {
                if ( layers[layerNo - 1].Remove(image) )
                {
                    image.Dispose();
                    image = null;
                }
            }
            return false;
        }

        public static void SetZoom(uint zoomX, uint zoomY)
        {
        }
        
        #endregion // 2DGraphic

        //===================================================================
        // 3D Graphic Implimentation
        //===================================================================
        #region 3DGraphic
    
        #endregion // 3DGraphic
    };
}
