namespace QM
{
    partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			menu = new MenuStrip();
			SuspendLayout();
			// 
			// menu
			// 
			menu.Dock = DockStyle.None;
			menu.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
			menu.Location = new Point(9,25);
			menu.Name = "menu";
			menu.Size = new Size(126,25);
			menu.TabIndex = 3;
			menu.Text = "menuStrip1";
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F,15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(199,284);
			Controls.Add(menu);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menu;
			Name = "Form1";
			ShowInTaskbar = false;
			Text = "Form1";
			Activated += Form1_ChangeActivation;
			Deactivate += Form1_ChangeActivation;
			Load += Form1_Load;
			Resize += Form1_Resize;
			Controls.SetChildIndex(menu,0);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menu;
		//private ToolStripMenuItem sadsdToolStripMenuItem;
		//private ToolStripMenuItem ssssToolStripMenuItem;
		//private ToolStripMenuItem sssToolStripMenuItem;
		//private ToolStripMenuItem ddToolStripMenuItem;
		//private ToolStripMenuItem ddddToolStripMenuItem;
		//private ToolStripMenuItem cccToolStripMenuItem;
		//private ToolStripMenuItem ccccToolStripMenuItem;
		//private ToolStripMenuItem dddToolStripMenuItem;
	}
}
