﻿using System;
using Hl7.Fhir.Model;
using Blaze.DataModel.DatabaseModel.Base;
using System.Collections.Generic;
using Blaze.Common.Interfaces.UriSupport;
using Blaze.DataModel.Repository.Interfaces;
using Blaze.DataModel.Support;

namespace Blaze.DataModel.IndexSetter
{
  public class TokenIndexSetter : ITokenIndexSetter
  {
    public ModelBase Set(Element FhirElement, ModelBase ModelBase, IDtoFhirRequestUri FhirRequestUri = null, ICommonRepository CommonRepository = null)
    {
      if (ModelBase == null)
        throw new ArgumentNullException("ModelBase cannot be null for method.");

      if (FhirElement == null)
        throw new ArgumentNullException("FhirElement cannot be null for method.");

      if (ModelBase is TokenIndex)
      {
        var StringIndex = ModelBase as TokenIndex;
        if (FhirCodeDataTypeSupport.IsGenericCodeDataType(FhirElement))
        {
          return SetCodeT(FhirElement, StringIndex);
        }
        else if (FhirElement is Code)
        {
          return SetCode(FhirElement as Code, StringIndex);
        }
        else if (FhirElement is CodeableConcept)
        {
          return SetCodeableConcept(FhirElement as CodeableConcept, StringIndex);
        }
        else if (FhirElement is Coding)
        {
          return SetCoding(FhirElement as Coding, StringIndex);
        }
        else if (FhirElement is ContactPoint)
        {
          return SetContactPoint(FhirElement as ContactPoint, StringIndex);
        }
        else if (FhirElement is FhirBoolean)
        {
          return SetFhirBoolean(FhirElement as FhirBoolean, StringIndex);
        }
        else if (FhirElement is FhirDateTime)
        {
          return SetFhirDateTime(FhirElement as FhirDateTime, StringIndex);
        }
        else if (FhirElement is FhirString)
        {
          return SetFhirString(FhirElement as FhirString, StringIndex);
        }
        else if (FhirElement is Id)
        {
          return SetId(FhirElement as Id, StringIndex);
        }
        else if (FhirElement is Identifier)
        {
          return SetIdentifier(FhirElement as Identifier, StringIndex);
        }
        else if (FhirElement is PositiveInt)
        {
          return SetPositiveInt(FhirElement as PositiveInt, StringIndex);
        }
        else if (FhirElement is Quantity)
        {
          return SetQuantity(FhirElement as Quantity, StringIndex);
        }
        else
        {
          throw new Exception(string.Format("FhirElement was unexpected type for TokenIndex, type was {0}", FhirElement.ToString()));
        }
      }
      else
      {
        throw new InvalidCastException(string.Format("TokenIndex expected typeof {0} yet was passed typeof {1}", typeof(TokenIndex).Name, ModelBase.GetType().Name));
      }

    }

