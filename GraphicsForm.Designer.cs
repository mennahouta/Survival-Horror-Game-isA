﻿namespace Graphics
{
    partial class GraphicsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.simpleOpenGlControl1 = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.SuspendLayout();
            // 
            // simpleOpenGlControl1
            // 
            this.simpleOpenGlControl1.AccumBits = ((byte)(0));
            this.simpleOpenGlControl1.AutoCheckErrors = false;
            this.simpleOpenGlControl1.AutoFinish = false;
            this.simpleOpenGlControl1.AutoMakeCurrent = true;
            this.simpleOpenGlControl1.AutoSwapBuffers = true;
            this.simpleOpenGlControl1.BackColor = System.Drawing.Color.Black;
            this.simpleOpenGlControl1.ColorBits = ((byte)(32));
            this.simpleOpenGlControl1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.simpleOpenGlControl1.DepthBits = ((byte)(16));
            this.simpleOpenGlControl1.Location = new System.Drawing.Point(0, 0);
            this.simpleOpenGlControl1.Margin = new System.Windows.Forms.Padding(2);
            this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
            this.simpleOpenGlControl1.Size = new System.Drawing.Size(574, 357);
            this.simpleOpenGlControl1.StencilBits = ((byte)(0));
            this.simpleOpenGlControl1.TabIndex = 0;
            this.simpleOpenGlControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.simpleOpenGlControl1_Paint);
            this.simpleOpenGlControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.simpleOpenGlControl1_KeyPress);
            this.simpleOpenGlControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.simpleOpenGlControl1_KeyUp);
            this.simpleOpenGlControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.simpleOpenGlControl1_MouseClick);
            this.simpleOpenGlControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.simpleOpenGlControl1_MouseMove);
            this.simpleOpenGlControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.simpleOpenGlControl1_PreviewKeyDown);
            // 
            // GraphicsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 357);
            this.Controls.Add(this.simpleOpenGlControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "GraphicsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Graphics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GraphicsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl simpleOpenGlControl1;
    }
}

