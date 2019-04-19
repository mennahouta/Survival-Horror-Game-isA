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
        
        #region md2 models
        /*
        md2 bag, bank1, barrel_plastic, BENCH, box, CHAIR, CHAIR1, chair2, chair3, CLOSET;
        md2 coffeemaker, comptable, container_closed, container_opened, curtain, curtain_long;
        md2 curtains, deerhead, desk, desklamp, fireplace, flowerpot2, HANDDRYER, KAST1, LAMP, lantern;
        md2 LIGHT2, LIGHT3, LIGHT5, LIGHT6, locker2, pellet, SOAP, SODAMACHINE, stleer, TABLE, TL, TOILET;
        md2 TPAPER, WATERDISP, wc, workbench, BED, SINK;
        */
        #endregion

        md2 car;

        int num_trees;
        Model3D[] Trees;

        int num_lights;
        md2[] Lights;

        int num_grass;
        Model3D[] Grass;
        #endregion

        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            #region Shaders intialization
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            #endregion


            Gl.glClearColor(0, 0, 0.4f, 1);
            
            cam = new Camera();
            cam.Reset(0, 20, 50, 0, 0, 0, 0, 1, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "trans");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            modelmatrix = glm.scale(new mat4(1), new vec3(50, 50, 50));

            sh.UseShader();

            Random random = new Random();
            #region 3D Models intialization

            #region Grass Models
            num_grass = random.Next(50, 70);
            Grass = new Model3D[num_grass];
            for (int i = 0; i < num_grass; i++)
            {
                Grass[i] = new Model3D();
                int rnd = random.Next(1, 3);
                Grass[i].LoadFile(projectPath + "\\ModelFiles\\grass", 4, "grass0" + (char)(rnd + '0') + ".3ds");
                Grass[i].rotmatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
                Grass[i].scalematrix = glm.scale(new mat4(1), new vec3(5f, 5f, 5f));
                int x = random.Next(-500, 500);
                int y = 0;
                int z = random.Next(-500, 500);
                Grass[i].transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            }
            #endregion

            #region Car Model
            car = new md2(projectPath + "\\ModelFiles\\car\\car.md2");
            car.StartAnimation(animType.STAND);
            car.scaleMatrix = glm.scale(new mat4(1), new vec3(0.2f, 0.2f, 0.2f));
            car.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            car.TranslationMatrix = glm.translate(new mat4(1), new vec3(-20, 0, 50));
            float car_radius = 5f;
            vec3 car_pos = new vec3(-20, 0, 50);
            #endregion

            #region Trees Models
            num_trees = random.Next(50, 100);
            Trees = new Model3D[num_trees];
            for(int i=0; i < num_trees; i++)
            {
                Trees[i] = new Model3D();
                Trees[i].LoadFile(projectPath + "\\ModelFiles\\tree", 1, "Tree.obj");
                Trees[i].scalematrix = glm.scale(new mat4(1), new vec3(13, 30, 10));
                int x = random.Next(-500, 500);
                int y = 0;
                int z = random.Next(-500, 500);
                vec3 pos = new vec3(x, y, z);

                float dist = (float)Math.Sqrt((pos.x - car_pos.x) * (pos.x - car_pos.x) + (pos.y - car_pos.y) * (pos.y - car_pos.y) + (pos.z - car_pos.z) * (pos.z - car_pos.z));
                if(dist <= car_radius)
                {
                    x -= (int)(2 * car_radius);
                    z -= (int)(2 * car_radius);
                }
                Trees[i].transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            }
            #endregion

            #region Lights Models
            num_lights = random.Next(20, 30);
            Lights = new md2[num_lights];
            for (int i = 0; i < num_lights; i++)
            {
                Lights[i] = new md2(projectPath + "\\ModelFiles\\LIGHT6.md2");
                Lights[i].scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
                Lights[i].rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
                int x = random.Next(-500, 500);
                int y = 0;
                int z = random.Next(-500, 500);
                vec3 pos = new vec3(x, y, z);

                float dist = (float)Math.Sqrt((pos.x - car_pos.x) * (pos.x - car_pos.x) + (pos.y - car_pos.y) * (pos.y - car_pos.y) + (pos.z - car_pos.z) * (pos.z - car_pos.z));
                if (dist <= car_radius)
                {
                    x -= (int)(2 * car_radius);
                    z -= (int)(2 * car_radius);
                }
                Lights[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            }
            #endregion

            #region Other
            #region Not working md2 models
            /*
            bag = new md2(projectPath + "\\ModelFiles\\bag.md2");
            bank1 = new md2(projectPath + "\\ModelFiles\\bank1.md2");
            barrel_plastic = new md2(projectPath + "\\ModelFiles\\barrel_plastic.md2");
            box = new md2(projectPath + "\\ModelFiles\\box.md2");
            chair2 = new md2(projectPath + "\\ModelFiles\\chair2.md2");
            chair3 = new md2(projectPath + "\\ModelFiles\\chair3.md2");
            coffeemaker = new md2(projectPath + "\\ModelFiles\\coffeemaker.md2");
            comptable = new md2(projectPath + "\\ModelFiles\\comptable.md2");
            container_closed = new md2(projectPath + "\\ModelFiles\\container_closed.md2");
            container_opened = new md2(projectPath + "\\ModelFiles\\container_opened.md2");
            curtain = new md2(projectPath + "\\ModelFiles\\curtain.md2");
            curtain_long = new md2(projectPath + "\\ModelFiles\\curtain_long.md2");
            curtains = new md2(projectPath + "\\ModelFiles\\curtains.md2");
            deerhead = new md2(projectPath + "\\ModelFiles\\deerhead.md2");
            desk = new md2(projectPath + "\\ModelFiles\\desk.md2");
            desklamp = new md2(projectPath + "\\ModelFiles\\desklamp.md2");
            fireplace = new md2(projectPath + "\\ModelFiles\\fireplace.md2");
            flowerpot2 = new md2(projectPath + "\\ModelFiles\\flowerpot2.md2");
            lantern = new md2(projectPath + "\\ModelFiles\\lantern.md2");
            locker2 = new md2(projectPath + "\\ModelFiles\\locker2.md2");
            pellet = new md2(projectPath + "\\ModelFiles\\pellet.md2");
            stleer = new md2(projectPath + "\\ModelFiles\\stleer.md2");
            wc = new md2(projectPath + "\\ModelFiles\\wc.md2");
            workbench = new md2(projectPath + "\\ModelFiles\\workbench.md2");       
            
            bag.StartAnimation(animType.STAND);
            bank1.StartAnimation(animType.STAND);
            barrel_plastic.StartAnimation(animType.STAND);
            box.StartAnimation(animType.STAND);
            chair2.StartAnimation(animType.STAND);
            chair3.StartAnimation(animType.STAND);
            coffeemaker.StartAnimation(animType.STAND);
            comptable.StartAnimation(animType.STAND);
            container_closed.StartAnimation(animType.STAND);
            container_opened.StartAnimation(animType.STAND);
            curtain.StartAnimation(animType.STAND);
            curtain_long.StartAnimation(animType.STAND);
            curtains.StartAnimation(animType.STAND);
            deerhead.StartAnimation(animType.STAND);
            desk.StartAnimation(animType.STAND);
            desklamp.StartAnimation(animType.STAND);
            fireplace.StartAnimation(animType.STAND);
            lantern.StartAnimation(animType.STAND);
            flowerpot2.StartAnimation(animType.STAND);
            locker2.StartAnimation(animType.STAND);
            pellet.StartAnimation(animType.STAND);
            stleer.StartAnimation(animType.STAND);
            wc.StartAnimation(animType.STAND);
            workbench.StartAnimation(animType.STAND);
             * */
            #endregion

            #region Open File
            /*
            SINK = new md2(projectPath + "\\ModelFiles\\SINK.md2");
            BED = new md2(projectPath + "\\ModelFiles\\BED.md2");
            BENCH = new md2(projectPath + "\\ModelFiles\\BENCH.md2");
            CHAIR = new md2(projectPath + "\\ModelFiles\\CHAIR.md2");
            CHAIR1 = new md2(projectPath + "\\ModelFiles\\CHAIR1.md2");
            CLOSET = new md2(projectPath + "\\ModelFiles\\CLOSET.md2");
            HANDDRYER = new md2(projectPath + "\\ModelFiles\\HANDDRYER.md2");
            KAST1 = new md2(projectPath + "\\ModelFiles\\KAST1.md2");
            LAMP = new md2(projectPath + "\\ModelFiles\\LAMP.md2");
            LIGHT2 = new md2(projectPath + "\\ModelFiles\\LIGHT2.md2");
            LIGHT3 = new md2(projectPath + "\\ModelFiles\\LIGHT3.md2");
            LIGHT5 = new md2(projectPath + "\\ModelFiles\\LIGHT5.md2");
            LIGHT6 = new md2(projectPath + "\\ModelFiles\\LIGHT6.md2");
            SOAP = new md2(projectPath + "\\ModelFiles\\SOAP.md2");
            SODAMACHINE = new md2(projectPath + "\\ModelFiles\\SODAMACHINE.md2");
            TABLE = new md2(projectPath + "\\ModelFiles\\TABLE.md2");
            TL = new md2(projectPath + "\\ModelFiles\\TL.md2");
            TOILET = new md2(projectPath + "\\ModelFiles\\TOILET.md2");
            TPAPER = new md2(projectPath + "\\ModelFiles\\TPAPER.md2");
            WATERDISP = new md2(projectPath + "\\ModelFiles\\WATERDISP.md2");
            */
            #endregion

            #region StartAnimation
            /*
            SINK.StartAnimation(animType.STAND);
            BED.StartAnimation(animType.STAND);
            BENCH.StartAnimation(animType.STAND);
            CHAIR.StartAnimation(animType.STAND);
            CHAIR1.StartAnimation(animType.STAND);
            CLOSET.StartAnimation(animType.STAND);
            HANDDRYER.StartAnimation(animType.STAND);
            KAST1.StartAnimation(animType.STAND);
            LAMP.StartAnimation(animType.STAND);
            LIGHT2.StartAnimation(animType.STAND);
            LIGHT3.StartAnimation(animType.STAND);
            LIGHT5.StartAnimation(animType.STAND);
            LIGHT6.StartAnimation(animType.STAND);
            SOAP.StartAnimation(animType.STAND);
            SODAMACHINE.StartAnimation(animType.STAND);
            TABLE.StartAnimation(animType.STAND);
            TL.StartAnimation(animType.STAND);
            TOILET.StartAnimation(animType.STAND);
            TPAPER.StartAnimation(animType.STAND);
            WATERDISP.StartAnimation(animType.STAND);
            */
            #endregion

            #region Transformation
            /*
            BED.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            BED.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));

            SINK.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            SINK.TranslationMatrix = glm.translate(new mat4(1), new vec3(40, -1, 10));

            BENCH.scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
            BENCH.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            BENCH.TranslationMatrix = glm.translate(new mat4(1), new vec3(20, -1, 20));

            CHAIR.scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
            CHAIR.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 1, 0));
            CHAIR.TranslationMatrix = glm.translate(new mat4(1), new vec3(-10, -1, -10));

            CHAIR1.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            CHAIR1.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            CLOSET.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            CLOSET.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            HANDDRYER.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            HANDDRYER.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            KAST1.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            KAST1.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            LAMP.scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 1f));
            LAMP.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            LIGHT2.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            //LIGHT2.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            LIGHT3.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            //LIGHT3.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            LIGHT5.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            //LIGHT5.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            LIGHT6.scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.7f));
            LIGHT6.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            SOAP.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            SOAP.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            SODAMACHINE.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            SODAMACHINE.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            TABLE.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            TABLE.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            TL.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            TL.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            TOILET.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            TOILET.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            TPAPER.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            TPAPER.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));

            WATERDISP.scaleMatrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f));
            WATERDISP.rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
            */
            #endregion
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
            #region working models
            //BED.Draw(transID);
            //SINK.Draw(transID);
            //BENCH.Draw(transID);
            //CHAIR.Draw(transID);
            //CHAIR1.Draw(transID);
            //CLOSET.Draw(transID);
            //HANDDRYER.Draw(transID);
            //KAST1.Draw(transID);
            //LAMP.Draw(transID);
            //LIGHT2.Draw(transID);
            //LIGHT3.Draw(transID);
            //LIGHT5.Draw(transID);
            //LIGHT6.Draw(transID);
            //SOAP.Draw(transID);
            //SODAMACHINE.Draw(transID);
            //TABLE.Draw(transID);
            //TL.Draw(transID);
            //TOILET.Draw(transID);
            //TPAPER.Draw(transID);
            //WATERDISP.Draw(transID);
            #endregion

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            for (int i=0; i < num_trees; i++)
            {
                Trees[i].Draw(transID);
            }
            Gl.glDisable(Gl.GL_BLEND);

            for(int i=0; i < num_lights; i++)
            {
                Lights[i].Draw(transID);
            }

            car.Draw(transID);

            for (int i = 0; i < num_grass; i++)
            {
                Grass[i].Draw(transID);
            }
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
