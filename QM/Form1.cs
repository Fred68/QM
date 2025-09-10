using NcForms;
using System.Text;
using Fred68.TreeItem;
using Tree;
using System.Diagnostics;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;

namespace QM
{

	/// <summary>
	/// Main form, derivato da NcForm
	/// </summary>
	public partial class Form1:NcForms.NcForm
	{
		const int IDLEN = 4;                // Numero di caratteri dell'item
		CFG cfg;                            // File di configurazione

		List<string>? linee;                // Linee con i comandi
		TreeItem<MnuItem>? menuTree;        // Albero con i comandi
		MenuStrip[] menus;                  // I menù caricati
		string?[][] comandi;                // Array degli array (jagged) dei comandi
		
		static System.Windows.Forms.Timer tmAuto;	// Timer per comandi autoexec
		static List<string> commAuto;				// Lista comandi autoexec
		static int delayAuto;						// Copia di cfg.AutoDelay
		static Form1? mainForm;						// Riferimento

		int iMenu;                          // Indice del menù corrente
		Font mnuFont, mnuFontBold;
		bool quitWhenActivated;

		static Form1()
		{
			commAuto = new List<string>();
			tmAuto = new System.Windows.Forms.Timer();
			tmAuto.Tick += new EventHandler(TimerTickProcessor);
		}

		/// <summary>
		/// Event handler del timer
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ea"></param>
		private static void TimerTickProcessor(Object? obj, EventArgs? ea)
		{
			tmAuto.Stop();						// Ferma il timer per lanciare il comando

			tmAuto.Interval = delayAuto * 1000;	// Imposta il nuovo intervallo ripetuto
			
			if(commAuto.Count > 0 )
			{
				string cmd = commAuto[0];		// Estrae il comando
				commAuto.RemoveAt(0);			// Lo toglie dalla lista
				try
				{								// Lo esegue

					// System.Diagnostics.Process.Start(comandi[id]) forse non funziona correttamente con .NET 8.0

					#if true
					ProcessStartInfo psinfo = new ProcessStartInfo();
					psinfo.FileName = cmd;
					psinfo.UseShellExecute = true;
					Process proc = new Process();
					proc.StartInfo = psinfo;
					proc.EnableRaisingEvents = true;
					proc.Start();
					#else
					if(mainForm != null)	NcMessageBox.Show(mainForm, $"Comando:\n{cmd}");
					#endif
				}
				catch(Exception ex)
				{
					if(mainForm != null)	NcMessageBox.Show(mainForm,ex.Message);
				}

			}

			if(commAuto.Count > 0)				// Fa ripartire il timer, se ci sono altri comandi
			{
				tmAuto.Start();
			}
			else
			{
				tmAuto.Stop();					// Se no, ferma il timer e rilascia le risorse
				tmAuto.Dispose();
			}
			

		}

		/// <summary>
		/// CTOR con parametri
		/// </summary>
		/// <param name="style"></param>
		/// <param name="color"></param>
		/// <param name="cfg"></param>
		public Form1(NcFormStyle style,NcFormColor color,CFG cfg) : base(style,color)
		{
			InitializeComponent();
			this.cfg = cfg;
			this.AskClose = !cfg.FastQuit;
			this.Opacity = cfg.Opacity;
			this.Title = cfg.Titolo;
			this.StatusText = string.Empty;
			this.ShowInTaskbar = cfg.ShowInTaskbar;
			this.Name = cfg.Titolo;
			this.Text = cfg.Titolo;
			mnuFont = new Font(this.Font.FontFamily.Name,cfg.MnuFontSize);
			mnuFontBold = new Font(this.Font.FontFamily.Name,cfg.MnuFontSize,FontStyle.Bold);
			comandi = new string[cfg.Comandi.Count][];
			
			menus = new MenuStrip[cfg.Comandi.Count];
			quitWhenActivated = false;

			delayAuto = cfg.AutoDelay;
			mainForm = this;
		}

		/// <summary>
		/// On Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender,EventArgs e)
		{
			iMenu = 0;

			menu.Hide();

			if(!SetupMenus())
			{
				NcMessageBox.Show(this,"Errore in uno dei menu. Fine programma.");

				quitWhenActivated = true;                   // Force Close() as soon as possible, but not when loading (it generates an error)
			}

			for(int i = 0;i < cfg.Comandi.Count;i++)        // Set MouseEnter/Leave base class event handler to new controls...
			{
				Controls.Add(menus[i]);
				SetupControlEvents(menus[i]);
				menus[i].Hide();
			}

			SetLayout(menu);

			#if false
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"First = {cfg.AutoFirstDelay} s.\t Delay = {cfg.AutoDelay} s.");
			foreach(string s in commAuto)
			{
				sb.AppendLine(s);
			}
			MessageBox.Show(sb.ToString());
			#endif

