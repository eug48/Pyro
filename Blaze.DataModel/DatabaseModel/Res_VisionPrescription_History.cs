﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_VisionPrescription_History
  {
    public int Res_VisionPrescriptionID {get; set;}
    public string FhirId {get; set;}
    public int versionId {get; set;}
    public DateTimeOffset lastUpdated {get; set;}
    public string XmlBlob {get; set;}
    public virtual Res_VisionPrescription Res_VisionPrescription { get; set; }
   
    public Res_VisionPrescription_History()
    {
    }
  }
}

