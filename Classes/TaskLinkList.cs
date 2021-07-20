using System.Collections.Generic;

namespace Flow.Plugin.WindowsSettings.Classes
{
    public class TaskLinkList
    {
        public string schema_version { get; set; }
        public string language { get; set; }
        public string windows_version { get; set; }
        public List<TaskLink> items { get; set; }
    }
    public class TaskLink
    {
        public string name { get; set; }
        public string cmd { get; set; }
        public List<List<string>> keywords { get; set; }
    }
}