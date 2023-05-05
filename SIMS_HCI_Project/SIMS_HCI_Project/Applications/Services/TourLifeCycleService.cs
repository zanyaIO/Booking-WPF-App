﻿using SIMS_HCI_Project.Domain.Models;
using SIMS_HCI_Project.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMS_HCI_Project.Applications.Services
{
    public class TourLifeCycleService
    {
        private readonly ITourTimeRepository _tourTimeRepository;
        private readonly IGuestTourAttendanceRepository _guestTourAttendanceRepository;
        private readonly ITourReservationRepository _tourReservationRepository;
        private readonly ITourVoucherRepository _tourVoucherRepository;
        private readonly int DefaultExpirationDays = 10;


        public TourLifeCycleService() 
        {
            _tourTimeRepository = Injector.Injector.CreateInstance<ITourTimeRepository>();
            _guestTourAttendanceRepository = Injector.Injector.CreateInstance<IGuestTourAttendanceRepository>();
            _tourReservationRepository = Injector.Injector.CreateInstance<ITourReservationRepository>();
            _tourVoucherRepository = Injector.Injector.CreateInstance<ITourVoucherRepository>();
        }

        public void StartTour(TourTime tourTime)
        {
            if (!tourTime.IsStartable || _tourTimeRepository.HasTourInProgress(tourTime.Tour.GuideId)) return;

            tourTime.Status = TourStatus.IN_PROGRESS;
            _tourTimeRepository.Update(tourTime);

            List<GuestTourAttendance> generatedAttendances = new List<GuestTourAttendance>();
            foreach (TourReservation tourReservation in _tourReservationRepository.GetAllByTourTimeId(tourTime.Id))
            {
                GuestTourAttendance newAttendance = new GuestTourAttendance(tourReservation.Guest2Id, tourTime.Id);
                newAttendance.TourTime = tourTime;
                generatedAttendances.Add(newAttendance);
            }
            _guestTourAttendanceRepository.AddMultiple(generatedAttendances);
        }

        public void MoveToNextKeyPoint(TourTime tourTime)
        {
            if (tourTime.IsAtLastKeyPoint)
            {
                EndTour(tourTime);
            }
            else
            {
                tourTime.CurrentKeyPointIndex++;
                tourTime.CurrentKeyPoint = tourTime.Tour.KeyPoints[tourTime.CurrentKeyPointIndex];
                _tourTimeRepository.Update(tourTime);
            }
        }

        public void EndTour(TourTime tourTime)
        {
            tourTime.Status = TourStatus.COMPLETED;
            _tourTimeRepository.Update(tourTime);

            List<GuestTourAttendance> attendancesToUpdate = _guestTourAttendanceRepository.GetAllByTourId(tourTime.Id);
            foreach (GuestTourAttendance guestTourAttendance in attendancesToUpdate)
            {
                if (guestTourAttendance.Status != AttendanceStatus.PRESENT)
                {
                    guestTourAttendance.Status = AttendanceStatus.NEVER_SHOWED_UP;
                }
            }
            _guestTourAttendanceRepository.BulkUpdate(attendancesToUpdate);
        }

        public void CancelTour(TourTime tourTime)
        {
            if (!tourTime.IsCancellable) return;

            tourTime.Status = TourStatus.CANCELED;
            _tourTimeRepository.Update(tourTime);

            List<TourReservation> tourReservationsToCancel = _tourReservationRepository.GetAllByTourTimeId(tourTime.Id);
            List<TourVoucher> givenTourVouchers = new List<TourVoucher>();
            foreach (TourReservation tourReservation in tourReservationsToCancel)
            {
                tourReservation.Status = TourReservationStatus.CANCELLED;
                givenTourVouchers.Add(new TourVoucher(tourReservation.Guest2Id, "EXTRAVOUCHER777", DateTime.Now, DateTime.Now.AddDays(DefaultExpirationDays)));
            }
            _tourReservationRepository.BulkUpdate(tourReservationsToCancel);
            _tourVoucherRepository.AddMultiple(givenTourVouchers);
        }
    }
}