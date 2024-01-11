﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class AWS_EC2Controller : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AWS_EC2Controller(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            string resultRead  = string.Empty;
            try
            {   // Open the text file using a stream reader.
                // string fileName = @"wwwroot/TechDocuments/AWS_EC2_Document.html";
                string wwwrootPath = _hostingEnvironment.WebRootPath;
                Console.WriteLine("wwwrootPath = " + wwwrootPath);
                string fileName = wwwrootPath + "/TechDocuments/AWS_EC2/AWS_EC2_Document.html";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    resultRead = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\nThe file could not be read:\n\n");
                Console.WriteLine(e.Message);
            }

            ViewBag.Message = resultRead;

            return View();
        }

        // The following method is another way to do it
        public async Task Write_AWS_EC2_Document()
        {
            string resultRead = string.Empty;
            try
            {   // Open the text file using a stream reader.
                string fileName = @"./Views/AWS_EC2/AWS_EC2_Document.html";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    resultRead = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\nThe file could not be read:\n\n");
                Console.WriteLine(e.Message);
            }

            await Response.WriteAsync(resultRead);
        }
    }
}
