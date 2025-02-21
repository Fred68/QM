using NcForms;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel.Design;
using System.Text;

namespace QM
{

	/// <summary>
	/// Main form, derivato da NcForm
	/// </summary>
	public partial class Form1 : NcForms.NcForm
	{
		CFG cfg;									// File di configurazione

		List<string> linee = new List<string>();	// Linee con i comandi
		int count;									// Contatore

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
			AnalizeLines();

			menu.Location = new Point(0,this.UpperBarHeight);
			menu.Items.Clear();

			ToolStripMenuItem x1 = new ToolStripMenuItem("Paperino");
			ToolStripMenuItem x2= new ToolStripMenuItem("zio Paperone");
			ToolStripMenuItem x3= new ToolStripMenuItem("Paperoga");
			ToolStripMenuItem y1 = new ToolStripMenuItem("Qui");
			ToolStripMenuItem y2 = new ToolStripMenuItem("Quo");
			ToolStripMenuItem y3 = new ToolStripMenuItem("Qua");

			ToolStripMenuItem z1 = new ToolStripMenuItem("1");
			ToolStripMenuItem z2 = new ToolStripMenuItem("2");
			ToolStripMenuItem z3 = new ToolStripMenuItem("2");

			menu.SuspendLayout();

			menu.Items.Add(x1);
			menu.Items.Add(x2);
			menu.Items.Add(x3);

			List<ToolStripMenuItem> itm = new List<ToolStripMenuItem>();

			itm.Add(y1);
			itm.Add(y2);
			itm.Add(y3);
			x1.DropDownItems.AddRange(itm.ToArray());

			itm.Clear();
			itm.Add(z1);
			itm.Add(z2);
			itm.Add(z3);

			y2.DropDownItems.AddRange(itm.ToArray());
			//menu.Items.Add(x);
			//menu.Items.Add(y);
			//ToolStripItem z = new ToolStripMenuItem();
			//z.
			//menu.Items.Add(z);
			//x.DropDownItems.AddRange(new ToolStripItem("Qui"));

			//ToolStripItem pippo = menu.Items.Add("pippo",null,null);
			//ToolStripItem pluto = menu.Items.Add("pluto",null,null);
			//ToolStripItem qui = new ToolStripItem(new me
			itm.Clear();
			menu.ResumeLayout(true);


		}

		/// <summary>
		/// Legge il file dei comandi
		/// Rimuove i commenti, scarta le linee non valide
		/// </summary>
		void ReadCommandFile()
		{
			try
			{
				string? line;
				StreamReader sr = new StreamReader(cfg.Comandi);
				while ((line = sr.ReadLine()) != null)
				{
					if( (line.Length > 0) && (!line.StartsWith(cfg.CHR_Commento)))
					{
						int indx;
						if((indx = line.IndexOf(cfg.CHR_Commento)) != -1)
						{
							line = line.Substring(0, indx);							
						}

						if( (line.IndexOf(cfg.Sep)!=-1) || (line.IndexOf(cfg.MnuIni)!=-1) || (line.IndexOf(cfg.MnuFin)!=-1))
						{
							//line = line.Replace("\t","    ");
							linee.Add(line.Trim());
						}
					}
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
	
		void AnalizeLines()
		{
			int il;
			int prof = 0;
			StringBuilder sb = new StringBuilder();
			for(il =0; il < linee.Count; il++)
			{
				sb.AppendLine(linee[il]);
			}
			NcMessageBox.Show(this, sb.ToString());

		}
		
		void Legge(ToolStripMenuItem tsmi)
		{

		}
	}
}
