using Assimp;
using Assimp.Configs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    public class Model
    {
        List<MTexture> textures_loaded = new List<MTexture>();
        List<MMesh> meshes = new List<MMesh>();

        public Model(string path, bool gamma = false)
        {
            LoadModel(path);
        }

        public void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }

        }

        public void LoadModel(string path)
        {
            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));

            var scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
            if (scene == null || scene.RootNode == null)
            {
                throw new Exception("load model error!");
            }

            ProcessNode(scene.RootNode, scene);
        }

        private void ProcessNode(Node node, Scene scene)
        {
            foreach (var index in node.MeshIndices)
            {
                var mesh = scene.Meshes[index];
                var mmesh = ProcessMesh(mesh, scene);
                meshes.Add(mmesh);
            }
            

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private MMesh ProcessMesh(Mesh mesh, Scene scene)
        {
            List<float> vertices = new List<float>();
            List<int> indices = new List<int>();
            List<MTexture> textures = new List<MTexture>();
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                vertices.Add(mesh.Vertices[i].X);
                vertices.Add(mesh.Vertices[i].Y);
                vertices.Add(mesh.Vertices[i].Z);

                if (mesh.HasNormals)
                {
                    vertices.Add(mesh.Normals[i].X);
                    vertices.Add(mesh.Normals[i].Y);
                    vertices.Add(mesh.Normals[i].Z);
                }
                else
                {
                    vertices.Add(0);
                    vertices.Add(0);
                    vertices.Add(0);
                }

                if (mesh.HasTextureCoords(0))
                {
                    vertices.Add(mesh.TextureCoordinateChannels[0][i].X);
                    vertices.Add(mesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    vertices.Add(0);
                    vertices.Add(0);
                }
            }


            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add(face.Indices[j]);
                }
            }

            var material = scene.Materials[mesh.MaterialIndex];

            List<MTexture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            textures.AddRange(diffuseMaps);

            List<MTexture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
            textures.AddRange(specularMaps);

            List<MTexture> normalMaps = LoadMaterialTextures(material, TextureType.Normals, "texture_normal");
            textures.AddRange(normalMaps);

            List<MTexture> heightMaps = LoadMaterialTextures(material, TextureType.Height, "texture_height");
            textures.AddRange(heightMaps);

            return new MMesh(vertices.ToArray(), indices.ToArray(), textures.ToArray());
        }

        private List<MTexture> LoadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<MTexture> mTextures = new List<MTexture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                mat.GetMaterialTexture(type, i, out TextureSlot slot);
                bool skip = false;
                for (int j = 0; j < textures_loaded.Count; j++)
                {
                    if (textures_loaded[j].path == slot.FilePath)
                    {
                        mTextures.Add(textures_loaded[j]);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    MTexture mTexture = new MTexture();
                    mTexture.id = LoadTexture(slot.FilePath);
                    mTexture.type = typeName;
                    mTexture.path = slot.FilePath;
                    mTextures.Add(mTexture);
                    textures_loaded.Add(mTexture);
                }
            }

            return mTextures;
        }

        private int LoadTexture(String fileName)
        {
            fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"objects", fileName);
            if (!File.Exists(fileName))
            {
                return 0;
            }
            Bitmap textureBitmap = new Bitmap(fileName);
            BitmapData TextureData =
                    textureBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );
            GL.GenTextures(1, out int m_texId);
            GL.BindTexture(TextureTarget.Texture2D, m_texId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureBitmap.Width, textureBitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, TextureData.Scan0);
            textureBitmap.UnlockBits(TextureData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return m_texId;
        }
    }
}
