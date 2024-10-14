using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;

namespace GenshenApp.ViewModels
{
    public class WorldViewModel:BindableBase,INavigationAware
    {
        private ProgramData programData;
        private ProgramData ProgramData
        {
            get => programData;
            set
            {
                if(programData != value)
                {
                    programData = value;
                    RaisePropertyChanged(nameof(WorldDatas));
                }
            }
        }

        private ProgramSettingData settingData;
        
        public List<WorldData> WorldDatas => ProgramData.WorldData;

        private List<String> worlds;
        public List<String> Worlds => worlds;

        private int selectIndex = 0;
        public int SelectIndex
        {
            get => selectIndex;
            set
            {
                if (value < 0 || value >= Worlds.Count)
                    return;
                SetProperty(ref selectIndex, value);
            }
        }

        public WorldViewModel()
        {
            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;
            ProgramData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;

            worlds = new List<String>();
            worlds.Add("首页");
            foreach(var item in WorldDatas) 
                worlds.Add(item.Name);
        }
        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ProgramData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        #endregion
    }
}
