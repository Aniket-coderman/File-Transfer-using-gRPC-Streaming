using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using server;
using System.IO;
using Microsoft.Extensions.Logging;
using server.Services;
namespace server.Services{
    
    public class UsersService : Users.UsersBase
    {
        const int chunkSize = 1024*300;
        private readonly ILogger<UsersService> _logger;
        public UsersService(ILogger<UsersService> logger)
        {
            _logger = logger;
        }

        byte[] ConvertFileToBytes(string path){
            //int byteSize = 1024*20;
            //FileStream file = File.Open(path,FileMode.Open,FileAccess.Read);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            //file.Read(bytes,0,System.Convert.ToInt32(file.Length));
            //file.Close();
            return bytes;
        }
        // List<UserResponse> GetFileFromDb(){
        //     List<UserResponse> users = new List<UserResponse>();
        //     string path = @"C:\Users\t-anigupta\Downloads\WhatsApp Chat with The Pentagon.txt";
        //     byte[] buffer = ConvertFileToBytes(path);
        //     users.Add(new UserResponse(){
        //         File = Google.Protobuf.ByteString.CopyFrom(buffer)
        //     }); 
        //     return users;
        // }
        public override async Task GetFiles(UserRequest request, IServerStreamWriter<UserResponse> responseStream,ServerCallContext context)
        {
           // var files = GetFileFromDb();
            string path = @"C:\Users\t-anigupta\Downloads\Sample file.txt";
            string fileName = "Sample file";
            List<UserResponse> users = new List<UserResponse>();
            int totalChunks = 0;
            using(var fileStream = File.OpenRead(path)) {
                long filesize = fileStream.Length;
                
                byte[] buffer = new byte[chunkSize];
                int bytesRead;
                while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
                    // users.Add(new UserResponse(){
                    //     File = Google.Protobuf.ByteString.CopyFrom(buffer)
                    // });
                    var response = new UserResponse(){
                       File = Google.Protobuf.ByteString.CopyFrom(buffer)
                    };
                    totalChunks++;
                    Console.WriteLine($"Sending chunks number {totalChunks}");
                    await responseStream.WriteAsync(response);
                    long leftSize = filesize-(chunkSize*totalChunks);
                    if(leftSize < chunkSize && leftSize > 0){
                        int size = (int)leftSize;
                        Array.Resize(ref buffer, size);
                    }
                }
            }
            // FileStream file = File.Open(path,FileMode.Open,FileAccess.Read);
            // int totalChunks = (int)(file.Length/chunkSize);
            // if(file.Length % chunkSize != 0){
            //     totalChunks++;
            // }
            // List<UserResponse> users = new List<UserResponse>();
            // byte[] buffer;
            // for(int i = 0 ; i < totalChunks ; i++){
            //     buffer = new byte[chunkSize];
            //     file.Read(buffer,0,chunkSize);
            //     users.Add(new UserResponse(){
            //         File = Google.Protobuf.ByteString.CopyFrom(buffer)
            //     });
            // }
            // File.Close(file);
            // foreach(var file in files){
            //     await responseStream.WriteAsync(file);
            // }
            // int uploadedChunks = 0;
            // while(uploadedChunks != totalChunks){
            //     await responseStream.WriteAsync(users[uploadedChunks]);
            //     Console.WriteLine($"Uploading chunks {uploadedChunks+1d}/{totalChunks}");
            //     uploadedChunks+=1;
            // }
        }
    }

}
