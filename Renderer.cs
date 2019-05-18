using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Threading;

namespace Graphics
{
    enum skyboxType
    {
        FOREST,
        LIVING,
        BEDROOM,
        KITCHEN,
        BATHROOM,
        CLOSET,
        BASEMENT,
        MAX_SKYBOXES
    }

    
    class Renderer : ScreenClass {
        #region Shaders declaration
        Shader sh, sh2D;
        #endregion

        #region Matricies declaration
        int transID, viewID, projID;
        mat4 ProjectionMatrix, ViewMatrix, modelmatrix;
        #endregion

        #region Light Data declaration
        int LightPositionID, EyePositionID, AmbientLightID, DataID, IntensityID;
        List<vec3> LightPosition = new List<vec3>()
        {
            new vec3(300f, 800f, 980f), //0: Forest
            new vec3(195f, 115f, 025f), //1: LivingRoom 
            new vec3(285f, 055f, 100f), //2: Bedroom
            new vec3(150f, 290f, 150f), //3: Kitchen
            new vec3(100f, 190f, 100f), //4: Bathroom
            new vec3(100f, 190f, 100f), //5: Closet
        };
        List<vec3> AmbientLight = new List<vec3>()
        {
            new vec3(0.5f, 0.5f, 0.42f),  //0: Forest
            new vec3(.1f, 0.1f, 0.1f),  //1: LivingRoom 
            new vec3(.1f, 0.1f, 0.1f),  //2: Bedroom
            new vec3(.3f, 0.3f, 0.3f),  //3: Kitchen
            new vec3(.1f, 0.1f, 0.1f),  //4: Bathroom
            new vec3(.1f, 0.1f, 0.1f),  //5: Closet
        };
        List<vec3> LightIntensity = new List<vec3>()
        {
            new vec3(.2f, .2f, .2f),   //0: Forest
            new vec3(.8f, .8f, 0.6f),  //1: LivingRoom
            new vec3(.8f, .8f, 0.7f),  //2: Bedroom
            new vec3(.8f, .8f, 0.8f),  //3: Kitchen
            new vec3(.9f, .9f, 0.82f), //4: Bathroom
            new vec3(.8f, .8f, 0.8f),  //5: Closet
        };
        List<vec3> LightData = new List<vec3>()
        {
            new vec3(1000, 100, 0),  //0: Forest
            new vec3(0180, 001, 1),  //1: LivingRoom
            new vec3(0200, 001, 1),  //2: Bedroom
            new vec3(0400, 050, 1),  //3: Kitchen
            new vec3(0300, 050, 1),  //4: Bathroom
            new vec3(0300, 100, 1),  //5: Closet
        };
        #endregion

        #region Models Lists Declaration
        //for BoundingBoxes and isDrawn
        public static List<Model3D> Models_3D;
        public static List<InteractiveModel> Models_Interactive;
        #endregion

        static public Camera cam;

        public float Speed = 1;

        Ground g_down;

        List<Boolean> PrevoiuslyLoaded;
        #region 3D Models declaration
        Model3D car, watchtower, house_obj;
        List<Model3D> bedroomFurni;
        List<Model3D> livingFurni;
        List<Model3D> bathroomFurni;
        List<Model3D> closetFurni;
        List<Model3D> kitchenFurni;

        public static List<InteractiveModel> doors;
        public static List<Skybox> skyboxes;
        public static int currentSkyboxID = 0; //FORREST

        InteractiveModel radio, phone;

        #region GARBAGE
        public static List<InteractiveModel> garbages;
        int numOfGarbages;
        public static int key_garbageID;
        public static bool playerHasKey = false;
        #endregion

        int texUnit_counter = 0;

        #region Trees Models
        int num_trees, 
            rnd_trees_L = /*5*/0, 
            rnd_trees_H = /*10*/0;
        Model3D[] Trees;
        #endregion

        #region Lights Models
        int num_lights, 
            rnd_lights_L = /*2*/0, 
            rnd_lights_H = /*5*/0;
        md2[] Lights;
        #endregion

        #region Grass Models
        int num_grass, 
            rnd_grass_L = 50, 
            rnd_grass_H = 70;
        Model3D[] Grass;
        #endregion

        #endregion

        public static bool drawText = false;
        #region 2D Models declaration
        Model obj2D;
        List<Texture> chapter;
        int modelMat2D_ID;
        #endregion

