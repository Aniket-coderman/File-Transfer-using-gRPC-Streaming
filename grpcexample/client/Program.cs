// See https://aka.ms/new-console-template for more information
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Grpc.Net.Client;
using server;
using System.Runtime.InteropServices;
using Google.Protobuf;
namespace client{
    class Program{
        
        static async Task Main(string[] args){
            Console.WriteLine("Calling a GRPC Service!");
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);
            var channel = GrpcChannel.ForAddress("https://localhost:7193",
            new GrpcChannelOptions { HttpClient = httpClient });
            var client =  new Users.UsersClient(channel);
            string fileName = @"C:\Users\t-anigupta\Downloads\Sample Response.txt";    
            FileInfo fi = new FileInfo(fileName);    
    
            try {    
            // Check if file already exists. If yes, delete it.     
                if (fi.Exists){    
                    fi.Delete();    
                }
            }  
            catch(Exception e){
                Console.WriteLine(e.ToString());
            }
    
            try
            {
                UserRequest request = new UserRequest();
                byte[] buffer; 
                using (var call = client.GetFiles(request))
                {
                    using(var stream = File.Create(fileName))
                    {
                        while (await call.ResponseStream.MoveNext(CancellationToken.None))
                        {
                            var currentFileChunk = call.ResponseStream.Current;
                            buffer = currentFileChunk.ToByteArray();
                            stream.Write(buffer, 0, buffer.Length);
                            Console.WriteLine("Receiving File");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}

