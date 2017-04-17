﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Pyro.Common.BusinessEntities.Dto;
using Pyro.Common.Interfaces.UriSupport;
using Pyro.DataLayer.DbModel.EntityGenerated;
using Pyro.DataLayer.DbModel.EntityBase;
using Pyro.DataLayer.Repository;
//using Pyro.DataModel.IndexSetter;


namespace Pyro.DataLayer.Support
{
  public static class IndexSettingSupport
  {

    private static void SetResourceBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>(Resource Resource, ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType> ResourceCurrentBase, string FhirResourceId, string Version, bool IsDeleted, Bundle.HTTPVerb Method)
      where ResourceCurrentType : ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>
      where ResourceHistoryType : ResourceHistoryBase<ResourceCurrentType>
      where ResourceIndexType : ResourceIndexBase
    {
      ResourceCurrentBase.IsDeleted = IsDeleted;
      ResourceCurrentBase.VersionId = Version;
      ResourceCurrentBase.Method = Method;
      if (!IsDeleted)
      {
        ResourceCurrentBase.FhirId = Resource.Id;
        ResourceCurrentBase.XmlBlob = Hl7.Fhir.Serialization.FhirSerializer.SerializeResourceToXml(Resource);
        ResourceCurrentBase.LastUpdated = (DateTimeOffset)Resource.Meta.LastUpdated;        
      }
      else
      {
        ResourceCurrentBase.FhirId = FhirResourceId;
        ResourceCurrentBase.XmlBlob = "";
        ResourceCurrentBase.LastUpdated = DateTimeOffset.Now;        
      }
    }

    public static DtoResource SetDtoResource<ResourceBaseType>(ResourceBaseType ResourceBase, FHIRAllTypes ResourceType, bool IsCurrent)
      where ResourceBaseType : ResourceBase
    {
      var DtoResource = new DtoResource();
      DtoResource.FhirId = ResourceBase.FhirId;
      DtoResource.IsCurrent = IsCurrent;
      DtoResource.IsDeleted = ResourceBase.IsDeleted;
      DtoResource.Received = ResourceBase.LastUpdated;
      DtoResource.Version = ResourceBase.VersionId;
      DtoResource.Xml = ResourceBase.XmlBlob;
      DtoResource.Method = ResourceBase.Method;
      DtoResource.ResourceType = ResourceType;
      return DtoResource;
    }

    public static void SetResourceBaseAsDelete<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>(ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType> ResourceCurrentBase, string FhirResourceId, string Version, Bundle.HTTPVerb Method)
      where ResourceCurrentType : ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>
      where ResourceHistoryType : ResourceHistoryBase<ResourceCurrentType>
      where ResourceIndexType : ResourceIndexBase
    {
      SetResourceBase(null, ResourceCurrentBase, FhirResourceId, Version, true, Method);
    }

    public static void SetResourceBaseAddOrUpdate<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>(Resource Resource, ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType> ResourceCurrentBase, string Version, bool IsDeleted, Bundle.HTTPVerb Method)
      where ResourceCurrentType : ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>
      where ResourceHistoryType : ResourceHistoryBase<ResourceCurrentType>
      where ResourceIndexType : ResourceIndexBase
    {
      SetResourceBase(Resource, ResourceCurrentBase, null, Version, false, Method);
    }

    public static void ResetResourceEntityBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>(ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType> ResourceCurrentBase)
      where ResourceCurrentType : ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>
      where ResourceHistoryType : ResourceHistoryBase<ResourceCurrentType>
      where ResourceIndexType : ResourceIndexBase
    {
      ResourceCurrentBase.FhirId = null;
      ResourceCurrentBase.IsDeleted = false;
      ResourceCurrentBase.LastUpdated = DateTimeOffset.MinValue;
      ResourceCurrentBase.Method = Bundle.HTTPVerb.GET;
      ResourceCurrentBase.VersionId = "Reset";
      ResourceCurrentBase.XmlBlob = null;      
    }

    public static void SetHistoryResourceEntity<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>(ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType> ResourceCurrentBase, ResourceHistoryBase<ResourceCurrentType> ResourceHistoryBase)
      where ResourceCurrentType : ResourceCurrentBase<ResourceCurrentType, ResourceHistoryType, ResourceIndexType>
      where ResourceHistoryType : ResourceHistoryBase<ResourceCurrentType>
      where ResourceIndexType : ResourceIndexBase
    {
      ResourceHistoryBase.FhirId = ResourceCurrentBase.FhirId;
      ResourceHistoryBase.IsDeleted = ResourceCurrentBase.IsDeleted;
      ResourceHistoryBase.XmlBlob = ResourceCurrentBase.XmlBlob;
      ResourceHistoryBase.LastUpdated = ResourceCurrentBase.LastUpdated;
      ResourceHistoryBase.VersionId = ResourceCurrentBase.VersionId;
      ResourceHistoryBase.Method = ResourceCurrentBase.Method;
    }

  }
}
