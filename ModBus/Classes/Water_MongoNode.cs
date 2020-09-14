using MongoDB.Bson;
using System;

namespace ModBus.Classes
{
    internal class Water_MongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public string name { get; set; }
        public float value { get; set; }
        public DateTime dateTime { get; set; }
    }
}