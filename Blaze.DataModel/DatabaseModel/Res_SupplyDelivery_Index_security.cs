﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze.DataModel.DatabaseModel.Base;

//This source file has been auto generated.

namespace Blaze.DataModel.DatabaseModel
{

  public class Res_SupplyDelivery_Index_security : TokenIndex
  {
    public int Res_SupplyDelivery_Index_securityID {get; set;}
    public virtual Res_SupplyDelivery Res_SupplyDelivery { get; set; }
   
    public Res_SupplyDelivery_Index_security()
    {
    }
  }
}
