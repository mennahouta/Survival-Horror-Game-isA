using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Graphics {
    public partial class GraphicsForm : Form {
        public static bool gameStarted = false;
        public static int sanityCount = 5;
        public static List<int> skybox_waitingTimes = new List<int> { 10, 30, 30, 30, 30, 30, 30 };

        string currentScreen;
        StartScreen startScreen = new StartScreen();
        LoadScreen loadScreen = new LoadScreen();

        Renderer renderer = new Renderer();
        Thread MainLoopThread;

        List<String> garbageMessages = new List<string>(){
            "You have found the key, I'm impressed!",
            "Garbage. Garbage everywhere.",
            "Nothing of use here.",
            "People can be very messy.",
            "You're going to be very smelly."
        };

        MP3_player stepsOut = new MP3_player();
        MP3_player stepsIn = new MP3_player();
        bool stepsOn = false;

        float deltaTime;
        public GraphicsForm(string currScreen)
        {
            currentScreen = currScreen;

            InitializeComponent();

            #region Full-Screen
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = GetScreen().Size;
            simpleOpenGlControl1.Size = this.Size;
            #endregion
            simpleOpenGlControl1.InitializeContexts();

            Cursor.Hide();

            MoveCursor();

            initialize();
            deltaTime = 0.005f;
            #region Threads
            if (Thread.CurrentThread.Name == "")
                Thread.CurrentThread.Name = "Main";

            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Name = "MainLoop Thread";
            MainLoopThread.Start();
            #endregion
        }

        void initialize() {
            if (currentScreen == "start")
                startScreen.Initialize();
            if (currentScreen == "load")
                loadScreen.Initialize();
            if (currentScreen == "renderer")
            {
                //loadScreen.Initialize();
                renderer.Initialize();
                stepsOut.open(Renderer.projectPath + @"\Sounds\forest_footsteps.mp3");
                stepsIn.open(Renderer.projectPath + @"\Sounds\footsteps_house.mp3");
            }
        }

        void MainLoop() {
            if (currentScreen == "start")
                startScreen.Draw();
            while (true) {
                try {
                    //if (!Renderer.PrevoiuslyLoaded[Renderer.currentSkyboxID])
                    //    loadScreen.Draw();
                    renderer.Draw();
                    renderer.Update(deltaTime);
                }
                catch { }
                simpleOpenGlControl1.Refresh();
            }
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {
            try {
                if (currentScreen == "start")
                    startScreen.Draw();
                if (currentScreen == "load")
                    loadScreen.Draw();
                if (currentScreen == "renderer") {
                    //if (!Renderer.PrevoiuslyLoaded[Renderer.currentSkyboxID])
                    //    loadScreen.Draw();
                    renderer.Draw();
                    renderer.Update(deltaTime);
                }
            }
            catch { }
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)(32) || e.KeyChar == (char)(13))//space or enter 
            {
                if (currentScreen == "start")
                {
                    if (StartScreen.currentBtnIDX == 0) //Play
                    {
                        gameStarted = true;
                        MainLoopThread.Abort();
                        startScreen.CleanUp();
                        GraphicsForm rendererForm = new GraphicsForm("renderer");
                        this.Hide();
                        rendererForm.ShowDialog();
                        this.Close();
                    }
                    else //Exit
                    {
                        if (gameStarted)
                        {
                            //loadScreen.CleanUp();
                            renderer.CleanUp();
                        }
                        else
                            startScreen.CleanUp();
                        MainLoopThread.Abort();
                        this.Close();
                    }
                }
            }

            if (currentScreen == "renderer") {
                if (e.KeyChar == (char)(27))
                {
                    renderer.pauseScreen = !renderer.pauseScreen;
                }

                if(e.KeyChar == (char)(32))
                {
                    if (Renderer.lostSanity || renderer.pauseScreen)
                    {
                        gameStarted = false;
                        MainLoopThread.Abort();
                        renderer.CleanUp();
                        GraphicsForm startForm = new GraphicsForm("start");
                        this.Hide();
                        startForm.ShowDialog();
                        this.Close();
                    }
                    if (Renderer.text_shown)
                    {
                        Renderer.text_shown = false;
                    }
                }

                float speed = 3f;
                if (Renderer.currentSkyboxID == 0)
                    speed = 1.5f;
                if (e.KeyChar == 'a')
                    Renderer.cam.Strafe(-speed);
                if (e.KeyChar == 'd')
                    Renderer.cam.Strafe(speed);
                if (e.KeyChar == 's')
                    Renderer.cam.Walk(-speed);
                if (e.KeyChar == 'w')
                    Renderer.cam.Walk(speed);
                if (Camera.Move && e.KeyChar != 'e')
                    WalkSound();
                //if (e.KeyChar == 'z')
                //    Renderer.cam.Fly(-speed);
                //if (e.KeyChar == 'c')
                //    Renderer.cam.Fly(speed);
                if (e.KeyChar == 'e') {
                    try {
                        modelType currentInteractionType = renderer.InteractiveCheck();

                        #region Garbage interaction
                        if (currentInteractionType == modelType.GARBAGE) {
                            //if (Renderer.playerHasKey)
                            //    MessageBox.Show(garbageMessages[0]);
                            //else
                            //{
                            //    Random random = new Random();
                            //    MessageBox.Show(garbageMessages[random.Next(1, garbageMessages.Count - 1)]);
                            //}
                        }
                        #endregion

                    }
                    catch { }
                }
                    
            }
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e) {
            if (currentScreen == "renderer") {
                float speed = 0.05f;
                float delta = e.X - prevX;
                if (delta > 2)
                    Renderer.cam.Yaw(-speed);
                else if (delta < -2)
                    Renderer.cam.Yaw(speed);

                delta = e.Y - prevY;
                if (delta > 2)
                    Renderer.cam.Pitch(-speed);
                else if (delta < -2)
                    Renderer.cam.Pitch(speed);

                MoveCursor();
            }
        }

        private void MoveCursor() {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width / 2 + p.X, simpleOpenGlControl1.Size.Height / 2 + p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X + simpleOpenGlControl1.Size.Width / 2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }

        private void simpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.X.ToString() + ", " + e.Y.ToString());
            if (currentScreen == "start") {
                if (startScreen.startButtonClicked(e.X, e.Y)) {
                    gameStarted = true;
                    this.Hide();
                    MainLoopThread.Abort();
                    startScreen.CleanUp();
                    GraphicsForm rendererForm = new GraphicsForm("renderer");
                    rendererForm.ShowDialog();
                    this.Close();
                }
            }
        }

        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MainLoopThread.Abort();
            }
            catch { }
        }

        private void simpleOpenGlControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                StartScreen.currentBtnIDX++;
                StartScreen.currentBtnIDX %= StartScreen.buttonsCount;
            }

            if (e.KeyCode == Keys.Up)
            {
                StartScreen.currentBtnIDX--;
                StartScreen.currentBtnIDX %= StartScreen.buttonsCount;
                StartScreen.currentBtnIDX += StartScreen.buttonsCount;
                StartScreen.currentBtnIDX %= StartScreen.buttonsCount;
            }
        }

        public void WalkSound()
        {
            if (!stepsOn)
            {
                stepsOn = true;
                if (Renderer.currentSkyboxID == 0)
                    stepsOut.PlayLooping();
                else
                {
                    stepsOut.stop();
                    stepsIn.PlayLooping();
                }
            }
        }

        private void simpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
        {
            stepsOn = false;
            //if (Renderer.currentSkyboxID == 0)
            stepsOut.stop();
            //else
            stepsIn.stop();
        }

        public Rectangle GetScreen() {
            return Screen.FromControl(this).Bounds;
        }
    }
}
