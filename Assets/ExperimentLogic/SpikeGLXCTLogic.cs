﻿/*
SpikeGLXCTLogic.cs is part of the Experica.
Copyright (c) 2016 Li Alex Zhang and Contributors

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace Experica
{
    /// <summary>
    /// Condition Test with SpikeGLX Data Acquisition System
    /// </summary>
    public class SpikeGLXCTLogic : ConditionTestLogic
    {
        protected override void OnStartExperiment()
        {
            recorder = new SpikeGLXRecorder(host: config.RecordHost, port: config.RecordHostPort);
            base.OnStartExperiment();
        }

        protected override void OnExperimentStopped()
        {
            base.OnExperimentStopped();
            recorder?.Dispose();
        }

        protected override void StartExperimentTimeSync()
        {
            if (ex.CondTestAtState != CONDTESTATSTATE.NONE)
            {
                recorder.RecordPath = ex.GetDataPath();
                /* 
                SpikeGLX recorder set path through network and remote server receive
                message and change file path, all of which need time to complete.
                Set record before file path change completion may not save to correct file path.
                */
                timer.Timeout(config.NotifyLatency);
                recorder.RecordStatus = RecordStatus.Recording;
                /* 
                SpikeGLX recorder set record status through network and remote server receive
                message and change record state, all of which need time to complete.
                Begin experiment before record may lose information.
                */
                timer.Timeout(config.NotifyLatency);
            }
            base.StartExperimentTimeSync();
        }

        protected override void StopExperimentTimeSync()
        {
            // Tail period to make sure lagged effect data are collected before stop recording
            timer.Timeout(ex.Display_ID.DisplayLatency(config.Display) + config.MaxDisplayLatencyError);
            recorder.RecordStatus = RecordStatus.Stopped;
            /* 
            SpikeGLX recorder set record status through network and remote server receive
            message and change record state, all of which need time to complete.
            Here wait recording ended before further processing.
            */
            timer.Timeout(config.NotifyLatency);
            base.StopExperimentTimeSync();
        }
    }
}