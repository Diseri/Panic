using AASA.NetCore.Lib.Helper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public class SystemNotes
    {
        public string Record { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        public string StatusTime { get { return CreatedOn.ToString("dd-MM-yyyy HH:mm:ss"); } }
        public string Origin { get; set; }
    }

    public class SystemNotesResponse : ResultBase
    {
        public List<SystemNotes> Data { get; set; }
    }
}
