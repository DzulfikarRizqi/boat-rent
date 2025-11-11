using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobaHW7
{
    public enum DialogKind { Success, Error, Info }

    public static class DialogService
    {
        public static void Show(string title, string message, DialogKind kind = DialogKind.Info)
        {
            var dlg = new PopupDialog(title, message, kind);
            dlg.ShowDialog();
        }

        public static void Success(string title, string message) => Show(title, message, DialogKind.Success);
        public static void Error(string title, string message) => Show(title, message, DialogKind.Error);
        public static void Info(string title, string message) => Show(title, message, DialogKind.Info);
    }
}
