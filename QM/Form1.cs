using NcForms;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel.Design;
using System.Text;
using Fred68.TreeItem;
using Tree;
using Microsoft.VisualBasic;

namespace QM
{

	/// <summary>
	/// Main form, derivato da NcForm
	/// </summary>
	public partial class Form1 : NcForms.NcForm
	{
		const int IDLEN = 4;						// Numero di caratteri dell'item
		CFG cfg;									// File di configurazione

		List<string>? linee;						// Linee con i comandi
		TreeItem<MnuItem>? menuTree;				// Albero con i comandi

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
			linee = ReadMenuFile();
			menuTree = LinesToTree(linee);
			#if DEBUG
			ShowLines();
			ShowTree();
			#endif
			linee.Clear();

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
		///  Legge il file con il menu
		///  Rimuove i commenti, scarta le linee non valide e numera le linee
		/// </summary>
		/// <returns>List<string> linee lette</returns>
		List<string> ReadMenuFile()
		{
			List<string> lines = new List<string>();
			try
			{
				int nItem = 1;		// Valid items count, starting from 1, number 0 is for the root. 
				string? line;
				StreamReader sr = new StreamReader(cfg.Comandi);
				while ((line = sr.ReadLine()) != null)
				{
					if( (line.Length > 0) && (!line.StartsWith(cfg.CHR_Commento)))		// Line is noot empty or is a comment
					{
						int indx;
						if((indx = line.IndexOf(cfg.CHR_Commento)) != -1)				// Remove from comment character to the end of the line
						{
							line = line.Substring(0, indx);							
						}
						//int _sep = (line.IndexOf(cfg.Sep)!=-1) ? 1 : 0; int _ini=...cfg.MnuIni...; int _fin=...cfg.MnuFin...; //if( _sep+_ini+_fin == 1)...
						bool _fin = line.IndexOf(cfg.MnuFin) != -1;						// If 'line' contains the end-menu separator or...
						if((line.IndexOf(cfg.Sep)!=-1) || (line.IndexOf(cfg.MnuIni)!=-1) || _fin )	// ...another separator...
						{
							int nIt = _fin ? 0 : nItem++;								// If it is not an end-menu, sets and increase item count.
							lines.Add($"{(nIt.ToString($"D{IDLEN}"))}\t{line.Trim()}");	// ...add 'line'
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
				MessageBox.Show($"Lette {lines.Count} linee");
				#endif
			}
			return lines;
		}

		/// <summary>
		/// Legge le linee del menù
		/// Crea l'albero con descrizioni e comandi
		/// </summary>
		/// <param name="lines">List<string></param>
		/// <returns>TreeItem<MnuItem> root node</returns>
		TreeItem<MnuItem>? LinesToTree(List<string> lines)
		{
			string[] seps = {cfg.Sep,cfg.MnuIni,cfg.MnuFin};
			bool ok = true;

			TreeItem<MnuItem>? ret = null;
			Stack<TreeItem<MnuItem>> stack = new Stack<TreeItem<MnuItem>>();
			
			stack.Push(new TreeItem<MnuItem>(new MnuItem(0),null));		// Create and push root tree item into the stack

			uint id;

			if(lines != null)
			{
				foreach(string line in lines)							// Read lines
				{
					if(line.Length > IDLEN)								// If length is enough...
					{
						string lineId = line.Substring(0,IDLEN);		// Extract parts
						string lineNoId = line.Substring(IDLEN+1);
						if(uint.TryParse(lineId,out id))				// Get ID
						{
							if(id > 0)									// If ID > 0 (not zero = root or end-of-menu line, no item)...
							{											// ...separate text
								string[] parts = lineNoId.Split(seps,StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
								if(parts.Length > 0)					// If one part (or more...)
								{
									string command = (parts.Length > 1) ? parts[1] : string.Empty;		// Extract command (if any...)

									TreeItem<MnuItem> itm = new TreeItem<MnuItem>(new MnuItem(id,parts[0],command) , stack.Peek());		// Add MnuItem to the tree

									if(lineNoId.Contains(cfg.MnuIni))		// If current item has sub menu...
									{
										itm.Data.Command = string.Empty;	// ...clear command
										stack.Push(itm);					// ...and push into the stack
									}
								}
							}
							else										// If ID = 0: end-of-line					
							{
								if(lineNoId.Contains(cfg.MnuFin))		// Sub-menu end
								{
									stack.Pop();
									if(stack.Count < 1)
									{
										NcMessageBox.Show(this, $"Errore: troppe linee con '{cfg.MnuFin}'.");
										ok = false;
										break;
									}
								}
							}
						}
					}
				}
			}
			else
			{
				ok = false;
			}

			if((stack.Count == 1) && ok)	// Only the root item should remain in the stack
			{
				ret = stack.Pop();
			}
			else
			{
				if (stack.Count > 1)
				{
					NcMessageBox.Show(this, $"Errore: troppe linee ({stack.Count-1}) con '{cfg.MnuIni}'.");
				}
			}

			stack.Clear();					// Free stack
			
			return ret;
		}
	
		/// <summary>
		/// NcMessageBox with lines content
		/// </summary>
		void ShowLines()
		{
			int il;
			StringBuilder sb = new StringBuilder();
			for(il =0; il < linee.Count; il++)
			{
				sb.AppendLine(linee[il]);
			}
			NcMessageBox.Show(this, sb.ToString());
		}

		/// <summary>
		/// NcMessageBox with tree diagram
		/// </summary>
		void ShowTree()
		{
			if(menuTree != null)
			{
				NcMessageBox.Show(this, menuTree.ToTreeString());
			}
		}

		void Legge(ToolStripMenuItem tsmi)
		{

		}
	}
}
