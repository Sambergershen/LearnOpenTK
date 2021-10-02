using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Camera
    {
        private Vector3 _Front = -Vector3.UnitZ;

        private Vector3 _Up = Vector3.UnitY;

        private Vector3 _Right = Vector3.UnitX;

        private float _Pitch;

        private float _Yaw = -MathHelper.PiOver2;

        private float _Fov = MathHelper.PiOver4;

        public Camera(Vector3 postion, float aspectRatio)
        {
            Position = postion;
            AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; set; }

        public float AspectRatio { private get; set; }

        public Vector3 Front
        {
            get
            {
                return _Front;
            }
        }

        public Vector3 Up
        {
            get
            {
                return _Up;
            }
        }

        public Vector3 Right
        {
            get
            {
                return _Right;
            }
        }

        public float Pitch
        {
            get
            {
                return MathHelper.RadiansToDegrees(_Pitch);
            }
            set
            {
                var angle = value;
                if(value < -89f)
                {
                    angle = -89f;
                }
                if(value > 89f)
                {
                    angle = 89f;
                }

                _Pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get
            {
                return MathHelper.RadiansToDegrees(_Yaw);
            }
            set
            {
                _Yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get
            {
                return MathHelper.RadiansToDegrees(_Fov);
            }
            set
            {
                var angle = value;
                if(angle < 1f)
                {
                    angle = 1f;
                }

                if(angle > 45f)
                {
                    angle = 45f;
                }

                _Fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _Front, _Up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_Fov, AspectRatio, 0.01f, 100f);
        }

        private void UpdateVectors()
        {
            _Front.X = (float)Math.Cos(_Pitch) * (float)Math.Cos(_Yaw);
            _Front.Y = (float)Math.Sin(_Pitch);
            _Front.Z = (float)Math.Cos(_Pitch) *(float)Math.Sin(_Yaw);

            _Front = Vector3.Normalize(_Front);

            _Right = Vector3.Normalize(Vector3.Cross(_Front, Vector3.UnitY));
            _Up = Vector3.Normalize(Vector3.Cross(_Right, _Front));
        }
    }
}
