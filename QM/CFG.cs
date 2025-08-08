using Fred68.CfgReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM
{
	public class CFG : CfgReader
	{
		public bool FastQuit;
		public bool Verbose;
		public float Opacity;
		public string Titolo;
		public List<string> Comandi;
		public string Sep;
		public string MnuIni;
		public string MnuFin;
		public int MinWidth;
		public int MaxEntries;
		public bool ShowInTaskbar;
		public int MnuFontSize;
		public string BoldChar;
		public string SubMenuStr;

		public string COL_bkgnd;
		public string COL_title;
		public string COL_status;
		public string COL_buttons;
		
	}


}
