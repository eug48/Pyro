﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure.Annotations;

//This is an Auto generated file do not change it's contents!!.

namespace Blaze.DataModel.DatabaseModel
{
  public class Res_Conformance_Index_meta_profile_Configuration : EntityTypeConfiguration<Res_Conformance_Index_meta_profile>
  {

    public Res_Conformance_Index_meta_profile_Configuration()
    {
      HasKey(x => x.Res_Conformance_Index_meta_profileID).Property(x => x.Res_Conformance_Index_meta_profileID).IsRequired();
      Property(x => x.Uri).IsRequired();
    }
  }
}
