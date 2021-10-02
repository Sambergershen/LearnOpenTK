using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace L4MultipleTextures
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl glc;
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 1.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 0.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _ElementBufferObject;
        private int _VertexBufferObject;
        private int _VertexArrayObject;
        private Shader _Shader;
        private Texture _Texture;
        private Texture _Texture1;

        public MainWindow()
        {
            InitializeComponent();

            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;
            glc.Resize += Glc_Resize;
            glc.Load += Glc_Load;
            glc.Paint += Glc_Paint;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        int i = 0;
        byte[] buffer = new byte[512 * 512 * 4];
        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindVertexArray(_VertexArrayObject);

            var transform = Matrix4.Identity;
            transform *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(45));
            transform *= Matrix4.Scale(0.5f);
            transform *= Matrix4.CreateTranslation(0.1f, 0.1f, 0.0f);

            
            _Texture1.Use(TextureUnit.Texture1);

            _Texture.Use(TextureUnit.Texture0);
            i++;
            if(i%2==0)
            {
                _Texture.Upadate("Resources/awesomeface.png");
            }
            else
            {
                using (var image = new Bitmap("Resources/container.png"))
                {
                    var data = image.LockBits(
                        new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    byte[] buffer = new byte[image.Width * image.Height * 4];
                    Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                    _Texture.Upadate(buffer, image.Width, image.Height);

                    image.UnlockBits(data);
                }

            }

            _Shader.Use();

            _Shader.SetMatrix4("transform", transform);

            GL.DrawElements(BeginMode.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            glc.SwapBuffers();
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.GenVertexArrays(1, out _VertexArrayObject);
            GL.BindVertexArray(_VertexArrayObject);

            GL.GenBuffers(1, out _VertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * sizeof(float)), _vertices, BufferUsageHint.StreamDraw);

            GL.GenBuffers(1, out _ElementBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indices.Length * sizeof(float)), _indices, BufferUsageHint.StaticDraw);

            _Shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            _Shader.Use();

            var vertexLocation = _Shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLocation = _Shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3*sizeof(float));

            _Texture = Texture.LoadFromFile("Resources/container.png");
            _Texture.Use(TextureUnit.Texture0);
            _Texture1 = Texture.LoadFromFile("Resources/awesomeface.png");
            _Texture1.Use(TextureUnit.Texture1);

            //_Shader.SetInt("texture0", 0);
            //_Shader.SetInt("texture1", 1);
            _Shader.SetInt("textureR", 0);
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glc.Size);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();
        }
    }
}
