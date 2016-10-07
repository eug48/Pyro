﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze.DataModel.DatabaseModel.Base;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_FamilyMemberHistory : ResourceIndexBase
  {
    public int Res_FamilyMemberHistoryID {get; set;}
    public DateTimeOffset? date_DateTimeOffset {get; set;}
    public string gender_Code {get; set;}
    public string gender_System {get; set;}
    public string patient_VersionId {get; set;}
    public string patient_FhirId {get; set;}
    public string patient_Type {get; set;}
    public virtual ServiceRootURL_Store patient_Url { get; set; }
    public int? patient_ServiceRootURL_StoreID { get; set; }
    public ICollection<Res_FamilyMemberHistory_History> Res_FamilyMemberHistory_History_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index_code> code_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index_identifier> identifier_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index_relationship> relationship_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index__profile> _profile_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index__security> _security_List { get; set; }
    public ICollection<Res_FamilyMemberHistory_Index__tag> _tag_List { get; set; }
   
    public Res_FamilyMemberHistory()
    {
      this.code_List = new HashSet<Res_FamilyMemberHistory_Index_code>();
      this.identifier_List = new HashSet<Res_FamilyMemberHistory_Index_identifier>();
      this.relationship_List = new HashSet<Res_FamilyMemberHistory_Index_relationship>();
      this._profile_List = new HashSet<Res_FamilyMemberHistory_Index__profile>();
      this._security_List = new HashSet<Res_FamilyMemberHistory_Index__security>();
      this._tag_List = new HashSet<Res_FamilyMemberHistory_Index__tag>();
      this.Res_FamilyMemberHistory_History_List = new HashSet<Res_FamilyMemberHistory_History>();
    }
  }
}

