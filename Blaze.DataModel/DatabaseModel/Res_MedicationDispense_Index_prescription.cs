﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze.DataModel.DatabaseModel.Base;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_MedicationDispense_Index_prescription : ReferenceIndex
  {
    public int Res_MedicationDispense_Index_prescriptionID {get; set;}
    public virtual Res_MedicationDispense Res_MedicationDispense { get; set; }
   
    public Res_MedicationDispense_Index_prescription()
    {
    }
  }
}
