using System;

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
    }
}