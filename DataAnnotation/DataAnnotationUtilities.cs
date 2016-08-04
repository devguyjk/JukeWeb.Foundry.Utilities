using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

#if !SILVERLIGHT
using System.ComponentModel;
using JukeWeb.Foundry.Utilities.Common;
#endif

namespace JukeWeb.Foundry.Utilities
{
    /// <summary>
    /// This class will be used to process class data annotations.
    /// THe values should be use cache
    /// </summary>
    public class DataAnnotationUtilities
    {
        private object _parentType;
        private List<MetadataAttributeInfo> _dataAnnotationAttributes;

        public DataAnnotationUtilities(object parentType)
        {
            _parentType = parentType;
        }

        public DisplayAttribute GetPropertyDisplayAttribute(string propertyName)
        {
            var displayAttributeInfo = DataAnnotationAttributes.FirstOrDefault(x => x.PropertyName == propertyName && x.Attribute.GetType() == typeof(DisplayAttribute));
            return displayAttributeInfo.Attribute as DisplayAttribute;
        }

        public List<ValidationResult> GetValidationResultsByProperty(object parentType, string propertyName)
        {
            var validationResults = GetValidationResults(parentType).Where(x => x.MemberNames.Contains(propertyName)).ToList();
            return validationResults;
        }

        public List<ValidationResult> GetValidationResults(object parentType)
        {
            
                List<ValidationResult> validationResultsList = new List<ValidationResult>();
                var validationAttributes = DataAnnotationAttributes.Where(x => (x.Attribute as ValidationAttribute) != null).ToList();

#if ! SILVERLIGHT
        try
        {
            var properties = TypeDescriptor.GetProperties(parentType.GetType()).Cast<PropertyDescriptor>();
            var validationResults = from attribute in validationAttributes
                                    from property in properties
                                    where property.Name == attribute.PropertyName
                                    select (attribute.Attribute as ValidationAttribute).GetValidationResult(property.GetValue(parentType), new ValidationContext(parentType, null, null) { MemberName = attribute.PropertyName });

            int count = 0;
            foreach (ValidationResult vResult in validationResults)
            {
                count++;
                if (vResult != null)
                    validationResultsList.Add(vResult);
            }

            return validationResultsList.ToList();
        }
        catch (Exception ex)
        {
            throw new FoundryGeneralException(ex.Message, ex);
        }
#else
                    var properties = parentType.GetType().GetProperties();
                    var validationResults = from attribute in validationAttributes
                                            from property in properties 
                                            where property.Name == attribute.PropertyName
                                            select (attribute.Attribute as ValidationAttribute).GetValidationResult(property.GetValue(parentType, null), new ValidationContext(parentType, null, null) { MemberName = property.Name });
                    foreach (ValidationResult vResult in validationResults)
                    if (vResult != null)
                        validationResultsList.Add(vResult);

                return validationResultsList.ToList();
#endif


        }


        public List<MetadataAttributeInfo> DataAnnotationAttributes
        {
            get 
            {
                if (_dataAnnotationAttributes == null)
                {
                    List<Attribute> attributes = new List<Attribute>();
                    List<MetadataAttributeInfo> metadataAttrInfoList = new List<MetadataAttributeInfo>();
                    Type baseType = _parentType.GetType();

                    while (baseType != null && !baseType.IsAbstract)
                    {
                        var metadataAttrib = baseType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();

                        if (metadataAttrib != null)
                        {
                            var metadataClassObj = metadataAttrib.MetadataClassType;
#if !SILVERLIGHT
                            var metadataClassPoperties = TypeDescriptor.GetProperties(metadataClassObj).Cast<PropertyDescriptor>();
                            var modelClassProperties = TypeDescriptor.GetProperties(baseType).Cast<PropertyDescriptor>();
                            var metadataAttrInfoShortList = from metadataProp in metadataClassPoperties
                                                            join modelProp in modelClassProperties on metadataProp.Name equals modelProp.Name
                                                            from attribute in metadataProp.Attributes.OfType<Attribute>()

#else   
                            var metadataClassPoperties = metadataClassObj.GetProperties();
                            var modelClassProperties = baseType.GetProperties();
                            var metadataAttrInfoShortList = from metadataProp in metadataClassPoperties
                                                            join modelProp in modelClassProperties on metadataProp.Name equals modelProp.Name
                                                            from attribute in metadataProp.GetCustomAttributes(true).OfType<Attribute>()
#endif
                                                            select new MetadataAttributeInfo() { Attribute = attribute, ParentType = baseType, PropertyName = metadataProp.Name };

                            metadataAttrInfoList.AddRange(metadataAttrInfoShortList.ToList());
                            baseType = baseType.BaseType;
                        }
                    }

                    _dataAnnotationAttributes = metadataAttrInfoList;
                }

                return _dataAnnotationAttributes;
            }
        }
    }

    public class MetadataAttributeInfo
    {
        public Attribute Attribute { get; set; }
        public string PropertyName { get; set; }
        public Type ParentType { get; set; }   
    }

}
