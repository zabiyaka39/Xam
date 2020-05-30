﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTMobile.jiraData
{
    public class InsightObject
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("objectTypeAttribute")]
        public ObjectTypeAttribute ObjectTypeAttribute { get; set; }

        [JsonProperty("objectTypeAttributeId")]
        public long ObjectTypeAttributeId { get; set; }

        [JsonProperty("objectAttributeValues")]
        public List<ObjectAttributeValue> ObjectAttributeValues { get; set; }

        [JsonProperty("objectId")]
        public long ObjectId { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }
    }

    public  class ObjectAttributeValue
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public Status Status { get; set; }
    }

    public  class Status
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("objectSchemaId")]
        public long ObjectSchemaId { get; set; }
    }

    public  class ObjectTypeAttribute
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public bool Label { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("defaultType", NullValueHandling = NullValueHandling.Ignore)]
        public DefaultType DefaultType { get; set; }

        [JsonProperty("editable")]
        public bool Editable { get; set; }

        [JsonProperty("system")]
        public bool System { get; set; }

        [JsonProperty("sortable")]
        public bool Sortable { get; set; }

        [JsonProperty("summable")]
        public bool Summable { get; set; }

        [JsonProperty("minimumCardinality")]
        public long MinimumCardinality { get; set; }

        [JsonProperty("maximumCardinality")]
        public long MaximumCardinality { get; set; }

        [JsonProperty("removable")]
        public bool Removable { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("includeChildObjectTypes")]
        public bool IncludeChildObjectTypes { get; set; }

        [JsonProperty("uniqueAttribute")]
        public bool UniqueAttribute { get; set; }

        [JsonProperty("options")]
        public string Options { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("suffix", NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        [JsonProperty("regexValidation", NullValueHandling = NullValueHandling.Ignore)]
        public string RegexValidation { get; set; }

        [JsonProperty("iql", NullValueHandling = NullValueHandling.Ignore)]
        public string Iql { get; set; }

        [JsonProperty("typeValueMulti", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> TypeValueMulti { get; set; }

        [JsonProperty("referenceType", NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceType ReferenceType { get; set; }

        [JsonProperty("referenceObjectTypeId", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReferenceObjectTypeId { get; set; }

        [JsonProperty("referenceObjectType", NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceObjectType ReferenceObjectType { get; set; }
    }

    public  class DefaultType
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public  class ReferenceObjectType
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("icon")]
        public Icon Icon { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }

        [JsonProperty("objectCount")]
        public long ObjectCount { get; set; }

        [JsonProperty("objectSchemaId")]
        public long ObjectSchemaId { get; set; }

        [JsonProperty("inherited")]
        public bool Inherited { get; set; }

        [JsonProperty("abstractObjectType")]
        public bool AbstractObjectType { get; set; }

        [JsonProperty("parentObjectTypeInherited")]
        public bool ParentObjectTypeInherited { get; set; }
    }

    public  class Icon
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url16")]
        public Uri Url16 { get; set; }

        [JsonProperty("url48")]
        public Uri Url48 { get; set; }
    }

    public  class ReferenceType
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("url16")]
        public Uri Url16 { get; set; }

        [JsonProperty("removable")]
        public bool Removable { get; set; }
    }
}
