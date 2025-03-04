
using Fred68.CfgReader;

namespace QM
{
    
    internal static class Program
    {
        public readonly static string _cfgFile = "QM.cfg";
        static CFG? cfg;


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            cfg = new CFG();
            cfg.CHR_ListSeparator = @";";
            try
            {
			    cfg.ReadConfiguration(_cfgFile);
                cfg.GetNames(true);
            }
            catch
            {
                MessageBox.Show("Error reading configuration file:" + Environment.NewLine + cfg.Message);
                return;
            }

            if(!cfg.IsOk)
            {
                MessageBox.Show(cfg.Message);
                return;
            }

            ApplicationConfiguration.Initialize();

            NcForms.NcFormStyle ncfs = new NcForms.NcFormStyle(
                NcForms.NcWindowsStyles.TopMost
                //| NcForms.NcWindowsStyles.MinMax
                | NcForms.NcWindowsStyles.Help
                //| NcForms.NcWindowsStyles.LowerBar
                //| NcForms.NcWindowsStyles.Menu
                | NcForms.NcWindowsStyles.Resizable
                ,
                NcForms.NcFormWindowStates.Normal
                );
            Color[] color = new Color[4];
            color[0] = Color.FromName(cfg.COL_bkgnd);
            color[1] = Color.FromName(cfg.COL_title);
            color[2] = Color.FromName(cfg.COL_status);
            color[3] = Color.FromName(cfg.COL_buttons);
            for(int i=0; i<color.Length; i++)
            {
                if(!color[i].IsKnownColor)
                {
                MessageBox.Show($"{color[i].Name} is not a valid colour");
                color[i] = Color.White;
                }
            }
            
            Application.Run(new Form1(ncfs, new NcForms.NcFormColor(color[0],color[1],color[2],color[3],1f), cfg));
        }
    }
}