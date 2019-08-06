using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Appointment.Pn.Abstractions;
using Appointment.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microting.AppointmentBase.Infrastructure.Data;
using Microting.AppointmentBase.Infrastructure.Data.Enums;
using Microting.eForm.Infrastructure.Constants;
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
        )
        {
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
                        Info = x.Info,
                        ColorHex = x.ColorHex,
                        RepeatUntil = x.RepeatUntil,
                        RepeatEvery = x.RepeatEvery,
                        RepeatType = x.RepeatType
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

        public async Task<OperationDataResult<AppointmentsListModel>> GetAppointmentsList(AppointmentRequestModel requestModel)
        {
            try
            {
                Debugger.Break();
                await UpdateRecurringAppointments();

                var list = await _dbContext.Appointments
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed
                                && requestModel.EndDate > x.StartAt
                                && (requestModel.StartDate < x.StartAt
                                    || x.RepeatType != null 
                                        && x.NextId == null
                                        && (x.RepeatUntil == null || x.RepeatUntil > requestModel.StartDate)))
                    .Select(x => new AppointmentSimpleModel()
                    {
                        Id = x.Id,
                        StartAt = x.StartAt,
                        ExpireAt = x.ExpireAt,
                        Title = x.Title,
                        ColorHex = x.ColorHex,
                        RepeatUntil = x.RepeatUntil,
                        RepeatEvery = x.RepeatEvery,
                        RepeatType = x.RepeatType,
                        NextId = x.NextId
                    }
                    ).ToListAsync();

                var listModel = new AppointmentsListModel { Total = list.Count(), Appointments = list };

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
                    if (appointmentModel.ExpireAt <= appointmentModel.StartAt || appointmentModel.StartAt <= DateTime.UtcNow)
                    {
                        return new OperationResult(
                            false,
                            _appointmentLocalizationService.GetString("AppointmentDateNotCorrect"));
                    }

                    var appointment = new Entities.Appointment
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUserId = UserId,
                        ExpireAt = appointmentModel.ExpireAt,
                        StartAt = appointmentModel.StartAt,
                        Info = appointmentModel.Info,
                        Description = appointmentModel.Description,
                        Title = appointmentModel.Title,
                        ColorHex = appointmentModel.ColorHex,
                        RepeatEvery = appointmentModel.RepeatEvery,
                        RepeatType = appointmentModel.RepeatType,
                        RepeatUntil = appointmentModel.RepeatUntil
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
                    var appointment = await _dbContext.Appointments.FindAsync(appointmentModel.Id);

                    if (appointment.StartAt <= DateTime.UtcNow)
                    {
                        return new OperationResult(
                            false,
                            _appointmentLocalizationService.GetString("CannotEditAppointment"));
                    }

                    if (appointmentModel.ExpireAt <= appointmentModel.StartAt || appointmentModel.StartAt <= DateTime.UtcNow)
                    {
                        return new OperationResult(
                            false,
                            _appointmentLocalizationService.GetString("AppointmentDateNotCorrect"));
                    }

                    appointment.UpdatedAt = DateTime.UtcNow;
                    appointment.UpdatedByUserId = UserId;
                    appointment.ExpireAt = appointmentModel.ExpireAt;
                    appointment.StartAt = appointmentModel.StartAt;
                    appointment.Info = appointmentModel.Info;
                    appointment.Description = appointmentModel.Description;
                    appointment.Title = appointmentModel.Title;
                    appointment.ColorHex = appointmentModel.ColorHex;
                    appointment.RepeatEvery = appointmentModel.RepeatEvery;
                    appointment.RepeatType = appointmentModel.RepeatType;
                    appointment.RepeatUntil = appointmentModel.RepeatUntil;

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
                    var appointment = await _dbContext.Appointments.FindAsync(id);

                    if (appointment.StartAt < DateTime.UtcNow)
                    {
                        return new OperationResult(
                            false,
                            _appointmentLocalizationService.GetString("CannotDeleteAppointment"));
                    }

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

        private async Task UpdateRecurringAppointments()
        {
            var recurringAppointments = await _dbContext.Appointments.Where(x => 
                    x.WorkflowState != Constants.WorkflowStates.Removed
                    && x.RepeatEvery != null
                    && x.StartAt != null
                    && x.RepeatType != null
                    && x.NextId == null
                    && (x.RepeatUntil == null || x.RepeatUntil > DateTime.UtcNow)
                ).ToListAsync();

            foreach (var appointment in recurringAppointments)
            {
                var prevAppointment = appointment;

                var duration = prevAppointment.ExpireAt - prevAppointment.StartAt;
                var repeatEvery = prevAppointment.RepeatEvery.GetValueOrDefault();
                var repeatType = prevAppointment.RepeatType.GetValueOrDefault();
                var repeatUntil = prevAppointment.RepeatUntil;
                var prevDate = prevAppointment.StartAt.GetValueOrDefault();
                var nextDate = GetNextAppointmentDate(prevDate, repeatType, repeatEvery);

                while ((repeatUntil == null || nextDate <= repeatUntil) && prevDate <= DateTime.UtcNow)
                {
                    var nextAppointment = new Entities.Appointment
                    {
                        CreatedByUserId = UserId,
                        ExpireAt = nextDate + duration,
                        StartAt = nextDate,
                        Info = prevAppointment.Info,
                        Description = prevAppointment.Description,
                        Title = prevAppointment.Title,
                        ColorHex = prevAppointment.ColorHex,
                        RepeatEvery = prevAppointment.RepeatEvery,
                        RepeatType = prevAppointment.RepeatType,
                        RepeatUntil = prevAppointment.RepeatUntil
                    };

                    await nextAppointment.Create(_dbContext);

                    prevAppointment.NextId = nextAppointment.Id;
                    await prevAppointment.Update(_dbContext);

                    prevDate = nextDate;
                    nextDate = GetNextAppointmentDate(prevDate, repeatType, repeatEvery);
                    prevAppointment = nextAppointment;
                }
            }
        }

        private DateTime GetNextAppointmentDate(DateTime prevDate, RepeatType repeatType, int repeatEvery)
        {
            switch (repeatType)
            {
                case RepeatType.Month:
                    return prevDate.AddMonths(repeatEvery);
                case RepeatType.Week:
                    return prevDate.AddDays(repeatEvery * 7);
                default:
                    return prevDate.AddDays(repeatEvery);
            }
        }
    }
}