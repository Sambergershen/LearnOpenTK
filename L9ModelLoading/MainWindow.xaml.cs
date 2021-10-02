using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace L9ModelLoading
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private GLControl glc;
        
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        const float sensitivity = 0.2f;

        Shader ourShader;
        Model ourModel;

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

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ourShader.Use();
            ourShader.SetMatrix4("model", Matrix4.Identity*Matrix4.Scale(0.1f));
            ourShader.SetMatrix4("view", _camera.GetViewMatrix());
            ourShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            ourModel.Draw(ourShader);

            glc.SwapBuffers();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glc.Size);
            if(_camera !=null)
            {
                _camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);
            }
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);

            ourShader = new Shader("model.vert", "model.frag");
            ourModel = new Model("backpack/backpack.obj");
            //ourModel = new Model("objects/nanosuit.obj");
        }
    }
}
