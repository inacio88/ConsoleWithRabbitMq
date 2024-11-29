using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit receiver2 app";

IConnection connection = factory.CreateConnection();

IModel channnel = connection.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channnel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channnel.QueueDeclare(queueName, false, false, false, null);
channnel.QueueBind(queueName, exchangeName, routingKey, null);

channnel.BasicQos(prefetchSize: 0, prefetchCount: 1,global: false);
var consumer = new EventingBasicConsumer(channnel);
consumer.Received += (sender, args) =>{
    //Task.Delay(TimeSpan.FromSeconds(2)).Wait();
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
    channnel.BasicAck(args.DeliveryTag, multiple: false);
};

string consumerTag = channnel.BasicConsume(queueName, autoAck: false, consumer);

Console.ReadLine();

channnel.BasicCancel(consumerTag);

channnel.Close();
connection.Close();