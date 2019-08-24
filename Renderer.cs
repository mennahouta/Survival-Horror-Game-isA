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

    class Renderer: ScreenClass{
        //public static string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

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

        public static List<Boolean> PrevoiuslyLoaded;
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

        InteractiveModel radio, phone, gun, paper1, paper2;

        #region GARBAGE
        public static List<InteractiveModel> garbages;
        int numOfGarbages;
        public static int key_garbageID;
        public static bool playerHasKey = false;
        #endregion

        int texUnit_counter = 0;

        #region Trees Models
        int num_trees = 60;
        Model3D[] Trees;
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
        public Boolean pauseScreen = false;
        Model pauseScreen_obj;
        #region Text
        Model obj2D;
        List<Texture> chapter;
        int modelMat2D_ID;
        Model lbl_pressSpace;
        public static bool text_shown = false;
        #endregion
        #region Sanity Data Declaration
        Model sanityBar;
        int fadingValueID, isFadingID;
        float fadingValue;
        public static Boolean lostSanity = false;
        #endregion
        #region GameOver
        Model gameover_background, lbl_gameover;
        #endregion
        #endregion

        public override void Initialize()
        {   
            #region Shaders intialization
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            sh2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            #endregion
            
            cam = new Camera();
            cam.Reset(180, 30, 800, 20, 20, 150, 0, 1, 0);

            #region 2D Models intializtion
            modelMat2D_ID = Gl.glGetUniformLocation(sh2D.ID, "model");
            fadingValueID = Gl.glGetUniformLocation(sh2D.ID, "fadingValue");
            isFadingID = Gl.glGetUniformLocation(sh2D.ID, "isFading");
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
                glm.scale(new mat4(1), new vec3(0.3f, 0.8f, 1)), //need to be updated with every skybox FML
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
            #region lbl_pressSpace
            lbl_pressSpace = new Model();
            lbl_pressSpace.vertices.Add(new vec3(-1, 1, 0));
            lbl_pressSpace.vertices.Add(new vec3(1, -1, 0));
            lbl_pressSpace.vertices.Add(new vec3(-1, -1, 0));
            lbl_pressSpace.vertices.Add(new vec3(1, 1, 0));
            lbl_pressSpace.vertices.Add(new vec3(-1, 1, 0));
            lbl_pressSpace.vertices.Add(new vec3(1, -1, 0));

            lbl_pressSpace.uvCoordinates.Add(new vec2(0, 0));
            lbl_pressSpace.uvCoordinates.Add(new vec2(1, 1));
            lbl_pressSpace.uvCoordinates.Add(new vec2(0, 1));
            lbl_pressSpace.uvCoordinates.Add(new vec2(1, 0));
            lbl_pressSpace.uvCoordinates.Add(new vec2(0, 0));
            lbl_pressSpace.uvCoordinates.Add(new vec2(1, 1));

            lbl_pressSpace.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>{
                glm.scale(new mat4(1), new vec3(0.75f, 0.1f, 1)), //need to be updated with every skybox FML
                glm.translate(new mat4(1), new vec3(0, -.9f, 0)) }
            );

            lbl_pressSpace.texture = new Texture(projectPath + "\\Textures\\lbl_pressSpace.png", (texUnit_counter++) % 32, false); ;
            lbl_pressSpace.Initialize();
            #endregion
            #region Sanity Bar
            sanityBar = new Model();
            sanityBar.vertices.Add(new vec3(-1, 1, 0));
            sanityBar.vertices.Add(new vec3(1, -1, 0));
            sanityBar.vertices.Add(new vec3(-1, -1, 0));
            sanityBar.vertices.Add(new vec3(1, 1, 0));
            sanityBar.vertices.Add(new vec3(-1, 1, 0));
            sanityBar.vertices.Add(new vec3(1, -1, 0));

            sanityBar.uvCoordinates.Add(new vec2(0, 0));
            sanityBar.uvCoordinates.Add(new vec2(1, 1));
            sanityBar.uvCoordinates.Add(new vec2(0, 1));
            sanityBar.uvCoordinates.Add(new vec2(1, 0));
            sanityBar.uvCoordinates.Add(new vec2(0, 0));
            sanityBar.uvCoordinates.Add(new vec2(1, 1));

            sanityBar.texture = new Texture(projectPath + "\\Textures\\brainicon.png", 9);

            sanityBar.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.15f,0.25f, 1)),
                glm.translate(new mat4(1),new vec3(0.85f,0.8f,0))
            });

            sanityBar.Initialize();

            fadingValue = 0;
            #endregion
            #region Game Over
            gameover_background = new Model();
            gameover_background.vertices.Add(new vec3(-1, 1, 0));
            gameover_background.vertices.Add(new vec3(1, -1, 0));
            gameover_background.vertices.Add(new vec3(-1, -1, 0));
            gameover_background.vertices.Add(new vec3(1, 1, 0));
            gameover_background.vertices.Add(new vec3(-1, 1, 0));
            gameover_background.vertices.Add(new vec3(1, -1, 0));

            gameover_background.uvCoordinates.Add(new vec2(0, 0));
            gameover_background.uvCoordinates.Add(new vec2(1, 1));
            gameover_background.uvCoordinates.Add(new vec2(0, 1));
            gameover_background.uvCoordinates.Add(new vec2(1, 0));
            gameover_background.uvCoordinates.Add(new vec2(0, 0));
            gameover_background.uvCoordinates.Add(new vec2(1, 1));

            gameover_background.texture = new Texture(projectPath + "\\Textures\\grimmnight_dn.jpg", (texUnit_counter++) % 32, false); ;
            gameover_background.Initialize();

            lbl_gameover = new Model();
            lbl_gameover.vertices.Add(new vec3(-1, 1, 0));
            lbl_gameover.vertices.Add(new vec3(1, -1, 0));
            lbl_gameover.vertices.Add(new vec3(-1, -1, 0));
            lbl_gameover.vertices.Add(new vec3(1, 1, 0));
            lbl_gameover.vertices.Add(new vec3(-1, 1, 0));
            lbl_gameover.vertices.Add(new vec3(1, -1, 0));

            lbl_gameover.uvCoordinates.Add(new vec2(0, 0));
            lbl_gameover.uvCoordinates.Add(new vec2(1, 1));
            lbl_gameover.uvCoordinates.Add(new vec2(0, 1));
            lbl_gameover.uvCoordinates.Add(new vec2(1, 0));
            lbl_gameover.uvCoordinates.Add(new vec2(0, 0));
            lbl_gameover.uvCoordinates.Add(new vec2(1, 1));

            lbl_gameover.transformationMatrix = MathHelper.MultiplyMatrices(new List<mat4>{
                glm.scale(new mat4(1), new vec3(0.85f, 0.5f, 1)),
                glm.translate(new mat4(1), new vec3(0, 0, 0)) }
            );

            lbl_gameover.texture = new Texture(projectPath + "\\Textures\\gameover.png", (texUnit_counter++) % 32, false); ;
            lbl_gameover.Initialize();
            #endregion
            #region pause
            pauseScreen_obj = new Model();
            pauseScreen_obj.vertices.Add(new vec3(-1, 1, 0));
            pauseScreen_obj.vertices.Add(new vec3(1, -1, 0));
            pauseScreen_obj.vertices.Add(new vec3(-1, -1, 0));
            pauseScreen_obj.vertices.Add(new vec3(1, 1, 0));
            pauseScreen_obj.vertices.Add(new vec3(-1, 1, 0));
            pauseScreen_obj.vertices.Add(new vec3(1, -1, 0));

            pauseScreen_obj.uvCoordinates.Add(new vec2(0, 0));
            pauseScreen_obj.uvCoordinates.Add(new vec2(1, 1));
            pauseScreen_obj.uvCoordinates.Add(new vec2(0, 1));
            pauseScreen_obj.uvCoordinates.Add(new vec2(1, 0));
            pauseScreen_obj.uvCoordinates.Add(new vec2(0, 0));
            pauseScreen_obj.uvCoordinates.Add(new vec2(1, 1));

            pauseScreen_obj.texture = new Texture(projectPath + "\\Textures\\pause.jpg", (texUnit_counter++) % 32, false); ;
            pauseScreen_obj.Initialize();

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
            Renderer.PrevoiuslyLoaded = new List<Boolean>();
            for (int i = 0; i < (int)skyboxType.MAX_SKYBOXES; i++)
                Renderer.PrevoiuslyLoaded.Add(false);
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
                new InteractiveModel("door", "door", texUnit_counter%32, 15, modelType.DOOR, 0),       //0: Front door, from 0 <-> 1
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
            setCollisionBoundingBox(doors[0].position, doors[0].obj);
            scaleBoundingBox(new vec3(.5f, .5f, .5f), doors[0].obj);
            Models_Interactive.Add(doors[0]);
            for (int i = 1; i < 5; i++) {
                setCollisionBoundingBox(doors[i].position, doors[i].obj);
                scaleBoundingBox(new vec3(1f, 1.3f, 1f), doors[i].obj);
                Models_Interactive.Add(doors[i]);
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

        public void LoadSkyboxModels()
        {

            skyboxType SKYBOX = (skyboxType)currentSkyboxID;
            if (Renderer.PrevoiuslyLoaded[(int)SKYBOX])
                return;
            Random random = new Random();
            switch (SKYBOX)
            {
                #region FOREST
                case skyboxType.FOREST:
                    #region Garbage
                    garbages = new List<InteractiveModel>();
                    List<vec3> garbagePos = new List<vec3>{
                        new vec3(580, 0, 450),
                        new vec3(622, 0, 525),
                        new vec3(622, 0, 575),
                        new vec3(700, 0, 630),
                        new vec3(750, 0, 530),
                    };
                    numOfGarbages = garbagePos.Count;
                    for (int i = 0; i < numOfGarbages; i++) {
                        garbages.Add(new InteractiveModel("garbage_bag", "garbage_bag", texUnit_counter % 32, 5, modelType.GARBAGE, i));

                        garbages[i].Scale(.35f, .35f, .35f);
                        float x = garbagePos[i].x;
                        float y = garbagePos[i].y;
                        float z = garbagePos[i].z;
                        garbages[i].Translate(x, y, z);
                        setCollisionBoundingBox(new vec3(x, y, z), garbages[i].obj);
                        scaleBoundingBox(new vec3(.35f, .35f, .35f), garbages[i].obj);
                        Models_Interactive.Add(garbages[i]);
                    }
                    texUnit_counter += 1;
                    key_garbageID = random.Next(0, numOfGarbages - 1);
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
                    scaleBoundingBox(new vec3(20, 30, 20), watchtower);
                    Models_3D.Add(watchtower);
                    #endregion
                    #region House
                    house_obj = new Model3D();
                    house_obj.LoadFile(projectPath + "\\ModelFiles\\house_obj", 7, "house-low-rus-obj.obj");
                    house_obj.scalematrix = glm.scale(new mat4(1), new vec3(.5f, .5f, .5f));
                    house_obj.transmatrix = glm.translate(new mat4(1), new vec3(700, 0, 500));
                    setCollisionBoundingBox(new vec3(690, 0, 500), house_obj);
                    scaleBoundingBox(new vec3(.47f, .5f, .5f), house_obj);
                    Models_3D.Add(house_obj);
                    #endregion
                    #region Phone Model
                    phone = new InteractiveModel("phone", "iPhone 6", texUnit_counter % 32, 200, modelType.TEXT, 0);
                    texUnit_counter++;
                    phone.Scale(0.4f, 0.4f, 0.4f);
                    phone.Translate(170, 2, 800);
                    setCollisionBoundingBox(new vec3(170, 2, 800), phone.obj);
                    scaleBoundingBox(new vec3(0, 0, 0), phone.obj);
                    Models_Interactive.Add(phone);
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
                    Trees = new Model3D[num_trees];
                    List<vec4> TreesRegions = new List<vec4>()
                    {
                        //xmin, xmax, zmin, zmax
                        new vec4(15, 160, 15, 980),
                        new vec4(160, 980, 15, 320),
                        new vec4(780, 980, 320, 980),
                        new vec4(320, 780, 675, 980),
                    };
                    for (int i = 0; i < num_trees; i++) {
                        Trees[i] = new Model3D();
                        Trees[i].LoadFile(projectPath + "\\ModelFiles\\tree", 3, "Tree.obj");
                        Trees[i].scalematrix = glm.scale(new mat4(1), new vec3(13, 30, 10));
                        int idxRegion;
                        if (i < 15)
                            idxRegion = 0;
                        else if (i < 30)
                            idxRegion = 1;
                        else if (i < 45)
                            idxRegion = 2;
                        else
                            idxRegion = 3;

                        int x = random.Next((int)(TreesRegions[idxRegion].x), (int)(TreesRegions[idxRegion].y));
                        int y = 0;
                        int z = random.Next((int)(TreesRegions[idxRegion].z), (int)(TreesRegions[idxRegion].w));
                        vec3 pos = new vec3(x, y, z);

                        Trees[i].transmatrix = glm.translate(new mat4(1), new vec3(x, y, z));
                        setCollisionBoundingBox(new vec3(x, y, z), Trees[i]);
                        scaleBoundingBox(new vec3(5, 30, 5), Trees[i]);
                        Models_3D.Add(Trees[i]);
                    }
                    #endregion
                    break;
                #endregion
                #region LIVING
                case skyboxType.LIVING:
                    #region FURNI
                    List<string> livinObjectsNames = new List<string>()
                    {
                        "sofa3", "rug", "tvset", "plant", "floor_lamp", "decoSet",
                    };
                    livingFurni = new List<Model3D>();
                    for (int i = 0; i < livinObjectsNames.Count; i++)
                        livingFurni.Add(new Model3D());
                    List<vec3> LivingFurniPos = new List<vec3>()
                    {
                        new vec3(210, 1, 160),
                        new vec3(210, 1, 107),
                        new vec3(225,  1, 16),
                        new vec3(270,  1, 20),
                        new vec3(180,  0, 15),
                        new vec3(15,  0, 150),
                    };
                    #region LoadFile
                    for (int i = 0; i < livinObjectsNames.Count; i++)
                    {
                        livingFurni[i].LoadFile(projectPath + "\\ModelFiles\\LIVING ROOM", texUnit_counter % 32, livinObjectsNames[i] + ".obj");
                        texUnit_counter++;
                    }
                    #endregion
                    #region Transformations
                    for (int i = 0; i < livingFurni.Count; i++)
                    {
                        livingFurni[i].transmatrix = glm.translate(new mat4(1), LivingFurniPos[i]);
                        if (i == 1) //rug, skip
                            continue;
                        setCollisionBoundingBox(LivingFurniPos[i], livingFurni[i]);
                        Models_3D.Add(livingFurni[i]);
                    }
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
                    List<string> bedroomFurniName = new List<string>()
                    {
                        "bed", "side", "closet", "chair", "lamp",
                    };
                    bedroomFurni = new List<Model3D>();
                    for (int i = 0; i < bedroomFurniName.Count; i++)
                        bedroomFurni.Add(new Model3D());
                    List<vec3> bedroomFurniPos = new List<vec3>()
                    {
                        new vec3(230,  0, 170),
                        new vec3(280,  0, 100),
                        new vec3(15 ,  0, 140),
                        new vec3(160,  0, 30),
                        new vec3(280, 45, 100),
                    };
                    #region LoadFile
                    for(int i=0; i < bedroomFurni.Count; i++)
                    {
                        bedroomFurni[i].LoadFile(projectPath + "\\ModelFiles\\bedroom", texUnit_counter % 32, bedroomFurniName[i] + ".obj");
                        texUnit_counter++;
                    }
                    #endregion
                    #region Transformations
                    for (int i = 0; i < bedroomFurni.Count; i++)
                    {
                        bedroomFurni[i].transmatrix = glm.translate(new mat4(1), bedroomFurniPos[i]);
                        setCollisionBoundingBox(bedroomFurniPos[i], bedroomFurni[i]);
                        Models_3D.Add(bedroomFurni[i]);
                    }
                    #endregion

                    #region Paper1
                    paper1 = new InteractiveModel("Folders", "messy_folder", texUnit_counter % 32, 30, modelType.TEXT, 1);
                    texUnit_counter++;
                    paper1.Translate(15, 77, 140);
                    setCollisionBoundingBox(new vec3(15, 77, 140), paper1.obj);
                    scaleBoundingBox(new vec3(0, 0, 0), paper1.obj);
                    Models_Interactive.Add(paper1);
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
                    scaleBoundingBox(new vec3(7.5f, 7, 7.4f), kitchenFurni[0]);
                    Models_3D.Add(kitchenFurni[0]);
                    setCollisionBoundingBox(new vec3(250, 0, 250), kitchenFurni[1]);
                    Models_3D.Add(kitchenFurni[1]);
                    setCollisionBoundingBox(new vec3(150, 300, 150), kitchenFurni[2]);
                    scaleBoundingBox(new vec3(0, 0, 0), kitchenFurni[2]);
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
                    scaleBoundingBox(new vec3(0, 0, 0), bathroomFurni[3]);
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
                        new Model3D(), //table
                    };
                    #region LoadFile
                    closetFurni[0].LoadFile(projectPath + "\\ModelFiles\\closet", texUnit_counter % 32, "lamp.obj");
                    texUnit_counter++;
                    closetFurni[1].LoadFile(projectPath + "\\ModelFiles\\closet", texUnit_counter % 32, "table.obj");
                    texUnit_counter++;
                    #endregion
                    #region Transformations
                    closetFurni[0].transmatrix = glm.translate(new mat4(1), new vec3(100, 200, 100));
                    setCollisionBoundingBox(new vec3(100, 200, 100), closetFurni[0]);
                    scaleBoundingBox(new vec3(0, 0, 0), closetFurni[0]);
                    Models_3D.Add(closetFurni[0]);
                    closetFurni[1].transmatrix = glm.translate(new mat4(1), new vec3(100, 0, 170));
                    setCollisionBoundingBox(new vec3(100, 0, 170), closetFurni[1]);
                    Models_3D.Add(closetFurni[1]);
                    #endregion
                    #endregion
                    #region Gun
                    gun = new InteractiveModel("closet", "gun", texUnit_counter, 10, modelType.GUN, 0);
                    gun.Translate(70, 60, 180);
                    setCollisionBoundingBox(new vec3(70, 60, 180), gun.obj);
                    Models_Interactive.Add(gun);
                    #endregion
                    #region Paper1
                    paper2 = new InteractiveModel("Folders", "messy_folder", texUnit_counter % 32, 40, modelType.TEXT, 1);
                    texUnit_counter++;
                    paper2.Translate(130, 58, 180);
                    setCollisionBoundingBox(new vec3(130, 58, 180), paper2.obj);
                    scaleBoundingBox(new vec3(0, 0, 0), paper2.obj);
                    Models_Interactive.Add(paper2);
                    #endregion
                    break;
                #endregion
                #region BASEMENT
                case skyboxType.BASEMENT:
                    break;
                    #endregion
            }
            Renderer.PrevoiuslyLoaded[(int)SKYBOX] = true;
        }

        public void Flush_Existing_OBJ()
        {
            for (int i = 0; i < Models_3D.Count; i++)
                Models_3D[i].isDrawn = false;

            for (int i = 0; i < Models_Interactive.Count; i++)
                Models_Interactive[i].obj.isDrawn = false;
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
            if (Renderer.PrevoiuslyLoaded[currentSkyboxID]) {
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
                        paper1.Draw(transID);
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
                        gun.Draw(transID);
                        paper2.Draw(transID);
                    }
                    #endregion
                    #region Basement
                    if (currentSkyboxID == 6) {

                    }
                    #endregion
                }
                catch { }
            }

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            sh2D.UseShader();
            Gl.glUniform1f(isFadingID, 0);
            Gl.glUniform1f(fadingValueID, fadingValue);
            #region 2D Drawing
            if(pauseScreen)
            {
                pauseScreen_obj.Draw(modelMat2D_ID);
            }

            if (text_shown)
            {
                if (currentSkyboxID == 0)
                {
                    obj2D.texture = chapter[0];
                    //apply transformtation
                }
                else if (currentSkyboxID == (int)(skyboxType.BEDROOM))
                {
                    obj2D.texture = chapter[1];
                }
                else if (currentSkyboxID == (int)(skyboxType.CLOSET))
                {
                    obj2D.texture = chapter[2];
                }
                obj2D.Draw(modelMat2D_ID);

                Gl.glEnable(Gl.GL_BLEND);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                lbl_pressSpace.Draw(modelMat2D_ID);
                Gl.glDisable(Gl.GL_BLEND);
            }
            #endregion
            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }

        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            sh2D.UseShader();
            #region Sanity Update and Check on Game Over
            if (!checkInLight())
            {
                //fadingValue += 0.00002f;
                fadingValue += 0.0002f;
                if (fadingValue >= 1)
                {
                    fadingValue = 1;
                    lostSanity = true;

                    gameover_background.Draw(modelMat2D_ID);
                    Gl.glEnable(Gl.GL_BLEND);
                    Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                    lbl_gameover.Draw(modelMat2D_ID);
                    lbl_pressSpace.Draw(modelMat2D_ID);
                    Gl.glDisable(Gl.GL_BLEND);
                }
            }
            Gl.glUniform1f(isFadingID, 1.0f);
            Gl.glUniform1f(fadingValueID, fadingValue);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            sanityBar.Draw(modelMat2D_ID);
            Gl.glDisable(Gl.GL_BLEND);
            #endregion
            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }

        public void SendLightData() {
            Gl.glUniform3fv(LightPositionID, 1, LightPosition[currentSkyboxID].to_array());
            Gl.glUniform3fv(AmbientLightID, 1, AmbientLight[currentSkyboxID].to_array());
            Gl.glUniform3fv(DataID, 1, LightData[currentSkyboxID].to_array());
            Gl.glUniform3fv(IntensityID, 1, LightIntensity[currentSkyboxID].to_array());
        }

        public Boolean checkInLight()
        {
            double dist = Math.Sqrt(Math.Pow(LightPosition[currentSkyboxID].x - cam.mPosition.x, 2) +
                                    Math.Pow(LightPosition[currentSkyboxID].y - cam.mPosition.y, 2) +
                                    Math.Pow(LightPosition[currentSkyboxID].z - cam.mPosition.z, 2));
            if (dist <= LightData[currentSkyboxID].x)
                return true;
            return false;
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
                if (!Models_Interactive[i].obj.isDrawn)
                    continue;
                DistanceX = Math.Abs(cam.mPosition.x - Models_Interactive[i].position.x);
                DistanceY = Math.Abs(cam.mPosition.y - Models_Interactive[i].position.y);
                DistanceZ = Math.Abs(cam.mPosition.z - Models_Interactive[i].position.z);
                if (DistanceX <= (cam.PersonBoundingBox.x + Models_Interactive[i].interactionBoundingBox.x) / 2
                 && DistanceY <= (cam.PersonBoundingBox.y + Models_Interactive[i].interactionBoundingBox.y) / 2
                 && DistanceZ <= (cam.PersonBoundingBox.z + Models_Interactive[i].interactionBoundingBox.z) / 2) {
                    if (Models_Interactive[i].type == modelType.DOOR)
                        Flush_Existing_OBJ();
                    Models_Interactive[i].Event();
                    if (Models_Interactive[i].type == modelType.DOOR)
                        Flush_Existing_OBJ();
                    return Models_Interactive[i].type;
                }
            }
            return modelType.NULL;
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

        public void scaleBoundingBox(vec3 scales, Model3D m)
        {
            m.collisionBoundingBox *= scales;
        }
    }
}
