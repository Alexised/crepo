using CargaAmbulatoria.EntityFramework.DataBase;
using CargaAmbulatoria.EntityFramework.Models;
using CargaAmbulatoria.Services.Helpers;
using CargaAmbulatoria.Services.Helpers.Log;
using CargaAmbulatoria.Services.Request;
using CargaAmbulatoria.Services.Responses;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaAmbulatoria.Services.Services
{
    public class DocumentService
    {
        private readonly IConfiguration _configuration;
        private readonly CargaAmbulatoriaDbContext dbContext = new CargaAmbulatoriaDbContext();

        public DocumentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Document>> GetDocumentsByUser(string userId) => dbContext.Documents.Include(x => x.User)
                                                                                        .ToList<Document>()
                                                                                        .Where(x => x.User.UserId == userId)
                                                                                        .OrderByDescending(x => x.DocumentDate);

        public async Task<IEnumerable<Cohort>> GetCohorts() => dbContext.Cohorts.ToList<Cohort>();

        public async Task<BaseResponse> DeleteDocument(string id)
        {
            try
            {
                var document = dbContext.Documents.FirstOrDefault(x => x.DocumentId == long.Parse(id));
                if (document == null)
                    return new BaseResponse { Success = false, Error = "El documento no existe" };

                string pathFull = System.IO.Path.Combine(document.Path, document.Name);

                if (File.Exists(pathFull))
                {
                    File.Delete(pathFull);
                }

                dbContext.Documents.Remove(document);

                await dbContext.SaveChangesAsync();
                return new BaseResponse { Success = true };

            }
            catch (Exception ex)
            {
                Log.LogEvent("User", "Delete Document", ex);
                return new BaseResponse { Success = false, Error = StringExtensions.InternalServerErrorMessage };
            }

            return new BaseResponse { Success = false, Error = StringExtensions.RequiredFieldsEmtpyMessage };

        }

        public async Task<BaseResponse> CreateDocument(DocumentRequest request)
        {
            try
            {
                if (request.DniType.IsFilled() && request.Dni.IsFilled() && request.DocumentFile.IsFilled() && request.UserId.IsFilled())
                {
                    var cohort = dbContext.Cohorts.FirstOrDefault(x => x.CohortId == request.CohortId);
                    if(cohort == null)
                    {
                        return new BaseResponse { Success = false, Error = "Cohorte no existente" };
                    }
                    string folderName = $"carga00001{cohort.Name}_{request.Regime}\\{request.DniType}{request.Dni}";
                    string nameFile = $"{request.DniType}{request.Dni} {DateTime.Now: yyyyMMdd}0";
                    int documentNumber = 1;
                    string[] formats = { "PDF", "PNG", "JPG", "JPEG" };

                    string path = System.IO.Path.Combine(_configuration["Application:Path"], folderName);
                    var file = request.DocumentFile.Split(',');
                    var ext = file[0].Split('/')[1].Split(';')[0];


                    if (formats.Contains(ext.ToUpperInvariant()))
                    {

                        byte[] documentFile = Convert.FromBase64String(file[1]);

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string pathFullPdf = System.IO.Path.Combine(path, $"{nameFile}{documentNumber}.pdf");

                        while (File.Exists(pathFullPdf))
                        {
                            documentNumber++;
                            pathFullPdf = System.IO.Path.Combine(path, $"{nameFile}{documentNumber}.pdf");
                        }
                        string pathFull = System.IO.Path.Combine(path, $"{nameFile}{documentNumber}.{ext}");

                        File.WriteAllBytes(pathFull, documentFile);


                        if (!"PDF".Equals(ext.ToUpperInvariant()))
                        {
                            using (MagickImageCollection pdf = new MagickImageCollection())
                            {
                                pdf.Add(pathFull);

                                pdf.Write(pathFullPdf);
                            }

                            File.Delete(pathFull);

                        }

                        dbContext.Documents.Add(
                            new Document
                            {
                                Name = $"{nameFile}{documentNumber}.pdf",
                                Path = path,
                                UserId = request.UserId,
                                DocumentDate = DateTime.Now,
                                CohortId = request.CohortId,
                                Dni = request.Dni,
                                Regime = request.Regime,
                            });

                        int id = await dbContext.SaveChangesAsync();

                        return new BaseResponse { Success = true, Data = "{id: " + id + "}" };
                    }

                    return new BaseResponse { Success = false, Error = "Formato del archivo no permitido" };
                }
            }
            catch (Exception ex)
            {
                Log.LogEvent("User", "Create Document", ex);
                return new BaseResponse { Success = false, Error = StringExtensions.InternalServerErrorMessage };

            }

            return new BaseResponse { Success = false, Error = StringExtensions.RequiredFieldsEmtpyMessage };

        }
    }
}
