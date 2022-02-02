using System;
using System.Collections.Generic;
using System.Text;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public partial class NotePackage
    {
        public string Identifier { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
    }
}
