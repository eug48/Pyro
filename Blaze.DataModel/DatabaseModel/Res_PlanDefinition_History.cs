﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze.DataModel.DatabaseModel.Base;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_PlanDefinition_History : ResourceIndexBase
  {
    public int Res_PlanDefinition_HistoryID {get; set;}
    public virtual Res_PlanDefinition Res_PlanDefinition { get; set; }
   
    public Res_PlanDefinition_History()
    {
    }
  }
}