        public override void Initialize()
        {
            #region Shaders intialization
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            sh2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            #endregion

            //Gl.glClearColor(0, 0, 0.4f, 1);
            
            cam = new Camera();
            cam.Reset(180, 30, 800, 20, 20, 150, 0, 1, 0);

            #region 2D Models intializtion
            modelMat2D_ID = Gl.glGetUniformLocation(sh2D.ID, "model");
            #region Text Obj
            obj2D = new Model();
            obj2D.vertices.Add(new vec3(-1, 1, 0));
            obj2D.vertices.Add(new vec3(1, -1, 0));
            obj2D.vertices.Add(new vec3(-1, -1, 0));
            obj2D.vertices.Add(new vec3(1, 1, 0));
            obj2D.vertices.Add(new vec3(-1, 1, 0));
            obj2D.vertices.Add(new vec3(1, -1, 0));

            obj2D.uvCoordinates.Add(new vec2(0, 0));
            obj2D.uvCoordinates.Add(new vec2(1, 1));
            obj2D.uvCoordinates.Add(new vec2(0, 1));
            obj2D.uvCoordinates.Add(new vec2(1, 0));
            obj2D.uvCoordinates.Add(new vec2(0, 0));
            obj2D.uvCoordinates.Add(new vec2(1, 1));

            obj2D.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>{
                glm.scale(new mat4(1), new vec3(0.3f, 1f, 1)), //need to be updated with every skybox FML
                glm.translate(new mat4(1), new vec3(0, 0, 0)) }
            );
            obj2D.Initialize();

            chapter = new List<Texture>()
            {
                new Texture(projectPath + "\\Textures\\ch1.png", 31),
                new Texture(projectPath + "\\Textures\\ch2.png", 30),
                new Texture(projectPath + "\\Textures\\ch3.png", 29),
            };
            #endregion

            #endregion
            #region Matricies intialization
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "trans");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            modelmatrix = glm.scale(new mat4(1), new vec3(50, 50, 50));
            #endregion

            sh.UseShader();

            g_down = new Ground(2100, 2100, 2, 2);


            #region Skyboxes intialization
        
            #region Initialize All Load of Skyboxes to False
            PrevoiuslyLoaded = new List<Boolean>();
            for (int i = 0; i < (int)skyboxType.MAX_SKYBOXES; i++)
                PrevoiuslyLoaded.Add(false);
            #endregion

            List<List<String>> skyboxes_textures = new List<List<String>>()
            {
                //texUp, texDown, texRight, texLeft, texBack, texFront
                new List<string>()
                {
                    "grimmnight_up.jpg", "grimmnight_dn.jpg", "grimmnight_rt.jpg", "grimmnight_lf.jpg", "grimmnight_bk.jpg", "grimmnight_ft.jpg"
                }, //0: Forest skybox
                new List<string>()
                {
                    "room.jpg", "room_bt.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg"
                }, //1: LivingRoom skybox
                new List<string>()
                {
                    "room.jpg", "room_bt.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg"
                }, //2: Bedroom skybox
                new List<string>()
                {
                    "room.jpg", "kit_dn.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg"
                }, //3: Kitchen skybox
                new List<string>()
                {
                    "room.jpg", "bath3.jpg", "bath3.jpg", "bath3.jpg", "bath3.jpg", "bath3.jpg"
                }, //4: Bathroom skybox
                new List<string>()
                {
                    "room.jpg", "closet_floor.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg"
                }, //5: Closet skybox (has a trapdoor)
                new List<string>()
                {
                    "room.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg", "room.jpg"
                }, //6: Basement skybox
            };
            skyboxes = new List<Skybox>()
            {
                new Skybox(1000, 1000, 1000, skyboxes_textures[(int) skyboxType.FOREST]),   //0: Forest skybox
                new Skybox(300, 300, 300, skyboxes_textures[(int) skyboxType.LIVING]),      //1: LivingRoom skybox
                new Skybox(300, 300, 300, skyboxes_textures[(int) skyboxType.BEDROOM]),     //2: Bedroom skybox
                new Skybox(300, 300, 300, skyboxes_textures[(int) skyboxType.KITCHEN]),     //3: Kitchen skybox
                new Skybox(200, 200, 200, skyboxes_textures[(int) skyboxType.BATHROOM]),    //4: Bathroom skybox
                new Skybox(200, 200, 200, skyboxes_textures[(int) skyboxType.CLOSET]),      //5: Closet skybox (has a trapdoor)
                new Skybox(300, 300, 300, skyboxes_textures[(int) skyboxType.BASEMENT]),    //6: Basement skybox
            };
            #endregion


