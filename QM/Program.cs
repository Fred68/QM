
using Fred68.CfgReader;
using NcForm;
using System.Reflection;
using System.Resources;
using System.Drawing;


namespace QM
{
    
    internal static class Program
    {
        public readonly static string _cfgFile = "QM.cfg";
        static CFG? cfg;
        static NcSplashScreen sps;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Image? logo = null;

            ApplicationConfiguration.Initialize();
            
            string[] resmanes = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            // ERRORE: NON RICONOSCE OGGETTI BITMAP IN FILE RESX
            //ResourceManager rm = new ResourceManager("QM.Resource1", Assembly.GetExecutingAssembly());
            //object? obj = rm.GetObject("logo03"); 
            //if(obj != null)
            //{
            //    logo = (Image?) obj;
            //}

            
            sps = new NcSplashScreen(new Size(200,100),50,logo,true,2500,false);
            //sps.Show();
            //sps.ShowDialog();

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

            //ApplicationConfiguration.Initialize();

            //sps = new NcSplashScreen(new Size(200,100),0,null,null);
            //sps.Show();

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