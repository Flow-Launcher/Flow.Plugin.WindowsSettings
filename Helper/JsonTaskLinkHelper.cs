// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flow.Plugin.WindowsSettings.Classes;

namespace Flow.Plugin.WindowsSettings.Helper
{
    internal static class JsonTaskLinkHelper
    {
        private const string _taskLinkFile = "TaskLink.json";

        internal static TaskLinkList? ReadAllPossibleTaskLink()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetTypes().FirstOrDefault(x => x.Name == nameof(Main));

            TaskLinkList? taskLinkList = null;
            try
            {
                var resourceName = $"{type?.Namespace}.{_taskLinkFile}";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream is null)
                {
                    throw new Exception("stream is null");
                }

                using var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();

                taskLinkList = JsonSerializer.Deserialize<TaskLinkList>(text);
            }
            catch (Exception exception)
            {
                Log.Exception("Error loading settings JSON file", exception, typeof(Main));
            }

            return taskLinkList;
        }
    }
}