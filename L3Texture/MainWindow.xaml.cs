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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace L3Texture
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly float[] _vertices =
         {
            // Position         Texture coordinates
             1f,  1f, 0.0f, 1.0f, 1.0f, // top right
             1f, -1f, 0.0f, 1.0f, 0.0f, // bottom right
            -1f, -1f, 0.0f, 0.0f, 0.0f, // bottom left
            -1f,  1f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly float[] _lines =
        {
            0,0,0,  //  0 1 2
            0,0,0,  //  3 4 5
            0,0,0,  //  6 7 8
            0,0,0,  //  9 10 11
            0,0,0,  //  12 13 14

        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _ElementBufferObject;
        private int _VertexBufferObject;
        private int _VertexArrayObject;

        private int _VertexArrayObject1;

        private int vext;
        private Shader _Shader;
        private Shader _Shader1;
        private Texture _Texture;

        private Texture _Texture1;

        private GLControl glc;
        private bool glLoaded = false;

        public MainWindow()
        {
            InitializeComponent();

            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;

            glc.Load += Glc_Load;
            glc.Resize += Glc_Resize;
            glc.Paint += Glc_Paint;
            glc.MouseDown += Glc_MouseDown;
            glc.MouseMove += Glc_MouseMove;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Glc_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                var x2 = (e.X - glc.Width / 2.0f) / glc.Width * 2;
                var y2 = -(e.Y - glc.Height / 2.0f) / glc.Height * 2;

                _lines[3] = x2;
                _lines[4] = y2;

                var arrowAngle = Math.PI / 12;
                var x1 = _lines[0];
                var y1 = _lines[1];

                var arrowLength = 0.05f;

                double angleOri = Math.Atan((y2 - y1) / (x2 - x1));      // 起始点线段夹角
                double angleDown = angleOri - arrowAngle;   // 箭头扩张角度
                double angleUp = angleOri + arrowAngle;     // 箭头扩张角度

                int directionFlag = (_lines[3] >= _lines[0]) ? -1 : 1;     // 方向标识
                var x3 = x2 + ((directionFlag * arrowLength) * Math.Cos(angleUp));   // 箭头第三个点的坐标
                var y3 = y2 + ((directionFlag * arrowLength) * Math.Sin(angleUp));
                var x4 = x2 + ((directionFlag * arrowLength) * Math.Cos(angleDown));     // 箭头第四个点的坐标
                var y4 = y2 + ((directionFlag * arrowLength) * Math.Sin(angleDown));

                _lines[6] = (float)x3;
                _lines[7] = (float)y3;
                _lines[9] = (float)x4;
                _lines[10] = (float)y4;

                _lines[12] = x2;
                _lines[13] = y2;
                GL.BindBuffer(BufferTarget.ArrayBuffer, vext);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_lines.Length * sizeof(float)), _lines, BufferUsageHint.StaticDraw);
                glc.Invalidate();
            }
        }

        private void Glc_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var x = (e.X - glc.Width / 2.0f) / glc.Width * 2;
            var y = -(e.Y - glc.Height / 2.0f) / glc.Height * 2;

            _lines[0] = x;
            _lines[1] = y;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vext);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_lines.Length * sizeof(float)), _lines, BufferUsageHint.StaticDraw);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();
        }

        int n = 0;
        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!glLoaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.BindVertexArray(_VertexArrayObject);

            n++;
            if (n % 2 == 0)
            {
                _Texture.Use(TextureUnit.Texture0);
            }
            else
            {
                _Texture1.Use(TextureUnit.Texture0);
            }

            //_Texture.Use(TextureUnit.Texture0);

            _Shader.Use();

            GL.DrawElements(BeginMode.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_VertexArrayObject1);
            _Shader1.Use();
            GL.LineWidth(2);
            GL.PointSize(50);
            _Shader1.SetVector3("aColor", new Vector3(1, 1, 0));
            GL.DrawArrays(BeginMode.LineStrip, 0, 4);
            //GL.UseProgram(0);
            //GL.Color3(System.Drawing.Color.Purple);
            //GL.LineWidth(1);
            //GL.Begin(BeginMode.LineLoop);
            //GL.Vertex2(0.5, -0.5);
            //GL.Vertex2(0, 0.5);
            //GL.Vertex2(-0.5, 0);
            //GL.End();

            glc.SwapBuffers();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glc.Size);
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(System.Drawing.Color.DarkCyan);

            #region 直线
            _Shader1 = new Shader("shaders/shader1.vert", "shaders/shader1.frag");

            GL.GenVertexArrays(1, out _VertexArrayObject1);
            GL.BindVertexArray(_VertexArrayObject1);
            GL.GenBuffers(1, out vext);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vext);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_lines.Length * sizeof(float)), _lines, BufferUsageHint.StaticDraw);
            var vertexLocation1 = _Shader1.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation1);
            GL.VertexAttribPointer(vertexLocation1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            #endregion

            GL.GenVertexArrays(1, out _VertexArrayObject);
            GL.BindVertexArray(_VertexArrayObject);

            GL.GenBuffers(1, out _VertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * sizeof(float)), _vertices, BufferUsageHint.StaticDraw);

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
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

           

            _Texture = Texture.LoadFromFile("Resources/container.png");
            //_Texture.Use(TextureUnit.Texture0);

            _Texture1 = Texture.LoadFromFile("Resources/crate.png");
            //_Texture1.Use(TextureUnit.Texture0);

            glLoaded = true;
        }

        private void SetupViewport()
        {
            var w = glc.Width;
            var h = glc.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
