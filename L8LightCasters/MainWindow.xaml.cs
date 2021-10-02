using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace L8LightCasters
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
             1f,  1f, -0.999f, 1.0f, 1.0f, // top right
             1f, -1f, -0.999f, 1.0f, 0.0f, // bottom right
            -1f, -1f, -0.999f, 0.0f, 0.0f, // bottom left
            -1f,  1f, -0.999f, 0.0f, 1.0f  // top left
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
        private readonly Vector3[] _LightPositions =
     {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };
        private int _LightVAO;
        private int _LightVBO;
        private Shader _LightShader;
        private readonly Vector3 _LightPos = new Vector3(1.2f, 1.0f, 2.0f);

        private readonly float[] _ObjVertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };
        private readonly Vector3[] _ObjPositions =
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3(2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f, 3.0f, -7.5f),
            new Vector3(1.3f, -2.0f, -2.5f),
            new Vector3(1.5f, 2.0f, -2.5f),
            new Vector3(1.5f, 0.2f, -1.5f),
            new Vector3(-1.3f, 1.0f, -1.5f)
        };
        private int _ObjVAO;
        private int _ObjVBO;
        private Shader _ObjShader;
        private Texture _DiffuseMap;
        private Texture _SpecularMap;

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

            _BGTexture = Texture.LoadFromFile("Resources/Vpzmue9.jpeg");
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
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = _ObjShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = _ObjShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

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

            _DiffuseMap = Texture.LoadFromFile("Resources/container2.png", TextureUnit.Texture1);
            _SpecularMap = Texture.LoadFromFile("Resources/Vpzmue9.jpeg", TextureUnit.Texture2);
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
            //_BGTexture.Use(TextureUnit.Texture0);
            _BGShader.SetMatrix4("model", _BGModel);
            _BGShader.SetMatrix4("view", _BGView);
            _BGShader.SetMatrix4("projection", _BGProjection);
            GL.DrawElements(BeginMode.Triangles, _BGIndices.Length, DrawElementsType.UnsignedInt, 0);

            // 物体
            GL.BindVertexArray(_ObjVAO);
            _ObjShader.Use();
            _DiffuseMap.Use(TextureUnit.Texture1);
            _SpecularMap.Use(TextureUnit.Texture2);

            _ObjShader.SetMatrix4("view", _Camera.GetViewMatrix());
            _ObjShader.SetMatrix4("projection", _Camera.GetProjectionMatrix());

            _ObjShader.SetVector3("viewPos", _Camera.Position);

            _ObjShader.SetInt("material.diffuse", 1);
            _ObjShader.SetInt("material.specular", 2);
            _ObjShader.SetFloat("material.shininess", 32f);

            _ObjShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _ObjShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _ObjShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            _ObjShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            for (int i = 0; i < _LightPositions.Length; i++)
            {
                _ObjShader.SetVector3($"pointLights[{i}].position", _LightPositions[i]);
                _ObjShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _ObjShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _ObjShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _ObjShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                _ObjShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                _ObjShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            _ObjShader.SetVector3("spotLight.position", _Camera.Position);
            _ObjShader.SetVector3("spotLight.direction", _Camera.Front);
            _ObjShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            _ObjShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            _ObjShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            _ObjShader.SetFloat("spotLight.constant", 1.0f);
            _ObjShader.SetFloat("spotLight.linear", 0.09f);
            _ObjShader.SetFloat("spotLight.quadratic", 0.032f);
            _ObjShader.SetFloat("spotLight.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));
            _ObjShader.SetFloat("spotLight.outterCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(17.5f)));

            for (int i = 0; i < _ObjPositions.Length; i++)
            {
                Matrix4 model = Matrix4.CreateTranslation(_ObjPositions[i]);
                float angle = 20.0f * i;
                model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                _ObjShader.SetMatrix4("model", model);
                GL.DrawArrays(BeginMode.Triangles, 0, 36);
            }



            // 光源
            GL.BindVertexArray(_LightVAO);
            _LightShader.Use();
            _LightShader.SetMatrix4("view", _Camera.GetViewMatrix());
            _LightShader.SetMatrix4("projection", _Camera.GetProjectionMatrix());

            for (int i = 0; i < _LightPositions.Length; i++)
            {
                Matrix4 lampMatrix = Matrix4.Scale(0.2f);
                lampMatrix = lampMatrix * Matrix4.CreateTranslation(_LightPositions[i]);

                _LightShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(BeginMode.Triangles, 0, 36);
            }

            glc.SwapBuffers();
        }
    }
}
