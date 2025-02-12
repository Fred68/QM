
using Fred68.CfgReader;

namespace QM
{
    
    internal static class Program
    {
        public readonly static string _cfgFile = "QM.cfg";
        static CFG cfg;


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            cfg = new CFG();
            cfg.CHR_ListSeparator = @";";
			cfg.ReadConfiguration(_cfgFile);

            //MessageBox.Show(cfg.Message);

            cfg.GetNames(true);

            if(!cfg.IsOk)
            {
                MessageBox.Show(cfg.Message);
                return;
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());

            NcForms.NcFormStyle ncfs = new NcForms.NcFormStyle(
                                                    NcForms.NcWindowsStyles.TopMost
                                                    //| NcForms.NcWindowsStyles.MinMax
                                                    | NcForms.NcWindowsStyles.Help
                                                    //| NcForms.NcWindowsStyles.LowerBar
                                                    //| NcForms.NcWindowsStyles.Menu
                                                    //| NcForms.NcWindowsStyles.Resizable
                                                    ,
                                                    
                                                    NcForms.NcFormWindowStates.Normal
                                                    );

            Application.Run(new Form1(/*NcForms.NcFormStyle.Normal*/ncfs, new NcForms.NcFormColor(Color.LightCyan,Color.PowderBlue,Color.LightCyan,Color.LightCyan,0.5f), cfg /*NcForms.NcFormColor.Simple*/));
        }
    }
}