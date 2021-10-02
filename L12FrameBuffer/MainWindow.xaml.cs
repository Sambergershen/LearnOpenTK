using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;


namespace L12FrameBuffer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private GLControl glc;

        float[] cubeVertices =
        {
            // positions          // texture Coords
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };
        private int cubeVAO;
        private int cubeVBO;


        float[] planeVertices =
        {
            // positions          // texture Coords (note we set these higher than 1 (together with GL_REPEAT as texture wrapping mode). this will cause the floor texture to repeat)
             5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
            -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

             5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
             5.0f, -0.5f, -5.0f,  2.0f, 2.0f
        };
        private int planeVAO;
        private int planeVBO;

        float[] quadVertices =
        {   // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
            // positions   // texCoords
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };
        private int quadVAO;
        private int quadVBO;

        private int frameBuffer;
        private int textureColorBuffer;
        private int rbo;
        private Camera _camera;
        private Shader shader;
        private Shader screenShader;
        private Texture cubeTexture;
        private Texture floorTexture;

        public MainWindow()
        {
            InitializeComponent();

            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;

            glc.Load += Glc_Load;
            glc.Resize += Glc_Resize;
            glc.Paint += Glc_Paint;
            glc.MouseMove += Glc_MouseMove;
            glc.KeyDown += Glc_KeyDown;
            glc.MouseWheel += Glc_MouseWheel;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Glc_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _camera.Fov -= e.Delta * 0.01f;
            glc.Invalidate();
        }

        private void Glc_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            const float cameraSpeed = 1.5f;
            var delta = 0.1f;

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.W)
            {
                _camera.Position += _camera.Front * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.S)
            {
                _camera.Position -= _camera.Front * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.A)
            {
                _camera.Position -= _camera.Right * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.D)
            {
                _camera.Position += _camera.Right * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.Q)
            {
                _camera.Position -= _camera.Up * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.E)
            {
                _camera.Position += _camera.Up * cameraSpeed * delta;
            }

            glc.Invalidate();
        }

        private bool _firstMove = true;
        private Vector2 _lastPos;
        const float sensitivity = 0.2f;
        private void Glc_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(e.X, e.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = e.X - _lastPos.X;
                var deltaY = e.Y - _lastPos.Y;
                _lastPos = new Vector2(e.X, e.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            glc.Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();

        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // cube VAO
            GL.GenVertexArrays(1, out cubeVAO);
            GL.BindVertexArray(cubeVAO);
            GL.GenBuffers(1, out cubeVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertices.Length * sizeof(float)), cubeVertices, BufferUsageHint.StaticDraw);
            shader = new Shader("framebuffers.vert", "framebuffers.frag");
            shader.Use();
            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLoaction = shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLoaction);
            GL.VertexAttribPointer(texCoordLoaction, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);

            // plane VAO
            GL.GenVertexArrays(1, out planeVAO);
            GL.BindVertexArray(planeVAO);
            GL.GenBuffers(1, out planeVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, planeVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(planeVertices.Length * sizeof(float)), planeVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(texCoordLoaction);
            GL.VertexAttribPointer(texCoordLoaction, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);

            // screen quad VAO
            GL.GenVertexArrays(1, out quadVAO);
            GL.BindVertexArray(quadVAO);
            GL.GenBuffers(1, out quadVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(quadVertices.Length * sizeof(float)), quadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(texCoordLoaction);
            GL.VertexAttribPointer(texCoordLoaction, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.BindVertexArray(0);


            cubeTexture = Texture.LoadFromFile("marble.jpg");
            floorTexture = Texture.LoadFromFile("metal.png");
            shader.Use();
            shader.SetInt("texture1", 0);

            screenShader = new Shader("framebuffers_screen.vert", "framebuffers_screen.frag");
            screenShader.Use();
            screenShader.SetInt("screenTexture", 0);

            _camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);

            // framebuffer configuration
            GL.GenFramebuffers(1, out frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            // create a color attachment texture
            GL.GenTextures(1, out textureColorBuffer);
            GL.BindTexture(TextureTarget.Texture2D, textureColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 800, 600, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureColorBuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.GenRenderbuffers(1, out rbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, 800, 600);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer is not complete!");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit);

            shader.Use();
            var model = Matrix4.Identity;
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            // cubes
            GL.BindVertexArray(cubeVAO);
            cubeTexture.Use(TextureUnit.Texture0);
            model *= Matrix4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);
            model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);
            // floor
            GL.BindVertexArray(planeVAO);
            floorTexture.Use(TextureUnit.Texture0);
            //shader.SetInt("texture1", 1);
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 6);
            GL.BindVertexArray(0);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            screenShader.Use();
            GL.BindVertexArray(quadVAO);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, textureColorBuffer);
            GL.DrawArrays(BeginMode.Triangles, 0, 6);

            glc.SwapBuffers();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glc.Size);
            if (_camera != null)
            {
                _camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);
            }
        }


    }
}
