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

namespace L6Light
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl glc;

        #region 背景
        // 背景相关
        private int _BGEBO;
        private int _BGVBO;
        private int _BGVAO;
        private Shader _BGShader;
        private Texture _BGTexture;
        private Matrix4 _BGModel;
        private Matrix4 _BGView;
        private Matrix4 _BGProjection;
        private readonly float[] _BGVertices =
        {
            // Position         Texture coordinates
             1f,  1f, -0.999f, 1.0f, 0.0f, // top right
             1f, -1f, -0.999f, 1.0f, 1.0f, // bottom right
            -1f, -1f, -0.999f, 0.0f, 1.0f, // bottom left
            -1f,  1f, -0.999f, 0.0f, 0.0f  // top left
        };
        private readonly uint[] _BGIndices =
        {
            0, 1, 3,
            1, 2, 3
        };
        #endregion

        private readonly float[] _LightVertices =
        {
            // Position
            -0.5f, -0.5f, -0.5f, // Front face
             0.5f, -0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f, -0.5f,  0.5f, // Back face
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,

            -0.5f,  0.5f,  0.5f, // Left face
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

             0.5f,  0.5f,  0.5f, // Right face
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,

            -0.5f, -0.5f, -0.5f, // Bottom face
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,

            -0.5f,  0.5f, -0.5f, // Top face
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f
        };
        private int _LightVAO;
        private int _LightVBO;
        private Shader _LightShader;
        private readonly Vector3 _LightPos = new Vector3(1.2f, 1.0f, 2.0f);

        private readonly float[] _ObjVertices =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Back face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Top face
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };
        private int _ObjVAO;
        private int _ObjVBO;
        private Shader _ObjShader;


        private Camera _Camera;

        public MainWindow()
        {
            InitializeComponent();

            glc = new GLControl();
            glc.Dock = System.Windows.Forms.DockStyle.Fill;
            host.Child = glc;

            glc.Paint += Glc_Paint;
            glc.Load += Glc_Load;
            glc.Resize += Glc_Resize;
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
            _Camera.Fov -= e.Delta * 0.01f;
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
                _Camera.Position += _Camera.Front * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.S)
            {
                _Camera.Position -= _Camera.Front * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.A)
            {
                _Camera.Position -= _Camera.Right * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.D)
            {
                _Camera.Position += _Camera.Right * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.Q)
            {
                _Camera.Position -= _Camera.Up * cameraSpeed * delta;
            }
            if (e.KeyCode == Keys.E)
            {
                _Camera.Position += _Camera.Up * cameraSpeed * delta;
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
                _Camera.Yaw += deltaX * sensitivity;
                _Camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            glc.Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            glc.Invalidate();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glc.Width, glc.Height);
            if (_Camera != null)
            {
                _Camera.AspectRatio = (glc.Width * 1.0f) / glc.Height;
            }
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);

            #region BG
            GL.GenVertexArrays(1, out _BGVAO);
            GL.BindVertexArray(_BGVAO);

            GL.GenBuffers(1, out _BGVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _BGVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_BGVertices.Length * sizeof(float)), _BGVertices, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out _BGEBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _BGEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_BGIndices.Length * sizeof(float)), _BGIndices, BufferUsageHint.StaticDraw);

            _BGShader = new Shader("shaders/bgshader.vert", "shaders/bgshader.frag");
            _BGShader.Use();

            var bgVertexLocation = _BGShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(bgVertexLocation);
            GL.VertexAttribPointer(bgVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var bgTexCoordLocation = _BGShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(bgTexCoordLocation);
            GL.VertexAttribPointer(bgTexCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _BGTexture = Texture.LoadFromFile("Resources/bg.jpg");
            _BGTexture.Use(TextureUnit.Texture0);
            _BGShader.SetInt("texture0", 0);
            _BGModel = Matrix4.Identity;
            _BGView = Matrix4.CreateTranslation(0.0f, 0.0f, -99.99f);
            _BGProjection = Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 0.1f, 100.99f);
            #endregion

            #region Obj
            GL.GenVertexArrays(1, out _ObjVAO);
            GL.BindVertexArray(_ObjVAO);

            GL.GenBuffers(1, out _ObjVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _ObjVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_ObjVertices.Length * sizeof(float)), _ObjVertices, BufferUsageHint.StaticDraw);

            _ObjShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            var vertexLocation = _ObjShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            var normalLocation = _ObjShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            #endregion


            #region Light
            GL.GenVertexArrays(1, out _LightVAO);
            GL.BindVertexArray(_LightVAO);

            GL.GenBuffers(1, out _LightVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _LightVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_LightVertices.Length * sizeof(float)), _LightVertices, BufferUsageHint.StaticDraw);
            _LightShader = new Shader("Shaders/light.vert", "Shaders/light.frag");
            var lightVertexLocation = _ObjShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(lightVertexLocation);
            GL.VertexAttribPointer(lightVertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            #endregion


            _Camera = new Camera(Vector3.UnitZ * 5, glc.Width / (float)glc.Height);
        }

        float rs = 0;

        private void Glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            rs += 0.1f;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // 背景
            GL.BindVertexArray(_BGVAO);
            _BGShader.Use();
            _BGShader.SetMatrix4("model", _BGModel);
            _BGShader.SetMatrix4("view", _BGView);
            _BGShader.SetMatrix4("projection", _BGProjection);
            GL.DrawElements(BeginMode.Triangles, _BGIndices.Length, DrawElementsType.UnsignedInt, 0);

            // 物体
            GL.BindVertexArray(_ObjVAO);
            _ObjShader.Use();
            _ObjShader.SetMatrix4("model", Matrix4.Identity);
            _ObjShader.SetMatrix4("view", _Camera.GetViewMatrix());
            _ObjShader.SetMatrix4("projection", _Camera.GetProjectionMatrix());
            _ObjShader.SetVector3("viewPos", _Camera.Position);

            _ObjShader.SetVector3("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
            _ObjShader.SetVector3("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
            _ObjShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _ObjShader.SetFloat("material.shininess", 32f);

            Vector3 lightColor = new Vector3();
            float time = DateTime.Now.Second + DateTime.Now.Millisecond / 1000f;
            lightColor.X = (float)(Math.Sin(time * 2) + 1) / 2f;
            lightColor.Y = (float)(Math.Sin(time * 0.7) + 1) / 2f;
            lightColor.Z = (float)(Math.Sin(time * 1.3) + 1) / 2f;

            Vector3 ambientColor = lightColor * 0.2f;
            Vector3 diffuseColor = lightColor * 0.5f;

            _ObjShader.SetVector3("light.position", _LightPos);
            _ObjShader.SetVector3("light.ambient", ambientColor);
            _ObjShader.SetVector3("light.diffuse", diffuseColor);
            _ObjShader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));


            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            // 光源
            GL.BindVertexArray(_LightVAO);
            _LightShader.Use();
            Matrix4 lightMatrix = Matrix4.Scale(0.2f);
            lightMatrix *= Matrix4.CreateTranslation(_LightPos);
            _LightShader.SetMatrix4("model", lightMatrix);
            _LightShader.SetMatrix4("view", _Camera.GetViewMatrix());
            _LightShader.SetMatrix4("projection", _Camera.GetProjectionMatrix());

            _LightShader.SetVector3("lightColor", lightColor);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            glc.SwapBuffers();
        }
    }
}
