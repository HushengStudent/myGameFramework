/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/15 00:19:53
** desc:  ZIP文件解压工具;
*********************************************************************************/

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public static class ZipHelper
    {
        private static float progressInterval = 0.5f;

        public static void Compress(string filePath, string outPath, string fileName, Action<float> action)
        {
            var compressProgress = 0f;
            var progress = 0f;
            string zipFile = outPath + "/" + fileName + ".zip";
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }
            Thread thread = new Thread(delegate ()
            {
                int fileCount = RecursiveFile(filePath);
                int finishCount = 0;
                FastZipEvents events = new FastZipEvents();
                events.Progress = new ProgressHandler((object sender, ProgressEventArgs e) =>
                {
                    progress = e.PercentComplete;
                    if (progress == 100)
                    {
                        finishCount++;
                        compressProgress = (float)finishCount / (float)fileCount;
                        if (action != null)
                        {
                            action(compressProgress);
                        }
                    }
                });
                events.ProgressInterval = TimeSpan.FromSeconds(progressInterval);
                events.ProcessFile = new ProcessFileHandler((object sender, ScanEventArgs e) => { });
                FastZip zip = new FastZip(events);
                zip.CreateZip(zipFile, filePath, true, "");
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void Decompress(string filePath, string outPath, Action<float> action)
        {
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);
            var deCompressProgress = 0f;
            var progress = 0f;
            Thread thread = new Thread(delegate ()
            {
                int fileCount = (int)new ZipFile(filePath).Count;
                int finishCount = 0;
                FastZipEvents events = new FastZipEvents();
                events.Progress = new ProgressHandler((object sender, ProgressEventArgs e) =>
                {
                    progress = e.PercentComplete;
                    if (progress == 100)
                    {
                        finishCount++;
                        deCompressProgress = (float)finishCount / (float)fileCount;
                        if (action != null)
                        {
                            action(deCompressProgress);
                        }
                    }
                });
                events.ProgressInterval = TimeSpan.FromSeconds(progressInterval);
                events.ProcessFile = new ProcessFileHandler((object sender, ScanEventArgs e) => { });
                FastZip fastZip = new FastZip(events);
                fastZip.ExtractZip(filePath, outPath, "");
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private static int RecursiveFile(string path)
        {
            int files = Directory.GetFiles(path).Length;
            string[] folders = Directory.GetDirectories(path);
            foreach (string target in folders)
                files += RecursiveFile(target);
            return files;
        }
    }
}