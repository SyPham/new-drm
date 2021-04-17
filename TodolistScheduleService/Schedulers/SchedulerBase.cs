using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TodolistScheduleService.Dto;
using TodolistScheduleService.Jobs;

namespace TodolistScheduleService.Schedulers
{

    public abstract class KeyType
    {
        public string Name { get; set; }
        public string Group { get; set; }
    }
    public class SchedulerBase<TClass> where TClass : IJob
    {
        IScheduler _scheduler;

        /// <summary>
        /// Bắt đầu lập lịch
        /// </summary>
        /// <returns></returns>
        public async Task StartAllJob()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();
        }
        public async Task Start(IntervalUnit intervalUnit, int hour, int minute)
        {

            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();
            var _job = JobBuilder.Create<TClass>().Build();

            var _trigger = TriggerBuilder.Create()
                 .WithDailyTimeIntervalSchedule
                   (s =>
                     s.WithInterval(1, intervalUnit)
                     .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                   )
                 .Build();
            await _scheduler.ScheduleJob(_job, _trigger);
        }
        public async Task Start(IntervalUnit intervalUnit, DayOfWeek dayofWeek, int hour, int minute)
        {

            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();
            var _job = JobBuilder.Create<TClass>().Build();

            var _trigger = TriggerBuilder.Create()
                 .WithDailyTimeIntervalSchedule
                   (s =>
                     s.WithInterval(1, intervalUnit)
                     .OnDaysOfTheWeek(dayofWeek)
                     .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                   )
                 .Build();
            await _scheduler.ScheduleJob(_job, _trigger);
        }
        public async Task Start(int repeatMinute, TimeSpan startHourAt, TimeSpan endHourAt)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await _scheduler.Start();

