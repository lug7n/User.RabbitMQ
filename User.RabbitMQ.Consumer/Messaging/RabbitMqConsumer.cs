using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using User.Producer.Messaging;

public class RabbitMqConsumer
{
    public async Task Start()
    {
        const string exchangeName = "user.exchange";
        const string queueName = "user.created.queue";
        const string routingKey = "user.created";

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "admin",
            Password = "admin",
            VirtualHost = "/",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey,
            arguments: null
        );

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = System.Text.Encoding.UTF8.GetString(body);
                var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedMessage>(json);

                if (userCreatedEvent is not null)
                {
                    Console.WriteLine("[Consumer] Conta de usuário registrada. ");
                    Console.WriteLine($"Id: {userCreatedEvent?.Id}");
                    Console.WriteLine($"Usuário: {userCreatedEvent?.Name}");
                    Console.WriteLine($"Email: {userCreatedEvent?.Email}");

                    await Task.Delay(2000);

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                }
                else
                {
                    Console.WriteLine("[Consumer] Dados nulos ou inválidos ");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[Consumer] Erro ao desserializar o pedido: {ex.Message}");
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Erro ao registrar usuário: {ex.Message}");
                channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer
        );

    }
}