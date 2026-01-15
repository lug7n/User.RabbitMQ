using User.Producer.Messaging;

var publisher = new RabbitMqPublisher();

var message = new UserCreatedMessage
{
    Id = 1,
    Name = "John Doe",
    Email = "jonhdoe@gmail.com",
    CreatedAt = DateTime.UtcNow
};

await publisher.PublishAsync(message);

Console.WriteLine("Mensagem publicada com sucesso. Aperte ENTER para fechar o terminal");
Console.ReadLine();
