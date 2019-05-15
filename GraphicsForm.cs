using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Graphics {
    public partial class GraphicsForm : Form {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;

        List<String> garbageMessages = new List<string>(){
            "You have found the key, I'm impressed!",
            "Garbage. Garbage everywhere.",
            "Nothing of use here.",
            "People can be very messy.",
            "You're going to be very smelly."
        };

        float deltaTime;
        public GraphicsForm() {
            InitializeComponent();
            #region Full-Screen
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = GetScreen().Size;
            simpleOpenGlControl1.Size = this.Size;
            #endregion
            simpleOpenGlControl1.InitializeContexts();

            MoveCursor();

            initialize();
            deltaTime = 0.005f;
            #region Threads
            Thread.CurrentThread.Name = "Main";

            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Name = "MainLoop Thread";
            MainLoopThread.Start();
            #endregion
        }
        void initialize() {
            //InteractiveModel.radio_sound.open(Renderer.projectPath + "tst.mp3");
            renderer.Initialize();
        }
        void MainLoop() {
            while (true) {
                try {
                    renderer.Flush_Existing_IOBJ();
                    renderer.Draw();
                    renderer.Update(deltaTime);
                }
                catch { }
                simpleOpenGlControl1.Refresh();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e) {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e) {
            try {
                renderer.Draw();
                renderer.Update(deltaTime);
            }
            catch { }
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e) {
            //Exit Application
            if (e.KeyChar == (char)(27)) {
                renderer.CleanUp();
                MainLoopThread.Abort();
                this.Close();
            }

            float speed = 3f;
            if (e.KeyChar == 'a')
                Renderer.cam.Strafe(-speed);
            if (e.KeyChar == 'd')
                Renderer.cam.Strafe(speed);
            if (e.KeyChar == 's')
                Renderer.cam.Walk(-speed);
            if (e.KeyChar == 'w')
                Renderer.cam.Walk(speed);
            if (e.KeyChar == 'z')
                Renderer.cam.Fly(-speed);
            if (e.KeyChar == 'c')
                Renderer.cam.Fly(speed);
            if (e.KeyChar == 'e') {
                try {
                    modelType currentInteractionType = renderer.InteractiveCheck();

                    #region Garbage interaction
                    if (currentInteractionType == modelType.GARBAGE) {
                        if (Renderer.playerHasKey)
                            MessageBox.Show(garbageMessages[0]);
                        else {
                            Random random = new Random();
                            MessageBox.Show(garbageMessages[random.Next(1, garbageMessages.Count)]);
                        }
                    }
                    #endregion

                }
                catch { }
            }

        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e) {
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

        private void MoveCursor() {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width / 2 + p.X, simpleOpenGlControl1.Size.Height / 2 + p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X + simpleOpenGlControl1.Size.Width / 2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }

        public Rectangle GetScreen() {
            return Screen.FromControl(this).Bounds;
        }
    }
}
