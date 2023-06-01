using Amazon.S3;
using Amazon.S3.Model;

namespace F2GTraining.Services
{
    public class ServiceS3Amazon
    {
        private string BucketName;
        private IAmazonS3 ClientS3;

        public ServiceS3Amazon(IConfiguration configuration
            , IAmazonS3 clientS3)
        {
            this.BucketName = configuration.GetValue<string>("AWS:BucketName");
            this.ClientS3 = clientS3;
        }

        public async Task<bool>UploadFileAsync(string fileName, Stream stream)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = this.BucketName
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
            GetObjectResponse response = await this.ClientS3.GetObjectAsync(this.BucketName, fileName);
            return response.ResponseStream;
        }
    }
}
