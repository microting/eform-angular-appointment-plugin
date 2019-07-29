using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Appointment.Pn.Abstractions;
using Appointment.Pn.Infrastructure.Models;
using eFormShared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microting.AppointmentBase.Infrastructure.Data;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Entities = Microting.AppointmentBase.Infrastructure.Data.Entities;

namespace Appointment.Pn.Services
{
    public class AppointmentsService : IAppointmentsService
    {
        private readonly IAppointmentLocalizationService _appointmentLocalizationService;
        private readonly AppointmentPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentsService(
            AppointmentPnDbContext dbContext, 
            IAppointmentLocalizationService appointmentLocalizationService, 
            IHttpContextAccessor httpContextAccessor, 
            IEFormCoreService coreHelper
        ) {
            _dbContext = dbContext;
            _appointmentLocalizationService = appointmentLocalizationService;
            _httpContextAccessor = httpContextAccessor;
            _coreHelper = coreHelper;
        }

        public async Task<OperationDataResult<AppointmentModel>> GetAppointment(int id)
        {
            try
            {
                var appointmentModel = await _dbContext.Appointments
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.Id == id)
                    .Select(x => new AppointmentModel
                        {
                            Id = x.Id,
                            StartAt = x.StartAt,
                            ExpireAt = x.ExpireAt,
                            Title = x.Title,
                            Description = x.Description,
                            Info = x.Info
                        }
                    ).FirstOrDefaultAsync();

                if (appointmentModel == null)
                {
                    return new OperationDataResult<AppointmentModel>(
                        false,
                        _appointmentLocalizationService.GetString("AppointmentNotFound"));
                }

                return new OperationDataResult<AppointmentModel>(true, appointmentModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<AppointmentModel>(false,
                    _appointmentLocalizationService.GetString("ErrorGettingAppointment"));
            }
        }

        public async Task<OperationDataResult<AppointmentsListModel>> GetAppointmentsList()
        {
            try
            {
                var list = await _dbContext.Appointments
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                    .Select(x => new AppointmentModel
                        {
                            Id = x.Id,
                            StartAt = x.StartAt,
                            ExpireAt = x.ExpireAt,
                            Title = x.Title,
                            Description = x.Description,
                            Info = x.Info
                        }
                    ).ToListAsync();

                var listModel = new AppointmentsListModel {Total = list.Count(), Appointments = list};

                return new OperationDataResult<AppointmentsListModel>(true, listModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<AppointmentsListModel>(false,
                    _appointmentLocalizationService.GetString("ErrorGettingAppointmentsList"));
            }
        }

        public async Task<OperationResult> CreateAppointment(AppointmentModel appointmentModel)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var appointment = new Entities.Appointment
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUserId = UserId,
                        ExpireAt = appointmentModel.ExpireAt,
                        StartAt = appointmentModel.StartAt,
                        Info = appointmentModel.Info,
                        Description = appointmentModel.Description,
                        Title = appointmentModel.Title
                    };

                    await appointment.Create(_dbContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _appointmentLocalizationService.GetString("AppointmentCreatedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false,
                        _appointmentLocalizationService.GetString("ErrorWhileCreatingAppointment"));
                }
            }
        }

        public async Task<OperationResult> UpdateAppointment(AppointmentModel appointmentModel)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var appointment = new Entities.Appointment
                    {
                        Id = appointmentModel.Id,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUserId = UserId,
                        ExpireAt = appointmentModel.ExpireAt,
                        StartAt = appointmentModel.StartAt,
                        Info = appointmentModel.Info,
                        Description = appointmentModel.Description,
                        Title = appointmentModel.Title
                    };
                    await appointment.Update(_dbContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _appointmentLocalizationService.GetString("AppointmentUpdatedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _appointmentLocalizationService.GetString("ErrorWhileUpdatingAppointment"));
                }
            }
        }

        public async Task<OperationResult> DeleteAppointment(int id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var appointment = new Entities.Appointment
                    {
                        Id = id
                    };
                    await appointment.Delete(_dbContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _appointmentLocalizationService.GetString("AppointmentDeletedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _appointmentLocalizationService.GetString("ErrorWhileDeletingAppointment"));
                }
            }
        }

        private int UserId
        {
            get
            {
                var value = _httpContextAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }
    }
}