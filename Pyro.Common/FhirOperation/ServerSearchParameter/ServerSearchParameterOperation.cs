﻿using Pyro.Common.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Pyro.Common.Enum;
using Pyro.Common.Tools.UriSupport;
using Pyro.Common.Search;
using Pyro.Common.RequestMetadata;
using Pyro.Common.CompositionRoot;
using Pyro.Common.Cache;
using Pyro.Common.Service.ResourceService;
using Pyro.Common.Service.SearchParameters;

namespace Pyro.Common.FhirOperation.ServerSearchParameter
{
  public class ServerSearchParameterOperation : IServerSearchParameterOperation
  {
    private readonly IResourceServices IResourceServices;
    private readonly IRequestMetaFactory IRequestMetaFactory;    
    private readonly IServiceSearchParameterService IServiceSearchParameterService;
    private readonly IResourceServiceOutcomeFactory IResourceServiceOutcomeFactory;
    private readonly IPyroFhirUriFactory IPyroFhirUriFactory;
    private readonly ISearchParameterServiceFactory ISearchParameterServiceFactory;
    private readonly ICacheClear ICacheClear;

    private const string _ParameterName = "ResourceType";
    private SearchParameter TargetSearchParameter;

    public ServerSearchParameterOperation(IResourceServices IResourceServices, IRequestMetaFactory IRequestMetaFactory, IResourceServiceOutcomeFactory IResourceServiceOutcomeFactory, IPyroFhirUriFactory IPyroFhirUriFactory, ICacheClear ICacheClear, ISearchParameterServiceFactory ISearchParameterServiceFactory, IServiceSearchParameterService IServiceSearchParameterService)
    {
      this.IResourceServices = IResourceServices;
      this.IRequestMetaFactory = IRequestMetaFactory;      
      this.IResourceServiceOutcomeFactory = IResourceServiceOutcomeFactory;
      this.IPyroFhirUriFactory = IPyroFhirUriFactory;
      this.ICacheClear = ICacheClear;
      this.ISearchParameterServiceFactory = ISearchParameterServiceFactory;
      this.IServiceSearchParameterService = IServiceSearchParameterService;
    }

