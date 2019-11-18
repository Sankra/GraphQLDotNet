﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQLDotNet.Mobile.Models;
using GraphQLDotNet.Mobile.OpenWeather;
using GraphQLDotNet.Mobile.OpenWeather.Persistence;
using GraphQLDotNet.Mobile.ViewModels.Commands;
using GraphQLDotNet.Mobile.ViewModels.Common;
using GraphQLDotNet.Mobile.ViewModels.Messages;
using GraphQLDotNet.Mobile.ViewModels.Navigation;
using GraphQLDotNet.Mobile.Views;
using Xamarin.Forms;

namespace GraphQLDotNet.Mobile.ViewModels
{
    public class LocationsViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private readonly ILocalStorage localStorage;

        // TODO: Current location øverst
        // TODO: Laste data ved oppstart...
        public LocationsViewModel(INavigationService navigationService, ILocalStorage localStorage)
        {
            this.navigationService = navigationService;
            this.localStorage = localStorage;
            Title = "Locations";
            RefreshCommand = new AsyncCommand(ExecuteRefreshLocations);
            locations = new ObservableCollection<OrderedWeatherSummary>();

            // TODO: this warning suxx
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
            MessagingCenter.Subscribe<AddLocationViewModel, AddLocationMessage>(this, nameof(AddLocationMessage), async (obj, locationMessage) =>
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
            {
                try
                {
                    // TODO: Sjekk om finnes fra før før legges til
                    var summary = await OpenWeatherClient.GetWeatherSummaryFor(locationMessage.Id);
                    var orderedSummary = new OrderedWeatherSummary(summary, Locations.Count);
                    Locations.Add(orderedSummary);
                    await localStorage.Save(Locations);
                }
                catch (Exception)
                {
                    // TODO: Error?!?
                    return;
                }
            });
        }

        public IAsyncCommand AddLocationCommand => new AsyncCommand(
            async () => await navigationService.NavigateModallyTo<AddLocationViewModel>());

        public AsyncCommand<OrderedWeatherSummary> RemoveLocationCommand => new AsyncCommand<OrderedWeatherSummary>(
            async (OrderedWeatherSummary weatherSummaryToDelete) =>
            {
                if (Locations.Remove(weatherSummaryToDelete))
                {
                    await localStorage.Save(Locations);
                }
            });

        public IAsyncCommand RefreshCommand { get; }

        ObservableCollection<OrderedWeatherSummary> locations;
        public ObservableCollection<OrderedWeatherSummary> Locations
        {
            get { return locations; }
            set { SetProperty(ref locations, value); }
        }

        bool isRefreshing;        
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public async override Task Initialize()
        {
            var weatherSummaries = localStorage.Load();
            Locations = new ObservableCollection<OrderedWeatherSummary>(weatherSummaries.OrderBy(w => w.Ordering));
            if (weatherSummaries.Length == 0)
            {
                return;
            }

            await ExecuteRefreshLocations();
        }

        async Task ExecuteRefreshLocations()
        {
            try
            {
                if (Locations.Count == 0)
                {
                    return;
                }

                var weatherSummaries = await OpenWeatherClient.GetWeatherUpdatesFor(Locations.Select(w => w.Id));
                if (!weatherSummaries.Any())
                {
                    return;
                }

                var updatedWeather =
                    from orderedWeatherSummary in Locations
                    join summary in weatherSummaries on orderedWeatherSummary.Id equals summary.Id
                    orderby orderedWeatherSummary.Ordering
                    select orderedWeatherSummary.UpdateWeather(summary);
                // TODO: Save updated values here or wait for app shutdown??
                Locations = new ObservableCollection<OrderedWeatherSummary>(updatedWeather);
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
