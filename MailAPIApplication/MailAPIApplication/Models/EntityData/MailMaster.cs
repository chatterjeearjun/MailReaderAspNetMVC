//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MailAPIApplication.Models.EntityData
{
    using System;
    using System.Collections.Generic;
    
    public partial class MailMaster
    {
        public int ID { get; set; }
        public string Guid { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentContent { get; set; }
        public Nullable<System.DateTime> ReceivedDateTime { get; set; }
        public Nullable<System.DateTime> LastRunDatetime { get; set; }
    }
}
