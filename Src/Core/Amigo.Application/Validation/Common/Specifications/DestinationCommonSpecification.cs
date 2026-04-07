using Amigo.Application.Mapping;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Amigo.Application.Validation.Common.Specifications
{
    public static class DestinationCommonSpecification 
    {
        public static Expression<Func<Destination, bool>> BuildCriteria(
       GetAllDestinationQuery requestQuery, bool isAdmin)
        {
            Language? language = null;
            if (!string.IsNullOrWhiteSpace(requestQuery.Language))
            { 
                language = EnumsMapping.ToLanguageEnum(requestQuery.Language);
            }

            return d =>

                    (
                        d.Translations.Any(
                            t =>
                            (
                                string.IsNullOrWhiteSpace(requestQuery.Name)
                                ||
                                t.Name.ToLower().Contains(requestQuery.Name.ToLower())
                            )
                            &&
                            (
                                string.IsNullOrWhiteSpace(requestQuery.Language)
                                ||
                                t.Language == language
                            )

                        )





                        &&
                        (
                            string.IsNullOrWhiteSpace(requestQuery.CountryCode)
                            ||
                            d.CountryCode == EnumsMapping.ToCountryCodeEnum(requestQuery.CountryCode)
                         )

                    &&
                        (isAdmin || d.IsActive)

                
                  );
                
        }

        public static Expression<Func<Destination, bool>> BuildGetDestinaionByIdCriteria(
        GetDestinationByIdQuery requestQuery ,Guid destinationId)
        {
            Language? language = null;
            if (!string.IsNullOrWhiteSpace(requestQuery.Language))
            {
                language = EnumsMapping.ToLanguageEnum(requestQuery.Language);
            }

         
           
                return d => 
                (
                    d.Id == destinationId 
                    && 
                    d.IsActive == true
                    &&
                     d.Translations.Any(
                            t =>
                            
                            (
                                string.IsNullOrWhiteSpace(requestQuery.Language)
                                ||
                                t.Language == language
                            )

                        )
                );

              

        }



    }
}
