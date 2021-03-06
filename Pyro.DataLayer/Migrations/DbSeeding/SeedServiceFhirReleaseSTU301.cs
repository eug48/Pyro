﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyro.DataLayer.DbModel.DatabaseContext;
using Pyro.DataLayer.DbModel.Entity;
using Pyro.Common.Search;
using Hl7.Fhir.Model;

namespace Pyro.DataLayer.Migrations.DbSeeding
{
  /// <summary>
  /// Seed service that adds an entry to the new FhirRelease table for FHIR Version V3.0.1
  /// </summary>
  public class SeedServiceFhirReleaseSTU301 : IPyroSeedService
  {
    private PyroDbContext _Context;
    public SeedServiceFhirReleaseSTU301(PyroDbContext Context)
    {
      _Context = Context;
    }

    public string ServiceName => "SeedServiceFhirReleaseSTU301";


    /// <summary>
    /// Returns True if the Seed method need to run and false if it is not required.
    /// This is needed as Migrations will run the seeding process or all migrations 
    /// </summary>
    /// <returns>True if Seed() method is required to run</returns>
    public bool DoesSeedNeedToRun()
    {
      if (_Context.FhirRelease.Where(x => x.FhirVersion == "3.0.1").Count() == 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public void Seed()
    {
      var DateTimeNow = DateTimeOffset.Now;
      _FhirRelease FhirReleaseSTU301 = new _FhirRelease()
      {
        FhirVersion = "3.0.1",
        Date = new DateTime(2017, 4, 19),
        Description = "FHIR Release 3 (STU) with 1 technical errata",
        CreatedDate = DateTimeNow,
        CreatedUser = "PyroHealthDev",
        LastUpdated = DateTimeNow,
        LastUpdatedUser = Pyro.Common.PyroHealthInformation.PyroHealthSystemUser.User,
      };
      _Context.FhirRelease.Add(FhirReleaseSTU301);
      _Context.SaveChanges();
    }
  }
}
