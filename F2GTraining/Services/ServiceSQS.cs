using Amazon.SQS.Model;
using Amazon.SQS;
using Newtonsoft.Json;
using System.Net;
using F2GTraining.Models;
using F2GTrainingAmazon.Helpers;

namespace F2GTraining.Services
{
    public class ServiceSQS
    {
        private IAmazonSQS clientSQS;

        public ServiceSQS(IAmazonSQS clientSQS)
        {
            this.clientSQS = clientSQS;
        }

        public async Task SendMessageAsync(Nota nota)
        {
            string urlQueue = await HelperSecretManager.GetSecretAsync("BucketName");

            string json = JsonConvert.SerializeObject(nota);
            SendMessageRequest request =
                new SendMessageRequest(urlQueue, json);

            Guid guid = Guid.NewGuid();

            request.MessageGroupId = "developers"+ guid.ToString();
            request.MessageDeduplicationId = "developers" + guid.ToString();

            SendMessageResponse response =
                await this.clientSQS.SendMessageAsync(request);
            HttpStatusCode statusCode = response.HttpStatusCode;
        }


        public async Task<List<Nota>> ReceiveMessagesAsync()
        {
            string urlQueue = await HelperSecretManager.GetSecretAsync("BucketName");

            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                QueueUrl = urlQueue,
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
