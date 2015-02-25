using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SampleWebApi.Models;

namespace SampleWebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values

        public IHttpActionResult Post(PostParams postParams)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"myfile.txt");
            using (var fileStream = File.Open(path, FileMode.OpenOrCreate))
            {
                var readBytes = new Byte[fileStream.Length];
                fileStream.Read(readBytes, 0, readBytes.Length);
                var readValues = Encoding.UTF8.GetString(readBytes);
                var myString = readValues.Split(',');
                if (myString.Length >= 5)
                {
                    //Task.Delay(TimeSpan.FromMilliseconds(3000));
                    return InternalServerError(new Exception("Exceeded the limit"));
                }

                var writeBytes = Encoding.UTF8.GetBytes(postParams.Value + ",");
                fileStream.Write(writeBytes, 0, writeBytes.Length);
                return Created(new Uri("/Get/1",UriKind.Relative),"createdContent");
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
