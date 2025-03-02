using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace QM
{
	public class MnuItem : IFormattable
	{
		uint _id;
		string _txt;
		string _command;

		/// <summary>
		/// Item text
		/// </summary>
		public string Txt { get {return _txt;} set {_txt = value;} }

		/// <summary>
		/// Item command text
		/// </summary>
		public string Command { get {return _command;} set {_command = value;} }

		/// <summary>
		/// Id
		/// </summary>
		public uint ID { get {return _id;} set {_id = value;} }
				
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="txt">Text</param>
		/// <param name="depth">Depth</param>
		/// <param name="item">Child item to add, or null</param>
		public MnuItem(uint ID, string? txt = null, string? command = null)
		{
			_id = ID;
			_txt = (txt != null) ? txt : ((ID == 0) ? "root" : string.Empty);
			_command = (command != null) ? command : string.Empty;
		}

		/// <summary>
		/// ToString(), override
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"[{_id}]   {_txt} : {_command}";	
		}

		public string ToString(string format)
		{
		   return this.ToString();
		}

		public string ToString(string? format, IFormatProvider? provider)
		{
		   return this.ToString();
		}

	}
}
