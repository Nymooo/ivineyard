using Domain.Repositories.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
namespace Domain.Repositories.Implementations;

public class RabbitMQRepository : IRabbitMQRepository {
    private readonly string _hostname = "api.ivineyard.eu"; // Dein RabbitMQ-Server
    private readonly string _queueName = "weather_data"; // Der Name deiner Queue

    public async Task<List<string>> GetMessagesAsync() {
        var messges = new List<string>();
        var factory = new ConnectionFactory() {HostName = _hostname, Port = 5672};
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        
        channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            messges.Add(message);
            await Task.CompletedTask;
        };
        
        channel.BasicConsumeAsync(queue:_queueName,autoAck: true, consumer: consumer);

        await Task.Delay(2000);//einfahc nur zum sicherstellen ned das zu schnell ist
        
        return messges;
    }
}