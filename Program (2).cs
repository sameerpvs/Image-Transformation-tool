
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageTranslat
{
     static class Program
        {
         enum Operation {SLICE,RESIZE,CONVERT,TRANSLATE,PROJECTION };
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main(string[] args)
            {
                if (args.Length > 0)
                {
                    string operation=args[0].ToString().ToUpper();
                    if (operation == Operation.CONVERT.ToString() && args.Length==5)
                    {
                        FrmImageTranslate.gInputPath = args[1];
                        FrmImageTranslate.gOutputPath = args[2];
                        FrmImageTranslate.gSrcFileType = args[3];
                        FrmImageTranslate.gTrgFileType = args[4];
                        FrmImageTranslate.ProcessConversion(); 
                    }
                    else if (operation == Operation.PROJECTION.ToString() && args.Length == 5)
                    {
                        FrmImageTranslate.ProcessReprojection(args[1], args[2], args[3], args[4]); 
                    }
                    else if (operation == Operation.PROJECTION.ToString() && args.Length == 2)
                    {
                        FrmImageTranslate.ProcessReprojection(args[1]);
                    }
                    else if (operation == Operation.RESIZE.ToString()  && args.Length == 2)
                    {
                        FrmImageTranslate.ProcessResize(args[1]);
                        
                    }
                    else if (operation == Operation.SLICE.ToString() && args.Length == 2)
                    {
                        FrmImageTranslate.ProcessSlice(args[1]);
                    }
                    else if (operation == Operation.TRANSLATE.ToString() && args.Length == 2)
                    {
                        FrmImageTranslate.ProcessTranlate(args[1]); 
                    }
                    else 
                    {
                        MessageBox.Show("Invalid parameter supplied,Hence please check and try again.","Image Transformation",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                        return;
                    }

                    
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmImageTranslate());
                }
            }
        }
    
}
