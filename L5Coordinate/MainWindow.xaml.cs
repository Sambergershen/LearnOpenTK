using Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace L5Coordinate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private GLControl glc;

        // 背景相关
        private int _BgEBO;
        private int _BgVBO;
        private int _BgVAO;
        private Shader _BgShader;
        private Texture _BgTexture;
        private Matrix4 _BgModel;
        private Matrix4 _BgView;
        private Matrix4 _BgProjection;
        private readonly float[] _BgVertices =
        {
            // Position         Texture coordinates
             1f,  1f, -0.999f, 1.0f, 0.0f, // top right
             1f, -1f, -0.999f, 1.0f, 1.0f, // bottom right
            -1f, -1f, -0.999f, 0.0f, 1.0f, // bottom left
            -1f,  1f, -0.999f, 0.0f, 0.0f  // top left
        };
        private readonly uint[] _BgIndices =
        {
            0, 1, 3,
            1, 2, 3
        };


        private int _VAO;
        private int _VBO;
        private Shader _Shader;
        private Texture _Texture;
        private Matrix4 _View;
        private Matrix4 _Projection;
        private Camera _camera;
        float[] _Vertices = 
        {
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

        float degree;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        const float sensitivity = 0.2f;

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
            _camera.Fov -= e.Delta*0.01f;
            glc.Invalidate();
        }

        private void Glc_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            const float cameraSpeed = 1.5f;
            var delta = 0.1f;

            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.W)
            {
                _camera.Position += _camera.Front * cameraSpeed*delta;
            }
            if(e.KeyCode == Keys.S)
            {
                _camera.Position -= _camera.Front * cameraSpeed * delta;
            }
            if(e.KeyCode == Keys.A)
            {
                _camera.Position -= _camera.Right * cameraSpeed * delta;
            }
            if(e.KeyCode == Keys.D)
            {
                _camera.Position += _camera.Right * cameraSpeed * delta;
            }
            if(e.KeyCode == Keys.Q)
            {
                _camera.Position -= _camera.Up * cameraSpeed * delta;
            }
            if(e.KeyCode == Keys.E)
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
            degree += 1;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);



            GL.BindVertexArray(_BgVAO);
            _BgTexture.Use(TextureUnit.Texture0);
            _BgShader.Use();
            _BgShader.SetMatrix4("model", _BgModel);
            _BgShader.SetMatrix4("view", _BgView);
            _BgShader.SetMatrix4("projection", _BgProjection);
            GL.DrawElements(BeginMode.Triangles, _BgIndices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_VAO);
            _Texture.Use(TextureUnit.Texture0);
            _Shader.Use();
            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(degree));

            _Shader.SetMatrix4("model", model);
            _Shader.SetMatrix4("view", _camera.GetViewMatrix());
            _Shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawArrays(BeginMode.Triangles, 0, 36);

            glc.SwapBuffers();
        }

        private void Glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glc.Size);
        }

        private void Glc_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);


            GL.GenVertexArrays(1, out _BgVAO);
            GL.BindVertexArray(_BgVAO);

            GL.GenBuffers(1, out _BgVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _BgVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_BgVertices.Length * sizeof(float)), _BgVertices, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out _BgEBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _BgEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_BgIndices.Length * sizeof(uint)), _BgIndices, BufferUsageHint.StaticDraw);

            _BgShader = new Shader("shaders/bgshader.vert", "shaders/bgshader.frag");
            _BgShader.Use();

            var bgVertexLocation = _BgShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(bgVertexLocation);
            GL.VertexAttribPointer(bgVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            var bgTexCoordLoaction = _BgShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(bgTexCoordLoaction);
            GL.VertexAttribPointer(bgTexCoordLoaction, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _BgTexture = Texture.LoadFromFile("Resources/bg.jpg");
            _BgTexture.Use(TextureUnit.Texture0);
            _BgShader.SetInt("texture0", 0);
            _BgModel = Matrix4.Identity;
            _BgView = Matrix4.CreateTranslation(0.0f, 0.0f, -99.99f);
            _BgProjection = Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 0.1f, 100.99f);



            GL.GenVertexArrays(1, out _VAO);
            GL.BindVertexArray(_VAO);

            GL.GenBuffers(1, out _VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_Vertices.Length * sizeof(float)), _Vertices, BufferUsageHint.StaticDraw);
            _Shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            _Shader.Use();
            var vertexLocation = _Shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _Shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _Texture = Texture.LoadFromFile("Resources/container.png");
            _Texture.Use(TextureUnit.Texture0);
            _Shader.SetInt("texture0", 0);

            _View = Matrix4.CreateTranslation(0.0f, 0.0f, -10f);
            _Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (glc.Width*1.0f) / glc.Height, 0.1f, 100.0f);

            _camera = new Camera(Vector3.UnitZ * 3, glc.Width / (float)glc.Height);

            this.Focus();
        }
    }
}
