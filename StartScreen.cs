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
        Model background, startButton, exitButton, lbl_gamename;
        public static int buttonsCount = 2;
        #endregion

        int texUnit = 0;

        #region Start button viewport boundaries
        float btn_min_x = 473, btn_max_x = 1062, btn_min_y = 570, btn_max_y = 640;
        #endregion

        public static int currentBtnIDX = 0;

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

            background.texture = new Texture(projectPath + "\\Textures\\grimmnight_bk.jpg", (texUnit++) % 32, true);

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

            startButton.texture = new Texture(projectPath + "\\Textures\\btn_play.png", (texUnit++) % 32, true);

            startButton.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.5f,.8f, 1)),
                glm.translate(new mat4(1),new vec3(-0.8f, -0.1f,0))
            });

            startButton.Initialize();
            #endregion
            #region Exit Button
            exitButton = new Model();

            exitButton.vertices.Add(new vec3(-.4f, -.5f, 0));
            exitButton.vertices.Add(new vec3(.4f, -.3f, 0));
            exitButton.vertices.Add(new vec3(.4f, -.5f, 0));
            exitButton.vertices.Add(new vec3(-.4f, -.5f, 0));
            exitButton.vertices.Add(new vec3(.4f, -.3f, 0));
            exitButton.vertices.Add(new vec3(-.4f, -.3f, 0));

            exitButton.uvCoordinates.Add(new vec2(0, 0));
            exitButton.uvCoordinates.Add(new vec2(1, 1));
            exitButton.uvCoordinates.Add(new vec2(1, 0));
            exitButton.uvCoordinates.Add(new vec2(0, 0));
            exitButton.uvCoordinates.Add(new vec2(1, 1));
            exitButton.uvCoordinates.Add(new vec2(0, 1));

            exitButton.texture = new Texture(projectPath + "\\Textures\\btn_exit.png", (texUnit++) % 32, true);

            exitButton.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.5f,.8f, 1)),
                glm.translate(new mat4(1),new vec3(-0.8f, -0.3f,0))
            });

            exitButton.Initialize();
            #endregion
            #region Game Name
            lbl_gamename = new Model();

            lbl_gamename.vertices.Add(new vec3(-.4f, -.5f, 0));
            lbl_gamename.vertices.Add(new vec3(.4f, -.3f, 0));
            lbl_gamename.vertices.Add(new vec3(.4f, -.5f, 0));
            lbl_gamename.vertices.Add(new vec3(-.4f, -.5f, 0));
            lbl_gamename.vertices.Add(new vec3(.4f, -.3f, 0));
            lbl_gamename.vertices.Add(new vec3(-.4f, -.3f, 0));

            lbl_gamename.uvCoordinates.Add(new vec2(0, 0));
            lbl_gamename.uvCoordinates.Add(new vec2(1, 1));
            lbl_gamename.uvCoordinates.Add(new vec2(1, 0));
            lbl_gamename.uvCoordinates.Add(new vec2(0, 0));
            lbl_gamename.uvCoordinates.Add(new vec2(1, 1));
            lbl_gamename.uvCoordinates.Add(new vec2(0, 1));

            lbl_gamename.texture = new Texture(projectPath + "\\Textures\\title3.png", (texUnit++) % 32, true);

            lbl_gamename.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(1,1.5f, 1)),
                glm.translate(new mat4(1),new vec3(.6f, -0.35f,0))
            });

            lbl_gamename.Initialize();
            #endregion
            #endregion
        }

        public override void Draw() {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            sh.UseShader();

            background.Draw(transID);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            #region Buttons
            if(currentBtnIDX == 0)
            {
                startButton.texture = new Texture(projectPath + "\\Textures\\btn_playH.png", (texUnit++) % 32, true);
            }else
                startButton.texture = new Texture(projectPath + "\\Textures\\btn_play.png", (texUnit++) % 32, true);
            startButton.Draw(transID);

            if (currentBtnIDX == 1)
            {
                exitButton.texture = new Texture(projectPath + "\\Textures\\btn_exitH.png", (texUnit++) % 32, true);
            }
            else
                exitButton.texture = new Texture(projectPath + "\\Textures\\btn_exit.png", (texUnit++) % 32, true);
            exitButton.Draw(transID);
            lbl_gamename.Draw(transID);
            #endregion
            Gl.glDisable(Gl.GL_BLEND);

        }

        public override void CleanUp() {
            sh.DestroyShader();
        }

        public bool startButtonClicked(float x, float y) {
            //if (x < btn_min_x || x > btn_max_x || y < btn_min_y || y > btn_max_y)
            //    return false;
            //return true;
            return false;
        }
    }
}