    public IResourceServiceOutcome ProcessIndex(
      IPyroRequestUri RequestUri,
      ISearchParameterGeneric SearchParameterGeneric,
      Resource Resource)
    {
      if (Resource == null)
        throw new NullReferenceException("Resource cannot be null.");
      if (RequestUri == null)
        throw new NullReferenceException("RequestUri cannot be null.");
      if (SearchParameterGeneric == null)
        throw new NullReferenceException("SearchParameterGeneric cannot be null.");
      if (IResourceServices == null)
        throw new NullReferenceException("IResourceServices cannot be null.");

      IResourceServiceOutcome ResourceServiceOutcome = IResourceServiceOutcomeFactory.CreateResourceServiceOutcome();

      ISearchParameterService SearchService = ISearchParameterServiceFactory.CreateSearchParameterService();
      ISearchParametersServiceOutcome SearchParametersServiceOutcome = SearchService.ProcessBaseSearchParameters(SearchParameterGeneric);
      if (SearchParametersServiceOutcome.FhirOperationOutcome != null)
      {
        ResourceServiceOutcome.ResourceResult = SearchParametersServiceOutcome.FhirOperationOutcome;
        ResourceServiceOutcome.HttpStatusCode = SearchParametersServiceOutcome.HttpStatusCode;
        ResourceServiceOutcome.FormatMimeType = SearchParametersServiceOutcome.SearchParameters.Format;
        return ResourceServiceOutcome;
      }

      List<DtoServiceSearchParameterHeavy> DbSearchParameterList = IServiceSearchParameterService.GetServiceSearchParametersHeavy(true);
      List<CompairisonResult> CompairisonResultList = CalculateDiff(DbSearchParameterList, RequestUri);
      var ErrorList = CompairisonResultList.Where(x => x.OperationOutcome != null).ToList();
      if (ErrorList.Count() > 0)
      {
        List<OperationOutcome.IssueComponent> IssueList = new List<OperationOutcome.IssueComponent>();
        foreach (var item in ErrorList)
        {
          IssueList.AddRange(item.OperationOutcome.Issue);
        }
        var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Generate(IssueList);
        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
        ResourceServiceOutcome.ResourceResult = OpOutCome;
        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
        ResourceServiceOutcome.SuccessfulTransaction = false;
        return ResourceServiceOutcome;
      }
      else
      {
        var ReturnParametersResource = new Parameters();
        var DropIndexParameter = new Parameters.ParameterComponent();
        DropIndexParameter.Name = "DroppedIndexList";
        ReturnParametersResource.Parameter.Add(DropIndexParameter);

        var DeleteSearchParameter = CompairisonResultList.Where(x => x.DeleteSearchParameterList.Count > 0);
        foreach (var Del1 in DeleteSearchParameter)
        {
          foreach (var Del2 in Del1.DeleteSearchParameterList)
          {
            foreach (var Del3 in Del2.Value)
            {
              IServiceSearchParameterService.DeleteServiceSearchParameters(Del3.Id);
            }
          }
        }

        var CreateSearchParameter = CompairisonResultList.Where(x => x.CreateSearchParameterList.Count > 0);
        foreach (var Create1 in CreateSearchParameter)
        {
          foreach (var Create2 in Create1.CreateSearchParameterList)
          {
            foreach (var Create3 in Create2.Value)
            {
              Create3.Id = IServiceSearchParameterService.AddServiceSearchParametersHeavy(Create3).Id;
            }
          }
        }




        var DeleteList = CompairisonResultList.Where(x => x.DeleteIndexList.Count > 0);
        foreach (var Del1 in DeleteList)
        {
          foreach (var Del2 in Del1.DeleteIndexList)
          {
            foreach (var Del3 in Del2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "DroppedIndexDetails";
              Para.Value = new FhirString($"Index for: Resource: {Del3.Resource}; SearchName: {Del3.Name}; Expression: {Del3.Expression}; SearchParameterResourceId: {Del3.SearchParameterResourceId}; SearchParameterVersion: {Del3.SearchParameterResourceVersion}");
              if (DropIndexParameter.Part == null)
                DropIndexParameter.Part = new List<Parameters.ParameterComponent>();
              DropIndexParameter.Part.Add(Para);
            }
          }
        }


        CreateResourceIndexesInDatabase(CompairisonResultList, RequestUri);


        var CreateIndexParameter = new Parameters.ParameterComponent();
        CreateIndexParameter.Name = "CreatedIndexList";
        ReturnParametersResource.Parameter.Add(CreateIndexParameter);
        var CreateList = CompairisonResultList.Where(x => x.CreateIndexList.Count > 0);
        foreach (var Create1 in CreateList)
        {
          foreach (var Create2 in Create1.CreateIndexList)
          {
            foreach (var Create3 in Create2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "CreatedIndexDetails";
              Para.Value = new FhirString($"Index for: Resource: {Create3.Resource}; SearchName: {Create3.Name}; Expression: {Create3.Expression}; SearchParameterResourceId: {Create3.SearchParameterResourceId}; SearchParameterVersion: {Create3.SearchParameterResourceVersion}");
              if (CreateIndexParameter.Part == null)
                CreateIndexParameter.Part = new List<Parameters.ParameterComponent>();
              CreateIndexParameter.Part.Add(Para);
            }
          }
        }

        var UpdateSearchName = new Parameters.ParameterComponent();
        UpdateSearchName.Name = "UpdatedSearchNameList";
        ReturnParametersResource.Parameter.Add(UpdateSearchName);
        var UpdateSearchNameList = CompairisonResultList.Where(x => x.UpdateSearchParameterList.Count > 0);
        foreach (var Update1 in UpdateSearchNameList)
        {
          foreach (var Update2 in Update1.UpdateSearchParameterList)
          {
            foreach (var Update3 in Update2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "UpdatedSearchNameDetails";
              Para.Value = new FhirString($"Search Name for: Resource: {Update3.New.Resource}; NewSearchName: {Update3.New.Name}; OldSearchName: {Update3.Old.Name}; SearchParameterResourceId: {Update3.New.SearchParameterResourceId}; NewSearchParameterVersion: {Update3.New.SearchParameterResourceVersion}; OldSearchParameterVersion: {Update3.Old.SearchParameterResourceVersion}");
              if (UpdateSearchName.Part == null)
                UpdateSearchName.Part = new List<Parameters.ParameterComponent>();
              UpdateSearchName.Part.Add(Para);
            }
          }
        }

        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.OK;
        ResourceServiceOutcome.ResourceResult = ReturnParametersResource;
        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
        ResourceServiceOutcome.SuccessfulTransaction = true;

        //Clear the Cache as stale search parameters now exists 
        ICacheClear.ClearCache();

        return ResourceServiceOutcome;

      }

    }

