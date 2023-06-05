using Amazon.SQS.Model;
using Amazon.SQS;
using Newtonsoft.Json;
using System.Net;
using F2GTraining.Models;



namespace F2GTraining.Services
{
    public class ServiceSQS
    {
        private IAmazonSQS clientSQS;
        private string UrlQueue;

        public ServiceSQS(IAmazonSQS clientSQS, IConfiguration configuration)
        {
            this.clientSQS = clientSQS;
            this.UrlQueue =
                configuration.GetValue<string>("ServicesAmazon:SQS:UrlQueue");
        }

        public async Task SendMessageAsync(Nota nota)
        {
            string json = JsonConvert.SerializeObject(nota);
            SendMessageRequest request =
                new SendMessageRequest(this.UrlQueue, json);
            request.MessageGroupId = "developers";
            request.MessageDeduplicationId = "developers" + nota.titulo;
            SendMessageResponse response =
                await this.clientSQS.SendMessageAsync(request);
            HttpStatusCode statusCode = response.HttpStatusCode;
        }


        public async Task<List<Nota>> ReceiveMessagesAsync()
        {
            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                QueueUrl = UrlQueue,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 1
            };
            ReceiveMessageResponse response =
                await this.clientSQS.ReceiveMessageAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response.Messages.Count != 0)
                {
                    List<Message> messages = response.Messages;
                    List<Nota> output = new List<Nota>();
                    foreach (Message msj in messages)
                    {
                        string json = msj.Body;
                        Nota data = JsonConvert.DeserializeObject<Nota>(json);
                        output.Add(data);
                    }
                    return output;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }
}
