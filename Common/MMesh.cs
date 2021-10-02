using Assimp;
using Assimp.Configs;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public class MVertex
    {
        public Vector3 Postion;
        public Vector3 Normal;
        public Vector2 TexCoords;
        public Vector3 Tangent;
        public Vector3 Bitangent;
    }

    public class MTexture
    {
        public int id;
        public string type;
        public string path;
    }


    public class MMesh
    {
        public float[] Vertices { get; set; }
        public MTexture[] Textures { get; set; }
        public int[] Indices { get; set; }

        public int VAO;

        private int VBO, EBO;

        private int[] offset =
        {
            3*sizeof(float),// 0 vertex
            6*sizeof(float),// 1 vertex + normals
            8*sizeof(float),// 2 vertex + normals + coord
            11*sizeof(float),// 3 vertex + normals + coord + tangent
            14*sizeof(float),// 4 vertex + normals + coord + tangent + bitangent
        };

        public MMesh(float[] vertices, int[] indices, MTexture[] textures)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            this.Textures = textures;

            SetupMesh();
        }

        public void Draw(Shader shader)
        {
            shader.Use();
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;
            for (int i = 0; i < Textures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                string number = string.Empty;
                string name = Textures[i].type;
                if (name == "texture_diffuse")
                    number = (diffuseNr++).ToString();
                else if (name == "texture_specular")
                    number = (specularNr++).ToString(); // transfer unsigned int to stream
                else if (name == "texture_normal")
                    number = (normalNr++).ToString(); // transfer unsigned int to stream
                else if (name == "texture_height")
                    number = (heightNr++).ToString(); // transfer unsigned int to stream

                // now set the sampler to the correct texture unit
                var location = GL.GetUniformLocation(shader.Handle, name + number);
                GL.Uniform1(location, i);
                // and finally bind the texture
                GL.BindTexture(TextureTarget.Texture2D, Textures[i].id);
            }

            // draw mesh
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            // always good practice to set everything back to defaults once configured.
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        private void SetupMesh()
        {
            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * sizeof(float)), Vertices, BufferUsageHint.StaticDraw);

            // set the vertex attribute pointers
            // vertex Positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);

            // vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 3);

            // vertex texture coords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);
           

            GL.GenBuffers(1, out EBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(float)), Indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }
    }
}
