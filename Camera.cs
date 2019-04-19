using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Camera
    {
        float mAngleX = 0;
        float mAngleY = 0;
        vec3 mDirection;
        vec3 mPosition;
        vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;
        public Camera()
        {
            Reset(0, 0, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 1000);
        }

        public vec3 GetLookDirection()
        {
            return mDirection;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            vec3 centerPos = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);

            mPosition = eyePos;
            mDirection = centerPos - mPosition;
            mRight = glm.cross(mDirection, upVec);
            mUp = upVec;
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);

            mViewMatrix = glm.lookAt(mPosition, centerPos, mUp);
        }

        public void UpdateViewMatrix()
        {
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , (float)(Math.Sin(mAngleY))
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            mUp = glm.cross(mRight, mDirection);

            vec3 center = mPosition + mDirection;

            mViewMatrix = glm.lookAt(mPosition, center, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }


        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Pitch(float angleDegrees)
        {
            mAngleY += angleDegrees;
        }

        public void Walk(float dist)
        {
            vec3 newPosition = mPosition + dist * mDirection;
            if (newPosition.x > 3 && newPosition.x < 997 &&
                newPosition.y > 3 && newPosition.y < 997 &&
                newPosition.z > 3 && newPosition.z < 997)
                    mPosition = newPosition;
        }
        public void Strafe(float dist)
        {
            vec3 newPosition = mPosition + dist * mRight;
            if (newPosition.x > 3 && newPosition.x < 997 &&
                newPosition.y > 3 && newPosition.y < 997 &&
                newPosition.z > 3 && newPosition.z < 997)
                mPosition = newPosition;
        }
        public void Fly(float dist)
        {
            vec3 newPosition = mPosition + dist * mUp;
            if (newPosition.x > 3 && newPosition.x < 997 &&
                newPosition.y > 3 && newPosition.y < 997 &&
                newPosition.z > 3 && newPosition.z < 997)
                mPosition = newPosition;
        }
    }
}
