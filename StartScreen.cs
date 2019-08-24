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
    class StartScreen : ScreenClass {

        #region Shaders Declaration
        Shader sh;
        #endregion

        #region Matrices declaration
        int transID;
        #endregion

        #region Models Declaration
        Model background, startButton;
        #endregion

        int texUnit = 0;

        #region Start button viewport boundaries
        float btn_min_x = 473, btn_max_x = 1062, btn_min_y = 570, btn_max_y = 640;
        #endregion

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

            background.texture = new Texture(projectPath + "\\Textures\\grimmnight_bk.jpg", texUnit++, true);

            background.Initialize();
            #endregion
            #region Start Button
            startButton = new Model();

            startButton.vertices.Add(new vec3(-.4f, -.5f, 0));
            startButton.vertices.Add(new vec3(.4f, -.3f, 0));
            startButton.vertices.Add(new vec3(.4f, -.5f, 0));
            startButton.vertices.Add(new vec3(-.4f, -.5f, 0));
            startButton.vertices.Add(new vec3(.4f, -.3f, 0));
            startButton.vertices.Add(new vec3(-.4f, -.3f, 0));

            startButton.uvCoordinates.Add(new vec2(0, 0));
            startButton.uvCoordinates.Add(new vec2(1, 1));
            startButton.uvCoordinates.Add(new vec2(1, 0));
            startButton.uvCoordinates.Add(new vec2(0, 0));
            startButton.uvCoordinates.Add(new vec2(1, 1));
            startButton.uvCoordinates.Add(new vec2(0, 1));

            startButton.texture = new Texture(projectPath + "\\Textures\\btn_start.png", texUnit++, true);

            startButton.Initialize();
            #endregion
            #endregion
        }

        public override void Draw() {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            sh.UseShader();

            background.Draw(transID);

            startButton.Draw(transID);
        }

        public override void CleanUp() {
            sh.DestroyShader();
        }

        public bool startButtonClicked(float x, float y) {
            if (x < btn_min_x || x > btn_max_x || y < btn_min_y || y > btn_max_y)
                return false;
            return true;
        }
    }
}
