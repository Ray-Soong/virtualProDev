using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace RedisReader
{
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
            string redisKey = "rgv_realtime_info";

            // Connect to Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            IDatabase db = redis.GetDatabase();

            // Subscribe to the Redis channel for notifications
            ISubscriber subscriber = redis.GetSubscriber();
            subscriber.Subscribe("rgv_realtime_info_channel", (channel, value) =>
            {
                // Read content from Redis while data is available
                while (true)
                {
                    // Read content from Redis when a notification is received
                    string redisContent = db.ListLeftPop(redisKey);

                    // If no more data, break the loop
                    if (redisContent == null)
                        break;

                    // Deserialize JSON to a list of objects
                    //List<RedisRgvDataModel> rgvDataList = JsonConvert.DeserializeObject<List<RedisRgvDataModel>>(redisContent);

                    // Process the received data as needed
                    Console.WriteLine($"Received data from Redis: {redisContent}");
                }
            });

            Console.WriteLine("Redis Reader waiting for notifications. Press Enter to exit.");
            Console.ReadLine();

            // Close the Redis connection
            redis.Close();
        }
    }
}
