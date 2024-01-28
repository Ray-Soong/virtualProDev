using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Collections.Generic;

// Define a class to represent the structure of your JSON objects
class RgvInfo
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

namespace RedisReader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace these values with your Redis server connection information
            string redisConnectionString = "localhost:6379";
            string redisKey = "exampleKey";

            // Connect to Redis
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            IDatabase db = redis.GetDatabase();

            // Read content from Redis
            string redisContent = db.StringGet(redisKey);

            // Display the retrieved content
            Console.WriteLine($"Content retrieved from Redis: {redisContent}");

            // Close the Redis connection
            redis.Close();

            Console.ReadLine();
        }
    }
}
