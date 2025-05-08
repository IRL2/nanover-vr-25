using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanover.Core;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Components
{
    public class VisualisationUtility
    {
        /// <summary>
        /// Get all fields on an object which are visualisation properties.
        /// </summary>
        public static IEnumerable<(string, IReadOnlyProperty)> GetAllPropertyFields(object obj)
        {
            var allFields = obj.GetType()
                               .GetFieldsInSelfOrParents(BindingFlags.Instance
                                                       | BindingFlags.NonPublic
                                                       | BindingFlags.Public);

            var validFields = allFields.Where(field => typeof(IReadOnlyProperty).IsAssignableFrom(
                                                  field.FieldType
                                              ));

            return validFields.Select(field => (field.Name,
                                                field.GetValue(obj) as
                                                    IReadOnlyProperty));
        }
    }
}