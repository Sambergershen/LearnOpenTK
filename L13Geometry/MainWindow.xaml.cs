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


namespace L13Geometry
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private GLControl glc;

        float[] points =
        {
            -0.5f,  0.5f, 1.0f, 0.0f, 0.0f, // top-left
             0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // top-right
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, // bottom-right
            -0.5f, -0.5f, 1.0f, 1.0f, 0.0f  // bottom-left
        };
        private int VAO;
        private int VBO;

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
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(points.Length * sizeof(float)), points, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag", "shader.geom");
            shader.Use();
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);
        }

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit);
            shader.Use();

            GL.BindVertexArray(VAO);
            GL.DrawArrays(BeginMode.Triangles, 0, 4);

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
