using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using GenshenApp.Services;
using GenshenApp.Services.Interface;
using GenshenApp.ViewModels;
using GenshenApp.Views;
using GenshenApp.UserControls;
using GenshenApp.UserControls.Dialog;
using System.Windows.Media;

namespace GenshenApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override async void OnInitialized()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            var programDataService = Container.Resolve<IProgramDataService>();

            programDataService.LoadSettingData();
            base.OnInitialized();

            await programDataService.LoadProgramData();
            regionManager.Regions["MainViewRegion"].RequestNavigate("HomeView");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<CharactorView,CharactorViewModel>();
            containerRegistry.RegisterForNavigation<WorldView,WorldViewModel>();
            containerRegistry.RegisterForNavigation<NewsView, NewsViewModel>();
            containerRegistry.RegisterForNavigation<FailedView>();
            containerRegistry.RegisterForNavigation<Loading>();

            containerRegistry.RegisterForNavigation<MyDialogView>();
            containerRegistry.RegisterForNavigation<YearView>();
            containerRegistry.RegisterForNavigation<WorldDetailView>();

            containerRegistry.Register<ILoadDataService, LoadDataService>();
            containerRegistry.RegisterSingleton<IProgramDataService, ProgramDataService>();
        }
    }
}