    public IResourceServiceOutcome ProcessSet(
      IPyroRequestUri RequestUri,
      ISearchParameterGeneric SearchParameterGeneric,
      Resource Resource)
    {
      if (Resource == null)
        throw new NullReferenceException("Resource cannot be null.");
      if (RequestUri == null)
        throw new NullReferenceException("RequestUri cannot be null.");
      if (SearchParameterGeneric == null)
        throw new NullReferenceException("SearchParameterGeneric cannot be null.");
      if (IResourceServices == null)
        throw new NullReferenceException("ResourceServicescannot be null.");

      IResourceServiceOutcome ResourceServiceOutcome = IResourceServiceOutcomeFactory.CreateResourceServiceOutcome();

      ISearchParameterService SearchServiceRequest = ISearchParameterServiceFactory.CreateSearchParameterService();
      ISearchParametersServiceOutcome SearchParametersServiceOutcome = SearchServiceRequest.ProcessBaseSearchParameters(SearchParameterGeneric);
      if (SearchParametersServiceOutcome.FhirOperationOutcome != null)
      {
        ResourceServiceOutcome.ResourceResult = SearchParametersServiceOutcome.FhirOperationOutcome;
        ResourceServiceOutcome.HttpStatusCode = SearchParametersServiceOutcome.HttpStatusCode;
        ResourceServiceOutcome.FormatMimeType = SearchParametersServiceOutcome.SearchParameters.Format;
        return ResourceServiceOutcome;
      }

      if (Resource != null && Resource is Parameters Parameters)
      {
        if (Parameters.Parameter != null || Parameters.Parameter.Count() == 0)
        {
          Parameters ReturnParametersResource = new Parameters();
          ReturnParametersResource.Parameter = new List<Parameters.ParameterComponent>();

          foreach (var Para in Parameters.Parameter)
          {
            if (Para.Value != null && Para.Value is ResourceReference Ref)
            {
              IPyroFhirUri FhirUri = IPyroFhirUriFactory.CreateFhirRequestUri();
              if (FhirUri.Parse(Ref.Reference))
              {
                if (FhirUri.IsRelativeToServer)
                {
                  if (!FhirUri.IsHistoryReferance)
                  {
                    if (!FhirUri.IsContained)
                    {
                      if (!FhirUri.IsOperation)
                      {
                        IRequestMeta RequestMeta = IRequestMetaFactory.CreateRequestMeta().Set($"{ResourceType.SearchParameter.GetLiteral()}/{FhirUri.ResourceId}");
                        IResourceServiceOutcome ResourceServiceOutcomeGetSearchParameterResource = IResourceServices.GetRead(FhirUri.ResourceId, RequestMeta);                        
                        if (ResourceServiceOutcomeGetSearchParameterResource.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                          TargetSearchParameter = ResourceServiceOutcomeGetSearchParameterResource.ResourceResult as SearchParameter;

                          OperationOutcome OperationOutcomeValidation = ValidateSearchParameterResource(TargetSearchParameter);
                          if (OperationOutcomeValidation == null)
                          {
                            var List = GenerateDbSearchParameterList(TargetSearchParameter);

                            foreach (var Item in List)
                            {
                              var DbSearchParamListForResource = IServiceSearchParameterService.GetServiceSearchParametersHeavyForResource(Item.Resource);
                              if (Item.Resource != ResourceType.Resource.GetLiteral())
                                DbSearchParamListForResource.AddRange(IServiceSearchParameterService.GetServiceSearchParametersHeavyForResource(ResourceType.Resource.GetLiteral()));

                              var CodeAlreadyIndexed = DbSearchParamListForResource.SingleOrDefault(x => x.Name == TargetSearchParameter.Code && x.SearchParameterResourceId != null);
                              if (CodeAlreadyIndexed != null)
                              {
                                var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported,
                                $"The SearchParameter referenced in already associated with the servers search indexes. If you wish to modify the SearchParameter then you need only to update the SearchParameter with the resource id = {CodeAlreadyIndexed.SearchParameterResourceId} and then call the Update indexes operation {Enum.FhirOperationEnum.OperationType.ServerIndexesIndex.GetPyroLiteral()}");
                                ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                                ResourceServiceOutcome.ResourceResult = OpOutCome;
                                ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                                ResourceServiceOutcome.SuccessfulTransaction = false;
                                return ResourceServiceOutcome;
                              }

                              CodeAlreadyIndexed = DbSearchParamListForResource.SingleOrDefault(x => x.Name == TargetSearchParameter.Code && x.SearchParameterResourceId == null);
                              if (CodeAlreadyIndexed != null)
                              {
                                var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported,
                                    $"The SearchParameter referenced has a Code element that in already indexed. This Code of {CodeAlreadyIndexed.Name} is a base FHIR search parameter and can not be modified. You could try a different Code value and thereby create a separate search parameter index.");
                                ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                                ResourceServiceOutcome.ResourceResult = OpOutCome;
                                ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                                ResourceServiceOutcome.SuccessfulTransaction = false;
                                return ResourceServiceOutcome;
                              }
                            }
                            //Add the new SearchParameterIndex to the database
                            foreach (var Item in List)
                              IServiceSearchParameterService.AddServiceSearchParametersHeavy(Item);
                          }
                          else
                          {
                            ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                            ResourceServiceOutcome.ResourceResult = OperationOutcomeValidation;
                            ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                            ResourceServiceOutcome.SuccessfulTransaction = false;
                            return ResourceServiceOutcome;
                          }
                        }
                        else
                        {
                          var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"No SearchParameter resource found in the server for the ResourceReference: {FhirUri.OriginalString}");
                          ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                          ResourceServiceOutcome.ResourceResult = OpOutCome;
                          ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                          ResourceServiceOutcome.SuccessfulTransaction = false;
                          return ResourceServiceOutcome;
                        }
                      }
                      else
                      {
                        var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element ResourceReference value must not be an $Operation reference, value was: {FhirUri.OriginalString}");
                        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                        ResourceServiceOutcome.ResourceResult = OpOutCome;
                        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                        ResourceServiceOutcome.SuccessfulTransaction = false;
                        return ResourceServiceOutcome;
                      }
                    }
                    else
                    {
                      var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element ResourceReference value must not be a #Contained reference, value was: {FhirUri.OriginalString}");
                      ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                      ResourceServiceOutcome.ResourceResult = OpOutCome;
                      ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                      ResourceServiceOutcome.SuccessfulTransaction = false;
                      return ResourceServiceOutcome;
                    }
                  }
                  else
                  {
                    var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element ResourceReference value must not be a Version specific reference, value was: {FhirUri.OriginalString}");
                    ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                    ResourceServiceOutcome.ResourceResult = OpOutCome;
                    ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                    ResourceServiceOutcome.SuccessfulTransaction = false;
                    return ResourceServiceOutcome;
                  }
                }
                else
                {
                  var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element ResourceReference value must be relative to the server, value was: {FhirUri.OriginalString}");
                  ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                  ResourceServiceOutcome.ResourceResult = OpOutCome;
                  ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                  ResourceServiceOutcome.SuccessfulTransaction = false;
                  return ResourceServiceOutcome;
                }
              }
              else
              {
                var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element ResourceReference value was not able to be parsed, Error message: {FhirUri.ParseErrorMessage}");
                ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
                ResourceServiceOutcome.ResourceResult = OpOutCome;
                ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
                ResourceServiceOutcome.SuccessfulTransaction = false;
                return ResourceServiceOutcome;
              }
            }
            else
            {
              var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource's parameter element either had no value or the value was not a ResourceReference value type.");
              ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
              ResourceServiceOutcome.ResourceResult = OpOutCome;
              ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
              ResourceServiceOutcome.SuccessfulTransaction = false;
              return ResourceServiceOutcome;
            }
          }
          var ReturnParameter = new Parameters.ParameterComponent();
          ReturnParameter.Name = "SearchParameterRegisteredForIndexing";
          ReturnParameter.Resource = TargetSearchParameter;
          ReturnParametersResource.Parameter.Add(ReturnParameter);

