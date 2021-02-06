using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodolistScheduleService.Jobs;

namespace TodolistScheduleService.Schedulers
{

    public class SchedulerMultiple
    {
        IScheduler _scheduler;
        IJobDetail _jobDaily;
        ITrigger _triggerDaily;

        IJobDetail _jobWeekly;
        ITrigger _triggerWeekly;

        IJobDetail _jobMonthly;

        ITrigger _triggerMonthly;

        IJobDetail _jobDispatch;
        ITrigger _triggerDisaptch;

        IJobDetail _jobTodo;
        ITrigger _triggerTodo;

        public async Task StartAllJob()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();
        }

        public async Task StartDaily(int hour, int minute)
        {


            _jobDaily = JobBuilder.Create<SendMailJob>().Build();

            _triggerDaily = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                  )

                .Build();

            await _scheduler.ScheduleJob(_jobDaily, _triggerDaily);
        }

        public async Task StartWeekly(DayOfWeek dayOfWeek, int hour, int minute)
        {
            _jobWeekly = JobBuilder.Create<SendMailWeeklyJob>().Build();

            _triggerWeekly = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                    s.WithInterval(1, IntervalUnit.Week)
                    .OnDaysOfTheWeek(dayOfWeek)
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                  )
                .Build();
            await _scheduler.ScheduleJob(_jobWeekly, _triggerWeekly);
        }
        public async Task StartMonthly(int hour, int minute)
        {
            _jobMonthly = JobBuilder.Create<SendMailMonthlyJob>().Build();
            _triggerMonthly = TriggerBuilder.Create()
                .WithCronSchedule
                  ($"0 {minute} {hour} L * ?") // Meaning: Fire at 10:15am on the last day of every month
                .Build();
            await _scheduler.ScheduleJob(_jobMonthly, _triggerMonthly);
        }
        public async Task StartDisaptch(int repeatMinute, TimeSpan startHourAt, TimeSpan endHourAt)
        {

            _jobDispatch = JobBuilder.Create<ReloadDispatchJob>().Build();
            var st = DateTime.Now.Date.Add(startHourAt);
            var end = DateTimeOffset.Now.Date.Add(endHourAt);
            Console.WriteLine(st);
            _triggerDisaptch = TriggerBuilder.Create()
                        .StartAt(st)
                        .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(repeatMinute))
                        .EndAt(end)
                        .Build();

            await _scheduler.ScheduleJob(_jobDispatch, _triggerDisaptch);
        }
        public async Task StartTodo(int repeatMinute, TimeSpan startHourAt, TimeSpan endHourAt)
        {
            _jobTodo = JobBuilder.Create<ReloadTodoJob>().Build();
            var st = DateTime.Now.Date.Add(startHourAt);
            var end = DateTimeOffset.Now.Date.Add(endHourAt);
            Console.WriteLine(st);
            _triggerTodo = TriggerBuilder.Create()
                        .StartAt(st)
                        .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(repeatMinute))
                        .EndAt(end)
                        .Build();
            await _scheduler.ScheduleJob(_jobTodo, _triggerTodo);
        }
        public async Task<bool> checkScheduleStart()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return _scheduler.IsStarted;
        }
        public async Task UpdateDailyTrigger(int hour, int minute)
        {
            _triggerDaily = TriggerBuilder.Create()
               .WithDailyTimeIntervalSchedule
                 (s =>
                    s.WithIntervalInHours(24)
                   .OnEveryDay()
                   .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                 )

               .Build();
            await _scheduler.RescheduleJob(_triggerDaily.Key, _triggerDaily);
        }
        public async Task UpdateWeeklyTrigger(DayOfWeek dayOfWeek, int hour, int minute)
        {
            _triggerWeekly = TriggerBuilder.Create()
               .WithDailyTimeIntervalSchedule
                 (s =>
                   s.WithInterval(1, IntervalUnit.Week)
                   .OnDaysOfTheWeek(dayOfWeek)
                   .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                 )
               .Build();
            await _scheduler.RescheduleJob(_triggerWeekly.Key, _triggerWeekly);
        }

        public async Task UpdateMonthlyTrigger(int hour, int minute)
        {
            _triggerMonthly = TriggerBuilder.Create()
                .WithCronSchedule
                  ($"0 {minute} {hour} L * ?") // Meaning: Fire at 10:15am on the last day of every month
                .Build();
            await _scheduler.RescheduleJob(_triggerMonthly.Key, _triggerMonthly);
        }
        public async Task Stop()
        {
            if (_scheduler.IsStarted)
            {
                await _scheduler.Shutdown();
            }
        }
    }
}
