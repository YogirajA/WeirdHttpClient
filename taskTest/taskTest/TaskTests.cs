using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace taskTest
{
   
    public class TaskTests
    {
        [Fact]
        public async Task FailTaskTest()
        {
            try
            {
                var r1 = await MyAsyncMethod1();
                Console.WriteLine(r1.Value);
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex.StackTrace);
            }
            var r2 = await MyAsyncMethod2();
            Console.WriteLine(r2.Value);
        }

        [Fact]
        public async Task CallPublicApi()
        {
           // await CallPublicApiUsingHttpClient();
            await CallMyApiThatFailsOnFifthPost();
        }

        public async Task CallMyApiThatFailsOnFifthPost()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:1965/", UriKind.RelativeOrAbsolute)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var source = new CancellationTokenSource(1000*30);
            source.Token.Register(MyTokenMethod);
            
            const string content = @"{ ""value"": ""myValue"" }";
            
            for (var i = 0; i < 5; i++)
            {
                var response =await  client.PostAsync("api/values", 
                    new StringContent(content,Encoding.UTF8,"application/json"), source.Token)
                    .ContinueWith(t =>
                    {
                        if(t.IsFaulted)
                            Console.WriteLine(t.Exception);
                        if(t.IsCanceled)
                            Console.WriteLine("Canceled");
                        return t.Result;
                    });
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content.ReadAsStringAsync());
            }
            


        }

        private static void MyTokenMethod()
        {
            Console.WriteLine("Failed the call");
        }

        public async Task CallPublicApiUsingHttpClient()
        {
            var client = new HttpClient {BaseAddress = new Uri("http://www.thomas-bayer.com/",UriKind.RelativeOrAbsolute)};
            var cancellationToken = new CancellationToken();
            cancellationToken.Register(() => Console.WriteLine("Task is cancelled and I am callback"));
            var response = await client.GetAsync(new Uri("sqlrest/CUSTOMER/",UriKind.RelativeOrAbsolute), cancellationToken);
            Console.WriteLine(response.IsSuccessStatusCode.ToString());
            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content.ReadAsStringAsync());

        }
        public Task<Result> MyAsyncMethod1()
        {
            Console.WriteLine("Inside method 1");
            Task.Delay(TimeSpan.FromSeconds(5));
            //throw new Exception("Test");

            return Task.FromResult(new Result("Returning method  1"));
        }
        public Task<Result> MyAsyncMethod2()
        {
            Console.WriteLine("Inside method 2");
            Task.Delay(TimeSpan.FromSeconds(5));

            return Task.FromResult(new Result("Returning method  2"));
        }
    }

    public class Result
    {
        public Result(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
