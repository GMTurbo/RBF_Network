namespace RBF_Network
{
    partial class RBFBframe
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
            this.loadingIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.loadingIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // loadingIcon
            // 
            this.loadingIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingIcon.Image = global::RBF_Network.Properties.Resources.loading;
            this.loadingIcon.Location = new System.Drawing.Point(0, 0);
            this.loadingIcon.Name = "loadingIcon";
            this.loadingIcon.Size = new System.Drawing.Size(449, 248);
            this.loadingIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.loadingIcon.TabIndex = 0;
            this.loadingIcon.TabStop = false;
            // 
            // RBFBframe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 248);
            this.Controls.Add(this.loadingIcon);
            this.Name = "RBFBframe";
            this.Text = "RBFBframe";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RBFBframe_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RBFBframe_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RBFBframe_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.loadingIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox loadingIcon;
    }
}