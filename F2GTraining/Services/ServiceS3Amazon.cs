using Amazon.S3;
using Amazon.S3.Model;
using F2GTrainingAmazon.Helpers;

namespace F2GTraining.Services
{
    public class ServiceS3Amazon
    {
        private IAmazonS3 ClientS3;

        public ServiceS3Amazon(IAmazonS3 clientS3)
        {
            this.ClientS3 = clientS3;
        }

        public async Task<bool>UploadFileAsync(string fileName, Stream stream)
        {
            string bucketName = await HelperSecretManager.GetSecretAsync("BucketName");

            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = bucketName
            };

            PutObjectResponse response = await this.ClientS3.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //METODO PARA RECUPERAR UN FILE POR CODIGO
        public async Task<Stream> GetFileAsync(string fileName)
        {
            string bucketName = await HelperSecretManager.GetSecretAsync("BucketName");

            GetObjectResponse response = await this.ClientS3.GetObjectAsync(bucketName, fileName);
            return response.ResponseStream;
        }
    }
}
