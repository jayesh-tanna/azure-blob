using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AzureBlob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("SaveProfilePic")]
        public async Task<IActionResult> SaveProfilePicAsync(IFormFile file)
        {
            var storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];

            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("profilepic");

                await container.CreateIfNotExistsAsync();

                //MS: Don't rely on or trust the FileName property without validation. The FileName property should only be used for display purposes.
                var picBlob = container.GetBlockBlobReference(file.FileName);

                await picBlob.UploadFromStreamAsync(file.OpenReadStream());

                return Ok(picBlob.Uri);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}