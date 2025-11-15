using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Linq;
using FreeWinBackup.Models;

namespace FreeWinBackup.Services
{
    public class ServiceControlService
    {
        private readonly LoggingService _loggingService;

        public ServiceControlService()
        {
            _loggingService = new LoggingService();
        }

        public void StopServices(List<string> serviceNames, Guid scheduleId, string scheduleName)
        {
            if (serviceNames == null || !serviceNames.Any())
                return;

            foreach (var serviceName in serviceNames)
            {
                try
                {
                    using (var service = new ServiceController(serviceName))
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                            
                            _loggingService.Log(new LogEntry
                            {
                                ScheduleId = scheduleId,
                                ScheduleName = scheduleName,
                                Message = $"Service '{serviceName}' stopped successfully",
                                Level = LogLevel.Info,
                                IsSuccess = true
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.Log(new LogEntry
                    {
                        ScheduleId = scheduleId,
                        ScheduleName = scheduleName,
                        Message = $"Failed to stop service '{serviceName}': {ex.Message}",
                        Level = LogLevel.Error,
                        IsSuccess = false
                    });
                }
            }
        }

        public void StartServices(List<string> serviceNames, Guid scheduleId, string scheduleName)
        {
            if (serviceNames == null || !serviceNames.Any())
                return;

            foreach (var serviceName in serviceNames)
            {
                try
                {
                    using (var service = new ServiceController(serviceName))
                    {
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                            
                            _loggingService.Log(new LogEntry
                            {
                                ScheduleId = scheduleId,
                                ScheduleName = scheduleName,
                                Message = $"Service '{serviceName}' started successfully",
                                Level = LogLevel.Info,
                                IsSuccess = true
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.Log(new LogEntry
                    {
                        ScheduleId = scheduleId,
                        ScheduleName = scheduleName,
                        Message = $"Failed to start service '{serviceName}': {ex.Message}",
                        Level = LogLevel.Error,
                        IsSuccess = false
                    });
                }
            }
        }
    }
}
