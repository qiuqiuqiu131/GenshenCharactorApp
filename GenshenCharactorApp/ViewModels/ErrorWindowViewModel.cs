using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenCharactorApp.ViewModels
{
    public class ErrorWindowViewModel : BindableBase, IDialogAware
    {
        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        #region IDialogAware
        public string Title => "出错";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => false;

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        #endregion
    }
}
