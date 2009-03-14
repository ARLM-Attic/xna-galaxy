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
    public sealed class Image2D
    {
        public UInt32      id;
        public Vector2     position;
        public Texture2D   texture;
        public bool        holdLife;

        public void Move(float x, float y)
        {
            position.X += x;
            position.Y += y;
        }
    };

    public static class Graphics
    {
        private static GraphicsDeviceManager grpDevMgr   = null;
        private static SpriteBatch           sprBatch    = null;
        private static LinkedList<Image2D>[] layers      = null;
        private static uint                  maxLayerNum = 0;

        private enum ImageLayer
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
         * GraphicsDeviceManager and image layer.
         * 
         * @param game          Game object
         * @param layerNum      The number of image layers
         * 
         * @return  'true' if sucess, 'false' otherwise.
         */
        public static bool Initialize(Game game, uint layerNum)
        {
            Debug.Assert(game != null);
            if ( game == null || layerNum <= 0 )
                return false;
            if ( layerNum > (uint)ImageLayer.LimitNumber )
            {
                Debug.Fail(
                    "Image Layer number you requested is over the limit.\n"
                    + "Please set the number under " + ImageLayer.LimitNumber
                    + " and less " + ImageLayer.LimitNumber + ".");
                return false;
            }

            grpDevMgr = new GraphicsDeviceManager(game);

            layers = new LinkedList<Image2D>[layerNum];
            Debug.Assert(layers != null);
            if ( layers == null )
            {
                Debug.Fail("Failed to create Image Layers. layerNum = "
                            + layerNum + ".");
                return false;
            }

            if ( layers != null )
            {
                uint     i;

                for ( i = 0; i < layerNum; i++ )
                {
                    layers[i] = new LinkedList<Image2D>();
                    if ( layers[i] == null )
                    {
                        Debug.Fail("Failed to create Image List. ("
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
         * All images in layer will be removed.
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
         * All images in all layers will be removed.
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
         * Updates the screen, it draws 2D images and 3D models to
         * the target screen.
         * 
         * @param gameTime     Provides a snapshot of timing values.
         */
        public static void UpdateScreen(GameTime gameTime)
        {
            int                     i;
            LinkedList<Image2D>     list;
            LinkedListNode<Image2D> node;
            Image2D                 image;

            /* Draws 2D images in the layers */
            sprBatch.Begin();
            for ( i = 0; i < maxLayerNum; i++ )
            {
                list = layers[i];
                if ( list.Count() > 0 )
                {
                    node = list.First;
                    while ( node != null )
                    {
                        image = node.Value;
                        sprBatch.Draw(image.texture, image.position,
                                      Color.White);
                        if ( image.holdLife == false )
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

        /**
         * Puts the image to an image layer.
         * 
         * The image added to the image layer is not drawn immediately,
         * it will be drawn when Update() method is called.
         * 
         * @param layerNo       Layer number which will have the image,
         *                      it must be 1 or above.
         * @param image         Image, 2D texture
         * @param position      The location, in screen coordinates,
         *                      where the image will be put.
         * 
         * @return  Image2D if success, null otherwise.
         */
        public static Image2D PutImage(uint layerNo, Texture2D image, Vector2 position)
        {
            Image2D myImage = null;

            Debug.Assert(layerNo <= maxLayerNum);
            if ( layerNo > maxLayerNum )
                return null;

            myImage = new Image2D();
            if ( myImage != null )
            {
                myImage.id = 1;
                myImage.position = position;
                myImage.texture = image;
                myImage.holdLife = true;

                if ( layers[layerNo - 1].AddLast(myImage) == null )
                {
                    myImage = null;
                }
            }

            return myImage;
        }
        
        #endregion // 2DGraphic

        //===================================================================
        // 3D Graphic Implimentation
        //===================================================================
        #region 2DGraphic
    
        #endregion // 3DGraphic
    };
}
