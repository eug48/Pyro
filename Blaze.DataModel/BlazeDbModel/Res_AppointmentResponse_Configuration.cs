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

namespace Blaze.DataModel.BlazeDbModel
{
  public class Res_AppointmentResponse_Configuration : EntityTypeConfiguration<Res_AppointmentResponse>
  {

    public Res_AppointmentResponse_Configuration()
    {
      HasKey(x => x.Id).Property(x => x.Id).IsRequired();
      Property(x => x.FhirId).IsRequired().HasMaxLength(500).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_FhirId") { IsUnique = true }));;
      Property(x => x.Received).IsRequired();
      Property(x => x.XmlBlob).IsRequired();      
      Property(x => x.actor_FhirId).IsOptional();
      Property(x => x.actor_Type).IsOptional();
      Property(x => x.actor_Url).IsOptional();
      Property(x => x.appointment_FhirId).IsOptional();
      Property(x => x.appointment_Type).IsOptional();
      Property(x => x.appointment_Url).IsOptional();
      Property(x => x.location_FhirId).IsOptional();
      Property(x => x.location_Type).IsOptional();
      Property(x => x.location_Url).IsOptional();
      Property(x => x.part_status_Code).IsOptional();
      Property(x => x.part_status_System).IsOptional();
      Property(x => x.patient_FhirId).IsOptional();
      Property(x => x.patient_Type).IsOptional();
      Property(x => x.patient_Url).IsOptional();
      Property(x => x.practitioner_FhirId).IsOptional();
      Property(x => x.practitioner_Type).IsOptional();
      Property(x => x.practitioner_Url).IsOptional();
     
     }
  }
}