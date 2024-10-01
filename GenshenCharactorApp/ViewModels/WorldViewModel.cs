using GenshenCharactorApp.Common;
using GenshenCharactorApp.Common.JosnData;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GenshenCharactorApp.ViewModels
{
    public class WorldViewModel:BindableBase,INavigationAware
    {
        private ProgramData programData;
        private ProgramSettingData settingData;
        public List<WorldData> WorldDatas => programData.WorldData;

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
            programData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;
            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;

            worlds = new List<String>();
            worlds.Add("首页");
            foreach(var item in WorldDatas) 
                worlds.Add(item.Name);
        }
        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        #endregion
    }
}
