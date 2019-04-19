using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;

namespace Graphics
{
    class Renderer
    {
        #region Shaders decleration
        Shader sh;
        #endregion

        #region Matricies decleration
        int transID, viewID, projID;
        mat4 ProjectionMatrix, ViewMatrix, modelmatrix;
        #endregion

        int EyePositionID,  AmbientLightID, DataID;

        public Camera cam;
        
        public float Speed = 1;

        #region 3D Models decleration
        #region Bedroom
        md2 bed;
        #endregion

        #region Bathroom
        md2 sink;
        #endregion

        #region Kitchen
        #endregion

        #region Living Room
        #endregion

        #region Bedroom2
        #endregion
        #endregion

        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            #region Shaders intialization
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            #endregion


            Gl.glClearColor(0, 0, 0.4f, 1);
            
            cam = new Camera();
            cam.Reset(0, 34, 100, 0, 0, 0, 0, 1, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "trans");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            modelmatrix = glm.scale(new mat4(1), new vec3(50, 50, 50));

            sh.UseShader();

            #region 3D Models intialization
            #region Bedroom
            bed = new md2(projectPath + "\\ModelFiles\\BED.md2");
            bed.StartAnimation(animType.STAND);
            bed.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            bed.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            #endregion

            #region Bathroom
            sink = new md2(projectPath + "\\ModelFiles\\SINK.md2");
            sink.StartAnimation(animType.STAND);
            sink.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            sink.TranslationMatrix = glm.translate(new mat4(1), new vec3(40, -1, 10));

            #endregion

            #region Kitchen
            #endregion

            #region Living Room
            #endregion

            #region Bedroom2
            #endregion
            #endregion

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            sh.UseShader();

            #region 3D Models drawing
            #region Bedroom
            bed.Draw(transID);
            #endregion

            #region Bathroom
            sink.Draw(transID);
            #endregion

            #region Kitchen
            #endregion

            #region Living Room
            #endregion

            #region Bedroom2
            #endregion
            #endregion
        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
        }
        public void SendLightData(float red, float green, float blue, float attenuation, float specularExponent)
        {
            vec3 ambientLight = new vec3(red, green, blue);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            vec2 data = new vec2(attenuation, specularExponent);
            Gl.glUniform2fv(DataID, 1, data.to_array());
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