			if(commAuto.Count > 0)
			{
				tmAuto.Interval = cfg.AutoFirstDelay * 1000;
				tmAuto.Start();
			}
		}



		void SetLayout(MenuStrip? mnu = null)
		{
			SuspendLayout();

			if(mnu != null)
			{
				MainMenuStrip = mnu;
			}

			if(MainMenuStrip != null)
			{
				MainMenuStrip.Hide();
			}

			MainMenuStrip = menus[iMenu];
			MainMenuStrip.Dock = DockStyle.None;
			MainMenuStrip.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
			MainMenuStrip.Location = new Point(0,this.UpperBarHeight);
			MainMenuStrip.AutoSize = true;
			MainMenuStrip.BackColor = Color.FromName(cfg.COL_bkgnd);

			MainMenuStrip.Show();

			ResumeLayout(true);

			Size mnuSz = MainMenuStrip.Size;
			Size formSz = this.Size;

			formSz.Width = mnuSz.Width;
			formSz.Height = mnuSz.Height + this.UpperBarHeight + this.LowerBarHeight;
			this.Size = formSz;
		}

		/// <summary>
		/// Setup all menus in the config file
		/// </summary>
		bool SetupMenus()
		{
			bool ok = true;
			for(int i = 0;i < cfg.Comandi.Count;i++)
			{
				menus[i] = new MenuStrip();
				menus[i].LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
				menus[i].Location = new Point(0,this.UpperBarHeight);
				menus[i].AutoSize = true;
				menus[i].Font = mnuFont;
				if(!ReadAndSetupMenu(i))
					ok = false;
			}
			return ok;
		}

		/// <summary>
		/// Read file and set-up iM menu
		/// </summary>
		/// <param name="iM"></param>
		bool ReadAndSetupMenu(int iM)
		{
			bool mnuOk = false;
			uint commandCount;
			bool isFirstEmptySet = false;

			menus[iM].Items.Clear();

			linee = ReadMenuFile(iM);
			menuTree = LinesToTree(linee,out commandCount);

			if(menuTree != null)
			{
				if(menuTree.ItemsCount() > cfg.MaxEntries)      // First level items
				{
					NcMessageBox.Show(this,$"Menu troppo lungo");
					return mnuOk;
				}
			}

			comandi[iM] = new string[commandCount + 1];

			if(cfg.Verbose)
			{
				ShowLines();
				ShowTree();
			}

			linee.Clear();
			linee = null;                   // Ready to dispose by GC

			if((menuTree != null) && (comandi[iM] != null))
			{
				foreach(TreeItem<MnuItem> item in menuTree.TreeItems(TreeSearchType.depth_first))
				{
					string txt;
					bool bold, auto, disable;

					txt = IdentifyFlags(item.Data.Txt,out bold,out auto,out disable);   // Identisy and remove flag chars

					if((!item.IsLeaf) && !txt.EndsWith(cfg.SubMenuStr))
						txt += cfg.SubMenuStr;
					item.Data.Tsmi = new ToolStripMenuItem(txt,null);
					item.Data.Tsmi.TextAlign = ContentAlignment.MiddleLeft;
					item.Data.Tsmi.Name = $"{item.Data.ID.ToString($"D{IDLEN}")}";
					item.Data.Tsmi.BackColor = Color.FromName(cfg.COL_bkgnd);

					if(item.Previous != null)                           // Disable, if father node is disabled
					{
						if(item.Previous.Data.Disabled)
						{
							disable = true;
						}
					}

					if(disable)
					{
						item.Data.Disabled = true;
					}

					if(bold)
					{
						item.Data.Tsmi.Font = mnuFontBold;
					}
					else
					{
						item.Data.Tsmi.Font = mnuFont;
					}

					if(item.Data.Command.Length > 1)                    // Add command handler
					{
						item.Data.Tsmi.Click += TsmiOnClick;
						comandi[iM][item.Data.ID] = item.Data.Command;

						if(auto)
						{
							commAuto.Add(item.Data.Command);
						}
					}
					else if((!isFirstEmptySet) && (!item.IsRoot))       // Add change menu handler (first item, not root)
					{
						item.Data.Tsmi.Click += menuTitleOnClick;
						item.Data.Tsmi.BackColor = Color.FromName(cfg.COL_buttons);
						item.Data.Tsmi.Font = mnuFontBold;
						isFirstEmptySet = true;
					}

					if(!item.Data.Disabled)                             // Add to menu, if not disabled
					{
						if(item.Depth == 1)
						{
							menus[iM].Items.Add(item.Data.Tsmi);
						}
						else
						{
							if(item.Previous != null)
							{
								item.Previous.Data.Tsmi.DropDownItems.Add(item.Data.Tsmi);
							}
						}
					}


				}
				menuTree.Clear();
				mnuOk = true;
			}
			return mnuOk;
		}

