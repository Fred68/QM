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
			menu = new MenuStrip();
			sadsdToolStripMenuItem = new ToolStripMenuItem();
			ssssToolStripMenuItem = new ToolStripMenuItem();
			sssToolStripMenuItem = new ToolStripMenuItem();
			ddToolStripMenuItem = new ToolStripMenuItem();
			ddddToolStripMenuItem = new ToolStripMenuItem();
			cccToolStripMenuItem = new ToolStripMenuItem();
			ccccToolStripMenuItem = new ToolStripMenuItem();
			dddToolStripMenuItem = new ToolStripMenuItem();
			menu.SuspendLayout();
			SuspendLayout();
			// 
			// menu
			// 
			menu.Dock = DockStyle.None;
			menu.Items.AddRange(new ToolStripItem[] { sadsdToolStripMenuItem,ccccToolStripMenuItem,dddToolStripMenuItem });
			menu.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
			menu.Location = new Point(9,25);
			menu.Name = "menu";
			menu.Size = new Size(55,63);
			menu.TabIndex = 3;
			menu.Text = "menuStrip1";
			// 
			// sadsdToolStripMenuItem
			// 
			sadsdToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ssssToolStripMenuItem,sssToolStripMenuItem,cccToolStripMenuItem });
			sadsdToolStripMenuItem.Name = "sadsdToolStripMenuItem";
			sadsdToolStripMenuItem.Size = new Size(48,19);
			sadsdToolStripMenuItem.Text = "sadsd";
			// 
			// ssssToolStripMenuItem
			// 
			ssssToolStripMenuItem.Name = "ssssToolStripMenuItem";
			ssssToolStripMenuItem.Size = new Size(94,22);
			ssssToolStripMenuItem.Text = "ssss";
			// 
			// sssToolStripMenuItem
			// 
			sssToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ddToolStripMenuItem,ddddToolStripMenuItem });
			sssToolStripMenuItem.Name = "sssToolStripMenuItem";
			sssToolStripMenuItem.Size = new Size(94,22);
			sssToolStripMenuItem.Text = "sss";
			// 
			// ddToolStripMenuItem
			// 
			ddToolStripMenuItem.Name = "ddToolStripMenuItem";
			ddToolStripMenuItem.Size = new Size(102,22);
			ddToolStripMenuItem.Text = "dd";
			// 
			// ddddToolStripMenuItem
			// 
			ddddToolStripMenuItem.Name = "ddddToolStripMenuItem";
			ddddToolStripMenuItem.Size = new Size(102,22);
			ddddToolStripMenuItem.Text = "dddd";
			// 
			// cccToolStripMenuItem
			// 
			cccToolStripMenuItem.Name = "cccToolStripMenuItem";
			cccToolStripMenuItem.Size = new Size(94,22);
			cccToolStripMenuItem.Text = "ccc";
			// 
			// ccccToolStripMenuItem
			// 
			ccccToolStripMenuItem.Name = "ccccToolStripMenuItem";
			ccccToolStripMenuItem.Size = new Size(48,19);
			ccccToolStripMenuItem.Text = "cccc";
			// 
			// dddToolStripMenuItem
			// 
			dddToolStripMenuItem.Name = "dddToolStripMenuItem";
			dddToolStripMenuItem.Size = new Size(48,19);
			dddToolStripMenuItem.Text = "ddd";
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F,15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(110,257);
			Controls.Add(menu);
			MainMenuStrip = menu;
			Name = "Form1";
			Text = "Form1";
			Load += Form1_Load;
			Controls.SetChildIndex(menu,0);
			menu.ResumeLayout(false);
			menu.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menu;
		private ToolStripMenuItem sadsdToolStripMenuItem;
		private ToolStripMenuItem ssssToolStripMenuItem;
		private ToolStripMenuItem sssToolStripMenuItem;
		private ToolStripMenuItem ddToolStripMenuItem;
		private ToolStripMenuItem ddddToolStripMenuItem;
		private ToolStripMenuItem cccToolStripMenuItem;
		private ToolStripMenuItem ccccToolStripMenuItem;
		private ToolStripMenuItem dddToolStripMenuItem;
	}
}
