﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Blaze.Engine.Search.SearchTermTypes;

namespace Blaze.Engine.Search
{
  public class PatientSearchPlan : SearchPlanBase, Interfaces.ISearchPlan
  {
    public SearchResult Search(SearchTerms oSearchTerms)
    {
      //Check that the SearchTerms have not already thrown an error 
      var oSearchResult = new SearchResult();
      if (oSearchTerms.HasError)
      {
        oSearchResult.OperationOutcome = oSearchTerms.OperationOutcome;
        return oSearchResult;
      }

      //The search plan;
      if (oSearchTerms.SearchTermList.Count == 1)
      {
        if (oSearchTerms.SearchTermList.TrueForAll(x => x.Modifier == SearchModifierType.None && x.Prefix == SearchPrefixType.None))
        {
          if (!oSearchTerms.SearchTermList.Any(x => x.HasLogicalOrProperties == true))
          {
            if (oSearchTerms.SearchTermList[0].Name == Support.EnumSupport.SearchTermName._Id)
            {
              Search.SearchTermTypes.SearchTermString _Id = oSearchTerms.SearchTermList[0] as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFhirId(_Id.Values[0]);
              return oSearchResult;
            }
            else if (oSearchTerms.SearchTermList[0].Name == Support.EnumSupport.SearchTermName.Family)
            {
              Search.SearchTermTypes.SearchTermString Family = oSearchTerms.SearchTermList[0] as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(Family.Values[0], string.Empty, string.Empty, string.Empty, 1, RequestUri);
              return oSearchResult;
            }
            else if (oSearchTerms.SearchTermList[0].Name == Support.EnumSupport.SearchTermName.Given)
            {
              Search.SearchTermTypes.SearchTermString Given = oSearchTerms.SearchTermList[0] as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(string.Empty, Given.Values[0], string.Empty, string.Empty, 1, RequestUri);
              return oSearchResult;
            }
            else if (oSearchTerms.SearchTermList[0].Name == Support.EnumSupport.SearchTermName.Name)
            {
              Search.SearchTermTypes.SearchTermString Name = oSearchTerms.SearchTermList[0] as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(string.Empty, string.Empty, Name.Values[0], string.Empty, 1, RequestUri);
              return oSearchResult;
            }
            else if (oSearchTerms.SearchTermList[0].Name == Support.EnumSupport.SearchTermName.Phonetic)
            {
              Search.SearchTermTypes.SearchTermString Phonetic = oSearchTerms.SearchTermList[0] as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(string.Empty, string.Empty, string.Empty, Phonetic.Values[0], 1, RequestUri);
              return oSearchResult;
            }
          }
        }
      }
      else if (oSearchTerms.SearchTermList.Count == 2)
      {
        if (oSearchTerms.SearchTermList.TrueForAll(x => x.Modifier == SearchModifierType.None && x.Prefix == SearchPrefixType.None))
        {
          if (!oSearchTerms.SearchTermList.Any(x => x.HasLogicalOrProperties == true))
          {
            if (oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Given)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Family))
            {
              Search.SearchTermTypes.SearchTermString Given = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Given) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermString Family = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Family) as Search.SearchTermTypes.SearchTermString;
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(Family.Values[0], Given.Values[0], string.Empty, string.Empty, 1, RequestUri);
              return oSearchResult;
            }

            if (oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Given)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Page))
            {
              Search.SearchTermTypes.SearchTermString Given = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Given) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermNumber Page = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Page) as Search.SearchTermTypes.SearchTermNumber;
              int Pagenumber = Convert.ToInt32(Page.Values[0]);
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(string.Empty, Given.Values[0], string.Empty, string.Empty, Pagenumber, RequestUri);
              return oSearchResult;
            }

            if (oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Family)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Page))
            {
              Search.SearchTermTypes.SearchTermString Family = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Family) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermNumber Page = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Page) as Search.SearchTermTypes.SearchTermNumber;
              int Pagenumber = Convert.ToInt32(Page.Values[0]);
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(Family.Values[0], string.Empty, string.Empty, string.Empty, Pagenumber, RequestUri);
              return oSearchResult;
            }
            if (oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Name)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Page))
            {
              Search.SearchTermTypes.SearchTermString Name = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Name) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermNumber Page = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Page) as Search.SearchTermTypes.SearchTermNumber;
              int Pagenumber = Convert.ToInt32(Page.Values[0]);
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(string.Empty, string.Empty, Name.Values[0], string.Empty, Pagenumber, RequestUri);
              return oSearchResult;
            }



          }
        }
      }
      else if (oSearchTerms.SearchTermList.Count == 3)
      {
        if (oSearchTerms.SearchTermList.TrueForAll(x => x.Modifier == SearchModifierType.None && x.Prefix == SearchPrefixType.None))
        {
          if (!oSearchTerms.SearchTermList.Any(x => x.HasLogicalOrProperties == true))
          {
            if (oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Given)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Family)
              && oSearchTerms.SearchTermList.Exists(x => x.Name == Support.EnumSupport.SearchTermName.Page))
            {
              Search.SearchTermTypes.SearchTermString Given = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Given) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermString Family = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Family) as Search.SearchTermTypes.SearchTermString;
              Search.SearchTermTypes.SearchTermNumber Page = oSearchTerms.SearchTermList.FirstOrDefault(x => x.Name == Support.EnumSupport.SearchTermName.Page) as Search.SearchTermTypes.SearchTermNumber;
              int Pagenumber = Convert.ToInt32(Page.Values[0]);
              oSearchResult.FhirBundle = _UnitOfWork.PatientRepository.SearchByFamilyAndGiven(Family.Values[0], Given.Values[0], string.Empty, string.Empty, Pagenumber, RequestUri);
              return oSearchResult;
            }
          }
        }
      }
      var OpOutComeIssueComp = new OperationOutcome.IssueComponent();
      OpOutComeIssueComp.Severity = OperationOutcome.IssueSeverity.Error;
      OpOutComeIssueComp.Code = OperationOutcome.IssueType.Invalid;
      string ParameterListForErrorMessage = string.Empty;
      foreach(var item in oSearchTerms.SearchTermList)
      {
        ParameterListForErrorMessage += item.RawValue + " & "; 
      }
      ParameterListForErrorMessage = ParameterListForErrorMessage.Substring(0, ParameterListForErrorMessage.Length - 3);
      OpOutComeIssueComp.Details = new CodeableConcept("http://hl7.org/fhir/operation-outcome", "MSG_PARAM_UNKNOWN", String.Format("Parameter '{0}' not understood", ParameterListForErrorMessage));
      OpOutComeIssueComp.Details.Text = String.Format("This search parameter combination provided is not supported by the server. Parameters were: {0}", ParameterListForErrorMessage);
      oSearchResult.AddOperationOutcomeIssue(OpOutComeIssueComp, System.Net.HttpStatusCode.Forbidden);     
      return oSearchResult;
    }
  }
}