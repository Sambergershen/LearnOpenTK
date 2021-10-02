using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace L11Blending
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

        float[] transparentVertices = 
        {
            // positions         // texture Coords (swapped y coordinates because texture is flipped upside down)
            0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
            0.0f, -0.5f,  0.0f,  0.0f,  1.0f,
            1.0f, -0.5f,  0.0f,  1.0f,  1.0f,

            0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
            1.0f, -0.5f,  0.0f,  1.0f,  1.0f,
            1.0f,  0.5f,  0.0f,  1.0f,  0.0f
        };
        private int transparentVAO;
        private int transparentVBO;
        private List<Vector3> vegetation = new List<Vector3>()
        {
            new Vector3(-1.5f, 0.0f, -0.4f),
            new Vector3( 1.5f, 0.0f, 0.55f),
            new Vector3( 0.0f, 0.0f, 0.7f),
            new Vector3(-0.3f, 0.0f, -2.3f),
            new Vector3(0.5f, 0.0f, -0.6f)
        };

        private Camera _camera;
        private Shader shader;
        private Texture cubeTexture;
        private Texture floorTexture;
        private Texture grassTexture;

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

        float degree;
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
            //GL.DepthFunc(DepthFunction.Always);

            GL.GenVertexArrays(1, out cubeVAO);
            GL.BindVertexArray(cubeVAO);

            GL.GenBuffers(1, out cubeVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertices.Length * sizeof(float)), cubeVertices, BufferUsageHint.StaticDraw);
            shader = new Shader("blending.vert", "blending.frag");
            shader.Use();
            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var texCoordLoaction = shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLoaction);
            GL.VertexAttribPointer(texCoordLoaction, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);

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

            GL.GenVertexArrays(1, out transparentVAO);
            GL.BindVertexArray(transparentVAO);
            GL.GenBuffers(1, out transparentVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, transparentVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(transparentVertices.Length * sizeof(float)), transparentVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(texCoordLoaction);
            GL.VertexAttribPointer(texCoordLoaction, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);


            cubeTexture = Texture.LoadFromFile("marble.jpg", TextureUnit.Texture0);
            cubeTexture.Use(TextureUnit.Texture0);

            floorTexture = Texture.LoadFromFile("metal.png", TextureUnit.Texture1);
            floorTexture.Use(TextureUnit.Texture1);

            grassTexture = Texture.LoadFromFile("grass.png", TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge, TextureUnit.Texture2);
            grassTexture.Use(TextureUnit.Texture2);

            _camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);
        }

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit
                | ClearBufferMask.StencilBufferBit);
            shader.Use();
            var model = Matrix4.Identity;
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.BindVertexArray(planeVAO);
            floorTexture.Use(TextureUnit.Texture1);
            shader.SetInt("texture1", 1);
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 6);

            GL.BindVertexArray(cubeVAO);
            cubeTexture.Use(TextureUnit.Texture0);
            shader.SetInt("texture1", 0);
            model *= Matrix4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
            shader.SetMatrix4("model", model);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            GL.BindVertexArray(transparentVAO);
            grassTexture.Use(TextureUnit.Texture2);
            shader.SetInt("texture1", 2);
            for (int i = 0; i < vegetation.Count; i++)
            {
                model = Matrix4.Identity;
                model *= Matrix4.CreateTranslation(vegetation[i]);
                shader.SetMatrix4("model", model);
                GL.DrawArrays(BeginMode.Triangles, 0, 6);
            }

            GL.BindVertexArray(0);

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
