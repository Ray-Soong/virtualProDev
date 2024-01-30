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
            string redisChannel = "rgv_realtime_info_channel";

            // Connect to Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            IDatabase db = redis.GetDatabase();
            ISubscriber subscriber = redis.GetSubscriber();

            // Run the writer loop
            while (true)
            {
                // Create a new list to store RGV data
                List<RedisRgvDataModel> rgvDataList = new List<RedisRgvDataModel>();

                // Sample JSON array
                string jsonRgvData = @"[
                    {'BankID':'3','id':'1','FaultCode':'0','floor':'1','enable':'2','HasBox':'1','Position':'3','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
                    {'BankID':'3','id':'2','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
                    {'BankID':'3','id':'3','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
                    {'BankID':'3','id':'4','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'},
                    {'BankID':'3','id':'5','FaultCode':'0','floor':'0','enable':'0','HasBox':'0','Position':'0','SourcePosition':'0','TargetPosition':'0','TaskID':'','Speed':null,'TimeSpan':'1705215686'}
                ]";

                // Deserialize JSON to a list of objects
                rgvDataList = JsonConvert.DeserializeObject<List<RedisRgvDataModel>>(jsonRgvData);

                // Set the TimeSpan property to the current time with the specified format
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd:HHmmss.fff");
                foreach (var rgvData in rgvDataList)
                {
                    rgvData.TimeSpan = currentTime;
                }

                // Convert the list of objects to RedisValue array
                RedisValue[] redisValues = rgvDataList.ConvertAll(x => (RedisValue)JsonConvert.SerializeObject(x)).ToArray();

                // Add or update the Redis list
                db.ListRightPush(redisRgvKey, redisValues);

                // Publish a notification on the channel
                subscriber.Publish(redisChannel, "NewDataAvailable");

                Console.WriteLine($"Data added to Redis list at {currentTime}");

                // Wait for one second
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
