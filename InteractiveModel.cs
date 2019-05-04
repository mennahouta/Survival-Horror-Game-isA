using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using Tao.OpenGl;
using System.IO;
using Assimp;

namespace Graphics
{
    enum modelType
    {
        DOOR,
        LIGHT,
        TEXT,
        PILLS,
        RADIO,
        BARRELS,
        GARBAGE,
        MAX_MODELS,
        NULL
    }
    class InteractiveModel
    {
        Model3D obj;
        public float range;
        public vec3 position;
        public vec3 boundingBox; //x: width, y: height, z: depth
        public modelType type;
        public int objID;
        public Boolean isDrawn;
        public static bool radio_ON = false; // for the interaction with the radio
        public static MP3_player radio_sound = new MP3_player();
        vec3 old_scaling = new vec3(1, 1, 1);
        public InteractiveModel(String folderName, String modelName, int texUnit, float rangeOfInteraction, modelType t, int ID)
        {
            obj = new Model3D();
            obj.LoadFile(Renderer.projectPath + "\\ModelFiles\\" + folderName, texUnit, modelName + ".obj");
            position = new vec3(0, 0, 0);
            range = rangeOfInteraction;
            type = t;
            objID = ID;
            isDrawn = false;
            #region BoundingBox intialization
            boundingBox = new vec3();
            float minWidth  = float.MaxValue, maxWidth  = float.MinValue;
            float minHeight = float.MaxValue, maxHeight = float.MinValue;
            float minDepth  = float.MaxValue, maxDepth  = float.MinValue;
            foreach(Model m in obj.meshes)
                foreach(vec3 v in m.vertices)
                {
                    minWidth = Math.Min(minWidth, v.x);
                    maxWidth = Math.Max(maxWidth, v.x);

                    minHeight = Math.Min(minHeight, v.y);
                    maxHeight = Math.Max(maxHeight, v.y);

                    minDepth = Math.Min(minDepth, v.z);
                    maxDepth = Math.Max(maxDepth, v.z);
                }
            boundingBox.x = range * (maxWidth - minWidth);
            boundingBox.y = range * (maxHeight - minHeight);
            boundingBox.z = range * (maxDepth - minDepth);
            #endregion
        }

        public void Scale(float x, float y, float z)
        {
            obj.scalematrix = glm.scale(new mat4(1), new vec3(x, y, z));
            boundingBox.x /= old_scaling.x;
            boundingBox.y /= old_scaling.y;
            boundingBox.z /= old_scaling.z;
            boundingBox *= new vec3(x, y, z);
            old_scaling = new vec3(x, y, z);
        }

        public void Rotate(float angle, vec3 v)
        {
            obj.rotmatrix = glm.rotate((angle) / 180 * 3.141592f, v);
        }

        public void Translate(float x, float y, float z)
        {
            obj.transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            position = new vec3(x, y, z);
        }

        public void Draw(int matID)
        {
            obj.Draw(matID);
            isDrawn = true;
        }

        public void Event()
        {
            switch (type)
            {
                case modelType.DOOR:
                    DOOR_Event();
                    break;
                case modelType.GARBAGE:
                    GARBAGE_Event();
                    break;
                case modelType.BARRELS:
                    BARRELS_Event();
                    break;
                case modelType.LIGHT:
                    LIGHT_Event();
                    break;
                case modelType.PILLS:
                    PILLS_Event();
                    break;
                case modelType.RADIO:
                    RADIO_Event();
                    break;
                case modelType.TEXT:
                    TEXT_Event();
                    break;
            }
        }

        public void DOOR_Event()
        {
            //access current loaded skybox,
            //update it, and set the renderer 
            //which room is the current
            switch (objID)
            {
                case 0:
                    if (Renderer.playerHasKey)
                        Renderer.currentSkyboxID = (Renderer.currentSkyboxID == 0) ? 1 : 0;
                    break;
                case 1:
                    Renderer.currentSkyboxID = (Renderer.currentSkyboxID == 1) ? 2 : 1;
                    break;
                case 2:
                    Renderer.currentSkyboxID = (Renderer.currentSkyboxID == 1) ? 3 : 1;
                    break;
                case 3:
                    Renderer.currentSkyboxID = (Renderer.currentSkyboxID == 2) ? 4 : 2;
                    break;
                case 4:
                    Renderer.currentSkyboxID = (Renderer.currentSkyboxID == 2) ? 5 : 2;
                    break;
                case 5:
                    Renderer.currentSkyboxID = 6;
                    break;
            }

            switch (Renderer.currentSkyboxID)
            {
                case 0:     //0: forest skybox
                    if (Renderer.playerHasKey)
                    {
                        Renderer.cam.Reset(612, 30, 500, 20, 20, 150, 0, 1, 0);
                        Scale(.5f, .5f, .5f);
                        Translate(700, 0, 500);
                    }
                    break;
                case 1:     //1: livingroom skybox
                    if (objID == 0)
                    {
                        Renderer.cam.Reset(95, 105, 280, 95, 50, 150, 0, 1, 0);
                        Scale(1f, 1.3f, 1f);
                        Translate(245, -17, 355);
                    }
                    else if (objID == 1) //bed -> living
                    {
                        Renderer.cam.Reset(280, 105, 245, 20, 50, 245, 0, 1, 0);
                        Translate(300, 0, 245);
                    }
                    else if (objID == 2) //kitchen -> living
                    {
                        Renderer.cam.Reset(80, 105, 20, 80, 50, 150, 0, 1, 0);
                        Translate(80, 0, 0);
                    }
                    break;
                case 2:     //2: bedroom skybox
                    Scale(1f, 1.3f, 1f);
                    if (objID == 1)
                    {
                        Renderer.cam.Reset(20, 105, 245, 50, 50, 245, 0, 1, 0);
                        Translate(0, 0, 245);
                    }
                    else if(objID == 3)
                    {
                        Renderer.cam.Reset(80, 105, 20, 50, 50, 245, 0, 1, 0);
                        Translate(80, 0, 0);
                    }
                    else if(objID == 4)
                    {
                        Renderer.cam.Reset(270, 105, 280, 50, 50, 245, 0, 1, 0);
                        Translate(270, 0, 300);
                    }
                    break;
                case 3:     //3: kitchen skybox
                    Renderer.cam.Reset(95, 105, 280, 20, 20, 150, 0, 1, 0);
                    Translate(80, 0, 300);
                    break;
                case 4:     //4: bathroom skybox
                    Renderer.cam.Reset(80, 105, 280, 20, 20, 150, 0, 1, 0);
                    Translate(80, 0, 300);
                    break;
                case 5:     //5: closet skybox(has a trapdoor)
                    Renderer.cam.Reset(100, 105, 20, 20, 20, 150, 0, 1, 0);
                    Translate(100, 0, 0);
                    break;
                case 6:     //6: basement skybox
                    Renderer.cam.Reset(95, 50, 280, 20, 20, 150, 0, 1, 0);
                    break;
            }
        }

        public void GARBAGE_Event()
        {
            //Menna
        }

        public void BARRELS_Event()
        {
            //random event based on the barrels ID
        }

        public void LIGHT_Event()
        {
            //recharge the light intensity?
        }

        public void PILLS_Event()
        {
            //refill the sanity bar
        }

        public void RADIO_Event()
        {
            //Esraa
        }

        public void TEXT_Event()
        {
            //open a certain text file and display
            //its text on the screen
        }

    }
}
