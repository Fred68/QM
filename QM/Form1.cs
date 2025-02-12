using NcForms;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel.Design;

namespace QM
{

	/// <summary>
	/// Main form, derivato da NcForm
	/// </summary>
	public partial class Form1 : NcForms.NcForm
	{
		
		List<string> linee = new List<string>();
		CFG cfg;

		/// <summary>
		/// CTOR con parametri
		/// </summary>
		/// <param name="style"></param>
		/// <param name="color"></param>
		/// <param name="cfg"></param>
		public Form1(NcFormStyle style,NcFormColor color, CFG cfg) : base(style,color)
		{
			InitializeComponent();
			this.cfg = cfg;
			this.AskClose = !cfg.FastQuit;
			this.Opacity = cfg.Opacity;
			this.Title = cfg.Titolo;
			
		}

		/// <summary>
		/// On Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender,EventArgs e)
		{
			ReadCommandFile();

			menu.Location = new Point(0,this.UpperBarHeight);
			menu.Items.Clear();

			ToolStripMenuItem x = new ToolStripMenuItem("pippo");
			ToolStripMenuItem y = new ToolStripMenuItem("pluto");
			menu.Items.Add(x);
			menu.Items.Add(y);
			//ToolStripItem pippo = menu.Items.Add("pippo",null,null);
			//ToolStripItem pluto = menu.Items.Add("pluto",null,null);
			//ToolStripItem qui = new ToolStripItem(new me

		}

		/// <summary>
		/// Legge il file dei comandi
		/// </summary>
		void ReadCommandFile()
		{
			try
			{
				string line;
				StreamReader sr = new StreamReader(cfg.Comandi);
				while ((line = sr.ReadLine()) != null)
				{
					if(line.Length>1)
						linee.Add(line);
				}
				sr.Close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				#if DEBUG
				MessageBox.Show($"Lette {linee.Count} linee");
				#endif
			}

		}
	}
}
