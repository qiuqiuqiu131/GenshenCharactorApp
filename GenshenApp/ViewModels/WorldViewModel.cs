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
using GenshenApp.Services.Interface;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace GenshenApp.ViewModels
{
    public class WorldViewModel:BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IBitmapImageManager _bitmapImageManager;
        private readonly IContainerProvider container;
        private readonly IProgramDataService programDataService;

        private ProgramData programData 
            => programDataService.ProgramData;

        private ProgramSettingData settingData 
            => programDataService.SettingData;

        private List<WorldFullData> _worldFullDatas;

        public List<WorldFullData> WorldDatas
        {
            get => _worldFullDatas;
            set => SetProperty(ref _worldFullDatas, value);
        }

        private List<String> worlds;
        public List<String> Worlds => worlds;

        private BitmapImage firstBackground;
        public BitmapImage FirstBackground
        {
            get => firstBackground;
            set => SetProperty(ref firstBackground, value);
        }

        private BitmapImage titleImage;
        public BitmapImage TitleImage
        {
            get => titleImage;
            set => SetProperty(ref titleImage, value);
        }

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

        public DelegateCommand<WorldFullData> ShowDetailCommand { get; private set; }

        public WorldViewModel(IEventAggregator eventAggregator,
            IBitmapImageManager bitmapImageManager,
            IContainerProvider container,IProgramDataService programDataService)
        {
            this.eventAggregator = eventAggregator;
            _bitmapImageManager = bitmapImageManager;
            this.container = container;
            this.programDataService = programDataService;

            worlds = new List<String>();
            worlds.Add("首页");
            foreach(var item in programData.WorldData) 
                worlds.Add(item.Name);

            ShowDetailCommand = new DelegateCommand<WorldFullData>(ShowDetail);
            
            Init();
        }

        private async void Init()
        {
            FirstBackground = await _bitmapImageManager.GetBitmapImage("https://ys.mihoyo.com/main/_nuxt/img/s1.5c125a1.png");
            TitleImage = await _bitmapImageManager.GetBitmapImage("https://ys.mihoyo.com/main/_nuxt/img/title.7e5cc2a.png");

            var worldDatas = programData.WorldData;
            var worldFullDatas = new List<WorldFullData>();
            foreach (var worldData in worldDatas)
            {
                var worldFullData = new WorldFullData();
                worldFullData.Init(worldData);
                worldFullDatas.Add(worldFullData);

                Task.Run(async () =>
                {
                    worldFullData.Icon = await _bitmapImageManager.GetBitmapImage(worldData.Icon);
                    worldFullData.Background = await _bitmapImageManager.GetBitmapImage(worldData.Background);
                    var tasks = worldData.Detail.AsParallel().AsOrdered().Select(async x =>
                    {
                        BitmapImage image = await _bitmapImageManager.GetBitmapImage(x.Image);
                        return new WorldFullDetail
                        {
                            Title = x.Title,
                            Content = x.Content,
                            Image = image
                        };
                    }).ToList();
                    await Task.WhenAll(tasks);
                    worldFullData.Details = tasks.Select(x => x.Result).ToList();
                });
            }

            WorldDatas = worldFullDatas;
        }

        private void ShowDetail(WorldFullData worldData)
        {
            var obj = container.Resolve<WorldDetailView>();
            obj.DataContext = worldData;
            eventAggregator.GetEvent<DialogShow>().Publish(obj);
        }
    }
}
