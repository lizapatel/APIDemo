using APIDemo.BAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace APIDemo.DAL.Common
{
    public class EntityConverter<T> where T : class
    {
        public string ConvertEntityToXml(T entity)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter))
                {
                    serializer.Serialize(xmlWriter, entity);
                }
                return stringWriter.ToString();
            }
        }
    }
}
