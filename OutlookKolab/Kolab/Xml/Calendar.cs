﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace OutlookKolab.Kolab.Xml {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class @event {
        
        private string uidField;
        
        private string bodyField;
        
        private string categoriesField;
        
        private System.DateTime creationdateField;
        
        private System.DateTime lastmodificationdateField;
        
        private string sensitivityField;
        
        private string[] inlineattachmentField;
        
        private string[] linkattachmentField;
        
        private string productidField;
        
        private string summaryField;
        
        private string locationField;
        
        private eventOrganizer organizerField;
        
        private System.DateTime startdateField;
        
        private int alarmField;
        
        private eventRecurrence recurrenceField;
        
        private eventAttendee[] attendeeField;
        
        private string showtimeasField;
        
        private string colorlabelField;
        
        private System.DateTime enddateField;
        
        private decimal versionField;
        
        /// <remarks/>
        public string uid {
            get {
                return this.uidField;
            }
            set {
                this.uidField = value;
            }
        }
        
        /// <remarks/>
        public string body {
            get {
                return this.bodyField;
            }
            set {
                this.bodyField = value;
            }
        }
        
        /// <remarks/>
        public string categories {
            get {
                return this.categoriesField;
            }
            set {
                this.categoriesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("creation-date")]
        public System.DateTime creationdate {
            get {
                return this.creationdateField;
            }
            set {
                this.creationdateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("last-modification-date")]
        public System.DateTime lastmodificationdate {
            get {
                return this.lastmodificationdateField;
            }
            set {
                this.lastmodificationdateField = value;
            }
        }
        
        /// <remarks/>
        public string sensitivity {
            get {
                return this.sensitivityField;
            }
            set {
                this.sensitivityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("inline-attachment")]
        public string[] inlineattachment {
            get {
                return this.inlineattachmentField;
            }
            set {
                this.inlineattachmentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("link-attachment")]
        public string[] linkattachment {
            get {
                return this.linkattachmentField;
            }
            set {
                this.linkattachmentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("product-id")]
        public string productid {
            get {
                return this.productidField;
            }
            set {
                this.productidField = value;
            }
        }
        
        /// <remarks/>
        public string summary {
            get {
                return this.summaryField;
            }
            set {
                this.summaryField = value;
            }
        }
        
        /// <remarks/>
        public string location {
            get {
                return this.locationField;
            }
            set {
                this.locationField = value;
            }
        }
        
        /// <remarks/>
        public eventOrganizer organizer {
            get {
                return this.organizerField;
            }
            set {
                this.organizerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("start-date")]
        public System.DateTime startdate {
            get {
                return this.startdateField;
            }
            set {
                this.startdateField = value;
            }
        }
        
        /// <remarks/>
        public int alarm {
            get {
                return this.alarmField;
            }
            set {
                this.alarmField = value;
            }
        }
        
        /// <remarks/>
        public eventRecurrence recurrence {
            get {
                return this.recurrenceField;
            }
            set {
                this.recurrenceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("attendee")]
        public eventAttendee[] attendee {
            get {
                return this.attendeeField;
            }
            set {
                this.attendeeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("show-time-as")]
        public string showtimeas {
            get {
                return this.showtimeasField;
            }
            set {
                this.showtimeasField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("color-label")]
        public string colorlabel {
            get {
                return this.colorlabelField;
            }
            set {
                this.colorlabelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("end-date")]
        public System.DateTime enddate {
            get {
                return this.enddateField;
            }
            set {
                this.enddateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class eventOrganizer {
        
        private string displaynameField;
        
        private string smtpaddressField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("display-name")]
        public string displayname {
            get {
                return this.displaynameField;
            }
            set {
                this.displaynameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("smtp-address")]
        public string smtpaddress {
            get {
                return this.smtpaddressField;
            }
            set {
                this.smtpaddressField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class eventRecurrence {
        
        private int intervalField;
        
        private string[] dayField;
        
        private int daynumberField;
        
        private string monthField;
        
        private eventRecurrenceRange rangeField;
        
        private System.DateTime[] exclusionField;
        
        private string cycleField;
        
        private string typeField;
        
        /// <remarks/>
        public int interval {
            get {
                return this.intervalField;
            }
            set {
                this.intervalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("day")]
        public string[] day {
            get {
                return this.dayField;
            }
            set {
                this.dayField = value;
            }
        }
        
        /// <remarks/>
        public int daynumber {
            get {
                return this.daynumberField;
            }
            set {
                this.daynumberField = value;
            }
        }
        
        /// <remarks/>
        public string month {
            get {
                return this.monthField;
            }
            set {
                this.monthField = value;
            }
        }
        
        /// <remarks/>
        public eventRecurrenceRange range {
            get {
                return this.rangeField;
            }
            set {
                this.rangeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("exclusion")]
        public System.DateTime[] exclusion {
            get {
                return this.exclusionField;
            }
            set {
                this.exclusionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cycle {
            get {
                return this.cycleField;
            }
            set {
                this.cycleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class eventRecurrenceRange {
        
        private string typeField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class eventAttendee {
        
        private string displaynameField;
        
        private string smtpaddressField;
        
        private string statusField;
        
        private bool requestresponseField;
        
        private string roleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("display-name")]
        public string displayname {
            get {
                return this.displaynameField;
            }
            set {
                this.displaynameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("smtp-address")]
        public string smtpaddress {
            get {
                return this.smtpaddressField;
            }
            set {
                this.smtpaddressField = value;
            }
        }
        
        /// <remarks/>
        public string status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("request-response")]
        public bool requestresponse {
            get {
                return this.requestresponseField;
            }
            set {
                this.requestresponseField = value;
            }
        }
        
        /// <remarks/>
        public string role {
            get {
                return this.roleField;
            }
            set {
                this.roleField = value;
            }
        }
    }
}
