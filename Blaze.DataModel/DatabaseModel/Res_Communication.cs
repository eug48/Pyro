﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze.DataModel.DatabaseModel.Base;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_Communication : ResourceIndexBase
  {
    public int Res_CommunicationID {get; set;}
    public string encounter_VersionId {get; set;}
    public string encounter_FhirId {get; set;}
    public string encounter_Type {get; set;}
    public virtual Blaze_RootUrlStore encounter_Url { get; set; }
    public int? encounter_Url_Blaze_RootUrlStoreID { get; set; }
    public string patient_VersionId {get; set;}
    public string patient_FhirId {get; set;}
    public string patient_Type {get; set;}
    public virtual Blaze_RootUrlStore patient_Url { get; set; }
    public int? patient_Url_Blaze_RootUrlStoreID { get; set; }
    public DateTimeOffset? received_DateTimeOffset {get; set;}
    public string request_VersionId {get; set;}
    public string request_FhirId {get; set;}
    public string request_Type {get; set;}
    public virtual Blaze_RootUrlStore request_Url { get; set; }
    public int? request_Url_Blaze_RootUrlStoreID { get; set; }
    public string sender_VersionId {get; set;}
    public string sender_FhirId {get; set;}
    public string sender_Type {get; set;}
    public virtual Blaze_RootUrlStore sender_Url { get; set; }
    public int? sender_Url_Blaze_RootUrlStoreID { get; set; }
    public DateTimeOffset? sent_DateTimeOffset {get; set;}
    public string status_Code {get; set;}
    public string status_System {get; set;}
    public string subject_VersionId {get; set;}
    public string subject_FhirId {get; set;}
    public string subject_Type {get; set;}
    public virtual Blaze_RootUrlStore subject_Url { get; set; }
    public int? subject_Url_Blaze_RootUrlStoreID { get; set; }
    public ICollection<Res_Communication_History> Res_Communication_History_List { get; set; }
    public ICollection<Res_Communication_Index_category> category_List { get; set; }
    public ICollection<Res_Communication_Index_identifier> identifier_List { get; set; }
    public ICollection<Res_Communication_Index_medium> medium_List { get; set; }
    public ICollection<Res_Communication_Index_recipient> recipient_List { get; set; }
    public ICollection<Res_Communication_Index_profile> profile_List { get; set; }
    public ICollection<Res_Communication_Index_security> security_List { get; set; }
    public ICollection<Res_Communication_Index_tag> tag_List { get; set; }
   
    public Res_Communication()
    {
      this.category_List = new HashSet<Res_Communication_Index_category>();
      this.identifier_List = new HashSet<Res_Communication_Index_identifier>();
      this.medium_List = new HashSet<Res_Communication_Index_medium>();
      this.recipient_List = new HashSet<Res_Communication_Index_recipient>();
      this.profile_List = new HashSet<Res_Communication_Index_profile>();
      this.security_List = new HashSet<Res_Communication_Index_security>();
      this.tag_List = new HashSet<Res_Communication_Index_tag>();
      this.Res_Communication_History_List = new HashSet<Res_Communication_History>();
    }
  }
}
