using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using Tao.OpenGl;
using System.IO;

namespace Graphics
{
    class Skybox
    {
        Model up, down, right, left, back, front;
        Texture texUp, texDown, texRight, texLeft, texBack, texFront;
        public Skybox() {
            #region Skybox Textures
            texUp = new Texture(Renderer.projectPath + "\\Textures\\rainforest_up.jpg", 0);
            texDown = new Texture(Renderer.projectPath + "\\Textures\\rainforest_dn.jpg", 0);
            texRight = new Texture(Renderer.projectPath + "\\Textures\\rainforest_rt.jpg", 0);
            texLeft = new Texture(Renderer.projectPath + "\\Textures\\rainforest_lf.jpg", 0);
            texBack = new Texture(Renderer.projectPath + "\\Textures\\rainforest_bk.jpg", 0);
            texFront = new Texture(Renderer.projectPath + "\\Textures\\rainforest_ft.jpg", 0);
            #endregion

            #region Skybox Faces
            #region Down Face
            down = new Model();
            vec3 v1 = new vec3(0, 0, 0);
            vec3 v2 = new vec3(1000, 0, 1000);
            vec3 v3 = new vec3(1000, 0, 0);
            vec3 v4 = new vec3(0, 0, 0);
            vec3 v5 = new vec3(1000, 0, 1000);
            vec3 v6 = new vec3(0, 0, 1000);
            down.vertices.Add(v1);
            down.vertices.Add(v2);
            down.vertices.Add(v3);
            down.vertices.Add(v4);
            down.vertices.Add(v5);
            down.vertices.Add(v6);

            down.texture = texDown;
            down.uvCoordinates.Add(new vec2(1, 1));
            down.uvCoordinates.Add(new vec2(0, 0));
            down.uvCoordinates.Add(new vec2(0, 1));
            down.uvCoordinates.Add(new vec2(1, 1));
            down.uvCoordinates.Add(new vec2(0, 0));
            down.uvCoordinates.Add(new vec2(1, 0));

            down.Initialize();
            #endregion

            #region Up face
            up = new Model();
            up.vertices.Add(v1);
            up.vertices.Add(v2);
            up.vertices.Add(v3);
            up.vertices.Add(v4);
            up.vertices.Add(v5);
            up.vertices.Add(v6);

            up.texture = texUp;
            up.uvCoordinates.Add(new vec2(1, 1));
            up.uvCoordinates.Add(new vec2(0, 0));
            up.uvCoordinates.Add(new vec2(1, 0));
            up.uvCoordinates.Add(new vec2(1, 1));
            up.uvCoordinates.Add(new vec2(0, 0));
            up.uvCoordinates.Add(new vec2(0, 1));

            up.transformationMatrix = glm.translate(new mat4(1), new vec3(0, 1000, 0));

            up.Initialize();
            #endregion

            #region Right face
            right = new Model();
            right.vertices.Add(v1);
            right.vertices.Add(v2);
            right.vertices.Add(v3);
            right.vertices.Add(v4);
            right.vertices.Add(v5);
            right.vertices.Add(v6);

            right.texture = texRight;
            right.uvCoordinates.Add(new vec2(1, 1));
            right.uvCoordinates.Add(new vec2(0, 0));
            right.uvCoordinates.Add(new vec2(1, 0));
            right.uvCoordinates.Add(new vec2(1, 1));
            right.uvCoordinates.Add(new vec2(0, 0));
            right.uvCoordinates.Add(new vec2(0, 1));

            right.transformationMatrix = glm.rotate(90.0f / 180.0f * 3.14f, new vec3(0, 0, 1));

            right.Initialize();
            #endregion

            #region Left face
            left = new Model();
            left.vertices.Add(v1);
            left.vertices.Add(v2);
            left.vertices.Add(v3);
            left.vertices.Add(v4);
            left.vertices.Add(v5);
            left.vertices.Add(v6);

            left.texture = texLeft;
            left.uvCoordinates.Add(new vec2(0, 1));
            left.uvCoordinates.Add(new vec2(1, 0));
            left.uvCoordinates.Add(new vec2(0, 0));
            left.uvCoordinates.Add(new vec2(0, 1));
            left.uvCoordinates.Add(new vec2(1, 0));
            left.uvCoordinates.Add(new vec2(1, 1));

            left.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>{
                glm.rotate(90.0f / 180.0f * 3.14f, new vec3(0, 0, 1)),
                glm.translate(new mat4(1), new vec3(1000, 0, 0)) }
            );

            left.Initialize();
            #endregion

            #region Back face
            back = new Model();
            v1 = new vec3(0, 0, 1000);
            v2 = new vec3(1000, 1000, 1000);
            v3 = new vec3(1000, 0, 1000);
            v4 = new vec3(0, 0, 1000);
            v5 = new vec3(1000, 1000, 1000);
            v6 = new vec3(0, 1000, 1000);
            back.vertices.Add(v1);
            back.vertices.Add(v2);
            back.vertices.Add(v3);
            back.vertices.Add(v4);
            back.vertices.Add(v5);
            back.vertices.Add(v6);

            back.texture = texBack;
            back.uvCoordinates.Add(new vec2(1, 1));
            back.uvCoordinates.Add(new vec2(0, 0));
            back.uvCoordinates.Add(new vec2(0, 1));
            back.uvCoordinates.Add(new vec2(1, 1));
            back.uvCoordinates.Add(new vec2(0, 0));
            back.uvCoordinates.Add(new vec2(1, 0));

            back.Initialize();
            #endregion

            #region Front face
            front = new Model();
            front.vertices.Add(v1);
            front.vertices.Add(v2);
            front.vertices.Add(v3);
            front.vertices.Add(v4);
            front.vertices.Add(v5);
            front.vertices.Add(v6);
            
            front.texture = texFront;
            front.uvCoordinates.Add(new vec2(0, 1));
            front.uvCoordinates.Add(new vec2(1, 0));
            front.uvCoordinates.Add(new vec2(1, 1));
            front.uvCoordinates.Add(new vec2(0, 1));
            front.uvCoordinates.Add(new vec2(1, 0));
            front.uvCoordinates.Add(new vec2(0, 0));

            front.transformationMatrix = glm.translate(new mat4(1), new vec3(0, 0, -1000.0f));

            front.Initialize();
            #endregion
            #endregion
        }

        public void Draw(int matID)
        {
            down.Draw(matID);
            back.Draw(matID);
            up.Draw(matID);
            right.Draw(matID);
            left.Draw(matID);
            front.Draw(matID);
        }
    }

}
