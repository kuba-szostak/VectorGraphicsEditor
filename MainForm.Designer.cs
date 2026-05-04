namespace VectorGraphicsEditor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnLine = new System.Windows.Forms.ToolStripButton();
            this.btnCircle = new System.Windows.Forms.ToolStripButton();
            this.btnPolygon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnColor = new System.Windows.Forms.ToolStripButton();
            this.lblThickness = new System.Windows.Forms.ToolStripLabel();
            this.txtThickness = new System.Windows.Forms.ToolStripTextBox();
            this.btnAntiAlias = new System.Windows.Forms.ToolStripButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMenuItem,
            this.saveMenuItem,
            this.clearMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // loadMenuItem
            // 
            this.loadMenuItem.Name = "loadMenuItem";
            this.loadMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadMenuItem.Text = "Load";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveMenuItem.Text = "Save";
            // 
            // clearMenuItem
            // 
            this.clearMenuItem.Name = "clearMenuItem";
            this.clearMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearMenuItem.Text = "Clear";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelect,
            this.btnLine,
            this.btnCircle,
            this.btnPolygon,
            this.toolStripSeparator1,
            this.btnColor,
            this.lblThickness,
            this.txtThickness,
            this.btnAntiAlias});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSelect
            // 
            this.btnSelect.CheckOnClick = true;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(42, 22);
            this.btnSelect.Text = "Select";
            // 
            // btnLine
            // 
            this.btnLine.CheckOnClick = true;
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(33, 22);
            this.btnLine.Text = "Line";
            // 
            // btnCircle
            // 
            this.btnCircle.CheckOnClick = true;
            this.btnCircle.Name = "btnCircle";
            this.btnCircle.Size = new System.Drawing.Size(41, 22);
            this.btnCircle.Text = "Circle";
            // 
            // btnPolygon
            // 
            this.btnPolygon.CheckOnClick = true;
            this.btnPolygon.Name = "btnPolygon";
            this.btnPolygon.Size = new System.Drawing.Size(54, 22);
            this.btnPolygon.Text = "Polygon";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.Black;
            this.btnColor.ForeColor = System.Drawing.Color.White;
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(40, 22);
            this.btnColor.Text = "Color";
            // 
            // lblThickness
            // 
            this.lblThickness.Name = "lblThickness";
            this.lblThickness.Size = new System.Drawing.Size(62, 22);
            this.lblThickness.Text = "Thickness:";
            // 
            // txtThickness
            // 
            this.txtThickness.Name = "txtThickness";
            this.txtThickness.Size = new System.Drawing.Size(30, 25);
            this.txtThickness.Text = "1";
            // 
            // btnAntiAlias
            // 
            this.btnAntiAlias.CheckOnClick = true;
            this.btnAntiAlias.Name = "btnAntiAlias";
            this.btnAntiAlias.Size = new System.Drawing.Size(26, 22);
            this.btnAntiAlias.Text = "AA";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 49);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 401);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Vector Graphics Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem loadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSelect;
        private System.Windows.Forms.ToolStripButton btnLine;
        private System.Windows.Forms.ToolStripButton btnCircle;
        private System.Windows.Forms.ToolStripButton btnPolygon;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnColor;
        private System.Windows.Forms.ToolStripLabel lblThickness;
        private System.Windows.Forms.ToolStripTextBox txtThickness;
        private System.Windows.Forms.ToolStripButton btnAntiAlias;
        private System.Windows.Forms.PictureBox pictureBox1;

        #endregion
    }
}
