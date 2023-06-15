using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public enum JellyfishBehaviour
    {
        Aggressive,
        Neutral,
        Fleeing,
    }

    public class Jellyfish
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        [EnumDataType(typeof(JellyfishBehaviour))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JellyfishBehaviour Behaviour { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string SellingUserId { get; set; }

        public DateTime AddDate { get; set; }

        public double Price { get; set; }

        public string ShopNote { get; set; }
    }

    public class JellyfishDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [EnumDataType(typeof(JellyfishBehaviour))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JellyfishBehaviour Behaviour { get; set; }

        public DateTime AddDate { get; set; }

        public double Price { get; set; }
    }

    public class AddJellyfishDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EnumDataType(typeof(JellyfishBehaviour))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JellyfishBehaviour Behaviour { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
