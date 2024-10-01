using GenshenCharactorApp.ViewModels;
using GenshenCharactorApp.Views;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;
using GenshenCharactorApp.Services;
using GenshenCharactorApp.Services.Interface;

namespace GenshenCharactorApp
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.Regions["MainViewRegion"].RequestNavigate("HomeView");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<CharactorView,CharactorViewModel>();
            containerRegistry.RegisterForNavigation<WorldView,WorldViewModel>();
            containerRegistry.RegisterForNavigation<NewsView, NewsViewModel>();

            containerRegistry.Register<ILoadDataService, LoadDataService>();
        }
    }
}
