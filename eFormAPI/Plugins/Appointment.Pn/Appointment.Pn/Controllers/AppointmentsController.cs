using System.Threading.Tasks;
using Appointment.Pn.Abstractions;
using Appointment.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace Appointment.Pn.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentsService _appointmentsService;

        public AppointmentsController(IAppointmentsService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet]
        [Route("api/appointment-pn/appointments")]
        public async Task<OperationDataResult<AppointmentsListModel>> GetAppointmentsList(AppointmentRequestModel requestModel)
        {
            return await _appointmentsService.GetAppointmentsList(requestModel);
        }

        [HttpGet]
        [Route("api/appointment-pn/appointments/{id}")]
        public async Task<OperationDataResult<AppointmentModel>> GetAppointment(int id)
        {
            return await _appointmentsService.GetAppointment(id);
        }

        [HttpPost]
        [Route("api/appointment-pn/appointments")]
        public async Task<OperationResult> CreateAppointment([FromBody] AppointmentModel createModel)
        {
            return await _appointmentsService.CreateAppointment(createModel);
        }

        [HttpPut]
        [Route("api/appointment-pn/appointments")]
        public async Task<OperationResult> UpdateAppointment([FromBody] AppointmentModel updateModel)
        {
            return await _appointmentsService.UpdateAppointment(updateModel);
        }

        [HttpDelete]
        [Route("api/appointment-pn/appointments/{id}")]
        public async Task<OperationResult> DeleteAppointment(int id)
        {
            return await _appointmentsService.DeleteAppointment(id);
        }
    }
}