﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : SecureDigital.cs
// Version        : 1.0
// Author(s)      : Natalia Portillo
//
// Component      : Component
//
// Revision       : $Revision$
// Last change by : $Author$
// Date           : $Date$
//
// --[ Description ] ----------------------------------------------------------
//
// Description
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright (C) 2011-2015 Claunia.com
// ****************************************************************************/
// //$Id$
using System;
using System.Collections.Generic;
using DiscImageChef.Console;
using DiscImageChef.Core.Logging;
using DiscImageChef.Decoders.MMC;
using DiscImageChef.Devices;

namespace DiscImageChef.Core.Devices.Scanning
{
    public static class SecureDigital
    {
        public static ScanResults Scan(string MHDDLogPath, string IBGLogPath, string devicePath, Device dev)
        {
            ScanResults results = new ScanResults();
            bool aborted;
            MHDDLog mhddLog;
            IBGLog ibgLog;
            byte[] cmdBuf;
            bool sense;
            results.blocks = 0;
            uint[] response;
            uint timeout = 5;
            double duration = 0;
            ushort currentProfile = 0x0001;
            uint blocksToRead = 1;
            uint blockSize = 512;

            if(dev.Type == DeviceType.MMC)
            {
                ExtendedCSD ecsd = new ExtendedCSD();
                CSD csd = new CSD();

                sense = dev.ReadExtendedCSD(out cmdBuf, out response, timeout, out duration);
                if(!sense)
                {
                    ecsd = Decoders.MMC.Decoders.DecodeExtendedCSD(cmdBuf);
                    blocksToRead = ecsd.OptimalReadSize;
                    results.blocks = ecsd.SectorCount;
                    blockSize = (uint)(ecsd.SectorSize == 1 ? 4096 : 512);
                }

                if(sense || results.blocks == 0)
                {
                    sense = dev.ReadCSD(out cmdBuf, out response, timeout, out duration);
                    if(!sense)
                    {
                        csd = Decoders.MMC.Decoders.DecodeCSD(cmdBuf);
                        results.blocks = (ulong)((csd.Size + 1) * Math.Pow(2, csd.SizeMultiplier + 2));
                        blockSize = (uint)Math.Pow(2, csd.ReadBlockLength);
                    }
                }
            }
            else if(dev.Type == DeviceType.SecureDigital)
            {
                Decoders.SecureDigital.CSD csd = new Decoders.SecureDigital.CSD();

                sense = dev.ReadCSD(out cmdBuf, out response, timeout, out duration);
                if(!sense)
                {
                    csd = Decoders.SecureDigital.Decoders.DecodeCSD(cmdBuf);
                    results.blocks = (ulong)(csd.Structure == 0 ? (csd.Size + 1) * Math.Pow(2, csd.SizeMultiplier + 2) : (csd.Size + 1) * 1024);
                    blockSize = (uint)Math.Pow(2, csd.ReadBlockLength);
                }
            }

            if(results.blocks == 0)
            {
                DicConsole.ErrorWriteLine("Unable to get device size.");
                return results;
            }

            sense = true;

            while(true)
            {
                sense = dev.Read(out cmdBuf, out response, 0, blockSize, blocksToRead, false, timeout, out duration);

                if(sense)
                    blocksToRead /= 2;

                if(!sense || blocksToRead == 1)
                    break;
            }

            if(sense)
            {
                blocksToRead = 1;
                DicConsole.ErrorWriteLine("Device error {0} trying to guess ideal transfer length.", dev.LastError);
                return results;
            }

            results.A = 0; // <3ms
            results.B = 0; // >=3ms, <10ms
            results.C = 0; // >=10ms, <50ms
            results.D = 0; // >=50ms, <150ms
            results.E = 0; // >=150ms, <500ms
            results.F = 0; // >=500ms
            results.errored = 0;
            DateTime start;
            DateTime end;
            results.processingTime = 0;
            double currentSpeed = 0;
            results.maxSpeed = double.MinValue;
            results.minSpeed = double.MaxValue;
            results.unreadableSectors = new List<ulong>();
            results.seekMax = double.MinValue;
            results.seekMin = double.MaxValue;
            results.seekTotal = 0;
            const int seekTimes = 1000;

            double seekCur = 0;

            Random rnd = new Random();

            uint seekPos = (uint)rnd.Next((int)results.blocks);

            aborted = false;
            System.Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = aborted = true;
            };

