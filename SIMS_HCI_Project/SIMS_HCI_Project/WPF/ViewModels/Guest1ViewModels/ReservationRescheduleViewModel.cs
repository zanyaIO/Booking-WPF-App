﻿using SIMS_HCI_Project.Applications.Services;
using SIMS_HCI_Project.Controller;
using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.WPF.Commands;
using SIMS_HCI_Project.WPF.Views.Guest1Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SIMS_HCI_Project.WPF.ViewModels.Guest1ViewModels
{
    internal class ReservationRescheduleViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private AccommodationReservationService _accommodationReservationService;
        private RescheduleRequestService _rescheduleRequestService;
        public ReservationRescheduleView ReservationRescheduleView { get; set; }
        public Guest1MainView Guest1MainView { get; set; }
        public ReservationsView ReservationsView { get; set; }
        public AccommodationReservation Reservation { get; set; }
        public ObservableCollection<RescheduleRequest> RescheduleRequests { get; set; }
        public RelayCommand SendReservationRescheduleRequestCommand { get; set; }

        private DateTime _wantedStart;
        public DateTime WantedStart
        {
            get => _wantedStart;
            set
            {
                if (value != _wantedStart)
                {
                    _wantedStart = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime _wantedEnd;
        public DateTime WantedEnd
        {
            get => _wantedEnd;
            set
            {
                if (value != _wantedEnd)
                {
                    _wantedEnd = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ReservationRescheduleViewModel(ReservationRescheduleView reservationRescheduleView, AccommodationReservationService reservationService, AccommodationReservation reservation)
        {
            _accommodationReservationService = reservationService;
            _rescheduleRequestService = new RescheduleRequestService(); 
            ReservationRescheduleView = reservationRescheduleView;
            Reservation = reservation;
            RescheduleRequests = new ObservableCollection<RescheduleRequest>(_rescheduleRequestService.GetAllByOwnerId(Reservation.Accommodation.OwnerId));
            WantedStart = DateTime.Now.AddDays(1);
            WantedEnd = DateTime.Now.AddDays(Reservation.Accommodation.MinimumReservationDays + 1);
            InitCommands();
        }
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "WantedStart" || columnName == "WantedEnd")
                {
                    result = PassedDayErrorMessage(WantedStart);
                    if(result.Equals(" "))
                        result = DateRangeErrorMessage();
                }
                return result;
            }
        }
        private string DateRangeErrorMessage()
        {
            bool isDateRangeValid = ((WantedEnd - WantedStart).TotalDays < Reservation.Accommodation.MinimumReservationDays - 1);
            return isDateRangeValid ? "Date range should be bigger, because of days for reseration" : " ";
        }

        private string PassedDayErrorMessage(DateTime date)
        {
            return (date <= DateTime.Now) ? "Start cannot be a day that has already passed" : " ";
        }

        private readonly string[] _validatedProperties = { "WantedStart", "WantedEnd" };

        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    if (this[property] != null)
                        return false;
                }

                return true;
            }
        }
        public void ExecutedSendReservationRescheduleRequestCommand(object obj)
        {
            MessageBoxResult result = ConfirmRescheduleRequest();
            if (result == MessageBoxResult.Yes && IsValid)
            {
               _rescheduleRequestService.Add(new RescheduleRequest(Reservation, WantedStart, WantedEnd));
                ReservationRescheduleView.ReservationRescheduleFrame.Content = new ReservationsView(_accommodationReservationService, Reservation.Guest);

            }
        }
        private MessageBoxResult ConfirmRescheduleRequest()
        {
            string sMessageBoxText = $"This reservation will be rescheduled, are you sure?";
            string sCaption = "Reschedule Confirm";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        public bool CanExecute(object obj)
        {
            return true;
        }
        public void InitCommands()
        {
            SendReservationRescheduleRequestCommand = new RelayCommand(ExecutedSendReservationRescheduleRequestCommand, CanExecute);
        }        
    }
}
