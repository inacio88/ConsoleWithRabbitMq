using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit sender app";

IConnection connection = factory.CreateConnection();

IModel channnel = connection.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channnel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channnel.QueueDeclare(queueName, false, false, false, null);
channnel.QueueBind(queueName, exchangeName, routingKey, null);

byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello Mundo");
channnel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

channnel.Close();

connection.Close();