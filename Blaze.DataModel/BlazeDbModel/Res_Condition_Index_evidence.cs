﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This source file has been auto generated.

namespace Blaze.DataModel.BlazeDbModel
{

  public class Res_Condition_Index_evidence
  {
    public int Id {get; set;}
    public string Code {get; set;}                  
    public string System {get; set;}                
    
    //Keyed
    public virtual Res_Condition Res_Condition { get; set; }                     
  }
}