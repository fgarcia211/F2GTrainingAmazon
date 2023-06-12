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

        public List<Nota> NotasXIdUsuario(List<Nota> notas, int idusuario)
        {
            List<Nota> notaDevolver = new List<Nota>();
            int id = 1;

            foreach (Nota nota in notas)
            {
                if (nota.IdUsuario == idusuario)
                {
                    nota.Id = id;
                    id++;
                    notaDevolver.Add(nota);
                }
            }

            return notaDevolver;
        }

        public async Task SendMessageAsync(Nota nota)
        {
            string urlQueue = await HelperSecretManager.GetSecretAsync("EnlaceFIFO");

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
            string urlQueue = await HelperSecretManager.GetSecretAsync("EnlaceFIFO");

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
