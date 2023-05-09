﻿using SIMS_HCI_Project.Domain.DTOs;
using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.WPF.ViewModels.GuideViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SIMS_HCI_Project.WPF.Views.GuideViews
{
    /// <summary>
    /// Interaction logic for TourRequestDatePickerView.xaml
    /// </summary>
    public partial class TourRequestDatePickerView : Window
    {
        public TourRequestDatePickerView(TourRequestsViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();

            //List<DateTime> blackoutDates = vm.UnavailableDates.ToList();

            calDepartureTimePicker.DisplayDateStart = vm.SelectedTourRequest.DateRange.Start < DateTime.Now ? DateTime.Now : vm.SelectedTourRequest.DateRange.Start;
            calDepartureTimePicker.DisplayDateEnd = vm.SelectedTourRequest.DateRange.End;

            /*calDepartureTimePicker.BlackoutDates.Clear();

            foreach (DateTime blackoutDate in blackoutDates)
            {
                calDepartureTimePicker.BlackoutDates.Add(new CalendarDateRange(blackoutDate));
            }*/
        }
    }
}
