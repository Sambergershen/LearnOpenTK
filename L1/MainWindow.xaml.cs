using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace L1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl glc;
        private bool glcLoaded = false;
        private readonly float[] vertices =
        {
             // positions        // colors
             0.5f, 0.5f, 0.0f,  1.0f, 0.0f, 0.0f,
             0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,
             -0.5f,  -0.5f, 0.0f,  0.0f, 0.0f, 1.0f,
             -0.5f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f
        };

        uint[] indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };


        int _VertexBufferObject;
        int _VertexArrayObject;
        Shader _Shader;

        int _ElementBufferObject;

        public MainWindow()
        {
            InitializeComponent();
            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;

            glc.Load += Glc_Load;
            glc.Resize += Glc_Resize;
            glc.Paint += Glc_Paint;
            glc.KeyDown += Glc_KeyDown;
            glc.MouseDown += Glc_MouseDown;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Glc_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var x = (e.X - glc.Width / 2.0) / glc.Width * 2;
            var y = -(e.Y - glc.Height / 2.0) / glc.Height * 2;

            vertices[18] = (float)x;
            vertices[19] = (float)y;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.DynamicDraw);

            //GL.BindVertexArray(_VertexArrayObject);

            //var aPosition = _Shader.GetAttribLocation("aPosition");
            //GL.VertexAttribPointer(aPosition, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(aPosition);

            //var aColor = _Shader.GetAttribLocation("aColor");
            //GL.VertexAttribPointer(aColor, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            //GL.EnableVertexAttribArray(aColor);

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            if (!glcLoaded)
            {
                return;
            }

            GL.Viewport(0, 0, glc.Width, glc.Height);
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            _Shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            _Shader.Use();

            GL.GenBuffers(1, out _VertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.DynamicDraw);

            GL.GenVertexArrays(1, out _VertexArrayObject);
            GL.BindVertexArray(_VertexArrayObject);

            var aPosition = _Shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(aPosition, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(aPosition);

            var aColor = _Shader.GetAttribLocation("aColor");
            GL.VertexAttribPointer(aColor, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(aColor);

            GL.GenBuffers(1, out _ElementBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            glcLoaded = true;
        }

        private void Glc_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                this.Close();
            }
        }

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _Shader.Use();
            GL.BindVertexArray(_VertexArrayObject);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            glc.SwapBuffers();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffers(1, ref _VertexBufferObject);
            GL.DeleteVertexArrays(1, ref _VertexArrayObject);
            GL.DeleteProgram(_Shader.Handle);
        }
    }
}
