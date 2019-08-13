using System.Threading.Tasks;
using Appointment.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace Appointment.Pn.Abstractions
{
    public interface IAppointmentsService
    {
        Task<OperationDataResult<AppointmentModel>> GetAppointment(int id);
        Task<OperationDataResult<AppointmentsListModel>> GetAppointmentsList(AppointmentRequestModel requestModel);
        Task<OperationResult> UpdateAppointment(AppointmentModel appointmentModel);
        Task<OperationResult> CreateAppointment(AppointmentModel appointmentModel);
        Task<OperationResult> DeleteAppointment(int id);
    }
}