            DicConsole.WriteLine("Reading {0} sectors at a time.", blocksToRead);

            mhddLog = new MHDDLog(MHDDLogPath, dev, results.blocks, blockSize, blocksToRead);
            ibgLog = new IBGLog(IBGLogPath, currentProfile);

            start = DateTime.UtcNow;
            for(ulong i = 0; i < results.blocks; i += blocksToRead)
            {
                if(aborted)
                    break;

                if((results.blocks - i) < blocksToRead)
                    blocksToRead = (byte)(results.blocks - i);

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if(currentSpeed > results.maxSpeed && currentSpeed != 0)
                    results.maxSpeed = currentSpeed;
                if(currentSpeed < results.minSpeed && currentSpeed != 0)
                    results.minSpeed = currentSpeed;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

                DicConsole.Write("\rReading sector {0} of {1} ({2:F3} MiB/sec.)", i, results.blocks, currentSpeed);

                bool error = dev.Read(out cmdBuf, out response, (uint)i, blockSize, blocksToRead, false, timeout, out duration);

                if(!error)
                {
                    if(duration >= 500)
                    {
                        results.F += blocksToRead;
                    }
                    else if(duration >= 150)
                    {
                        results.E += blocksToRead;
                    }
                    else if(duration >= 50)
                    {
                        results.D += blocksToRead;
                    }
                    else if(duration >= 10)
                    {
                        results.C += blocksToRead;
                    }
                    else if(duration >= 3)
                    {
                        results.B += blocksToRead;
                    }
                    else
                    {
                        results.A += blocksToRead;
                    }

                    mhddLog.Write(i, duration);
                    ibgLog.Write(i, currentSpeed * 1024);
                }
                else
                {
                    results.errored += blocksToRead;
                    for(ulong b = i; b < i + blocksToRead; b++)
                        results.unreadableSectors.Add(b);
                    if(duration < 500)
                        mhddLog.Write(i, 65535);
                    else
                        mhddLog.Write(i, duration);

                    ibgLog.Write(i, 0);
                }

#pragma warning disable IDE0004 // Without this specific cast, it gives incorrect values
                currentSpeed = ((double)blockSize * blocksToRead / (double)1048576) / (duration / (double)1000);
#pragma warning restore IDE0004 // Without this specific cast, it gives incorrect values
                GC.Collect();
            }
            end = DateTime.UtcNow;
            DicConsole.WriteLine();
            mhddLog.Close();
#pragma warning disable IDE0004 // Without this specific cast, it gives incorrect values
            ibgLog.Close(dev, results.blocks, blockSize, (end - start).TotalSeconds, currentSpeed * 1024, (((double)blockSize * (double)(results.blocks + 1)) / 1024) / (results.processingTime / 1000), devicePath);
#pragma warning restore IDE0004 // Without this specific cast, it gives incorrect values

            for(int i = 0; i < seekTimes; i++)
            {
                if(aborted)
                    break;

                seekPos = (uint)rnd.Next((int)results.blocks);

                DicConsole.Write("\rSeeking to sector {0}...\t\t", seekPos);

                dev.Read(out cmdBuf, out response, (uint)seekPos, blockSize, blocksToRead, false, timeout, out seekCur);

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if(seekCur > results.seekMax && seekCur != 0)
                    results.seekMax = seekCur;
                if(seekCur < results.seekMin && seekCur != 0)
                    results.seekMin = seekCur;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

                results.seekTotal += seekCur;
                GC.Collect();
            }

            DicConsole.WriteLine();

            results.processingTime /= 1000;
            results.totalTime = (end - start).TotalSeconds;
#pragma warning disable IDE0004 // Without this specific cast, it gives incorrect values
            results.avgSpeed = (((double)blockSize * (double)(results.blocks + 1)) / 1048576) / results.processingTime;
#pragma warning restore IDE0004 // Without this specific cast, it gives incorrect values
            results.seekTimes = seekTimes;

            return results;
        }
    }
}
