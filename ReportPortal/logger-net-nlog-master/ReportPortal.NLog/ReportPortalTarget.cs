﻿using System;
using System.Collections.Generic;
using ReportPortal.Client.Models;
using ReportPortal.Shared;
using NLog.Targets;

namespace ReportPortal.Logging
{
    /// <summary>
    /// NLog custom target for reporting logs directly to Report Portal.
    /// Logs will be viewable under current test item from shared context.
    /// </summary>
    [Target("ReportPortal")]
    public class ReportPortalTarget : TargetWithLayout
    {
        protected Dictionary<NLog.LogLevel, LogLevel> LevelMap = new Dictionary<NLog.LogLevel, LogLevel>();

        public ReportPortalTarget()
        {
            LevelMap[NLog.LogLevel.Fatal] = LogLevel.Error;
            LevelMap[NLog.LogLevel.Error] = LogLevel.Error;
            LevelMap[NLog.LogLevel.Warn] = LogLevel.Warning;
            LevelMap[NLog.LogLevel.Info] = LogLevel.Info;
            LevelMap[NLog.LogLevel.Debug] = LogLevel.Debug;
            LevelMap[NLog.LogLevel.Trace] = LogLevel.Trace;
        }

        protected override void Write(NLog.LogEventInfo logEvent)
        {
            if (!LevelMap.TryGetValue(logEvent.Level, out LogLevel level))
            {
                level = LogLevel.Info;
            }

            try
            {
                Bridge.LogMessage(level, Layout.Render(logEvent));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
