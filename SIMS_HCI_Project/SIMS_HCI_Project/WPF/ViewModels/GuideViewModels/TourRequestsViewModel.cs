﻿using SIMS_HCI_Project.Applications.Services;
using SIMS_HCI_Project.Domain.DTOs;
using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.WPF.Commands;
using SIMS_HCI_Project.WPF.Commands.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIMS_HCI_Project.WPF.ViewModels.GuideViewModels
{
    public class TourRequestsViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Commands
        public RelayCommand FilterRequests { get; set; }
        public RelayCommand ResetFilter { get; set; }
        public RelayCommand AcceptRequest { get; set; }
        public RelayCommand ConfirmPickedDate { get; set; }
        public GuideNavigationCommands NavigationCommands { get; set; }
        #endregion

        private RegularTourRequestService _regularTourRequestService;
        private TourService _tourService;

        private ObservableCollection<RegularTourRequest> _tourRequests;
        public ObservableCollection<RegularTourRequest> TourRequests
        {
            get { return _tourRequests; }
            set
            {
                _tourRequests = value;
                OnPropertyChanged();
            }
        }
        public RegularTourRequest SelectedTourRequest { get; set; }

        private Location _location;
        public Location Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }
        public int _guestNumber;
        public int GuestNumber
        {
            get { return _guestNumber; }
            set
            {
                _guestNumber = value;
                OnPropertyChanged();
            }
        }
        public string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }
        public DateRange _dateRange;
        public DateRange DateRange
        {
            get { return _dateRange; }
            set
            {
                _dateRange = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _availableLocations;
        public ObservableCollection<Location> AvailableLocations
        {
            get { return _availableLocations; }
            set
            {
                _availableLocations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _availableLanguages;
        public ObservableCollection<string> AvailableLanguages
        {
            get { return _availableLanguages; }
            set
            {
                _availableLanguages = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DateTime> UnavailableDates { get; set; }
        public Tour Tour { get; set; }
        public DateTime PickedDate { get; set; }
        public DateTime PickedTime { get; set; }

        public TourRequestsViewModel()
        {
            _regularTourRequestService = new RegularTourRequestService();
            _tourService = new TourService();
            DateRange = new DateRange(DateTime.Now, DateTime.Now.AddMonths(6));
            PickedDate = DateTime.Now;  

            InitCommands();
            LoadRequests();
            LoadPossibleFilters();
        }

        private void InitCommands()
        {
            FilterRequests = new RelayCommand(ExecutedFilterRequestsCommand, CanExecuteCommand);
            ResetFilter = new RelayCommand(ExecutedResetFilterCommand, CanExecuteCommand);
            ConfirmPickedDate = new RelayCommand(ExecutedConfirmPickedDateCommand, CanExecuteCommand);
            AcceptRequest = new RelayCommand(ExecutedAcceptRequestCommand, CanExecuteCommand);

            NavigationCommands = new GuideNavigationCommands();
        }

        private void LoadRequests()
        {
            TourRequests = new ObservableCollection<RegularTourRequest>(_regularTourRequestService.GetAllValidByParams(Location, GuestNumber, Language, DateRange));
        }

        private void LoadPossibleFilters()
        {
            if (TourRequests != null)
            {
                AvailableLocations = new ObservableCollection<Location>(TourRequests.Select(t => t.Location).Distinct());
                AvailableLanguages = new ObservableCollection<string>(TourRequests.Select(t => t.Language).Distinct());
            }
        }

        private void ExecutedFilterRequestsCommand(object obj)
        {
            LoadRequests();
        }

        private void ExecutedResetFilterCommand(object obj)
        {
            Location = null;
            GuestNumber = 0;
            Language = null;
            //DateRange.Start = DateTime.Now;
            //DateRange.End = DateTime.Now.AddMonths(6);
            DateRange = new DateRange(DateTime.Now, DateTime.Now.AddMonths(6));

            LoadRequests();
        }

        private void ExecutedAcceptRequestCommand(object obj)
        {
            PickedDate = SelectedTourRequest.DateRange.Start;
        }
        
        private void ExecutedConfirmPickedDateCommand(object obj)
        {
            Tour = _regularTourRequestService.AcceptRequest(SelectedTourRequest, ((User)App.Current.Properties["CurrentUser"]).Id, new DateTime(PickedDate.Year, PickedDate.Month, PickedDate.Day, PickedTime.Hour, PickedTime.Minute, 0));
            if(Tour == null)
            {
                MessageBox.Show("You already have tour in that time slot.", "Acceptance failed", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            LoadRequests();
            LoadPossibleFilters();
        }

        private bool CanExecuteCommand(object obj)
        {
            return true;
        }
    }
}