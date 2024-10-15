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
using GenshenApp.Events;
using Prism.Events;
using Prism.Commands;
using Prism.Ioc;
using GenshenApp.UserControls.Dialog;

namespace GenshenApp.ViewModels
{
    public class WorldViewModel:BindableBase
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

        private readonly IEventAggregator eventAggregator;
        private readonly IContainerProvider container;

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

        public DelegateCommand<WorldData> ShowDetailCommand { get; private set; }

        public WorldViewModel(IEventAggregator eventAggregator,
            IContainerProvider container)
        {
            this.eventAggregator = eventAggregator;
            this.container = container;

            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;
            ProgramData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;

            worlds = new List<String>();
            worlds.Add("首页");
            foreach(var item in WorldDatas) 
                worlds.Add(item.Name);

            ShowDetailCommand = new DelegateCommand<WorldData>(ShowDetail);
        }

        private void ShowDetail(WorldData worldData)
        {
            var obj = container.Resolve<WorldDetailView>();
            obj.DataContext = worldData;
            eventAggregator.GetEvent<DialogShow>().Publish(obj);
        }
    }
}
