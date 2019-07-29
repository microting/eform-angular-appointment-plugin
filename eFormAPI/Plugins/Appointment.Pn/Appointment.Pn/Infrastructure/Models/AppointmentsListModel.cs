using System.Collections.Generic;

namespace Appointment.Pn.Infrastructure.Models
{
    public class AppointmentsListModel
    {
        public int Total { get; set; }
        public List<AppointmentModel> Appointments { get; set; }

        public AppointmentsListModel()
        {
            Appointments = new List<AppointmentModel>();
        }
    }
}