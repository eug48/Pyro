﻿using Hl7.Fhir.Model;
using Pyro.Common.Enum;
using Pyro.Engine.Services;
using Pyro.Engine.Services.ResourceTrigger;

namespace Pyro.Engine.TriggerServices.CompartmentDefinitionResource
{
  public interface ITriggerCompartmentDefinition
  {
    ITriggerOutcome ProcessTrigger(RestEnum.CrudOperationType CrudOperationType, ResourceTriggerService.TriggerRaisedType TriggerRaised, string ResourceId, FHIRAllTypes ResourceType, Resource Resource = null);
  }
}