		/// <summary>
		/// Identify and remove cfg chars at the beginning of the line
		/// </summary>
		/// <param name="line"></param>
		/// <param name="_bold"></param>
		/// <param name="_auto"></param>
		/// <param name="_disable"></param>
		/// <returns>line w/out cfg chars</returns>
		string IdentifyFlags(string line,out bool _bold,out bool _auto,out bool _disable)
		{
			string _prefix = cfg.BoldChar + cfg.AutoChar + cfg.DisabledChar;    // Prefix
			string txt = line;
			_bold = _auto = _disable = false;

			if(txt.Length > 0)
			{
				char ch;
				for(ch = txt[0];_prefix.Contains(ch);ch = txt[0])
				{
					if(ch == cfg.BoldChar[0])
						_bold = true;
					if(ch == cfg.AutoChar[0])
						_auto = true;
					if(ch == cfg.DisabledChar[0])
						_disable = true;
					txt = txt.Substring(1);
				}
			}
			return txt;
		}

		/// <summary>
		/// Chamge menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void menuTitleOnClick(object? sender,EventArgs e)
		{
			iMenu++;
			if(iMenu >= cfg.Comandi.Count)
				iMenu = 0;
			SetLayout();
		}

		/// <summary>
		/// Menu item click handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TsmiOnClick(object? sender,EventArgs e)
		{
			uint id;

			if((sender != null) && (comandi != null))
			{
				if(((ToolStripMenuItem)sender).Name != null)
				{
					if(uint.TryParse(((ToolStripMenuItem)sender).Name,out id))
					{
						if(iMenu < comandi.Length)
						{
							if((comandi[iMenu] != null) && id < comandi[iMenu].Length)
							{
								bool ok = true;
								if(cfg.Verbose)
								{
									if(comandi[iMenu][id] != null)
									{
										if(NcMessageBox.Show(this,comandi[iMenu][id],"Execute ?",MessageBoxButtons.YesNo) != DialogResult.Yes)
										{
											ok = false;
										}
									}
								}
								if(ok)
								{
									try
									{
										// System.Diagnostics.Process.Start(comandi[id]) forse non funziona correttamente con .NET 8.0

										ProcessStartInfo psinfo = new ProcessStartInfo();
										psinfo.FileName = comandi[iMenu][id];
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
			}
		}

		/// <summary>
		///  Legge il file con il menu
		///  Rimuove i commenti, scarta le linee non valide e numera le linee
		/// </summary>
		/// <returns>List<string> linee lette</returns>
		List<string> ReadMenuFile(int iM = 0)
		{
			List<string> lines = new List<string>();
			try
			{
				int nItem = 1;      // Valid items count, starting from 1, number 0 is for the root. 
				string? line;
				StreamReader sr = new StreamReader(cfg.Comandi[iM]);
				while((line = sr.ReadLine()) != null)
				{
					if((line.Length > 0) && (!line.StartsWith(cfg.CHR_Commento)))       // Line is noot empty or is a comment
					{
						int indx;
						if((indx = line.IndexOf(cfg.CHR_Commento)) != -1)       // Remove from comment character to the end of the line
						{
							line = line.Substring(0,indx);
						}
						//int _sep = (line.IndexOf(cfg.Sep)!=-1) ? 1 : 0; int _ini=...; int _fin=...; //if( _sep+_ini+_fin == 1)...
						bool _fin = line.IndexOf(cfg.MnuFin) != -1;             // 'line' contains the end-menu separator.
						bool _ini = line.IndexOf(cfg.MnuIni) != -1;             // 'line' contains the start-menu separator.
						if((line.IndexOf(cfg.Sep) != -1) || _ini || _fin)           // If line contains a command
						{
							int nIt = _fin ? 0 : nItem++;                       // If it is not an end-menu, set and increase item count.
							lines.Add($"{(nIt.ToString($"D{IDLEN}"))}\t{line.Trim()}"); // Add 'line'
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
		/// Imposta un id solo se la linea contiene un comando (e non è un menù)
		/// </summary>
		/// <param name="lines">List<string></param>
		/// <returns>TreeItem<MnuItem> root node</returns>
		TreeItem<MnuItem>? LinesToTree(List<string> lines,out uint itemCount)
		{
			string[] seps = { cfg.Sep,cfg.MnuIni,cfg.MnuFin };
			bool ok = true;

			TreeItem<MnuItem>? ret = null;
			itemCount = 0;
			Stack<TreeItem<MnuItem>> stack = new Stack<TreeItem<MnuItem>>();

			stack.Push(new TreeItem<MnuItem>(new MnuItem(0),null));     // Create and push root tree item into the stack

			uint id;
			uint idCounter = 0;

			if(lines != null)
			{
				foreach(string line in lines)                           // Read lines
				{
					if(line.Length > IDLEN)                             // If length is enough...
					{
						string lineId = line.Substring(0,IDLEN);        // Extract parts
						string lineNoId = line.Substring(IDLEN + 1);
						if(uint.TryParse(lineId,out id))                // Get ID
						{
							if(id > 0)                                  // If ID > 0 (not zero = root or end-of-menu line, no item)...
							{                                           // ...separate text
								string[] parts = lineNoId.Split(seps,StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
								if(parts.Length > 0)                    // If one part (or more...)
								{
									string command;
									if(parts.Length > 1)                // If a second part exists...
									{
										command = parts[1];             // ...set the command
										id = ++idCounter;               // ...increases and set counter as id
									}
									else
									{
										command = string.Empty;
										id = 0;
									}

									TreeItem<MnuItem> itm = new TreeItem<MnuItem>(new MnuItem(id,parts[0],command),stack.Peek());   // Add item to the tree

									if(lineNoId.Contains(cfg.MnuIni))       // If current item has sub menu...
									{
										itm.Data.Command = string.Empty;    // ...clear command
										stack.Push(itm);                    // ...and push into the stack
									}
								}
							}
							else                                        // If ID = 0: end-of-line					
							{
								if(lineNoId.Contains(cfg.MnuFin))       // Sub-menu end
								{
									stack.Pop();
									if(stack.Count < 1)
									{
										NcMessageBox.Show(this,$"Errore: troppe linee con '{cfg.MnuFin}'.");
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

			if((stack.Count == 1) && ok)    // Only the root item should remain in the stack
			{
				ret = stack.Pop();
				itemCount = idCounter;
			}
			else
			{
				if(stack.Count > 1)
				{
					NcMessageBox.Show(this,$"Errore: troppe linee ({stack.Count - 1}) con '{cfg.MnuIni}'.");
				}
			}

			stack.Clear();                  // Free stack

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
				for(il = 0;il < linee.Count;il++)
				{
					sb.AppendLine(linee[il]);
				}
			}
			NcMessageBox.Show(this,sb.ToString(),"Lines");
		}

		/// <summary>
		/// NcMessageBox with tree diagram
		/// </summary>
		void ShowTree()
		{
			if(menuTree != null)
			{
				NcMessageBox.Show(this,menuTree.ToTreeString(),"Menu tree");
			}
		}

		/// <summary>
		/// Form is activater oe deactivated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_ChangeActivation(object sender,EventArgs e)
		{
			if(quitWhenActivated)       // Quit if an error is detected during Form Load
				Close();
		}

		private void Form1_Resize(object sender,EventArgs e)
		{
			if(this.Width < cfg.MinWidth)
			{
				this.Width = cfg.MinWidth;
				Invalidate();
			}
		}

		/// <summary>
		/// Override to get correct assembly reference
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public override string Version(Assembly asm,bool details = false)
		{
			StringBuilder strb = new StringBuilder();
			try
			{
				strb.AppendLine(Application.ProductName);
				if(asm != null)
				{
					System.Version? v = asm.GetName().Version;
					if(v != null)
						strb.AppendLine($"Version: {v.ToString()} ({BuildTime(asm)})");
					if(details)
					{
						string? n = asm.GetName().Name;
						if(n != null)
							strb.AppendLine("Assembly name: " + n);
						strb.AppendLine("BuildTime time: " + File.GetCreationTime(asm.Location).ToString());
						strb.AppendLine("BuildTime number: " + BuildTime(asm,true));
						strb.AppendLine("Executable path: " + Application.ExecutablePath);
					}
				}
				strb.AppendLine("Copyright: " + Application.CompanyName);
			}
			catch { }
			return strb.ToString();
		}

		protected override void OnHelp()
		{
			MessageBox.Show(Version(Assembly.GetExecutingAssembly(),true));
		}

		protected override bool OnClosingCancelEvent()
		{
			bool cancel = false;
			if(MessageBox.Show("Quit","Quit ?",MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				cancel = true;			// La risposta a Quit ? non è yes: continua l'esecuzione
			}
			else
			{							// Quit: ferma il timer. Non serve eseguire Dispose()
				tmAuto.Stop();
			}
			return cancel;
		}
	}
}