          //All good return ParametersResource with the SearchParameter Resource registered.
          ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.OK;
          ResourceServiceOutcome.ResourceResult = ReturnParametersResource;
          ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
          ResourceServiceOutcome.SuccessfulTransaction = true;
          return ResourceServiceOutcome;
        }
        else
        {
          var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Parameters resource contains no parameter elements.");
          ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
          ResourceServiceOutcome.ResourceResult = OpOutCome;
          ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
          ResourceServiceOutcome.SuccessfulTransaction = false;
          return ResourceServiceOutcome;
        }
      }
      else
      {
        //The Resource given was not a Parameters resource
        var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Create(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.NotSupported, $"The Resource given on this operation must be a SearchParameter type FHIR resource.");
        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
        ResourceServiceOutcome.ResourceResult = OpOutCome;
        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
        ResourceServiceOutcome.SuccessfulTransaction = false;
        return ResourceServiceOutcome;
      }
    }

    public IResourceServiceOutcome ProcessReport(
      IPyroRequestUri RequestUri,
      ISearchParameterGeneric SearchParameterGeneric,
      Resource Resource)
    {
      if (RequestUri == null)
        throw new NullReferenceException("Resource cannot be null.");
      if (Resource == null)
        throw new NullReferenceException("Resource cannot be null.");
      if (SearchParameterGeneric == null)
        throw new NullReferenceException("SearchParameterGeneric cannot be null.");
      if (IResourceServices == null)
        throw new NullReferenceException("ResourceServices cannot be null.");

      IResourceServiceOutcome ResourceServiceOutcome = IResourceServiceOutcomeFactory.CreateResourceServiceOutcome();

      ISearchParameterService SearchService = ISearchParameterServiceFactory.CreateSearchParameterService();
      ISearchParametersServiceOutcome SearchParametersServiceOutcome = SearchService.ProcessBaseSearchParameters(SearchParameterGeneric);
      if (SearchParametersServiceOutcome.FhirOperationOutcome != null)
      {
        ResourceServiceOutcome.ResourceResult = SearchParametersServiceOutcome.FhirOperationOutcome;
        ResourceServiceOutcome.HttpStatusCode = SearchParametersServiceOutcome.HttpStatusCode;
        ResourceServiceOutcome.FormatMimeType = SearchParametersServiceOutcome.SearchParameters.Format;
        return ResourceServiceOutcome;
      }

      List<DtoServiceSearchParameterHeavy> DbSearchParameterList = IServiceSearchParameterService.GetServiceSearchParametersHeavy(true);
      List<CompairisonResult> CompairisonResultList = CalculateDiff(DbSearchParameterList, RequestUri);
      var ErrorList = CompairisonResultList.Where(x => x.OperationOutcome != null).ToList();
      if (ErrorList.Count() > 0)
      {
        List<OperationOutcome.IssueComponent> IssueList = new List<OperationOutcome.IssueComponent>();
        foreach (var item in ErrorList)
        {
          IssueList.AddRange(item.OperationOutcome.Issue);
        }
        var OpOutCome = Common.Tools.FhirOperationOutcomeSupport.Generate(IssueList);
        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.BadRequest;
        ResourceServiceOutcome.ResourceResult = OpOutCome;
        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
        ResourceServiceOutcome.SuccessfulTransaction = false;
        return ResourceServiceOutcome;
      }
      else
      {
        var ReturnParametersResource = new Parameters();


        var DropIndexParameter = new Parameters.ParameterComponent();
        DropIndexParameter.Name = "DropIndexList";
        ReturnParametersResource.Parameter.Add(DropIndexParameter);

        var DeleteList = CompairisonResultList.Where(x => x.DeleteIndexList.Count > 0);
        foreach (var Del1 in DeleteList)
        {
          foreach (var Del2 in Del1.DeleteIndexList)
          {
            foreach (var Del3 in Del2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "DropIndexDetails";
              Para.Value = new FhirString($"Index for: Resource: {Del3.Resource}; SearchName: {Del3.Name}; Expression: {Del3.Expression}; SearchParameterResourceId: {Del3.SearchParameterResourceId}; SearchParameterVersion: {Del3.SearchParameterResourceVersion}");
              if (DropIndexParameter.Part == null)
                DropIndexParameter.Part = new List<Parameters.ParameterComponent>();
              DropIndexParameter.Part.Add(Para);
            }
          }
        }

        var CreateIndexParameter = new Parameters.ParameterComponent();
        CreateIndexParameter.Name = "CreateIndexList";
        ReturnParametersResource.Parameter.Add(CreateIndexParameter);

        var CreateList = CompairisonResultList.Where(x => x.CreateIndexList.Count > 0);
        foreach (var Create1 in CreateList)
        {
          foreach (var Create2 in Create1.CreateIndexList)
          {
            foreach (var Create3 in Create2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "CreateIndexDetails";
              Para.Value = new FhirString($"Index for: Resource: {Create3.Resource}; SearchName: {Create3.Name}; Expression: {Create3.Expression}; SearchParameterResourceId: {Create3.SearchParameterResourceId}; SearchParameterVersion: {Create3.SearchParameterResourceVersion}");
              if (CreateIndexParameter.Part == null)
                CreateIndexParameter.Part = new List<Parameters.ParameterComponent>();
              CreateIndexParameter.Part.Add(Para);
            }
          }
        }

        var UpdateSearchName = new Parameters.ParameterComponent();
        UpdateSearchName.Name = "UpdateSearchNameList";
        ReturnParametersResource.Parameter.Add(UpdateSearchName);
        var UpdateSearchNameList = CompairisonResultList.Where(x => x.UpdateSearchParameterList.Count > 0);
        foreach (var Update1 in UpdateSearchNameList)
        {
          foreach (var Update2 in Update1.UpdateSearchParameterList)
          {
            foreach (var Update3 in Update2.Value)
            {
              var Para = new Parameters.ParameterComponent();
              Para.Name = "UpdateSearchNameDetails";
              Para.Value = new FhirString($"Search Name for: Resource: {Update3.New.Resource}; NewSearchName: {Update3.New.Name}; OldSearchName: {Update3.Old.Name}; SearchParameterResourceId: {Update3.New.SearchParameterResourceId}; NewSearchParameterVersion: {Update3.New.SearchParameterResourceVersion}; OldSearchParameterVersion: {Update3.Old.SearchParameterResourceVersion}");
              if (UpdateSearchName.Part == null)
                UpdateSearchName.Part = new List<Parameters.ParameterComponent>();
              UpdateSearchName.Part.Add(Para);
            }
          }
        }

        ResourceServiceOutcome.HttpStatusCode = System.Net.HttpStatusCode.OK;
        ResourceServiceOutcome.ResourceResult = ReturnParametersResource;
        ResourceServiceOutcome.OperationType = Enum.RestEnum.CrudOperationType.Update;
        ResourceServiceOutcome.SuccessfulTransaction = true;
        return ResourceServiceOutcome;
      }
    }

    private void CreateResourceIndexesInDatabase(List<CompairisonResult> CompairisonResultList, IPyroRequestUri RequestUri)
    {
      var CreateList = CompairisonResultList.Where(x => x.CreateIndexList.Count > 0);
      foreach (string ResourceName in ModelInfo.SupportedResources)
      {
        List<DtoServiceSearchParameterLight> LightList = new List<DtoServiceSearchParameterLight>();
        foreach (var Item in CreateList)
        {
          var ResourceIndexCreateList = Item.CreateIndexList.Where(y => y.Key == ResourceName || y.Key == ResourceType.Resource.GetLiteral());
          foreach (var Item2 in ResourceIndexCreateList)
          {
            foreach (var Item3 in Item2.Value)
            {
              LightList.Add(Item3 as DtoServiceSearchParameterLight);
            }
          }
        }
        if (LightList.Count() > 0)
        {          
          ResourceType ResourceType = Common.Tools.ResourceNameResolutionSupport.GetResourceType(ResourceName);
          IResourceServices.AddResourceIndexs(ResourceType, LightList, RequestUri);
        }
      }

      //Update the DB SearchParameters to IsIndex = true
      foreach (var Item in CreateList)
      {
        foreach (var Updated in Item.CreateIndexList)
        {
          foreach (var Updated2 in Updated.Value)
          {
            Updated2.IsIndexed = true;
            IServiceSearchParameterService.UpdateServiceSearchParametersHeavy(Updated2);
          }
        }
      }

      //Update the DB SearchParameters where name changed
      var UpdateList = CompairisonResultList.Where(x => x.UpdateSearchParameterList.Count > 0);
      foreach (var Item in UpdateList)
      {
        foreach (var Updated in Item.UpdateSearchParameterList)
        {
          foreach (var Updated2 in Updated.Value)
          {
            Updated2.New.Id = Updated2.Old.Id;
            Updated2.New.IsIndexed = Updated2.Old.IsIndexed;
            IServiceSearchParameterService.UpdateServiceSearchParametersHeavy(Updated2.New);
          }
        }
      }

    }

    private List<CompairisonResult> CalculateDiff(List<DtoServiceSearchParameterHeavy> DbSearchParameterList, IPyroRequestUri RequestUri)
    {
      List<CompairisonResult> CompairisonResultList = new List<CompairisonResult>();
      var GroupList = DbSearchParameterList.GroupBy(x => x.SearchParameterResourceId);      

      foreach (var OldList in GroupList)
      {        
        var FirstInstance = OldList.FirstOrDefault();
        IRequestMeta RequestMeta = IRequestMetaFactory.CreateRequestMeta().Set($"{ResourceType.SearchParameter.GetLiteral()}/{FirstInstance.SearchParameterResourceId}");
        IResourceServiceOutcome CurrentParameter = IResourceServices.GetRead(FirstInstance.SearchParameterResourceId, RequestMeta);        
        CompairisonResult Result;
        if (CurrentParameter.ResourceVersionNumber != FirstInstance.SearchParameterResourceVersion)
        {
          if (CurrentParameter.IsDeleted.HasValue && CurrentParameter.IsDeleted.Value)
          {
            //need to drop all?
            Result = new CompairisonResult();
            foreach (var Old in OldList)
            {
              Result.AddDeleteSearchParameter(Old);
              Result.AddIndexDelete(Old);
            }
            CompairisonResultList.Add(Result);
          }
          else
          {
            var NewSearchParameterResource = CurrentParameter.ResourceResult as SearchParameter;
            var ValadationOperationOutCome = ValidateSearchParameterResource(NewSearchParameterResource);
            if (ValadationOperationOutCome == null)
            {
              List<DtoServiceSearchParameterHeavy> NewList = GenerateDbSearchParameterList(NewSearchParameterResource);
              Result = GetCompairisonResult(OldList.ToList(), NewList);
              CompairisonResultList.Add(Result);
            }
            else
            {
              Result = new CompairisonResult();
              Result.OperationOutcome = ValadationOperationOutCome;
              CompairisonResultList.Add(Result);
            }
          }
        }
        else
        {
          //SearchParameter resource versions are the same but check if it has been indexed as yet
          // if not it needs to be
          //need to drop all?
          Result = new CompairisonResult();
          foreach (var Old in OldList)
          {
            //If the Version is the same and IsIndex is true then no work to do.
            //otherwise, if IsIndex is false then it needs to be indexed so ad to CreateIndex
            if (!Old.IsIndexed)
              Result.AddCreateIndex(Old);
          }
          CompairisonResultList.Add(Result);
        }
      }
      return CompairisonResultList;
    }

    private CompairisonResult GetCompairisonResult(List<DtoServiceSearchParameterHeavy> OldList, List<DtoServiceSearchParameterHeavy> NewList)
    {
      CompairisonResult CompairisonResult = new CompairisonResult();
      //OldList = OldList.OrderBy(x => x.Resource).ToList();
      //NewList = NewList.OrderBy(x => x.Resource).ToList();

      //Find the Old items not in the new list so they can be removed
      foreach (var Old in OldList)
      {
        var New = NewList.SingleOrDefault(x => x.Resource == Old.Resource);
        if (New == null)
        {
          CompairisonResult.AddIndexDelete(Old);
          CompairisonResult.AddDeleteSearchParameter(Old);
        }
      }

      foreach (var New in NewList)
      {
        //New items not in the old list can be added
        var Old = OldList.SingleOrDefault(x => x.Resource == New.Resource);
        if (Old == null)
        {
          CompairisonResult.AddCreateIndex(New);
          CompairisonResult.AddCreateSearchParameter(New);
        }
        else
        {
          //Is in the Old and new list so check the properties of each

          bool ChangeDetected = false;

          //Check the Publication Status has not changed
          if (ChangeDetected == false && Old.Status != New.Status)
          {
            if (Old.Status == PublicationStatus.Active && New.Status != PublicationStatus.Active)
            {
              ChangeDetected = true;

              //Delete index List
              CompairisonResult.AddIndexDelete(Old);

              //Delete the Custom SearchParameter reference
              CompairisonResult.AddDeleteSearchParameter(Old);

            }
            else if (Old.Status != PublicationStatus.Active && New.Status == PublicationStatus.Active)
            {
              ChangeDetected = true;

              //Delete the old Add the new 
              CompairisonResult.AddIndexDelete(Old);
              CompairisonResult.AddCreateIndex(New);

              //Update the Custom SearchParameter reference
              CompairisonResult.AddDeleteSearchParameter(Old);
              CompairisonResult.AddCreateSearchParameter(New);
            }
          }

          //Check the Expression or Type has not changed
          if (ChangeDetected == false &&
            Old.Expression != New.Expression ||
            Old.Type != New.Type)
          {
            ChangeDetected = true;
            //Delete index List
            CompairisonResult.AddIndexDelete(Old);

            //Add index List
            CompairisonResult.AddCreateIndex(New);

            //Update the Custom SearchParameter reference
            CompairisonResult.AddDeleteSearchParameter(Old);
            CompairisonResult.AddCreateSearchParameter(New);
          }

          if (ChangeDetected == false &&
            Old.Name != New.Name)
          {
            CompairisonResult.AddUpdateSearchParameter(Old, New);
          }

        }
      }
      return CompairisonResult;
    }

    private OperationOutcome ValidateSearchParameterResource(SearchParameter Resource)
    {
      var IssueList = new List<OperationOutcome.IssueComponent>();

      if (string.IsNullOrWhiteSpace(Resource.Code))
      {
        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = OperationOutcome.IssueSeverity.Error;
        Issue.Diagnostics = $"The SearchParameter Resource  with the id={Resource.Id} must have a Code element that is not empty.";
        IssueList.Add(Issue);
      }

      if (Resource.Base == null || Resource.Base.Count() == 0)
      {
        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = OperationOutcome.IssueSeverity.Error;
        Issue.Diagnostics = $"The SearchParameter Resource  with the id={Resource.Id} must have a least one base element.";
        IssueList.Add(Issue);
      }
      else
      {
        foreach (var Item in Resource.Base)
        {
          if (!Item.HasValue)
          {
            var Issue = new OperationOutcome.IssueComponent();
            Issue.Severity = OperationOutcome.IssueSeverity.Error;
            Issue.Diagnostics = $"The SearchParameter Resource  with the id={Resource.Id} base element was null or perhaps not a known FHIR Resource type.";
            IssueList.Add(Issue);
          }
        }
      }



      if (!Resource.Type.HasValue)
      {
        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = OperationOutcome.IssueSeverity.Error;
        Issue.Diagnostics = $"The SearchParameter Resource with the id={Resource.Id} must have a Type element that is not empty.";
        IssueList.Add(Issue);
      }


      if (string.IsNullOrWhiteSpace(Resource.Expression))
      {
        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = OperationOutcome.IssueSeverity.Error;
        Issue.Diagnostics = $"The SearchParameter Resource  with the id={Resource.Id} must have an Expression element that is not empty.";
        IssueList.Add(Issue);
      }

      if (!Resource.Status.HasValue)
      {
        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = OperationOutcome.IssueSeverity.Error;
        Issue.Diagnostics = $"The SearchParameter Resource  with the id={Resource.Id} must have a Status element that is not empty.";
        IssueList.Add(Issue);
      }

      if (IssueList.Count > 0)
      {
        var OperationOutCome = Common.Tools.FhirOperationOutcomeSupport.Generate(IssueList);
        return OperationOutCome;
      }
      else
      {
        return null;
      }
    }

    private List<DtoServiceSearchParameterHeavy> GenerateDbSearchParameterList(SearchParameter SearchParameterResource)
    {
      var ReturnList = new List<DtoServiceSearchParameterHeavy>();
      foreach (var Base in SearchParameterResource.Base)
      {
        var New = new DtoServiceSearchParameterHeavy();
        if (SearchParameterResource.Description != null)
          New.Description = SearchParameterResource.Description.Value;
        New.Expression = SearchParameterResource.Expression;
        New.Name = SearchParameterResource.Code;
        New.Resource = Base.GetLiteral();
        New.SearchParameterResourceId = SearchParameterResource.Id;
        New.SearchParameterResourceVersion = SearchParameterResource.Meta.VersionId;
        New.TargetResourceTypeList = new List<IServiceSearchParameterTargetResource>();
        if (SearchParameterResource.Target != null)
        {
          foreach (var Target in SearchParameterResource.Target)
          {
            if (Target.HasValue)
              New.TargetResourceTypeList.Add(new DtoServiceSearchParameterTargetResource() { ResourceType = Target.Value });
          }
        }
        New.Type = SearchParameterResource.Type.Value;
        New.Url = SearchParameterResource.Url;
        New.XPath = SearchParameterResource.Xpath;
        New.Status = SearchParameterResource.Status.Value;
        New.IsIndexed = false;
        ReturnList.Add(New);
      }
      return ReturnList;
    }

    private class CompairisonResult
    {
      public void AddIndexDelete(DtoServiceSearchParameterHeavy item)
      {
        AddToList(this.DeleteIndexList, item);
      }
      public void AddCreateIndex(DtoServiceSearchParameterHeavy item)
      {
        AddToList(this.CreateIndexList, item);
      }

      public void AddCreateSearchParameter(DtoServiceSearchParameterHeavy item)
      {
        AddToList(this.CreateSearchParameterList, item);
      }
      public void AddUpdateSearchParameter(DtoServiceSearchParameterHeavy Old, DtoServiceSearchParameterHeavy New)
      {
        var Update = new Update();
        Update.Old = Old;
        Update.New = New;

        if (UpdateSearchParameterList.ContainsKey(Old.Resource))
        {
          UpdateSearchParameterList[Old.Resource].Add(Update);
        }
        else
        {
          UpdateSearchParameterList.Add(Old.Resource, new List<Update>() { Update });
        }


      }
      public void AddDeleteSearchParameter(DtoServiceSearchParameterHeavy item)
      {
        AddToList(this.DeleteSearchParameterList, item);
      }

      public OperationOutcome OperationOutcome { get; set; }
      public Dictionary<string, List<DtoServiceSearchParameterHeavy>> DeleteIndexList;
      public Dictionary<string, List<DtoServiceSearchParameterHeavy>> CreateIndexList;

      public Dictionary<string, List<DtoServiceSearchParameterHeavy>> CreateSearchParameterList;
      public Dictionary<string, List<Update>> UpdateSearchParameterList;
      public Dictionary<string, List<DtoServiceSearchParameterHeavy>> DeleteSearchParameterList;

      private void AddToList(Dictionary<string, List<DtoServiceSearchParameterHeavy>> List, DtoServiceSearchParameterHeavy item)
      {
        if (List.ContainsKey(item.Resource))
        {
          List[item.Resource].Add(item);
        }
        else
        {
          List.Add(item.Resource, new List<DtoServiceSearchParameterHeavy>() { item });
        }
      }

      public CompairisonResult()
      {
        this.DeleteIndexList = new Dictionary<string, List<DtoServiceSearchParameterHeavy>>();
        this.CreateIndexList = new Dictionary<string, List<DtoServiceSearchParameterHeavy>>();

        this.CreateSearchParameterList = new Dictionary<string, List<DtoServiceSearchParameterHeavy>>();
        this.UpdateSearchParameterList = new Dictionary<string, List<Update>>();
        this.DeleteSearchParameterList = new Dictionary<string, List<DtoServiceSearchParameterHeavy>>();
      }

      public class Update
      {
        public DtoServiceSearchParameterHeavy Old { get; set; }
        public DtoServiceSearchParameterHeavy New { get; set; }
      }
    }

  }
}