            var _job = JobBuilder.Create<TClass>()
                .Build();
            var st = DateTime.Now.Date.Add(startHourAt);
            var end = DateTimeOffset.Now.Date.Add(endHourAt);
            var _trigger = TriggerBuilder.Create()
                        .StartAt(st)
                        .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(repeatMinute))
                        .EndAt(end)
                        .Build();
            await _scheduler.ScheduleJob(_job, _trigger);
        }

        /// <summary>
        /// Lập lịch hàng ngày
        /// </summary>
        /// <param name="hour">Giờ</param>
        /// <param name="minute">Phút</param>
        /// <param name="map">Các tham số muốn tiêm vào JobDetail</param>
        /// <returns></returns>
        public async Task StartDaily(int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);
            var jobKey = new JobKey(data.Name, data.Group);

            var jobDaily = JobBuilder.Create<TClass>()
                                .WithIdentity(jobKey)
                                .SetJobData(new JobDataMap(map))
                                .Build();

            var triggerDaily = TriggerBuilder.Create()
                 .WithIdentity(triggerKey)
                 .WithDailyTimeIntervalSchedule
                   (s =>
                      s.WithIntervalInHours(24)
                     .OnEveryDay()
                     .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                   )

                 .Build();
            await _scheduler.ScheduleJob(jobDaily, triggerDaily);
            Console.WriteLine($"Add {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");

        }

        /// <summary>
        /// Lập lịch hàng tuần
        /// </summary>
        /// <param name="dayOfWeek">Ngày trong tuần</param>
        /// <param name="hour">Giờ</param>
        /// <param name="minute">Phút</param>
        /// <param name="map">Các tham số muốn tiêm vào JobDetail</param>
        /// <returns></returns>
        public async Task StartWeekly(DayOfWeek dayOfWeek, int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);
            var jobKey = new JobKey(data.Name, data.Group);
            var jobWeekly = JobBuilder.Create<TClass>()
                .WithIdentity(jobKey)
                            .SetJobData(new JobDataMap(map))
                            .Build();

            var triggerWeekly = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute))
                .Build();

            await _scheduler.ScheduleJob(jobWeekly, triggerWeekly);
            Console.WriteLine($"Add {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");

        }
        /// <summary>
        /// Lập lịch hàng tháng
        /// </summary>
        /// <param name="hour">Giờ</param>
        /// <param name="minute">Phút</param>
        /// <param name="map">Các tham số muốn tiêm vào JobDetail</param>
        /// <returns></returns>
        public async Task StartMonthly(int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);
            var jobKey = new JobKey(data.Name, data.Group);
            var jobMonthly = JobBuilder.Create<TClass>()
                .WithIdentity(jobKey)
                .SetJobData(new JobDataMap(map))
                .Build();

            var triggerMonthly = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule
                  ($"0 {minute} {hour} L * ?") // Meaning: Fire at 10:15am on the last day of every month
                .ForJob(jobMonthly.Key)
                .Build();
            await _scheduler.ScheduleJob(jobMonthly, triggerMonthly);
            Console.WriteLine($"Add {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");

        }

        public async Task StartDisaptch(int repeatMinute, TimeSpan startHourAt, TimeSpan endHourAt)
        {

            var jobDispatch = JobBuilder.Create<TClass>().Build();
            var st = DateTime.Now.Date.Add(startHourAt);
            var end = DateTimeOffset.Now.Date.Add(endHourAt);
            Console.WriteLine(st);
            var triggerDisaptch = TriggerBuilder.Create()
                         .StartAt(st)
                         .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(repeatMinute))
                         .EndAt(end)
                         .Build();

            await _scheduler.ScheduleJob(jobDispatch, triggerDisaptch);
        }
        public async Task StartTodo(int repeatMinute, TimeSpan startHourAt, TimeSpan endHourAt)
        {
            var jobTodo = JobBuilder.Create<TClass>().Build();
            var st = DateTime.Now.Date.Add(startHourAt);
            var end = DateTimeOffset.Now.Date.Add(endHourAt);
            Console.WriteLine(st);
            var triggerTodo = TriggerBuilder.Create()
                         .StartAt(st)
                         .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(repeatMinute))
                         .EndAt(end)
                         .Build();
            await _scheduler.ScheduleJob(jobTodo, triggerTodo);
        }


        /// <summary>
        /// Kiểm tra đã bắt đầu lập lịch
        /// </summary>
        /// <returns></returns>
        public async Task<bool> checkScheduleStart()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return _scheduler.IsStarted;
        }
        // You can't change (update) a job once it has been scheduled. 
        //You can only re-schedule it (with any changes you might want to make) or delete it and create a new one.
        public async Task UpdateDailyTrigger(int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);

            var jobKey = new JobKey(data.Name, data.Group);
            var trigger = await _scheduler.GetTrigger(triggerKey);


            var checkExists = await _scheduler.CheckExists(jobKey);
            if (checkExists)
            {

                var builder = trigger.GetTriggerBuilder();
                builder.StartNow();

                var newtriggerDaily = builder
                   .WithDailyTimeIntervalSchedule
                     (s =>
                        s.WithIntervalInHours(24)
                       .OnEveryDay()
                       .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute))
                     )
                   .Build();

                await _scheduler.RescheduleJob(trigger.Key, newtriggerDaily);
                Console.WriteLine($"Update {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");

            }
            else
            {
                await StartDaily(hour, minute, map);
            }
        }

        /// <summary>
        /// Cập nhật lịch hàng tuần
        /// </summary>
        /// <param name="dayOfWeek">Ngày trong tuần</param>
        /// <param name="hour">Giờ</param>
        /// <param name="minute">Phút</param>
        /// <param name="map">Các tham số muốn tiêm vào JobDetail</param>
        /// <returns></returns>
        public async Task UpdateWeeklyTrigger(DayOfWeek dayOfWeek, int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);

            var jobKey = new JobKey(data.Name, data.Group);
            var trigger = await _scheduler.GetTrigger(triggerKey);
            var job = await _scheduler.GetJobDetail(jobKey);
            var checkExists = await _scheduler.CheckExists(jobKey);
            if (checkExists)
            {
                var builderJob = job.GetJobBuilder();
                builderJob.UsingJobData(new JobDataMap(map));
                builderJob.Build();

                var builder = trigger.GetTriggerBuilder();
                builder.StartNow();
                var newtriggerWeekly = builder
                    .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute))
                   .Build();

                await _scheduler.RescheduleJob(trigger.Key, newtriggerWeekly);
                Console.WriteLine($"Update {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");

            }
            else
            {
                await StartWeekly(dayOfWeek, hour, minute, map);
            }
        }
        /// <summary>
        /// Cập nhật lịch hàng tháng
        /// </summary>
        /// <param name="hour">Giờ</param>
        /// <param name="minute">Phút</param>
        /// <param name="map">Các tham số muốn tiêm vào JobDetail</param>
        /// <returns></returns>
        public async Task UpdateMonthlyTrigger(int hour, int minute, IDictionary<string, object> map)
        {
            var json = map["Data"].ToString();
            KeyType data = JsonConvert.DeserializeObject<KeyType>(json);
            var triggerKey = new TriggerKey(data.Name, data.Group);

            var jobKey = new JobKey(data.Name, data.Group);
            var trigger = await _scheduler.GetTrigger(triggerKey);
            var job = await _scheduler.GetJobDetail(jobKey);
            var checkExists = await _scheduler.CheckExists(jobKey);
            if (checkExists)
            {
                var builderJob = job.GetJobBuilder();
                builderJob.UsingJobData(new JobDataMap(map));
                builderJob.Build();

                var builder = trigger.GetTriggerBuilder();
                builder.StartNow();
                var newtriggerMonthly = builder
                    .WithCronSchedule
                      ($"0 {minute} {hour} L * ?") // Meaning: Fire at 10:15am on the last day of every month
                      .ForJob(jobKey)
                    .Build();

                // if you don't want that it starts now, pass 'false' for the 'startNow' parameter
                await _scheduler.RescheduleJob(trigger.Key, newtriggerMonthly);
                Console.WriteLine($"Update {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");


            }
            else
            {
                await StartMonthly(hour, minute, map);
            }

        }
        /// <summary>
        /// Hủy lịch
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <returns></returns>
        public async Task UnscheduleJob(TriggerKey triggerKey)
        {
            var trigger = await _scheduler.GetTrigger(triggerKey);
            if (trigger != null)
            {
                await _scheduler.UnscheduleJob(trigger.Key);
                Console.WriteLine($"UnscheduleJob {triggerKey.Group} at {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}");
            }
            await Task.CompletedTask;
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