            #region Collision Boundingboxes list intialization
            Models_3D = new List<Model3D>();
            Models_Interactive = new List<InteractiveModel>();
            #endregion

            #region Doors initialization
            #region Doors list
            doors = new List<InteractiveModel>()
            {
                new InteractiveModel("door", "door", texUnit_counter%32, 30, modelType.DOOR, 0),        //0: Front door, from 0 <-> 1
                new InteractiveModel("door", "door_in", texUnit_counter%32, 7, modelType.DOOR, 1),     //1: livingBED, from 1<->2
                new InteractiveModel("door", "door_in", texUnit_counter%32, 7, modelType.DOOR, 2),     //2: livingKIT, from 1<->3
                new InteractiveModel("door", "door_in", texUnit_counter%32, 7, modelType.DOOR, 3),     //3: bathR, from 2<->4
                new InteractiveModel("door", "door_in", texUnit_counter%32, 7, modelType.DOOR, 4),     //4: closet, from 2<->5
                new InteractiveModel("door", "door_in", texUnit_counter%32, 7, modelType.DOOR, 5),     //5: trapDr, from 5 ->6 (game over)
            };
            texUnit_counter++;
            #endregion
            #region Transformations
            doors[0].Scale(.5f, .5f, .5f);
            for (int i = 1; i < doors.Count; i++)
                doors[i].Scale(1f, 1.3f, 1f);
            doors[0].Translate(700, 0, 500);
            doors[1].Rotate(-90, new vec3(0, 1, 0));
            doors[1].Translate(300, 0, 245);
            doors[2].Translate(80, 0, 0);
            doors[3].Translate(80, 0, 0);
            doors[4].Translate(270, 0, 300);
            #endregion
            #region Doors collision boundingboxes
            for (int i = 0; i < 5; i++) {
                setCollisionBoundingBox(doors[i].position, doors[i].obj);
                Models_Interactive.Add(doors[i]);
            }
            #endregion
            #endregion

            Random random = new Random();
            #region 3D Models intialization            

