using System;
using Microting.AppointmentBase.Infrastructure.Data.Enums;

namespace Appointment.Pn.Infrastructure.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }

        public DateTime? StartAt { get; set; }
        public DateTime? ExpireAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Info { get; set; }
        public string ColorHex { get; set; }

        public int? RepeatEvery { get; set; }
        public RepeatType? RepeatType { get; set; }
        public DateTime? RepeatUntil { get; set; }
    }
}