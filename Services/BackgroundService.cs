using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

internal class BackgroundTaskService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;

    public BackgroundTaskService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    // public Task StartAsync(CancellationToken stoppingToken)
    // { 
    //     _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    //     return Task.CompletedTask;
    // }
    private async void DoWork() //object state
    {
        Log.Error("Background Service is working. "+System.DateTime.Now);
        //Todo: no mostrar errores en ruso, mantenimiento para order defaults, actualizar manuales
        //verificar porque se detiene el service worker, verificar porque los codigos no estan descargando, analizar como manejar error de concurrencia
        using (var scope = serviceProvider.CreateScope()) 
        {
            try
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var omsRepository = serviceProvider.GetRequiredService<OmsRepository>();  
                var orderRepository = serviceProvider.GetRequiredService<OrderRepository>();
                var codesRepository = serviceProvider.GetRequiredService<CodesRepository>(); 
                var orderSettingsRepository = serviceProvider.GetRequiredService<OrderSettingsRepository>(); 
                var emailSender = serviceProvider.GetRequiredService<EmailSender>(); 
                var emailRepository =  serviceProvider.GetRequiredService<EmailAccountRepository>();
                var hostingEnvironment = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
                var orderDefaults = await orderSettingsRepository.Get(1);
                var pendingOrders = await FetchPendingOrders(orderRepository);   

                // var logoPathh =  Path.Combine(hostingEnvironment.WebRootPath, @"Images/logo117.jpg");
                // var emailBodyy = $"<b>Detalles de la orden:</b><br><br>"+
                //                 $"<b>Id:</b> <br>"+
                //                 $"<b>Cantidad de códigos:</b> <br>"+
                //                 $"<b>Producto:</b> <br>"+
                //                 $"<b>Fecha de creación:</b><br><br>"+
                //                 $"path:{logoPathh}"+
                //                 $"<a href='https://www.laaurora.com.do'><img src='{logoPathh}' style='width:192px;height:96px'></img></a>";
                               
                // var recipientes = await emailRepository.GetAll();
                // recipientes = recipientes.Where(a => !a.Email.Contains("neska")).ToList();
                // await emailSender.SendBulkEmailAsync(recipientes,"Orden de códigos Datamatrix disponible",emailBodyy);
                // return;  

                if(pendingOrders.Count > 0)
                {
                    var orderObject = new Order();
                    orderObject.OmsId = orderDefaults.OmsId;
                    orderObject.OmsUrl = orderDefaults.OmsUrl;
                    orderObject.Token = orderDefaults.Token;
                    var availabilityResponse = await omsRepository.CheckAvailability(orderObject);
                    var response = availabilityResponse.Item1;
                    var responseContent = availabilityResponse.Item2;
                    if(response.IsSuccessStatusCode)
                    {
                        var availabilityResponseInfo = JsonConvert.DeserializeObject<OrderAvailabilityResponse>(responseContent).orderInfos;
                        if(availabilityResponseInfo.Count > 0)
                        {
                            availabilityResponseInfo = availabilityResponseInfo.Where(oi=>pendingOrders.Any(o=>o.OrderId == oi.orderId)).ToList();
                            foreach (var orderInfo in availabilityResponseInfo)
                            {
                                if(orderInfo.orderStatus == "READY")  //&& orderInfo.buffers[0].bufferStatus == "ACTIVE"
                                {
                                    var order = pendingOrders.Where(o=>o.OrderId == orderInfo.orderId).SingleOrDefault();
                                    order.OmsId = orderDefaults.OmsId;
                                    order.OmsUrl = orderDefaults.OmsUrl;
                                    order.Token = orderDefaults.Token;
                                    try
                                    {
                                         await orderRepository.UpdateOrderStatus(order.Id,"Disponible");
                                    }
                                    catch (DbUpdateConcurrencyException ex)
                                    {
                                       Log.Error(ex,ex.Message);
                                       continue;
                                    }
                                    var isDownloadSucessfull = await DownloadOrderCodes(order,codesRepository,omsRepository);
                                    if(isDownloadSucessfull)
                                    {
                                        //Enviar correo
                                        var logoPath =  Path.Combine(hostingEnvironment.WebRootPath, @"Images/logo117.jpg");
                                        var emailBody = $"<b>Detalles de la orden:</b><br><br>"+
                                                        $"<b>Id:</b> {order.OrderId}<br>"+
                                                        $"<b>Cantidad de códigos:</b> {order.Quantity}<br>"+
                                                        $"<b>Producto:</b> {order.ProductDescription + "("+order.ProductCode+")"}<br>"+
                                                        $"<b>Fecha de creación:</b> {order.CreationDate}<br><br>";
                                                        // $"<a href='https://www.laaurora.com.do'><img src='{logoPath}' style='width:192px;height:96px'></img></a>";
                                        var recipients = await emailRepository.GetAll();
                                        recipients = recipients.Where(a => !a.Email.Contains("neska")).ToList();
                                        await emailSender.SendBulkEmailAsync(recipients,"Orden de códigos Datamatrix disponible",emailBody);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
                        var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                        Log.Error("Error verificando disponibilidad en background service, detalles: {0}",translatedStringMessage);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex,ex.Message);
            }
        }   
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
             while(!stoppingToken.IsCancellationRequested)
             {
                  DoWork();
                  await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
             }
            // _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            // DoWork();
            // await Task.Delay(TimeSpan.FromMinutes(0), stoppingToken);
    }
    private async Task<List<Order>> FetchPendingOrders(OrderRepository orderRepository)
    {
        try
        {
            return await orderRepository.FetchPendingOrders();
        }
        catch (System.Exception ex)
        {
            Log.Error(ex,ex.Message);
            throw ex;
        } 
    }
    private async Task<bool> DownloadOrderCodes(Order order, CodesRepository codesRepository, OmsRepository omsRepository)
    {
        var codeResponse = await omsRepository.GetCodes(order);
        var response = codeResponse.Item1;
        var responseContent = codeResponse.Item2;
        if(response.IsSuccessStatusCode)
        {
            var codeResponseObject = JsonConvert.DeserializeObject<CodeResponse>(responseContent);
            var codes = codeResponseObject.codes;
            if(codes.Length > 0)
            {
                var codeList = new List<Code>();
                foreach (var code in codes)
                {
                    codeList.Add(
                        new Code{ CodeValue = code, OrderId = order.Id, CisType = order.CisType}
                    );
                }
                await codesRepository.ProcessOrder(codeList);
                return true;
            }
            else
            {
                var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
                var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                Log.Error("Error descargando los códigos datamatrix de la orden: {0}, detalles: {1}", order.OrderId,translatedStringMessage);
                return false;
            }
        }
        else
        {
            if(responseContent.Contains("EXHAUSTED"))
            {
                return await GetCodesRetry(order,codesRepository,omsRepository);
            }
            var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
            var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
            Log.Error("Error descargando los códigos datamatrix de la orden: {0}, detalles: {1}", order.OrderId, translatedStringMessage);
            return false;
        }
    }
    private async Task<bool> GetCodesRetry(Order order,CodesRepository codesRepository, OmsRepository omsRepository)
    {
        try
        {
            var blockIds = await omsRepository.GetBlockIds(order);
            var codeList = new List<Code>();
            foreach (var blockId in blockIds)
            {
                var codes = await omsRepository.CodesRetry(order,blockId); 
                if(codes.Count > 0)
                {
                    codeList.AddRange(codes);
                }
            }
            if(codeList.Count == order.Quantity)
            {
                await codesRepository.ProcessOrder(codeList);
                return true;
            }
            else
            {
                Log.Error("Error durante la descarga de códigos de los blockId, orden: {0}",order.OrderId);
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Log.Error(ex,ex.Message);
            throw ex;
        }
    }  
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        using (var scope = serviceProvider.CreateScope()) 
        {
            Log.Error("Background service stopping...");
            var emailSender = serviceProvider.GetRequiredService<EmailSender>(); 
            var emailBody = $"<b>El service worker de BoxTracking se detuvo</b>";
            emailSender.SendEmailAsync("Guseppe.Rodriguez@laaurora.do","Service worker de boxtracking detenido...",emailBody);
        }
        return Task.CompletedTask;
    }

    // public void Dispose()
    // {
    //     _timer?.Dispose();
    // }

}