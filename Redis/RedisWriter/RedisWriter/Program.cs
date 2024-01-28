using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RedisWriter
{
    // Define a class to represent the structure of your JSON objects
    public class RedisRgvDataModel
    {
        public string BankID { get; set; }
        public string id { get; set; }
        public string FaultCode { get; set; }
        public string floor { get; set; }
        public string enable { get; set; }
        public string HasBox { get; set; }
        public string Position { get; set; }
        public string SourcePosition { get; set; }
        public string TargetPosition { get; set; }
        public string TaskID { get; set; }
        public string Speed { get; set; }
        public string TimeSpan { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Replace these values with your Redis server connection information
            string redisConnectionString = "localhost:6379";
            string redisRgvKey = "rgv_realtime_info";
            string redisHoistKey = "hoist_realtime_info";

            // Sample JSON array
            string jsonRgvData = @"[
            {'BankID':'3','id':'1','FaultCode':'0','floor':'1','enable':'2','HasBox':'1','Position':'3','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
            {'BankID':'3','id':'2','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
            {'BankID':'3','id':'3','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
            {'BankID':'3','id':'4','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
            {'BankID':'3','id':'5','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'}
            ]";
            // Connect to Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            IDatabase db = redis.GetDatabase();

            // Deserialize JSON to a list of objects
            List<RedisRgvDataModel> rgvdataList = JsonConvert.DeserializeObject<List<RedisRgvDataModel>>(jsonRgvData);

            // Convert the list of objects to RedisValue array
            RedisValue[] redisValues = rgvdataList.ConvertAll(x => (RedisValue)JsonConvert.SerializeObject(x)).ToArray();

            // Add or update the Redis list
            db.ListRightPush(redisRgvKey, redisValues);

            // Close the Redis connection
            redis.Close();

            Console.WriteLine("Data added to Redis list successfully.");
            Console.ReadLine(); // To keep the console window open for viewing results
        }
    }
}
