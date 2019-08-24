using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Threading;

namespace Graphics {
    class LoadScreen :ScreenClass {
        #region Shaders Declaration
        Shader sh;
        #endregion

        #region Matrices declaration
        int transID;
        #endregion

        #region Models Declaration
        Model background;
        #endregion

        int texUnit = 0;

        public override void Initialize() {
            #region Shaders Initialization
            sh = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            #endregion

            transID = Gl.glGetUniformLocation(sh.ID, "model");

            Gl.glClearColor(1, 0, 0, 1);

            #region Models Initialization
            #region Background
            background = new Model();

            background.vertices.Add(new vec3(-1, -1, 0));
            background.vertices.Add(new vec3(1, 1, 0));
            background.vertices.Add(new vec3(1, -1, 0));
            background.vertices.Add(new vec3(-1, -1, 0));
            background.vertices.Add(new vec3(1, 1, 0));
            background.vertices.Add(new vec3(-1, 1, 0));

            background.uvCoordinates.Add(new vec2(0, 0));
            background.uvCoordinates.Add(new vec2(1, 1));
            background.uvCoordinates.Add(new vec2(1, 0));
            background.uvCoordinates.Add(new vec2(0, 0));
            background.uvCoordinates.Add(new vec2(1, 1));
            background.uvCoordinates.Add(new vec2(0, 1));

            background.texture = new Texture(projectPath + "\\Textures\\loading.jpg", (texUnit++) % 32, true);

            background.Initialize();
            #endregion
            #endregion
        }

        public override void Draw() {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            sh.UseShader();

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            background.Draw(transID);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }

        public override void CleanUp() {
            sh.DestroyShader();
        }
    }
}