            //md2 needs boundingboxes
            #region Lights Models
            num_lights = random.Next(rnd_lights_L, rnd_lights_H);
            Lights = new md2[num_lights];
            for (int i = 0; i < num_lights; i++)
            {
                Lights[i] = new md2(projectPath + "\\ModelFiles\\LIGHT6.md2");
                Lights[i].scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
                Lights[i].rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
                int x = random.Next(10, 990);
                int y = 0;
                int z = random.Next(10, 990);
                vec3 pos = new vec3(x, y, z);
                Lights[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            }
            #endregion

            #endregion

            #region Light Data intialization
            //get location of specular and attenuation then send their values
            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            //get location of light position and send its value
            LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            //setup the ambient light component.
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            //setup the eye position.
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");
            //setup the light intensity
            IntensityID = Gl.glGetUniformLocation(sh.ID, "Il");

            SendLightData();
            #endregion

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        #region
        public void LoadSkyboxModels()
        {
            skyboxType SKYBOX = (skyboxType)currentSkyboxID;
            if (PrevoiuslyLoaded[(int)SKYBOX])
                return;

            Random random = new Random();
            switch (SKYBOX)
            {
                #region FOREST
                case skyboxType.FOREST:
                    #region Garbage
                    garbages = new List<InteractiveModel>();
                    List<vec3> garbagePos = new List<vec3>{
                        new vec3(500, 0, 500),
                        new vec3(470, 0, 570),
                        new vec3(500, 0, 620),
                        new vec3(470, 0, 670),
                        new vec3(500, 0, 720)
                    };
                    numOfGarbages = garbagePos.Count;
                    for (int i = 0; i < numOfGarbages; i++) {
                        garbages.Add(new InteractiveModel("garbage_bag", "garbage_bag", texUnit_counter % 32, 7, modelType.GARBAGE, i));

                        garbages[i].Scale(.5f, .5f, .5f);
                        float x = garbagePos[i].x;
                        float y = garbagePos[i].y;
                        float z = garbagePos[i].z;
                        garbages[i].Translate(x, y, z);
                        setCollisionBoundingBox(new vec3(x, y, z), garbages[i].obj);
                        Models_Interactive.Add(garbages[i]);
                    }
                    texUnit_counter += 1;
                    key_garbageID = random.Next(0, numOfGarbages);
                    #endregion
                    #region Car Model
                    car = new Model3D();
                    car.LoadFile(projectPath + "\\ModelFiles\\car", texUnit_counter % 32, "delorean.obj");
                    texUnit_counter++;
                    car.transmatrix = glm.translate(new mat4(1), new vec3(200, 0, 800));
                    setCollisionBoundingBox(new vec3(200, 0, 800), car);
                    Models_3D.Add(car);
                    #endregion
                    #region Watchtower
                    watchtower = new Model3D();
                    watchtower.LoadFile(projectPath + "\\ModelFiles\\watchtower", 0, "wooden watch tower.obj");
                    watchtower.scalematrix = glm.scale(new mat4(1), new vec3(20, 30, 20));
                    watchtower.transmatrix = glm.translate(new mat4(1), new vec3(400, 0, 400));
                    setCollisionBoundingBox(new vec3(400, 0, 400), watchtower);
                    Models_3D.Add(watchtower);
                    #endregion
                    #region House
                    house_obj = new Model3D();
                    house_obj.LoadFile(projectPath + "\\ModelFiles\\house_obj", 7, "house-low-rus-obj.obj");
                    house_obj.scalematrix = glm.scale(new mat4(1), new vec3(.5f, .5f, .5f));
                    house_obj.transmatrix = glm.translate(new mat4(1), new vec3(700, 0, 500));
                    setCollisionBoundingBox(new vec3(700, 0, 500), house_obj);
                    Models_3D.Add(house_obj);
                    #endregion
                    #region Phone Model
                    phone = new InteractiveModel("phone", "iPhone 6", texUnit_counter % 32, 8, modelType.TEXT, 0);
                    texUnit_counter++;
                    phone.Scale(0.4f, 0.4f, 0.4f);
                    phone.Translate(180, 2, 800);
                    #endregion
                    #region Grass Models
                    num_grass = random.Next(rnd_grass_L, rnd_grass_H);
                    Grass = new Model3D[num_grass];
                    for (int i = 0; i < num_grass; i++) {
                        Grass[i] = new Model3D();
                        int rnd = random.Next(1, 3);
                        Grass[i].LoadFile(projectPath + "\\ModelFiles\\grass", 2, "grass0" + (char)(rnd + '0') + ".3ds");
                        Grass[i].rotmatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
                        Grass[i].scalematrix = glm.scale(new mat4(1), new vec3(5f, 5f, 5f));
                        int x = random.Next(10, 990);
                        int y = 0;
                        int z = random.Next(10, 990);
                        Grass[i].transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
                        //setCollisionBoundingBox(new vec3(x, y, z), Grass[i]);
                        //Models_3D.Add(Grass[i]);
                    }
                    #endregion
                    #region Trees Models
                    num_trees = random.Next(rnd_trees_L, rnd_trees_H);
                    Trees = new Model3D[num_trees];
                    for (int i = 0; i < num_trees; i++) {
                        Trees[i] = new Model3D();
                        Trees[i].LoadFile(projectPath + "\\ModelFiles\\tree", 3, "Tree.obj");
                        Trees[i].scalematrix = glm.scale(new mat4(1), new vec3(13, 30, 10));
                        int x = random.Next(10, 990);
                        int y = 0;
                        int z = random.Next(10, 990);
                        vec3 pos = new vec3(x, y, z);
                        Trees[i].transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
                        setCollisionBoundingBox(new vec3(x, y, z), Trees[i]);
                        Models_3D.Add(Trees[i]);
                    }
                    #endregion
                    #region Lights Models
                    num_lights = random.Next(rnd_lights_L, rnd_lights_H);
                    Lights = new md2[num_lights];
                    for (int i = 0; i < num_lights; i++)
                    {
                        Lights[i] = new md2(projectPath + "\\ModelFiles\\LIGHT6.md2");
                        Lights[i].scaleMatrix = glm.scale(new mat4(1), new vec3(0.3f, 0.3f, 0.3f));
                        Lights[i].rotationMatrix = glm.rotate(-90.0f / 180 * 3.1412f, new vec3(1, 0, 0));
                        int x = random.Next(10, 990);
                        int y = 0;
                        int z = random.Next(10, 990);
                        vec3 pos = new vec3(x, y, z);
                        Lights[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(x, y, z));
                    }

                    #endregion
                    break;
                #endregion
                #region LIVING
                case skyboxType.LIVING:
                    #region FURNI
                    livingFurni = new List<Model3D>();
                    for (int i = 0; i < 7; i++)
                        livingFurni.Add(new Model3D());
                    #region LoadFile
                    livingFurni[0].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "sofa3.obj");
                    texUnit_counter++;
                    livingFurni[1].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "rug.obj");
                    texUnit_counter++;
                    livingFurni[2].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "oldtv.obj");
                    texUnit_counter++;
                    livingFurni[3].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "table2.obj");
                    texUnit_counter++;
                    livingFurni[4].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "plant.obj");
                    texUnit_counter++;
                    livingFurni[5].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "floor_lamp.obj");
                    texUnit_counter++;
                    livingFurni[6].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, "decoSet.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    for (int i = 0; i < 5; i++) {
                        livingFurni[i].transmatrix = glm.translate(new mat4(1), new vec3(210, 1, 107));
                        setCollisionBoundingBox(new vec3(210, 1, 107), livingFurni[i]);
                        Models_3D.Add(livingFurni[i]);
                    }
                    livingFurni[5].transmatrix = glm.translate(new mat4(1), new vec3(180, 0, 15));
                    livingFurni[6].transmatrix = glm.translate(new mat4(1), new vec3(15, 0, 150));
                    #endregion
                    #region Living Furni Collision BB
                    setCollisionBoundingBox(new vec3(180, 0, 15), livingFurni[5]);
                    Models_3D.Add(livingFurni[5]);
                    setCollisionBoundingBox(new vec3(15, 0, 150), livingFurni[6]);
                    Models_3D.Add(livingFurni[6]);
                    #endregion
                    #endregion
                    #region Radio Model
                    radio = new InteractiveModel("radio", "radio", texUnit_counter % 32, 8, modelType.RADIO, 0);
                    texUnit_counter++;
                    radio.Translate(15, 30, 150);
                    setCollisionBoundingBox(new vec3(15, 30, 150), radio.obj);
                    Models_Interactive.Add(radio);
                    #endregion
                    break;
                #endregion
                #region BEDROOM
                case skyboxType.BEDROOM:
                    #region Bedroom Furni
                    bedroomFurni = new List<Model3D>()
                {
                    new Model3D(), //Bed
                    new Model3D(), //Side1
                    new Model3D(), //Side2
                    new Model3D(), //Closet
                    new Model3D(), //Chair
                    new Model3D(), //lamp
                };
                    #region LoadFile
                    bedroomFurni[0].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "bed.obj");
                    texUnit_counter++;
                    bedroomFurni[1].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "side1.obj");
                    texUnit_counter++;
                    bedroomFurni[2].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "side2.obj");
                    texUnit_counter++;
                    bedroomFurni[3].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "closet.obj");
                    texUnit_counter++;
                    bedroomFurni[4].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "chair.obj");
                    texUnit_counter++;
                    bedroomFurni[5].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, "lamp.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    for (int i = 0; i < bedroomFurni.Count - 1; i++)
                    {
                        bedroomFurni[i].scalematrix = glm.scale(new mat4(1), new vec3(.3f, .3f, .3f));
                        bedroomFurni[i].rotmatrix = glm.rotate(-90.0f / 180 * 3.141592f, new vec3(0, 1, 0));
                    }
                    for (int i = 0; i < 3; i++) {
                        bedroomFurni[i].transmatrix = glm.translate(new mat4(1), new vec3(245, 0, 150));
                        setCollisionBoundingBox(new vec3(245, 0, 150), bedroomFurni[i]);
                        Models_3D.Add(bedroomFurni[i]);
                    }
                    bedroomFurni[3].transmatrix = glm.translate(new mat4(1), new vec3(10, 0, 150));
                    bedroomFurni[4].transmatrix = glm.translate(new mat4(1), new vec3(110, 0, 250));
                    bedroomFurni[5].transmatrix = glm.translate(new mat4(1), new vec3(285f, 35f, 105f));
                    #endregion
                    #region Collision BBs
                    setCollisionBoundingBox(new vec3(10, 0, 150), bedroomFurni[3]);
                    Models_3D.Add(bedroomFurni[3]);
                    setCollisionBoundingBox(new vec3(110, 0, 250), bedroomFurni[4]);
                    Models_3D.Add(bedroomFurni[4]);
                    setCollisionBoundingBox(new vec3(285f, 35f, 105f), bedroomFurni[5]);
                    Models_3D.Add(bedroomFurni[5]);
                    #endregion
                    #endregion
                    break;
                #endregion
                #region KITCHEN
                case skyboxType.KITCHEN:
                    #region FURNI
                    kitchenFurni = new List<Model3D>()
                    {
                        new Model3D(), //kitSet
                        new Model3D(), //dine
                        new Model3D(), //lamp
                    };
                    #region LoadFile
                    kitchenFurni[0].LoadFile(projectPath + "\\ModelFiles\\kitchen", texUnit_counter % 32, "kitSet.obj");
                    texUnit_counter++;
                    kitchenFurni[1].LoadFile(projectPath + "\\ModelFiles\\kitchen", texUnit_counter % 32, "dine.obj");
                    texUnit_counter++;
                    kitchenFurni[2].LoadFile(projectPath + "\\ModelFiles\\kitchen", texUnit_counter % 32, "lamp.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    kitchenFurni[0].scalematrix = glm.scale(new mat4(1), new vec3(7, 7, 7));
                    kitchenFurni[0].transmatrix = glm.translate(new mat4(1), new vec3(235, 0, 10));
                    kitchenFurni[1].transmatrix = glm.translate(new mat4(1), new vec3(250, 0, 250));
                    kitchenFurni[2].transmatrix = glm.translate(new mat4(1), new vec3(150, 300, 150));
                    #endregion
                    #region Collision BBs
                    setCollisionBoundingBox(new vec3(235, 0, 10), kitchenFurni[0]);
                    Models_3D.Add(kitchenFurni[0]);
                    setCollisionBoundingBox(new vec3(250, 0, 250), kitchenFurni[1]);
                    Models_3D.Add(kitchenFurni[1]);
                    setCollisionBoundingBox(new vec3(150, 300, 150), kitchenFurni[2]);
                    Models_3D.Add(kitchenFurni[2]);
                    #endregion
                    #endregion
                    break;
                #endregion
                #region BATHROOM
                case skyboxType.BATHROOM:
                    #region FURNI
                    bathroomFurni = new List<Model3D>()
                    {
                        new Model3D(), //seat
                        new Model3D(), //sink
                        new Model3D(), //oldbathtub
                        new Model3D(), //lamp
                    };
                    #region LoadFile
                    bathroomFurni[0].LoadFile(projectPath + "\\ModelFiles\\bathroom", texUnit_counter % 32, "seat.obj");
                    texUnit_counter++;
                    bathroomFurni[1].LoadFile(projectPath + "\\ModelFiles\\bathroom", texUnit_counter % 32, "sink.obj");
                    texUnit_counter++;
                    bathroomFurni[2].LoadFile(projectPath + "\\ModelFiles\\bathroom", texUnit_counter % 32, "oldbathtub.obj");
                    texUnit_counter++;
                    bathroomFurni[3].LoadFile(projectPath + "\\ModelFiles\\bathroom", texUnit_counter % 32, "lamp.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    bathroomFurni[0].transmatrix = glm.translate(new mat4(1), new vec3(30, 0, 15));
                    bathroomFurni[1].transmatrix = glm.translate(new mat4(1), new vec3(110, 0, 15));
                    bathroomFurni[2].transmatrix = glm.translate(new mat4(1), new vec3(170, 0, 142));
                    bathroomFurni[3].transmatrix = glm.translate(new mat4(1), new vec3(100, 200, 100));
                    #endregion
                    #region Collision BBs
                    setCollisionBoundingBox(new vec3(30, 0, 15), bathroomFurni[0]);
                    Models_3D.Add(bathroomFurni[0]);
                    setCollisionBoundingBox(new vec3(110, 0, 15), bathroomFurni[1]);
                    Models_3D.Add(bathroomFurni[1]);
                    setCollisionBoundingBox(new vec3(170, 0, 142), bathroomFurni[2]);
                    Models_3D.Add(bathroomFurni[2]);
                    setCollisionBoundingBox(new vec3(100, 200, 100), bathroomFurni[3]);
                    Models_3D.Add(bathroomFurni[3]);
                    #endregion
                    #endregion
                    break;
                #endregion
                #region CLOSET
                case skyboxType.CLOSET:
                    #region Furni
                    closetFurni = new List<Model3D>()
                    {
                        new Model3D(), //lamp
                    };
                    #region LoadFile
                    closetFurni[0].LoadFile(projectPath + "\\ModelFiles\\closet", texUnit_counter % 32, "lamp.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    closetFurni[0].transmatrix = glm.translate(new mat4(1), new vec3(100, 200, 100));
                    #endregion
                    #region Collision BBs
                    setCollisionBoundingBox(new vec3(100, 200, 100), closetFurni[0]);
                    Models_3D.Add(closetFurni[0]);
                    #endregion
                    #endregion
                    break;
                #endregion
                #region BASEMENT
                case skyboxType.BASEMENT:
                    break;
                    #endregion
            }
            PrevoiuslyLoaded[(int)SKYBOX] = true;
        }
        #endregion

        public void Flush_Existing_IOBJ()
        {
            #region Doors
            for (int i = 0; i < doors.Count; i++)
                doors[i].isDrawn = false;
            #endregion

            #region Garbages
            for (int i = 0; i < numOfGarbages; i++)
                garbages[i].isDrawn = false;
            #endregion
        }

        public override void Draw() {
            LoadSkyboxModels();

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();

            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            //send the value of camera position (eye position)
            Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());

            #region Screens Drawing [not implemented]
            //Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferID);
            //Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, modelmatrix.to_array());
            //Gl.glEnableVertexAttribArray(0);
            //Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), IntPtr.Zero);
            //Gl.glEnableVertexAttribArray(1);
            //Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            //Gl.glEnableVertexAttribArray(2);
            //Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(6 * sizeof(float)));
            //Gl.glEnableVertexAttribArray(3);
            //Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(8 * sizeof(float)));
            //tex1.Bind();
            //Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            #endregion

            skyboxes[currentSkyboxID].Draw(transID);
            if (PrevoiuslyLoaded[currentSkyboxID]) {
                try {
                    SendLightData();

                    #region Skybox Forrest
                    if (currentSkyboxID == 0) {
                        g_down.Draw(transID);

                        #region Garbages Models
                        for (int i = 0; i < numOfGarbages; i++)
                            garbages[i].Draw(transID);
                        #endregion

                        #region 3D Models drawing
                        watchtower.Draw(transID);
                        house_obj.Draw(transID);
                        car.Draw(transID);
                        phone.Draw(transID);
                        doors[0].Draw(transID);

                        #region Trees Models
                        Gl.glEnable(Gl.GL_BLEND);
                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                        for (int i = 0; i < num_trees; i++)
                            Trees[i].Draw(transID);
                        Gl.glDisable(Gl.GL_BLEND);
                        #endregion

                        #region Lights Models
                        for (int i = 0; i < num_lights; i++)
                            Lights[i].Draw(transID);
                        #endregion

                        #region Grass Models
                        for (int i = 0; i < num_grass; i++)
                            Grass[i].Draw(transID);
                        #endregion

                        #endregion
                    }
                    #endregion
                    #region Living Room
                    if (currentSkyboxID == 1) {
                        for (int i = 0; i < 3; i++)
                            doors[i].Draw(transID);
                        for (int i = 0; i < livingFurni.Count; i++)
                            livingFurni[i].Draw(transID);
                        radio.Draw(transID);
                    }
                    #endregion
                    #region Bedroom
                    if (currentSkyboxID == 2) {
                        doors[1].Draw(transID);
                        doors[3].Draw(transID);
                        doors[4].Draw(transID);
                        for (int i = 0; i < bedroomFurni.Count; i++)
                            bedroomFurni[i].Draw(transID);
                    }
                    #endregion
                    #region Kitchen
                    if (currentSkyboxID == 3) {
                        doors[2].Draw(transID);
                        for (int i = 0; i < kitchenFurni.Count; i++)
                            kitchenFurni[i].Draw(transID);
                    }
                    #endregion
                    #region Bathroom
                    if (currentSkyboxID == 4) {
                        doors[3].Draw(transID);
                        for (int i = 0; i < bathroomFurni.Count; i++)
                            bathroomFurni[i].Draw(transID);
                    }
                    #endregion
                    #region Closet
                    if (currentSkyboxID == 5) {
                        doors[4].Draw(transID);
                        for (int i = 0; i < closetFurni.Count; i++)
                            closetFurni[i].Draw(transID);
                    }
                    #endregion
                    #region Basement
                    if (currentSkyboxID == 6) {

                    }
                    #endregion
                }
                catch { }
            }

        }

        public void Update(float deltaTime) {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
        }

        public void SendLightData() {
            Gl.glUniform3fv(LightPositionID, 1, LightPosition[currentSkyboxID].to_array());
            Gl.glUniform3fv(AmbientLightID, 1, AmbientLight[currentSkyboxID].to_array());
            Gl.glUniform3fv(DataID, 1, LightData[currentSkyboxID].to_array());
            Gl.glUniform3fv(IntensityID, 1, LightIntensity[currentSkyboxID].to_array());
        }

        public override void CleanUp()
        {
            sh.DestroyShader();
            sh2D.DestroyShader();
        }

        public modelType InteractiveCheck()
        {
            float DistanceX, DistanceY, DistanceZ;

            for (int i = 0; i < Models_Interactive.Count; i++) {
                if (!Models_Interactive[i].isDrawn)
                    continue;
                DistanceX = Math.Abs(cam.mPosition.x - Models_Interactive[i].position.x);
                DistanceY = Math.Abs(cam.mPosition.y - Models_Interactive[i].position.y);
                DistanceZ = Math.Abs(cam.mPosition.z - Models_Interactive[i].position.z);
                if (DistanceX < Models_Interactive[i].interactionBoundingBox.x / 2
                 && DistanceY < Models_Interactive[i].interactionBoundingBox.y / 2
                 && DistanceZ < Models_Interactive[i].interactionBoundingBox.z / 2) {
                    Models_Interactive[i].Event();
                    return Models_Interactive[i].type;
                }
            }
            return modelType.NULL;

            #region Old Code

            //#region Doors
            //for (int i = 0; i < doors.Count; i++) {
            //    if (!doors[i].obj.isDrawn)
            //        continue;
            //    DistanceX = Math.Abs(cam.mPosition.x - doors[i].position.x);
            //    DistanceY = Math.Abs(cam.mPosition.y - doors[i].position.y);
            //    DistanceZ = Math.Abs(cam.mPosition.z - doors[i].position.z);
            //    if (DistanceX < doors[i].interactionBB.x / 2
            //     && DistanceY < doors[i].interactionBB.y / 2
            //     && DistanceZ < doors[i].interactionBB.z / 2) {
            //        doors[i].Event();
            //        return modelType.DOOR;
            //    }
            //}
            //#endregion

            //#region Garbages
            //for (int i = 0; i < numOfGarbages; i++) {
            //    if (!garbages[i].obj.isDrawn)
            //        continue;
            //    DistanceX = Math.Abs(cam.mPosition.x - garbages[i].position.x);
            //    DistanceY = Math.Abs(cam.mPosition.y - garbages[i].position.y);
            //    DistanceZ = Math.Abs(cam.mPosition.z - garbages[i].position.z);
            //    if (DistanceX < garbages[i].interactionBB.x / 2
            //      && DistanceY < garbages[i].interactionBB.y / 2
            //      && DistanceZ < garbages[i].interactionBB.z / 2) {
            //        garbages[i].Event();
            //        return modelType.GARBAGE;
            //    }
            //}
            //#endregion

            //#region radio check
            //DistanceX = Math.Abs(cam.mPosition.x - radio.position.x);
            //DistanceY = Math.Abs(cam.mPosition.y - radio.position.y);
            //DistanceZ = Math.Abs(cam.mPosition.z - radio.position.z);
            //if (DistanceX < radio.interactionBB.x / 2
            //  && DistanceY < radio.interactionBB.y / 2
            //  && DistanceZ < radio.interactionBB.z / 2) {
            //    radio.Event();
            //    return modelType.RADIO;
            //}
            //#endregion

            //return modelType.NULL;
            #endregion
        }

        public void setCollisionBoundingBox(vec3 objPosition, Model3D modelObj)
        {
            float minWidth  = float.MaxValue, maxWidth  = float.MinValue;
            float minHeight = float.MaxValue, maxHeight = float.MinValue;
            float minDepth  = float.MaxValue, maxDepth  = float.MinValue;
            foreach (Model m in modelObj.meshes)
                foreach (vec3 v in m.vertices)
                {
                    minWidth = Math.Min(minWidth, v.x);
                    maxWidth = Math.Max(maxWidth, v.x);

                    minHeight = Math.Min(minHeight, v.y);
                    maxHeight = Math.Max(maxHeight, v.y);

                    minDepth = Math.Min(minDepth, v.z);
                    maxDepth = Math.Max(maxDepth, v.z);
                }
            modelObj.collisionBoundingBox.x = (maxWidth - minWidth);
            modelObj.collisionBoundingBox.y = (maxHeight - minHeight);
            modelObj.collisionBoundingBox.z = (maxDepth - minDepth);
            modelObj.position = objPosition;
        }
    }
}
