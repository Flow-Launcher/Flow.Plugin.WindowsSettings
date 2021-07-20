// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin;
using Flow.Plugin.WindowsSettings.Classes;

namespace Flow.Plugin.WindowsSettings.Helper
{
    internal static class TaskLinkResultHelper
    {
        private static IPublicAPI? _api;

        public static void Init(IPublicAPI api) => _api = api;

        internal static List<Result> GetResultList(
            TaskLinkList? list,
            Query query,
            string iconPath)
        {
            var returnList = new List<Result>();

            if (list == null)
                return returnList;

            Debug.Assert(_api != null, nameof(_api) + " != null");


            foreach (var tl in list.items)
            {
                int score = 0;
                var nameMatch = _api.FuzzySearch(query.Search, tl.name);
                if (tl.name.IndexOf(query.Search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    score += nameMatch.Score + 20;
                }

                if (score < 20)
                {
                    nameMatch = _api.FuzzySearch(query.Search, tl.cmd);
                    if (tl.cmd.IndexOf(query.Search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        score += nameMatch.Score + 20;
                    }
                }

                if (score > 0)
                {
                    var r = new Result()
                    {
                        Action = _ => DoOpenTaskLinkAction(tl),
                        SubTitle = tl.cmd,
                        Title = tl.name,
                        ContextData = tl,
                        Score = score
                    };
                    returnList.Add(r);
                }
            }


            return returnList;
        }

        private static bool DoOpenTaskLinkAction(TaskLink taskLink)
        {
            ProcessStartInfo processStartInfo;
            var command = Environment.ExpandEnvironmentVariables(taskLink.cmd);

            if (command.Contains(' '))
            {
                var commandSplit = command.Split(' ');
                var file = commandSplit.First();
                var arguments = command[file.Length..].TrimStart();

                processStartInfo = new ProcessStartInfo(file, arguments)
                {
                    UseShellExecute = false,
                };
            }
            else
            {
                processStartInfo = new ProcessStartInfo(command)
                {
                    UseShellExecute = true,
                };
            }


            try
            {
                Process.Start(processStartInfo);
                return true;
            }
            catch (Exception exception)
            {
                Log.Exception("can't open task link", exception, typeof(TaskLinkResultHelper));
                return false;
            }
        }
    }
}