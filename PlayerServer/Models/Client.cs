using System;

namespace PlayerServer.Models;

public class Client(Connection connection, string? id = null, string? name = null, string? desc = null)
    : Connection(connection.Protocol, connection.EndPoint, connection.ProtocolClient)
{
    public string? Id { get; set; } = id ?? Guid.NewGuid().ToString();
    public string? Name { get; set; } = name ?? GenerateRandomString();
    public string? Desc { get; set; } = desc;
    public DateTime ConnectionTime { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"Id={Id}," +
               $" Name={Name}," +
               $" Desc={Desc}," +
               $" ConnectionTime={ConnectionTime}," +
               $" {base.ToString()}";
    }

    private static string GenerateRandomString()
    {
        // 从喜欢的水果里挑一个作为随机名称
        string[] fruits =
        [
            "Pineapple", "Strawberry", "Grapes", "Mango", "Watermelon", "Orange",
            "Kiwi", "Honeydew", "Cherry", "Blueberry", "Lychee",
            "Longan", "Persimmon", "Peach", "Mulberry"
        ];
        var random = new Random();
        return fruits[random.Next(fruits.Length)];
    }
}