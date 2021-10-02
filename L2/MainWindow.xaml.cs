using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace L2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl glc;
        private bool glcLoaded;
        private Shader _Shader;
        private int _VertexBufferObject;
        private int _VertexArrayObject;
        private readonly float[] vertices =
        {
             -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };
        private Stopwatch sw;

        public MainWindow()
        {
            InitializeComponent();
            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;
            glc.Paint += Glc_Paint;
            glc.Load += Glc_Load;
            glc.Resize += Glc_Resize;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            if (!glcLoaded)
                return;

            GL.Viewport(0, 0, glc.Width, glc.Height);
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.GenBuffers(1, out _VertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out _VertexArrayObject);
            GL.BindVertexArray(_VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _Shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            _Shader.Use();

            sw = new Stopwatch();
            sw.Start();

            glcLoaded = true;
        }

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!glcLoaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _Shader.Use();

            double timeValue = sw.Elapsed.TotalSeconds;

            float greenValue = (float)Math.Sin(timeValue) / (2.0f + 0.5f);
            _Shader.SetVector4("ourColor", new Vector4(0.0f, greenValue, 0.0f, 1.0f));

            GL.BindVertexArray(_VertexArrayObject);
            GL.DrawArrays(BeginMode.Triangles, 0, 3);


            GL.UseProgram(0);
            GL.Color3(System.Drawing.Color.Red);
            GL.LineWidth(2);
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(0.5, -0.5);
            GL.Vertex2(0, 0.5);
            GL.Vertex2(-0.5, 0);
            GL.End();

            glc.SwapBuffers();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();
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
