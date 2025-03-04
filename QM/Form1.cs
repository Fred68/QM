using NcForms;
using static System.Windows.Forms.LinkLabel;
using System.ComponentModel.Design;
using System.Text;
using Fred68.TreeItem;
using Tree;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace QM
{

	/// <summary>
	/// Main form, derivato da NcForm
	/// </summary>
	public partial class Form1 : NcForms.NcForm
	{
		const int IDLEN = 4;				// Numero di caratteri dell'item
		CFG cfg;							// File di configurazione

		List<string>? linee;				// Linee con i comandi
		TreeItem<MnuItem>? menuTree;		// Albero con i comandi
		string[][]? comandi;				// Array degli array (jagged) dei comandi

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
			this.StatusText = string.Empty;
			comandi = new string[cfg.Comandi.Count][];
		}

		/// <summary>
		/// On Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender,EventArgs e)
		{
			//uint commandCount;

			menu.AutoSize = true;

			menu.Location = new Point(0,this.UpperBarHeight);
			menu.Items.Clear();
			menu.BackColor = Color.FromName(cfg.COL_bkgnd);

			menu.SuspendLayout();
			SuspendLayout();
			
			ReadAndSetupMenu();

			ResumeLayout(true);
			menu.ResumeLayout(true);

			Size mnuSz = menu.Size;
			Size formSz = this.Size;

			formSz.Width = mnuSz.Width;
			formSz.Height = mnuSz.Height + this.UpperBarHeight + this.LowerBarHeight;
			this.Size = formSz;
			
		}

		void ReadAndSetupMenu(int iMenu = 0)
		{
			uint commandCount;
			
			bool isFirstEmptySet = false;

			linee = ReadMenuFile();
			menuTree = LinesToTree(linee, out commandCount);
			comandi[iMenu] = new string[commandCount+1];

			if(cfg.Verbose)
			{
				ShowLines();
				ShowTree();
			}

			linee.Clear();
			linee = null;					// Ready to dispose by GC

			if(menuTree != null)
			{
				//menu.Items.Add(new ToolStripMenuItem(cfg.Comandi, null));
				foreach(TreeItem<MnuItem> item in menuTree.TreeItems(TreeSearchType.depth_first))
				{
					item.Data.Tsmi = new ToolStripMenuItem(item.Data.Txt, null);
					item.Data.Tsmi.TextAlign = ContentAlignment.MiddleLeft;
					item.Data.Tsmi.Name = $"{item.Data.ID.ToString($"D{IDLEN}")}";
					item.Data.Tsmi.BackColor = Color.FromName(cfg.COL_bkgnd);

					if(item.Data.Command.Length > 1)				// Add command handler
					{
						item.Data.Tsmi.Click += TsmiOnClick;
						comandi[iMenu][item.Data.ID] = item.Data.Command;
					}
					else if((!isFirstEmptySet) && (!item.IsRoot))		// Add change menu handler (first item, not root)
					{
						item.Data.Tsmi.Click += menuTitleOnClick;
						isFirstEmptySet = true;
					}

					if(item.Depth == 1)
					{
						menu.Items.Add(item.Data.Tsmi);
					}
					else
					{
						if(item.Previous != null)
						{
							item.Previous.Data.Tsmi.DropDownItems.Add(item.Data.Tsmi);
						}
					}
				}
				menuTree.Clear();
			}
		}

		void menuTitleOnClick(object? sender, EventArgs e)
		{
			NcMessageBox.Show(this, "CLICK !!!","Change menu");	
		}

		/// <summary>
		/// Menu item click handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TsmiOnClick(object? sender, EventArgs e)
		{
			uint id;
			
			if((sender != null)&&(comandi!=null))
			{
				if(((ToolStripMenuItem)sender).Name != null)
				{
					if(uint.TryParse(((ToolStripMenuItem)sender).Name, out id))
					{
						bool ok = true;
						if(cfg.Verbose)
						{
							#warning METTERE ...comandi[iMenu][id]...
							if(NcMessageBox.Show(this,comandi[0][id],"Execute ?",MessageBoxButtons.YesNo) != DialogResult.Yes)
							{
								ok = false;
							}
						}
						if(ok)
						{
							try
							{
								// System.Diagnostics.Process.Start(comandi[id]) forse non funziona correttamente con .NET 8.0

								ProcessStartInfo psinfo= new ProcessStartInfo();
								psinfo.FileName = comandi[0][id];
								psinfo.UseShellExecute = true;
							
								Process proc = new Process();
								proc.StartInfo = psinfo;
								proc.EnableRaisingEvents = true;
								proc.Start();

							}
							catch(Exception ex)
							{
								NcMessageBox.Show(this,ex.Message);
							}
						}
					}

				}
			}
		}

		/// <summary>
		///  Legge il file con il menu
		///  Rimuove i commenti, scarta le linee non valide e numera le linee
		/// </summary>
		/// <returns>List<string> linee lette</returns>
		List<string> ReadMenuFile(int iMenu = 0)
		{
			List<string> lines = new List<string>();
			try
			{
				int nItem = 1;		// Valid items count, starting from 1, number 0 is for the root. 
				string? line;
				StreamReader sr = new StreamReader(cfg.Comandi[iMenu]);
				while ((line = sr.ReadLine()) != null)
				{
					if( (line.Length > 0) && (!line.StartsWith(cfg.CHR_Commento)))		// Line is noot empty or is a comment
					{
						int indx;
						if((indx = line.IndexOf(cfg.CHR_Commento)) != -1)		// Remove from comment character to the end of the line
						{
							line = line.Substring(0, indx);							
						}
						//int _sep = (line.IndexOf(cfg.Sep)!=-1) ? 1 : 0; int _ini=...; int _fin=...; //if( _sep+_ini+_fin == 1)...
						bool _fin = line.IndexOf(cfg.MnuFin) != -1;				// 'line' contains the end-menu separator.
						bool _ini = line.IndexOf(cfg.MnuIni) != -1;				// 'line' contains the start-menu separator.
						if((line.IndexOf(cfg.Sep)!=-1) || _ini || _fin)			// If line contains a command
						{
							int nIt = _fin ? 0 : nItem++;						// If it is not an end-menu, set and increase item count.
							lines.Add($"{(nIt.ToString($"D{IDLEN}"))}\t{line.Trim()}");	// Add 'line'
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
		/// Legge le linee del men�
		/// Crea l'albero con descrizioni e comandi
		/// Imposta un id solo se la linea contiene un comando (e non � un men�)
		/// </summary>
		/// <param name="lines">List<string></param>
		/// <returns>TreeItem<MnuItem> root node</returns>
		TreeItem<MnuItem>? LinesToTree(List<string> lines, out uint itemCount)
		{
			string[] seps = {cfg.Sep,cfg.MnuIni,cfg.MnuFin};
			bool ok = true;

			TreeItem<MnuItem>? ret = null;
			itemCount = 0;
			Stack<TreeItem<MnuItem>> stack = new Stack<TreeItem<MnuItem>>();
			
			stack.Push(new TreeItem<MnuItem>(new MnuItem(0),null));		// Create and push root tree item into the stack

			uint id;
			uint idCounter = 0;

			if(lines != null)
			{
				foreach(string line in lines)							// Read lines
				{
					if(line.Length > IDLEN)								// If length is enough...
					{
						string lineId = line.Substring(0, IDLEN);		// Extract parts
						string lineNoId = line.Substring(IDLEN + 1);
						if(uint.TryParse(lineId,out id))				// Get ID
						{
							if(id > 0)									// If ID > 0 (not zero = root or end-of-menu line, no item)...
							{											// ...separate text
								string[] parts = lineNoId.Split(seps,StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
								if(parts.Length > 0)					// If one part (or more...)
								{
									string command;
									if(parts.Length > 1)				// If a second part exists...
									{
										command = parts[1];				// ...set the command
										id = ++idCounter;				// ...increases and set counter as id
									}
									else
									{
										command = string.Empty;
										id = 0;
									}

									TreeItem<MnuItem> itm = new TreeItem<MnuItem>(new MnuItem(id,parts[0],command) , stack.Peek());	// Add item to the tree

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
				itemCount = idCounter;
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
			if(linee != null)
			{
				for(il =0; il < linee.Count; il++)
				{
					sb.AppendLine(linee[il]);
				}
			}
			NcMessageBox.Show(this, sb.ToString(),"Lines");
		}

		/// <summary>
		/// NcMessageBox with tree diagram
		/// </summary>
		void ShowTree()
		{
			if(menuTree != null)
			{
				NcMessageBox.Show(this, menuTree.ToTreeString(),"Menu tree");
			}
		}

	}
}