    public TokenIndex SetCodeT(Element Element, TokenIndex TokenIndex)
    {
      if (Element == null)
        throw new ArgumentNullException("Element cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (FhirCodeDataTypeSupport.TokenIndexSetter(Element, TokenIndex))
      {
        if (TokenIndex.Code == null)
          return null;
        else
          return TokenIndex;
      }
      else
      {
        return null;
      }
    }

    public TokenIndex SetCode(Code Code, TokenIndex TokenIndex)
    {
      if (Code == null)
        throw new ArgumentNullException("Code cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (!string.IsNullOrWhiteSpace(Code.Value))
      {
        TokenIndex.Code = Code.Value.Trim();
        TokenIndex.System = null;
        return TokenIndex;
      }
      else
      {
        return null;
      }

    }

    public TokenIndex SetCodeableConcept(CodeableConcept CodeableConcept, TokenIndex TokenIndex)
    {
      var oIssueComponent = new OperationOutcome.IssueComponent();
      oIssueComponent.Severity = OperationOutcome.IssueSeverity.Fatal;
      oIssueComponent.Code = OperationOutcome.IssueType.Exception;
      oIssueComponent.Diagnostics = "Attempt set a CodeableConcept directly in the TokenIndexSetter. This should not happen as all CodeableConcept must be iterated and passed in as Coding.";
      var oOperationOutcome = new OperationOutcome();
      oOperationOutcome.Issue = new List<OperationOutcome.IssueComponent>() { oIssueComponent };
      throw new Blaze.Common.BusinessEntities.Dto.DtoBlazeException(System.Net.HttpStatusCode.BadRequest, oOperationOutcome, oIssueComponent.Diagnostics);
    }

    public TokenIndex SetCoding(Coding Coding, TokenIndex TokenIndex)
    {
      if (Coding == null)
        throw new ArgumentNullException("Coding cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (string.IsNullOrWhiteSpace(Coding.Code) && string.IsNullOrWhiteSpace(Coding.System))
        return null;

      if (!string.IsNullOrWhiteSpace(Coding.Code))
      {
        TokenIndex.Code = Coding.Code.Trim();
      }
      if (!string.IsNullOrWhiteSpace(Coding.System))
      {
        TokenIndex.System = Coding.System.Trim();
      }
      return TokenIndex;      
    }

    public TokenIndex SetContactPoint(ContactPoint ContactPoint, TokenIndex TokenIndex)
    {
      if (ContactPoint == null)
        throw new ArgumentNullException("ContactPoint cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (string.IsNullOrWhiteSpace(ContactPoint.Value) && ContactPoint.System == null)
        return null;

      if (!string.IsNullOrWhiteSpace(ContactPoint.Value))
      {
        TokenIndex.Code = ContactPoint.Value.Trim();
      }
      if (ContactPoint.System != null)
      {
        TokenIndex.System = Hl7.Fhir.Introspection.EnumMapping.Create(typeof(Hl7.Fhir.Model.ContactPoint.ContactPointSystem)).GetLiteral(ContactPoint.System);
      }
      return TokenIndex;
    }

    public TokenIndex SetFhirBoolean(FhirBoolean FhirBoolean, TokenIndex TokenIndex)
    {
      if (FhirBoolean == null)
        throw new ArgumentNullException("FhirBoolean cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");
      if (FhirBoolean.Value != null)
      {
        TokenIndex.Code = FhirBoolean.Value.ToString().ToLower();
        TokenIndex.System = null;
        return TokenIndex;
      }
      else
      {
        return null;
      }
    }

    public TokenIndex SetFhirDateTime(FhirDateTime FhirDateTime, TokenIndex TokenIndex)
    {
      if (FhirDateTime == null)
        throw new ArgumentNullException("FhirDateTime cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (FhirDateTime.Value == null)
        return null;

      if (!Hl7.Fhir.Model.FhirDateTime.IsValidValue(FhirDateTime.Value))
        return null;

      TokenIndex.Code = FhirDateTime.Value;
      TokenIndex.System = null;
      return TokenIndex;        
    }

    public TokenIndex SetFhirString(FhirString FhirString, TokenIndex TokenIndex)
    {
      if (FhirString == null)
        throw new ArgumentNullException("FhirString cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (string.IsNullOrWhiteSpace(FhirString.Value))
        return null;

      TokenIndex.Code = FhirString.Value;
      TokenIndex.System = null;
      return TokenIndex;
    }

    public TokenIndex SetId(Id Id, TokenIndex TokenIndex)
    {
      if (Id == null)
        throw new ArgumentNullException("Id cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (string.IsNullOrWhiteSpace(Id.Value))
        return null;
      TokenIndex.Code = Id.Value.Trim();
      TokenIndex.System = null;
      return TokenIndex;
    }

    public TokenIndex SetIdentifier(Identifier Identifier, TokenIndex TokenIndex)
    {
      if (Identifier == null)
        throw new ArgumentNullException("Identifier cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (string.IsNullOrWhiteSpace(Identifier.Value) && string.IsNullOrWhiteSpace(Identifier.System))
        return null;

      if (!string.IsNullOrWhiteSpace(Identifier.Value))
      {
        TokenIndex.Code = Identifier.Value.Trim();
      }

      if (!string.IsNullOrWhiteSpace(Identifier.System))
      {
        TokenIndex.System = Identifier.System.Trim();
      }
      return TokenIndex;
    }

    public TokenIndex SetPositiveInt(PositiveInt PositiveInt, TokenIndex TokenIndex)
    {
      if (PositiveInt == null)
        throw new ArgumentNullException("PositiveInt cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (PositiveInt.Value == null)
        return null;

      TokenIndex.Code = Convert.ToString(PositiveInt.Value);
      TokenIndex.System = null;
      return TokenIndex;
    }

    public TokenIndex SetQuantity(Quantity Quantity, TokenIndex TokenIndex)
    {
      if (Quantity == null)
        throw new ArgumentNullException("Quantity cannot be null for method.");

      if (TokenIndex == null)
        throw new ArgumentNullException("TokenIndex cannot be null for method.");

      if (Quantity.Value == null && string.IsNullOrWhiteSpace(Quantity.System))
        return null;

      if (Quantity.Value.HasValue)
      {
        TokenIndex.Code = Convert.ToString(Quantity.Value.Value);
      }

      if (!string.IsNullOrWhiteSpace(Quantity.Unit))
      {
        TokenIndex.System = Quantity.Unit.Trim();
      }
      return TokenIndex;
    }
  }
}
