using AASA.NetCore.Lib.Helper.Extensions;
using AASA.NetCore.Lib.Helper.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AASA.NetCore.IDomainService.Panic.Models
{
    public partial class PanicRequestModel
    {
        public ClientInformation ClientInformation { get; set; }
        public Vehicle Vehicle { get; set; }
        public GeoProperties Location { get; set; }
        public GeoProperties Destination { get; set; }
        public IncidentInformation SceneInfo { get; set; }
        public ValidationData ValidationInfo { get; set; }
    }

    public partial class IncidentInformation
    {
        [IsAccessible]
        public string IsAccessible { get; set; } = "Yes";
        [GenericStringValidation]
        public string ReasonForInaccessibility { get; set; }

        [TowType]
        [GenericStringValidation]
        public string TowType { get; set; }
        [GenericStringValidation]
        public string TowDescription { get; set; }

        [NoOfPassengers]
        [GenericStringValidation]
        public int NoOfPassengers { get; set; } = 0;
    }

    public partial class ValidationData
    {
        public List<ValidationKeyVal> ValidationFields { get; set; }
        public List<ValidationKeyVal> SchemeRuleData { get; set; }
    }

    public class ValidationKeyVal
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }

    public partial class ClientInformation
    {
        [GenericStringValidation]
        public string Firstname { get; set; }
        [GenericStringValidation]
        public string LastName { get; set; }
        [Email]
        public string Email { get; set; }
        [GenericStringValidation]
        public string IDorPassportNumber { get; set; }
        [ContactNumber]
        public string ContactNumber { get; set; }
        public bool? IsSafe { get; set; }
        public bool? ArmedResponse { get; set; }
        [ClientServiceType]
        [GenericStringValidation]
        public string ClientServiceType { get; set; }
        [CaseType]
        [GenericStringValidation]
        public string CaseType { get; set; }
        [SchemeId]
        [GenericStringValidation]
        public string SchemeId { get; set; }
        [GenericStringValidation]
        public string MembershipNumber { get; set; }
        public bool? CallMeBack { get; set; } = false;

        [GenericStringValidation]
        public string ExternalReferenceNumber { get; set; }
    }

    public partial class GeoProperties
    {
        [GenericStringValidation]
        public string AddressLine { get; set; }
        [GenericStringValidation]
        public string ContactPerson { get; set; }
        [GenericStringValidation]
        public string ContactNumber { get; set; }
        [GenericStringValidation]
        public string AdditionalInformation { get; set; }
        [LatLng]
        public string Latitude { get; set; }
        [LatLng]
        public string Longitude { get; set; }
    }

    public partial class Vehicle
    {
        [GenericStringValidation]
        public string VehicleMake { get; set; }
        [GenericStringValidation]
        public string VehicleModel { get; set; }
        [GenericStringValidation]
        public string VehicleRegistrationNumber { get; set; }
        [GenericStringValidation]
        public long? VehicleYear { get; set; }
        [GenericStringValidation]
        public string VehicleColour { get; set; }
        [GenericStringValidation]
        public string VehicleTransmissionType { get; set; }
        [GenericStringValidation]
        public string VehicleVinNumber { get; set; }
    }

    public class PanicResult
    {
        public string CaseNumber { get; set; }
    } 

    public class ResultPanicResponse : ResultBase
    {
        public PanicResult Data { get; set; }
    }
}
