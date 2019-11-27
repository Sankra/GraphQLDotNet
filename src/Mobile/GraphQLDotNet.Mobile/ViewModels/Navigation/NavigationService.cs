﻿using System;
using System.Threading.Tasks;
using GraphQLDotNet.Mobile.ViewModels.Common;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace GraphQLDotNet.Mobile.ViewModels.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly ViewModelLocator viewModelLocator;

        public NavigationService(ViewModelLocator viewModelLocator)
            => this.viewModelLocator = viewModelLocator;

        public async Task NavigateTo<TViewModel>() where TViewModel : PageViewModelBase
            => await NavigateTo<TViewModel>((Page page) => ((TViewModel)page.BindingContext).Initialize());
        
        public async Task NavigateTo<TViewModel, TPageArgument>(TPageArgument argument) where TViewModel : PageViewModelBase<TPageArgument>
            => await NavigateTo<TViewModel>((Page page) => ((TViewModel)page.BindingContext).Initialize(argument));

        public async Task NavigateModallyTo<TViewModel>() where TViewModel : PageViewModelBase
        {
            try
            {
                var page = viewModelLocator.CreatePage(typeof(TViewModel)); 
                var init = ((PageViewModelBase)page.BindingContext).Initialize();
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(page), true);
                await init;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public async Task PopModal() =>
            await Application.Current.MainPage.Navigation.PopModalAsync(true);

        public async Task Pop()
        {
            var (hasNavigation, navigationPage) = GetNavigationPage();
            if (hasNavigation)
            {
                await navigationPage!.PopAsync(true);
            }
        }

        private async Task NavigateTo<TViewModel>(Func<Page, Task> initialise)
        {
            try
            {
                var page = viewModelLocator.CreatePage(typeof(TViewModel));
                var init = initialise(page);
                var (hasNavigation, navigationPage) = GetNavigationPage();
                if (hasNavigation)
                {
                    await navigationPage!.PushAsync(page, true);
                }
                else
                {
                    Application.Current.MainPage = page;
                }

                await init;
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        // This is specific for the structure of the current application, not too proud of this one 🙈
        private static (bool hasNavigation, NavigationPage? navigationPage) GetNavigationPage() =>
            ((TabbedPage)Application.Current.MainPage)?.CurrentPage is NavigationPage navigationPage
                ? (true, navigationPage)
                : ((bool hasNavigation, NavigationPage? navigationPage))(false, default);
    }
}
