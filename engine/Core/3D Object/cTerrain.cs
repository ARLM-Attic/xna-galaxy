using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;


namespace Galaxy.Core
{
    public class cTerrain
    {
        private VertexBuffer m_VertexBuffer;
        private IndexBuffer m_IndexBuffer;

        private UInt32 m_Width;
        private UInt32 m_Height;

        private Texture2D m_Texture;

        private BasicEffect m_TerrainEffect;

        // 지형을 생성합니다.
        public cTerrain()
        {
            m_VertexBuffer = null;
            m_IndexBuffer = null;
            m_TerrainEffect = null;
        }

        public bool Init(GraphicsDevice device, string fileName)
        {
            int[,] heightmap_Data = null;

            if (LoadHeightData(fileName, out heightmap_Data) == true)
            {
                SetupVertices(device, ref heightmap_Data, m_Width, m_Height);
                SetupIndices(device, m_Width, m_Height);
                CreateTerrainEffect(device);
            }

            return true;
        }

        private void CreateTerrainEffect(GraphicsDevice device)
        {
            m_TerrainEffect = new BasicEffect(device, null);
        }

        public bool LoadTexture(ContentManager contentManager, string textureName)
        {
            try
            {
                m_Texture = contentManager.Load<Texture2D>(textureName);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private bool LoadHeightData(string fileName, out int[,] heightmap_Data)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);

            br.BaseStream.Seek(10, SeekOrigin.Current);

            int offset = (int)br.ReadUInt32();

            br.BaseStream.Seek(4, SeekOrigin.Current);

            m_Width = br.ReadUInt32();
            m_Height = br.ReadUInt32();

            br.BaseStream.Seek(offset - 26, SeekOrigin.Current);

            heightmap_Data = new int[m_Width, m_Height];

            const int Yscale = 20;

            for (UInt32 z = 0; z < m_Height; z++)
            {
                for (UInt32 x = 0; x < m_Width; x++)
                {
                    int tempheight = ((int)(br.ReadByte()) + (int)(br.ReadByte()) + (int)(br.ReadByte())) / Yscale;

                    heightmap_Data[m_Width - 1 - x, m_Height - 1 - z] = tempheight;
                }
            }

            br.Close();

            return true;
        }

        private bool SetupVertices(GraphicsDevice device, ref int[,] heightMap_data, UInt32 width, UInt32 height)
        {
            Vector3 position;
            Vector2 textureCoordinates;

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    position = new Vector3(x, heightMap_data[x, z], z);
                    vertices[x + z * width].Normal = new Vector3(0, -1, 0);

                    textureCoordinates = new Vector2(x / (float)width, z / (float)height);
                    vertices[x + z * width] = new VertexPositionNormalTexture(position, Vector3.Forward, textureCoordinates);
                }
            }
            for (int x = 1; x < width - 1; x++)
            {
                for (int z = 1; z < height - 1; z++)
                {
                    Vector3 normX = new Vector3((vertices[x - 1 + z * width].Position.Y - vertices[x + 1 + z * width].Position.Y) / 2, 1, 0);
                    Vector3 normZ = new Vector3(0, 1, (vertices[x + (z - 1) * width].Position.Y - vertices[x + (z + 1) * width].Position.Y) / 2);
                    vertices[x + z * width].Normal = -normX + -normZ;
                    vertices[x + z * width].Normal.Normalize();
                }
            }

            int sizeInBytes = (int)(VertexPositionNormalTexture.SizeInBytes * width * height);

            if (device != null)
            {
                m_VertexBuffer = new VertexBuffer(device, sizeInBytes, BufferUsage.WriteOnly);       //버텍스 버퍼 생성
                m_VertexBuffer.SetData(vertices);   //버텍스 버퍼 저장
            }

            return true;
        }

        private bool SetupIndices(GraphicsDevice device, UInt32 width, UInt32 height)
        {
            UInt32[] indices = new UInt32[(width - 1) * (height - 1) * 3 * 2];

            for (UInt32 x = 0; x < width - 1; x++)
            {
                for (UInt32 z = 0; z < height - 1; z++)
                {
                    indices[(x + z * (width - 1)) * 3 * 2 + 0] = (x + 1) + (z + 1) * width;
                    indices[(x + z * (width - 1)) * 3 * 2 + 1] = (x + 1) + (z + 0) * width;
                    indices[(x + z * (width - 1)) * 3 * 2 + 2] = (x + 0) + (z + 0) * width;

                    indices[(x + z * (width - 1)) * 3 * 2 + 3] = (x + 1) + (z + 1) * width;
                    indices[(x + z * (width - 1)) * 3 * 2 + 4] = (x + 0) + (z + 0) * width;
                    indices[(x + z * (width - 1)) * 3 * 2 + 5] = (x + 0) + (z + 1) * width;
                }
            }

            int sizeInBytes = (int)((width - 1) * (height - 1) * 6);

            if (device != null)
            {
                m_IndexBuffer = new IndexBuffer(device, typeof(int), sizeInBytes, BufferUsage.WriteOnly);   //인덱스 버퍼 생성
                m_IndexBuffer.SetData(indices);  //인데스 버퍼 저장
            }

            return true;
        }


        public void Update(Matrix projMat, Matrix viewMat)
        {
            m_TerrainEffect.Projection = projMat;
            m_TerrainEffect.View = viewMat;
            m_TerrainEffect.World = Matrix.Identity;
        }


        public void Draw(GraphicsDevice device)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);  //Z 버퍼 기능을 킨다.

            device.RenderState.CullMode = CullMode.CullClockwiseFace;

            m_TerrainEffect.TextureEnabled = true;
            m_TerrainEffect.Texture = m_Texture;

            //조명 효과            
            m_TerrainEffect.EnableDefaultLighting();
            m_TerrainEffect.DirectionalLight0.Direction = new Vector3(0.0f, 0.4f, 0.3f);
            m_TerrainEffect.SpecularColor = new Vector3(0.4f, 0.4f, 0.4f);
            m_TerrainEffect.SpecularPower = 8;

            //안개 효과
            m_TerrainEffect.FogEnabled = true;
            m_TerrainEffect.FogColor = new Vector3(0.15f);
            m_TerrainEffect.FogStart = 100;
            m_TerrainEffect.FogEnd = 120;

            m_TerrainEffect.Begin();

            foreach (EffectPass pass in m_TerrainEffect.CurrentTechnique.Passes) // Technique는 Pass를 여러개 가질 수 있습니다. (MultiPass)
            {
                pass.Begin();

                // scene은 반드시 이 사이에 와야 합니다.
                device.Vertices[0].SetSource(m_VertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                device.Indices = m_IndexBuffer;
                device.VertexDeclaration = new VertexDeclaration(device, VertexPositionNormalTexture.VertexElements); // vertex의 속성을 설정
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)(m_Width * m_Height), 0, (int)((m_Width - 1) * (m_Height - 1) * 2)); // Draw.

                pass.End();
            }

            m_TerrainEffect.End();
        }
    }
}
