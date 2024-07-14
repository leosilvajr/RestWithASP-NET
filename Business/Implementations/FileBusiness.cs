using RestWithASPNET.Data.VO;

namespace RestWithASPNET.Business.Implementations
{
    public class FileBusiness : IFileBusiness
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _context;

        public FileBusiness(IHttpContextAccessor context)
        {
            _context = context;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadDir");


        }

        public byte[] GetFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileDetailVO>> SaveFilesToDisk(IList<IFormFile> file)
        {
            throw new NotImplementedException();
        }

        //Salvar arquivo em disco.
        public async Task<FileDetailVO> SaveFileToDisk(IFormFile file)
        {
            FileDetailVO fileDatail = new FileDetailVO();

            var fileType = Path.GetExtension(file.FileName); //Descobrindo a extensão do arquivo
            var baseUrl = _context.HttpContext.Request.Host; //Monta a base URL Usando o Host da API

            if (fileType.ToLower() == ".pdf" || fileType.ToLower() == ".jpg" ||
                fileType.ToLower() == ".png" || fileType.ToLower() == ".jpeg")
            {
                var docName = Path.GetFileName(file.FileName);
                if (file != null && file.Length > 0)
                {
                    //Setar configurações antes de salvar no disco.
                    var destination = Path.Combine(_basePath, docName);
                    fileDatail.DocumentName = docName;
                    fileDatail.DocType = fileType;
                    fileDatail.DocUrl = Path.Combine(baseUrl + "/api/file/v1" + fileDatail.DocumentName); // Link para download

                    //Gravação no disco
                    using var stream = new FileStream(destination, FileMode.Create); //Abriu File Stream do Disco em modo de Gravação
                    await file.CopyToAsync(stream);
                    
                }
            }
            return fileDatail;
        }
    }
}
