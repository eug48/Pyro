﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Blaze.Common.Interfaces.Services;
using Blaze.Common.Interfaces.UriSupport;

namespace Blaze.Web.BlazeService
{
  public class BlazeServiceRequest : IBlazeServiceRequest
  {
    public string ResourceId { get; set; }
    public Resource Resource { get; set; }
    public IDtoFhirRequestUri FhirRequestUri { get; set; }

    public BlazeServiceRequest(string ResourceId, Resource Resource, IDtoFhirRequestUri DtoFhirRequestUri)
    {
      this.ResourceId = ResourceId;
      this.Resource = Resource;
      this.FhirRequestUri = DtoFhirRequestUri;
    }

    public BlazeServiceRequest(Resource Resource, IDtoFhirRequestUri DtoFhirRequestUri)
    {
      this.ResourceId = null;
      this.Resource = Resource;
      this.FhirRequestUri = DtoFhirRequestUri;
    }
  }
}
