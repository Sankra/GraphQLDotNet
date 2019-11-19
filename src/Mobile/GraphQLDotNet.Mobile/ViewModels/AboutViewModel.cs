﻿using System.Threading.Tasks;
using GraphQLDotNet.Mobile.ViewModels.Commands;
using GraphQLDotNet.Mobile.ViewModels.Common;
using GraphQLDotNet.Mobile.ViewModels.Navigation;

namespace GraphQLDotNet.Mobile.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public AboutViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            Title = "About2";
        }

        public IAsyncCommand OpenWebCommand => new AsyncCommand(async () =>
        {
            await navigationService.NavigateTo<AboutViewModel>();
            //Launcher.OpenAsync(new Uri("https://xamarin.com/platform"));
        });

        public override async Task Initialize()
        {
            await Task.Delay(500);
            Title = "Finished";
        }
    }
}