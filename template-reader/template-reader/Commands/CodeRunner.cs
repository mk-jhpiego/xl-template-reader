using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.excelProcessing;
using template_reader.model;
using template_reader.views;

namespace template_reader.Commands
{
    public class CodeRunner<T> : ICommandExecutor where T : class
    {
        public IQueryHelper<T> CodeToExcute { get; internal set; }

        public Action<T> AsyncCallBack { private get; set; }

        delegate void closeFormDelegate();

        SplashScreen defaultSplashScreen = null;
        public bool ShowSplash { get; internal set; }
        void closeForm()
        {
            defaultSplashScreen.Close();
            defaultSplashScreen.Dispose();
        }

        void closeSplash()
        {
            if (defaultSplashScreen != null)
            {
                if (defaultSplashScreen.InvokeRequired)
                {
                    defaultSplashScreen.Invoke(new closeFormDelegate(() => { closeForm(); }));
                }
                else
                {
                    closeForm();
                }
            }
        }

        public void Execute()
        {
            if (CodeToExcute == null)
            {
                return;
            }

            defaultSplashScreen = new SplashScreen() { StartPosition = FormStartPosition.CenterParent };
            CodeToExcute.progressDisplayHelper = defaultSplashScreen;
            var task = new Task(() =>
            {
                var res = CodeToExcute.Execute();
                closeSplash();
                if (AsyncCallBack != null)
                {
                    AsyncCallBack(res);
                }
            });
            task.Start();
            defaultSplashScreen.ShowDialog();
        }
    }
}
