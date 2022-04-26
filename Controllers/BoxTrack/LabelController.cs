using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Models.BoxTrack;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LabelController : ControllerBase
    {
        private readonly ProductsRepository _productsRepository;
        private readonly ShiftRepository _shiftRepository;
        private readonly LabelRepository _labelRepository;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly ProductionRepository productionRepository;
        private readonly ProcessRepository processRepository;
        private readonly ModuleRepository moduleRepository;
        private readonly CodesRepository _codesRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly OmsRepository omsRepository;
        private const int PRODUCT_END_CODE = 400;


        public LabelController(ProductsRepository productsRepository, ShiftRepository shiftRepository, LabelRepository labelRepository, IHostingEnvironment hostEnvironment, ProductionRepository productionRepository, ProcessRepository processRepository, ModuleRepository moduleRepository, CodesRepository codesRepository, IHttpContextAccessor httpContextAccessor, OmsRepository omsRepository)        
        {
           _productsRepository = productsRepository;
           _shiftRepository = shiftRepository;
           _labelRepository = labelRepository;
           _hostEnvironment = hostEnvironment;
            this.productionRepository = productionRepository;
            this.processRepository = processRepository;
            this.moduleRepository = moduleRepository;
            _codesRepository = codesRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.omsRepository = omsRepository;
        }
    
        [HttpPost]
        [Route("printlabels")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult> PrintLabel(PrintLabelRequest labelRequest)
        {
            try
            {    //to do: probar reimpresión en formato correcto, observar registros, publicar todo nuevamente y volver a probar
                var production = await productionRepository.GetProductionById(labelRequest.IdProduccion);
                var availableCodes = new List<Code>(); 
                if(!labelRequest.EsReimpresion) //Validar petición en caso que no sea reimpresión
                {
                    int validationResult = 0;
                    if(labelRequest.DataMatrixOrderId != 0)
                    {
                        availableCodes = await _codesRepository.getAvailableOrderCodes(labelRequest.DataMatrixOrderId);
                        validationResult = await ValidatePrintRequest(labelRequest,production,availableCodes.Count);
                        availableCodes = availableCodes.Take(labelRequest.CantidadEtiquetas).ToList();
                    }
                    else
                    {  
                        validationResult = await ValidatePrintRequest(labelRequest,production);
                    }
                    if(validationResult == PRODUCT_END_CODE)
                    {
                        ModelState.AddModelError("Finalizado", "Producto finalizado, seleccione el módulo para cargar la nueva configuración");
                        return BadRequest(ModelState);
                    }
                    else if(validationResult != StatusCodes.Status200OK)
                    {
                        return StatusCode(validationResult,availableCodes.Count);
                    }
                }
                if (production != null)
                {
                    var userClaim = HttpContext.User.Claims.Where(c=>c.Type==Values.UserClaimKey).SingleOrDefault();
                    string userName = userClaim != null ? userClaim.Value : null; 
                    var labelRecords = await GenerateLabelsRecords(production,labelRequest,availableCodes, userName);
                    var labelObject = labelRecords.Item1;
                    var labelsToPrint = labelRecords.Item2;
                    if(labelRequest.DataMatrixOrderId != 0)
                    {
                        await _labelRepository.AddWithDataMatrixOrder(labelObject,availableCodes,userName);
                    }
                    else
                    {
                        await _labelRepository.Add(labelObject);
                    }
                    return GenerateLabels(labelsToPrint, labelRequest);
                }
                else
                {
                    ModelState.AddModelError("", "El turno no ha iniciado");
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            } 
        }

        [HttpGet]
        [Route("reprintlabelrequest")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult> RePrintLabelRequest(int? idTurno, int? idModulo, int? idProceso, DateTime fechaProduccion, string idProducto)
        {
            try
            {
                var sameLabelRecords = await productionRepository.GetProductionsByProduct(idProceso,idModulo,idTurno, fechaProduccion, idProducto);
                return Ok(sameLabelRecords);
            }
            catch(System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        [Route("getproducts")]
        [Authorize(Permissions.Menus.Etiquetado)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productsRepository.GetAll();
                return products;
            }
            catch(SqlException ex)
            {
                Log.Error(ex,ex.Message);
                var translateRequestObject = new {sourceLang = "en", targetLang="es", sourceText = ex.Message}; 
                var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                ModelState.AddModelError("","Error interno: "+translatedStringMessage+" Contacte a tecnología");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [Route("getlabels")]
        [Authorize(Permissions.Menus.Etiquetado)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Label>>> GetLabels(int idProceso, int idModulo, int idTurno, DateTime fechaProduccion)
        {
            try
            {
                var labels = await _labelRepository.GetLabels(idProceso, idModulo, idTurno, fechaProduccion);
                return labels;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        
        // [HttpPost]
        // [Route("reprintlabels")]
        // [Authorize(Permissions.Menus.Etiquetado)]
        // public ActionResult Reprint(List<Label> labels)
        // {
        //     var labelsToPrint = new List<LabelToPrint>();
            
        //     foreach (var label in labels)
        //     {
        //         var labelToPrint = new LabelToPrint()
        //         {
        //             Description = label.DescripcionProducto, Qr = label.CodigoQr,
        //             Ean = label.CodigoBarra, Address = label.LabelText.Direccion,
        //             Quantity = label.CantidadCigarros + " " + label.LabelText.TextoCantidad,
        //             Message = label.LabelText.Advertencia, Country = label.LabelText.TextoPais
        //         };
        //         labelsToPrint.Add(labelToPrint);
        //     }
        //     return GenerateLabels(labelsToPrint);
        // }
        
        // [HttpPost]
        // [Route("cancelLabels")]
        // [Authorize(Permissions.Menus.Etiquetado)]
        // public async Task<ActionResult> CancelLabels(CancelLabelsRequest req)
        // {
        //     await _labelRepository.CancelLabels(req.Labels, req.UsuarioAnulacion);
        //     return Ok();
        // }

        // [HttpPost]
        // [Route("cancellabelbyqr")]
        // [Authorize(Permissions.Menus.Etiquetado)]
        // public async Task<ActionResult> CancelLabelByQr(CancelLabelByQrRequest req)
        // {
        //     Label result = await _labelRepository.CancelLabelByQr(req.QrCode,req.Usuario);
        //     if(result != null)
        //     {
        //         return Ok();
        //     }
        //     else
        //     {
        //         ModelState.AddModelError("", "Código QR no encontrado");
        //         return BadRequest(ModelState);
        //     }
        // }

        [Route("getproduct/{codigo}")]
        [Authorize(Permissions.Menus.Etiquetado)]
        [HttpGet]
        public async Task<ActionResult<Product>> GetProduct(string codigo)
        {
            try
            {
                var product = await _productsRepository.Get(codigo);
                if (product == null)
                {
                    ModelState.AddModelError("", "Código de producto incorrecto");
                    return BadRequest(ModelState);
                }
                return product;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [Route("getprocesses")]
        [Authorize(Permissions.Menus.Etiquetado)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcesses()
        {
            try
            {
                 //var userRol = Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "Rol").Value);
                var processes = await processRepository.GetAll();
                return processes;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
           
        }

        [Route("getmodules")]
        [Authorize(Permissions.Menus.Etiquetado)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Module>>> GetModules()
        {
            try
            {
                var modules = await moduleRepository.GetAll();
                return modules;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        
        private async Task<Tuple<Label,List<LabelToPrint>>> GenerateLabelsRecords(Production production, PrintLabelRequest labelRequest, List<Code> availableCodes, string userName)
        {
            var currentDate = labelRequest.EsReimpresion ? labelRequest.FechaHoraReimpresion:DateTime.Now;
            var currentTime = currentDate.TimeOfDay;
            int dayOfYear = currentDate.DayOfYear;
            int lastYearDigit = currentDate.Year % 10;
            var currentShift = production.Shift;
            //var shiftHour = GetShiftHour(production, currentShift);
            //var labelText = await _labelRepository.getLabelText(labelRequest.Cliente);
            var labelConfig = await _labelRepository.getLabelText(labelRequest.EsUsa, labelRequest.TipoEtiqueta, labelRequest.lleva_Logo_TextoInferior);
            var countryText = labelConfig.TextoPais;
            string countryTextEs="", countryTextEn="";
            if(countryText != null)
            {
                countryTextEs = countryText.Substring(0, countryText.IndexOf('/')).Trim();
                countryTextEn = countryText.Substring(countryText.IndexOf('/') + 1).Trim();
            }
            
            //Console.WriteLine("es:" + countryTextEs);
            //Console.WriteLine("en:" + countryTextEn);
            //var country = labelText.IdPais;
            var labelsToPrint = new List<LabelToPrint>();
            var centro = labelRequest.Centro;
            var nextSecuence = await GetNextSecuence(production);
            var startSecuence = nextSecuence;
            string qrData=null;
            string dataMatrixCode = null;
            for(int i = 0; i < labelRequest.CantidadEtiquetas; i++)
            {
                if(labelRequest.TipoEtiqueta == "Box")
                {
                    qrData = "B" + labelRequest.IdAlmacenamiento + dayOfYear.ToString("D3")
                    + currentShift.LetraRepresentacion + lastYearDigit + labelRequest.NumeroModulo + " " + currentTime.ToString("hhmmss") + labelRequest.IdProducto.PadRight(18, ' ')
                    + nextSecuence.ToString("D4") + "   " + labelRequest.CantidadCigarros.ToString("D6") + centro.Substring(centro.Length - 2)
                    + String.Format(CultureInfo.InvariantCulture, "{0:0000.000}", labelRequest.PesoNeto) + labelRequest.UnidadPeso.PadRight(3, ' ');//country
                }
                if(labelRequest.DataMatrixOrderId != 0)
                {
                    dataMatrixCode = availableCodes.ElementAt(i).CodeValue;
                }
                var label = new LabelToPrint()
                {
                    Description = labelRequest.IdProducto+" "+labelRequest.DescripcionProducto, Qr = qrData,
                    Ean = labelRequest.CodigoEan, Address = labelConfig.Direccion,
                    Country = countryTextEs, CountryEn = countryTextEn, Module = labelRequest.TextoModulo,
                    DataMatrixCode = dataMatrixCode
                }; //Quantity = labelRequest.CantidadCigarros + " " + labelText.TextoCantidad, Message = labelText.Advertencia, Country = labelText.TextoPais
                labelsToPrint.Add(label);
                nextSecuence++;
            }
            var labelObject = new Label(){
                Almacenamiento = labelRequest.IdAlmacenamiento, CantidadCigarros = labelRequest.CantidadCigarros,
                CodigoBarra = labelRequest.CodigoEan, CodigoQr = qrData, DescripcionProducto = labelRequest.DescripcionProducto,
                FechaHoraCalendario = DateTime.Now, ProduccionId = production.Id, SecuenciaInicial = startSecuence, ConfiguracionEtiquetaId = labelConfig.Id,
                UsuarioGeneracion = userName, CantidadImpresa = labelRequest.CantidadEtiquetas, EsReimpresion = labelRequest.EsReimpresion
            };
            if(labelRequest.EsReimpresion)
            {  
                labelObject.IdEtiquetaReimpresa = labelRequest.IdEtiquetaReimpresa;
            }
            return Tuple.Create(labelObject,labelsToPrint);
        } 
        private int GetShiftHour(Production production, Shift currentShift)
        {
            var currentDate = DateTime.Now;
            var productionDate = production.FechaProduccion;
            var shiftStartDate = new DateTime(productionDate.Year, productionDate.Month, productionDate.Day,
                                             currentShift.HoraInicio.Hours, currentShift.HoraInicio.Minutes,
                                              currentShift.HoraInicio.Seconds);
            int shiftHour = currentDate.Subtract(shiftStartDate).Hours + 1;
            if (shiftHour == 0)
            {
                shiftHour = 1;
            }
            else if (shiftHour > 9)
            {
                shiftHour = 9;
            }
            return shiftHour;
        }

        private async Task<int> GetNextSecuence(Production production)
        {
            int secuence = 1;
            if(production.Labels.Count > 0) 
            {
                var secuenceValue = GetSecuence(production);
                secuence = secuenceValue != 0 ? secuenceValue: secuence;
            }
            else
            {
                var sameProduction = await productionRepository.GetLastProductionByProduct(production.IdProceso,production.IdModulo, production.IdTurno, production.FechaProduccion, production.IdProducto);
                if(sameProduction != null)
                {
                    if(sameProduction.Labels != null)
                    {
                        var secuenceValue = GetSecuence(sameProduction);
                        secuence = secuenceValue != 0 ? secuenceValue: secuence;
                    }
                }
            } 
            return secuence;
        }
        private int GetSecuence(Production production)
        {
            int secuence = 0;
            if(production.Labels.Count > 0)
            {
                var lastLabel = production.Labels.LastOrDefault();
                if(lastLabel != null)
                {
                    secuence = lastLabel.SecuenciaInicial + lastLabel.CantidadImpresa;
                }
            }
            return secuence;
        }
        private FileContentResult GenerateLabels(List<LabelToPrint> labelsToPrint, PrintLabelRequest labelRequest)
        {
            Report report = new Report();

            string labelTemplate = ""; 
            if(labelRequest.TipoEtiqueta == "Box")
            {
                if(labelRequest.lleva_Logo_TextoInferior)
                {
                    labelTemplate = labelRequest.DataMatrixOrderId == 0 ? "BoxTrackLabel.frx" : "BoxTrackLabel_DataMatrix.frx";
                }
                else
                {
                    labelTemplate = labelRequest.DataMatrixOrderId == 0 ? "BoxTrackLabelSinDireccion.frx"  : "BoxTrackLabelSinDireccion_DataMatrix.frx";
                }
            }
            else
            {
                if(labelRequest.lleva_Logo_TextoInferior)
                {
                    labelTemplate = "CigarrosConLogo.frx";
                }
                else
                {
                    labelTemplate = "CigarrosSinLogo.frx";
                }
            }
            var reportPath = Path.Combine(_hostEnvironment.WebRootPath, @"Reports/"+labelTemplate);
            report.Load(reportPath);
            report.RegisterData(labelsToPrint, "LabelsToPrint");
            report.Prepare(); 
            var memStream = new MemoryStream();
            PDFSimpleExport pdfExport = new PDFSimpleExport(); 
            pdfExport.Export(report, memStream);
            return new FileContentResult(memStream.ToArray(), "application/pdf");
        }
     
        private async Task<int> ValidatePrintRequest(PrintLabelRequest labelRequest, Production production, int availableCodesQuantity=0)
        {
            var scheduleProduct = await productionRepository.GetScheduleProduct(production);
            if(labelRequest.DataMatrixOrderId == 0)
            {
                if (scheduleProduct == null)
                {
                    return 400;
                }
                if(scheduleProduct.IdProducto != labelRequest.IdProducto)
                {
                    //se hizo un cambio de producto desde otra estación
                    return 409;
                }
            }
            if(production.IdModulo != labelRequest.IdModulo)
            {
                //se hizo un cambio de módulo desde otra pestaña en el mismo cliente
                return 452;
            }
            if(labelRequest.DataMatrixOrderId != 0)
            {
                 if(labelRequest.CantidadEtiquetas > availableCodesQuantity)
                 {
                     //códigos insuficientes
                     return 453;
                 }
            }
            return 200; 
        }
        private ActionResult ManageException(Exception ex)
        {
            Log.Error(ex,ex.Message);
            ModelState.AddModelError("", "Error interno. Favor de intentarlo nuevamente.");
            return BadRequest(ModelState);
        }
    }
            // if (scheduleProduct == null)
            // {
            //     ModelState.AddModelError("Finalizado", "Producto finalizado, seleccione el módulo para cargar la nueva configuración");
            //     return BadRequest(ModelState);
            // }
            // if(scheduleProduct.IdProducto != labelRequest.IdProducto)
            // {
            //     //se hizo un cambio de producto desde otra estación
            //     return StatusCode(409);
            // }
            // if(production.IdModulo != labelRequest.IdModulo)
            // {
            //     //se hizo un cambio de módulo desde otra pestaña en el mismo cliente
            //     return StatusCode(452);
            // }
    // public class CancelLabelByQrRequest
    // {
    //    public string QrCode { get; set; }
    //    public string Usuario { get; set; }
    // }
}