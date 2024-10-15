using Prism.Mvvm;
using Newtonsoft.Json;
using Prism.Commands;
using System.IO;
using System.Collections.Generic;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;
using Prism.Regions;
using System.Threading.Tasks;
using System;

namespace GenshenApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IProgramDataService programDataService;

        private ProgramSettingData settingData 
            => programDataService.SettingData;

        public List<NavigateBar> NavigateBars => settingData.NavigateBar;

        public DelegateCommand<NavigateBar> SelectionChangedCommand { get; private set; }
        public DelegateCommand<string> ClickCommand { get; private set; }

        private int selectIndex = 0;
        public int SelectIndex
        {
            get => selectIndex;
            set => SetProperty(ref selectIndex, value);
        }

        public MainWindowViewModel(IRegionManager regionManager,IProgramDataService programDataService)
        {
            this.regionManager = regionManager;
            this.programDataService = programDataService;

            SelectionChangedCommand = new DelegateCommand<NavigateBar>(SelectionChanged);
            ClickCommand = new DelegateCommand<string>(Click);
        }

        private void SelectionChanged(NavigateBar navigateBar)
        {
            if (navigateBar.Target != null)
            {
                regionManager.Regions["MainViewRegion"].RequestNavigate(navigateBar.Target);
            }
            else if(navigateBar.Url != null) 
            {
                HttpHelper.OpenWebPage(navigateBar.Url);
                SelectIndex = 0;
            }
        }

        private void Click(string obj)
        {
            HttpHelper.OpenWebPage(obj);
        }
    }
}
