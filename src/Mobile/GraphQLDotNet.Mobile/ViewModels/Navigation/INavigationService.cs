﻿using System.Threading.Tasks;
using GraphQLDotNet.Mobile.ViewModels.Common;

namespace GraphQLDotNet.Mobile.ViewModels.Navigation
{
    public interface INavigationService
    {
        Task NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
        Task NavigateTo<TViewModel, TPageArgument>(TPageArgument argument) where TViewModel : ViewModelBase;
        Task NavigateModallyTo<TViewModel>() where TViewModel : ViewModelBase;
        Task Pop();
        Task PopModal();
    }
}
